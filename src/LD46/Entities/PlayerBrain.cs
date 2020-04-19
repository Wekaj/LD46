using Floppy.Extensions;
using Floppy.Input;
using Floppy.Physics;
using LD46.Input;
using LD46.Levels;
using LD46.Physics;
using Microsoft.Xna.Framework;
using System;

namespace LD46.Entities {
    public class PlayerBrain : IBrain {
        private const float _movementSpeed = 9.4f;

        private const float _jumpImpulse = 20f;
        private const float _jumpTime = 0.1f;
        private const int _maxJumps = 2;

        private const float _dashSpeed = 50f;
        private const float _dashTime = 0.1f;
        private const float _dashHImpulse = 7.5f;
        private const float _dashImpulse = 18.8f;

        private const float _kickImpulse = 18.8f;
        private const float _kickHMultiplier = 12f;
        private const float _kickDistance = 1.5f;

        private const float _gracePeriod = 0.1f;

        private readonly InputBindings _bindings;

        private readonly int _torchEntityID;

        private float _jumpTimer = 0f;
        private bool _isJumping = false;
        private int _jumpsLeft = _maxJumps;

        private Vector2 _dashDir;

        private float _graceTimer;

        private bool _hasBumped = false;

        public PlayerBrain(InputBindings bindings, int torchEntityID) {
            _bindings = bindings;

            _torchEntityID = torchEntityID;
        }

        public void Update(Entity entity, Level level, float deltaTime) {
            if (!level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                return;
            }

            if (level.EntityWorld.TryGetEntity(_torchEntityID, out Entity? torchEntity)) {
                if (torchEntity.IsPutOut) {
                    entity.HasLostAllHope = true;

                    body.Velocity = body.Velocity.SetX(0f);
                    return;
                }
            }

            float speedModifier = 1f;

            float waterHeight = level.TileMap.Height * PhysicsConstants.TileSize - level.WaterLevel;

            if (body.Position.Y + body.Bounds.Center.Y > waterHeight) {
                body.Position = body.Position.SetY(waterHeight - body.Bounds.Center.Y);

                if (body.Velocity.Y > 0f) {
                    body.Velocity = body.Velocity.SetY(0f);
                }

                speedModifier = 0.5f;
                _jumpsLeft = _maxJumps;
            }

            float speed = body.Velocity.Length();

            body.IgnoresPlatforms = _bindings.IsPressed(Bindings.Drop);

            if (_graceTimer > 0f) {
                _graceTimer -= deltaTime;
            }

            if (speed < entity.DangerSpeed && entity.DashTimer <= 0f) {
                if (_bindings.IsPressed(Bindings.MoveRight)) {
                    body.Velocity = body.Velocity.SetX(_movementSpeed * speedModifier);
                }
                if (_bindings.IsPressed(Bindings.MoveLeft)) {
                    body.Velocity = body.Velocity.SetX(-_movementSpeed * speedModifier);
                }

                if (!_bindings.IsPressed(Bindings.MoveRight) && !_bindings.IsPressed(Bindings.MoveLeft)) {
                    body.Velocity = body.Velocity.SetX(0f);
                }
            }

            if (body.Contact.Y > 0f) {
                _jumpsLeft = _maxJumps;
                _graceTimer = _gracePeriod;
            }

            if (_bindings.JustPressed(Bindings.Jump) && _jumpsLeft > 0) {
                body.Velocity = body.Velocity.SetY(0f);
                body.Impulse += new Vector2(0f, -_jumpImpulse * Math.Min(Math.Max(_jumpTime - _jumpTimer, 0f), deltaTime) / _jumpTime);

                _jumpTimer = deltaTime;
                _isJumping = true;
                _jumpsLeft--;

                KickTorch(entity, body, level);
            }

            if (entity.DashTimer <= 0f) {
                _hasBumped = false;

                if (_bindings.JustPressed(Bindings.Dash) && _jumpsLeft > 0) {
                    _dashDir = new Vector2();
                    if (_bindings.IsPressed(Bindings.MoveRight)) {
                        _dashDir.X++;
                    }
                    if (_bindings.IsPressed(Bindings.MoveLeft)) {
                        _dashDir.X--;
                    }

                    if (_dashDir.X != 0f) {
                        entity.DashTimer = _dashTime;

                        _jumpsLeft--;
                    }
                }
            }

            if (entity.DashTimer > 0f) {
                entity.DashTimer -= deltaTime;

                body.Velocity = _dashDir * _dashSpeed;

                BumpTorch(entity, body, level);
            }

            if (_bindings.IsPressed(Bindings.Jump) && _isJumping && _jumpTimer < _jumpTime) {
                body.Impulse += new Vector2(0f, -_jumpImpulse * Math.Min(Math.Max(_jumpTime - _jumpTimer, 0f), deltaTime) / _jumpTime);

                _jumpTimer += deltaTime;
            }

            if (_jumpTimer >= _jumpTime) {
                _isJumping = false;
            }

            if (!_bindings.IsPressed(Bindings.Jump)) {
                if (body.Velocity.Y < 0f) {
                    body.Velocity = body.Velocity.SetY(0f);
                }
                _jumpTimer = 0f;
            }
        }

        private void KickTorch(Entity entity, Body body, Level level) {
            if (!level.EntityWorld.TryGetEntity(_torchEntityID, out Entity? torchEntity)) {
                return;
            }

            if (!level.PhysicsWorld.TryGetBody(torchEntity.BodyID, out Body? torchBody)) {
                return;
            }

            Vector2 playerCenter = body.Position + body.Bounds.Center;
            Vector2 torchCenter = torchBody.Position + torchBody.Bounds.Center;

            float distance = Vector2.Distance(playerCenter, torchCenter);

            if (distance <= _kickDistance) {
                torchBody.Velocity = torchBody.Velocity.SetY(0f);

                var impulse = new Vector2((torchCenter.X - playerCenter.X) * _kickHMultiplier, -_kickImpulse);

                torchBody.Impulse += impulse;

                _jumpsLeft = 1;

                entity.Kick = true;

                level.SlowMoTimer = 0.2f;
            }
            else if (body.Contact.Y <= 0f && _graceTimer <= 0f) {
                _jumpsLeft = 0;

                entity.Kick = true;
            }
        }

        private void BumpTorch(Entity entity, Body body, Level level) {
            if (_hasBumped) {
                return;
            }

            if (!level.EntityWorld.TryGetEntity(_torchEntityID, out Entity? torchEntity)) {
                return;
            }

            if (!level.PhysicsWorld.TryGetBody(torchEntity.BodyID, out Body? torchBody)) {
                return;
            }

            Vector2 playerCenter = body.Position + body.Bounds.Center;
            Vector2 torchCenter = torchBody.Position + torchBody.Bounds.Center;

            float distance = Vector2.Distance(playerCenter, torchCenter);

            if (distance <= _kickDistance) {
                torchBody.Velocity = torchBody.Velocity.SetY(0f);

                var impulse = new Vector2((body.Velocity.X > 0f ? 1f : -1f) * _dashHImpulse, -_dashImpulse);

                torchBody.Impulse += impulse;

                level.SlowMoTimer = 0.15f;

                _hasBumped = true;
            }
        }
    }
}
