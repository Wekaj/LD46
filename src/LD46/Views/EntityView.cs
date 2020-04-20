using Floppy.Extensions;
using Floppy.Graphics;
using Floppy.Physics;
using LD46.Audio;
using LD46.Entities;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD46.Views {
    public enum EntityViewProfile {
        Player,
        Torch,
    }

    public class EntityView {
        private readonly int _entityID;
        private readonly EntityViewProfile _profile;

        private readonly Sprite _sprite;

        private readonly EntityAnimations _animations;
        private readonly ParticleFactory _particleFactory;
        private readonly SoundEffects _soundEffects;

        private IAnimation? _animation;
        private float _animationTimer = 0f;

        private const float _torchParticleRate = 0.05f;
        private float _torchParticleTimer = 0f;

        private bool _facingRight = true;

        private readonly Random _random = new Random();

        private Vector2 _previousTorchPosition;

        public EntityView(int entityID, EntityViewProfile profile, Texture2D texture, 
            EntityAnimations animations, ParticleFactory particleFactory, SoundEffects soundEffects) {

            _entityID = entityID;
            _profile = profile;

            _sprite = new Sprite(texture);

            _animations = animations;
            _particleFactory = particleFactory;
            _soundEffects = soundEffects;
        }

        public ParticlesView? Particles { get; set; }

        public void Update(Level level, float deltaTime) {
            if (!level.EntityWorld.TryGetEntity(_entityID, out Entity? entity)) {
                return;
            }

            if (!level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                return;
            }

            if (body.Velocity.X > 0f) {
                _facingRight = true;
            }
            else if (body.Velocity.X < 0f) {
                _facingRight = false;
            }

            switch (_profile) {
                case EntityViewProfile.Player: {
                    if (entity.Sleeping) {
                        PlayAnimation(_animations.PlayerSleeping);
                    }
                    else if (entity.HasLostAllHope) {
                        PlayAnimation(_animations.PlayerDefeatedRight, _animations.PlayerDefeatedLeft);
                    }
                    else if (entity.Kick) {
                        ReplayAnimation(_animations.PlayerKickRight, _animations.PlayerKickLeft);
                        entity.Kick = false;
                    }
                    else if ((_animation != _animations.PlayerKickRight && _animation != _animations.PlayerKickLeft)
                        || _animationTimer >= 0.3f) {

                        if (body.Contact.Y > 0f) {
                            if (body.Velocity.X != 0f) {
                                PlayAnimation(_animations.PlayerRunRight, _animations.PlayerRunLeft);
                            }
                            else {
                                PlayAnimation(_animations.PlayerIdleRight, _animations.PlayerIdleLeft);
                            }
                        }
                        else {
                            PlayAnimation(_animations.PlayerFallRight, _animations.PlayerFallLeft);
                        }
                    }
                    break;
                }
                case EntityViewProfile.Torch: {
                    if (body.Contact != Vector2.Zero && body.Velocity.Length() > 5f) {
                        _soundEffects
                            .GetRandom(_soundEffects.Thud1, _soundEffects.Thud2, _soundEffects.Thud3)
                            .Play();
                    }

                    if (entity.Sleeping) {
                        PlayAnimation(_animations.Fireplace);
                    }
                    else if (entity.IsPutOut) {
                        PlayAnimation(_animations.TorchOut);
                    }
                    else {
                        PlayAnimation(_animations.Torch);

                        _torchParticleTimer += deltaTime;

                        var torchCenter = new Vector2(16f, 16f);
                        var torchTip = new Vector2(14f, 6f);

                        var particlePosition = GraphicsConstants.PhysicsToView(body.Position + body.Bounds.Center) + Rotate(torchTip - torchCenter, entity.Rotation);

                        Particles?.Add(_particleFactory.CreateBlackLineParticle(particlePosition, _previousTorchPosition));
                        Particles?.Add(_particleFactory.CreateRedLineParticle(particlePosition, _previousTorchPosition));
                        Particles?.Add(_particleFactory.CreateOrangeLineParticle(particlePosition, _previousTorchPosition));
                        Particles?.Add(_particleFactory.CreateYellowLineParticle(particlePosition, _previousTorchPosition));

                        _previousTorchPosition = particlePosition;
                    }
                    break;
                }
            }

            _animationTimer += deltaTime;
        }

        public void Draw(Level level, SpriteBatch spriteBatch) {
            if (!level.EntityWorld.TryGetEntity(_entityID, out Entity? entity)) {
                return;
            }

            if (!level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                return;
            }

            _sprite.Rotation = entity.Rotation;

            _animation?.Apply(_sprite, _animationTimer);

            _sprite.Draw(spriteBatch, Vector2.Round(GraphicsConstants.PhysicsToView(body.Position + body.Bounds.Center)));
        }

        private void PlayAnimation(IAnimation rightAnimation, IAnimation leftAnimation) {
            PlayAnimation(_facingRight ? rightAnimation : leftAnimation);
        }

        private void PlayAnimation(IAnimation animation) {
            if (_animation != animation) {
                ReplayAnimation(animation);
            }
        }

        private void ReplayAnimation(IAnimation rightAnimation, IAnimation leftAnimation) {
            ReplayAnimation(_facingRight ? rightAnimation : leftAnimation);
        }

        private void ReplayAnimation(IAnimation animation) {
            _animation = animation;
            _animationTimer = 0f;
        }

        private Vector2 Rotate(Vector2 vector, float angle) {
            return Vector2.Transform(vector, Matrix.CreateRotationZ(angle));
        }
    }
}
