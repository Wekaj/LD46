using Floppy.Extensions;
using LD46.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD46.Views {
    public class ParticleFactory {
        private readonly Texture2D _particleTexture, _pixelTexture;

        public ParticleFactory(ContentManager content) {
            _particleTexture = content.Load<Texture2D>("Textures/Particle");
            _pixelTexture = content.Load<Texture2D>("Textures/Pixel");
        }

        public Particle CreateRedFireParticle(Vector2 position, Vector2 velocity) {
            var particle = new Particle(_particleTexture, 1f) {
                Position = position,
                Velocity = velocity + new Vector2(32f, -38f),
                AngularVelocity = 1f,
                ScaleFunc = p => new Vector2(1f - p),
                ColorFunc = p => Color.Red
            };

            particle.Sprite.LayerDepth = 0.11f;

            return particle;
        }

        public Particle CreateOrangeFireParticle(Vector2 position, Vector2 velocity) {
            var particle = new Particle(_particleTexture, 0.75f) {
                Position = position,
                Velocity = velocity + new Vector2(32f, -38f),
                AngularVelocity = 2f,
                ScaleFunc = p => 0.75f * new Vector2(1f - p),
                ColorFunc = p => Color.Orange
            };

            particle.Sprite.LayerDepth = 0.12f;

            return particle;
        }

        public Particle CreateYellowFireParticle(Vector2 position, Vector2 velocity) {
            var particle = new Particle(_particleTexture, 0.5f) {
                Position = position,
                Velocity = velocity + new Vector2(32f, -38f),
                AngularVelocity = 3f,
                ScaleFunc = p => 0.4f * new Vector2(1f - p),
                ColorFunc = p => Color.Yellow
            };

            particle.Sprite.LayerDepth = 0.13f;

            return particle;
        }

        public Particle CreateBlackLineParticle(Vector2 position1, Vector2 position2) {
            Vector2 diff = position2 - position1;
            float dist = diff.Length() + 1f;

            var particle = new Particle(_pixelTexture, 0.8f) {
                Position = position1,
                ScaleFunc = p => new Vector2(dist, 2f * (1f - p) + 12f * (float)Math.Sin(p * Math.PI * 2f)),
                ColorFunc = p => new Color(31, 31, 31)
            };

            particle.Sprite.Rotation = (position2 - position1).GetAngle();
            particle.Sprite.LayerDepth = 0.10f;

            return particle;
        }

        public Particle CreateRedLineParticle(Vector2 position1, Vector2 position2) {
            Vector2 diff = position2 - position1;
            float dist = diff.Length() + 1f;

            var particle = new Particle(_pixelTexture, 0.5f) {
                Position = position1,
                ScaleFunc = p => new Vector2(dist, 2f * (1f - p) + 10f * (float)Math.Sin(p * Math.PI * 2f)),
                ColorFunc = p => new Color(233, 64, 64)
            };

            particle.Sprite.Rotation = (position2 - position1).GetAngle();
            particle.Sprite.LayerDepth = 0.11f;

            return particle;
        }

        public Particle CreateOrangeLineParticle(Vector2 position1, Vector2 position2) {
            Vector2 diff = position2 - position1;
            float dist = diff.Length() + 1f;

            var particle = new Particle(_pixelTexture, 0.4f) {
                Position = position1,
                ScaleFunc = p => new Vector2(dist, 2f * (1f - p) + 8f * (float)Math.Sin(p * Math.PI * 2f)),
                ColorFunc = p => new Color(233, 113, 38)
            };

            particle.Sprite.Rotation = (position2 - position1).GetAngle();
            particle.Sprite.LayerDepth = 0.12f;

            return particle;
        }

        public Particle CreateYellowLineParticle(Vector2 position1, Vector2 position2) {
            Vector2 diff = position2 - position1;
            float dist = diff.Length() + 1f;

            var particle = new Particle(_pixelTexture, 0.3f) {
                Position = position1,
                ScaleFunc = p => new Vector2(dist, 2f * (1f - p) + 4f * (float)Math.Sin(p * Math.PI * 2f)),
                ColorFunc = p => new Color(251, 242, 54)
            };

            particle.Sprite.Rotation = (position2 - position1).GetAngle();
            particle.Sprite.LayerDepth = 0.13f;

            return particle;
        }

        public Particle CreateWindParticle(Vector2 position, Vector2 direction) {
            var particle = new Particle(_particleTexture, 3f) {
                Position = position,
                Velocity = direction * 500f,
                AngularVelocity = 10f,
                ScaleFunc = p => new Vector2(0.5f * (float)Math.Sin(p * MathHelper.Pi))
            };

            particle.Sprite.LayerDepth = 0.09f;
            particle.Sprite.Scale = Vector2.Zero;

            return particle;
        }
    }
}
