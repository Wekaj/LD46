using Floppy.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD46.Graphics {
    public class Particle {
        public Particle(Texture2D texture, float life) {
            Sprite = new Sprite(texture) {
                Origin = new Vector2(texture.Width / 2f, texture.Height / 2f)
            };

            Life = life;
        }

        public Sprite Sprite { get; }

        public float Life { get; set; }
        public float LifeTimer { get; set; }
        public bool IsDead => LifeTimer >= Life;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }

        public Func<float, Vector2> ScaleFunc { get; set; } = p => Vector2.One;
        public Func<float, Color> ColorFunc { get; set; } = p => Color.White;

        public void Update(float deltaTime) {
            LifeTimer += deltaTime;

            Position += Velocity * deltaTime;
            Sprite.Rotation += AngularVelocity * deltaTime;

            float p = Math.Clamp(LifeTimer / Life, 0f, 1f);
            Sprite.Scale = ScaleFunc(p);
            Sprite.Color = ColorFunc(p);
        }

        public void Draw(SpriteBatch spriteBatch) {
            Sprite.Draw(spriteBatch, Vector2.Round(Position));
        }
    }
}
