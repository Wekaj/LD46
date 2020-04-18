using Floppy.Extensions;
using Floppy.Input;
using Floppy.Physics;
using LD46.Input;
using LD46.Levels;
using Microsoft.Xna.Framework;
using System;

namespace LD46.Entities {
    public class PlayerBrain : IBrain {
        private const float _movementSpeed = 150f;
        private const float _jumpImpulse = 300f;
        private const float _jumpTime = 0.1f;
        private const float _kickImpulse = 300f;
        private const float _kickHMultiplier = 5f;
        private const float _kickDistance = 24f;

        private readonly InputBindings _bindings;

        private readonly int _torchEntityID;

        private float _jumpTimer = 0f;

        public PlayerBrain(InputBindings bindings, int torchEntityID) {
            _bindings = bindings;

            _torchEntityID = torchEntityID;
        }

        public void Update(Entity entity, Level level, float deltaTime) {
            if (!level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                return;
            }

            float speed = body.Velocity.Length();

            if (speed < entity.DangerSpeed) {
                if (_bindings.IsPressed(Bindings.MoveRight)) {
                    body.Velocity = body.Velocity.SetX(_movementSpeed);
                }
                if (_bindings.IsPressed(Bindings.MoveLeft)) {
                    body.Velocity = body.Velocity.SetX(-_movementSpeed);
                }

                if (!_bindings.IsPressed(Bindings.MoveRight) && !_bindings.IsPressed(Bindings.MoveLeft)) {
                    body.Velocity = body.Velocity.SetX(0f);
                }
            }

            if (_bindings.JustPressed(Bindings.Jump)) {
                body.Velocity = body.Velocity.SetY(0f);
                body.Impulse += new Vector2(0f, -_jumpImpulse * Math.Min(Math.Max(_jumpTime - _jumpTimer, 0f), deltaTime) / _jumpTime);

                _jumpTimer += deltaTime;

                KickTorch(body, level);
            }

            if (_bindings.IsPressed(Bindings.Jump) && _jumpTimer < _jumpTime) {
                body.Impulse += new Vector2(0f, -_jumpImpulse * Math.Min(Math.Max(_jumpTime - _jumpTimer, 0f), deltaTime) / _jumpTime);

                _jumpTimer += deltaTime;
            }

            if (!_bindings.IsPressed(Bindings.Jump)) {
                if (body.Velocity.Y < 0f) {
                    body.Velocity = body.Velocity.SetY(0f);
                }
                _jumpTimer = 0f;
            }

            if (_bindings.JustPressed(Bindings.Kick)) {
                KickTorch(body, level);
            }
        }

        private void KickTorch(Body body, Level level) {
            if (!level.EntityWorld.TryGetEntity(_torchEntityID, out Entity? torchEntity)) {
                return;
            }

            if (!level.PhysicsWorld.TryGetBody(torchEntity.BodyID, out Body? torchBody)) {
                return;
            }

            float distance = Vector2.Distance(body.Position + body.Bounds.Center, torchBody.Position + torchBody.Bounds.Center);

            if (distance <= _kickDistance) {
                torchBody.Velocity = torchBody.Velocity.SetY(0f);

                var impulse = new Vector2((torchBody.Position.X - body.Position.X) * _kickHMultiplier, -_kickImpulse);

                torchBody.Impulse += impulse;
            }
        }
    }
}
