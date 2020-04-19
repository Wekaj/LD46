using Floppy.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD46.Views {
    public class EntityAnimations {
        public EntityAnimations(ContentManager content) {
            Texture2D playerTexture = content.Load<Texture2D>("Textures/Player");

            PlayerIdleRight = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                IsLooping = true
            }
            .AddFrame(new Rectangle(0, 0, 64, 64), 0.1f)
            .AddFrame(new Rectangle(64, 0, 64, 64), 0.1f)
            .AddFrame(new Rectangle(128, 0, 64, 64), 0.1f)
            .AddFrame(new Rectangle(192, 0, 64, 64), 0.05f);

            PlayerIdleLeft = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                IsLooping = true,
                Effects = SpriteEffects.FlipHorizontally
            }
            .AddFrame(new Rectangle(0, 0, 64, 64), 0.1f)
            .AddFrame(new Rectangle(64, 0, 64, 64), 0.1f)
            .AddFrame(new Rectangle(128, 0, 64, 64), 0.1f)
            .AddFrame(new Rectangle(192, 0, 64, 64), 0.05f);

            PlayerRunRight = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                IsLooping = true
            }
            .AddFrame(new Rectangle(0, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(64, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(128, 64, 64, 64), 0.1f)
            .AddFrame(new Rectangle(192, 64, 64, 64), 0.1f)
            .AddFrame(new Rectangle(256, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(320, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(384, 64, 64, 64), 0.1f)
            .AddFrame(new Rectangle(448, 64, 64, 64), 0.1f);

            PlayerRunLeft = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                IsLooping = true,
                Effects = SpriteEffects.FlipHorizontally
            }
            .AddFrame(new Rectangle(0, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(64, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(128, 64, 64, 64), 0.1f)
            .AddFrame(new Rectangle(192, 64, 64, 64), 0.1f)
            .AddFrame(new Rectangle(256, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(320, 64, 64, 64), 0.05f)
            .AddFrame(new Rectangle(384, 64, 64, 64), 0.1f)
            .AddFrame(new Rectangle(448, 64, 64, 64), 0.1f);

            PlayerFallRight = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                IsLooping = true
            }
            .AddFrame(new Rectangle(0, 128, 64, 64), 0.1f);

            PlayerFallLeft = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                IsLooping = true,
                Effects = SpriteEffects.FlipHorizontally
            }
            .AddFrame(new Rectangle(0, 128, 64, 64), 0.1f);

            PlayerKickRight = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
            }
            .AddFrame(new Rectangle(0, 192, 64, 64), 0.1f)
            .AddFrame(new Rectangle(64, 192, 64, 64), 0.15f)
            .AddFrame(new Rectangle(128, 192, 64, 64), 0.05f)
            .AddFrame(new Rectangle(192, 192, 64, 64), 0.05f);

            PlayerKickLeft = new SingleTextureAnimation(playerTexture) {
                Origin = new Vector2(32f, 32f),
                Effects = SpriteEffects.FlipHorizontally
            }
            .AddFrame(new Rectangle(0, 192, 64, 64), 0.1f)
            .AddFrame(new Rectangle(64, 192, 64, 64), 0.15f)
            .AddFrame(new Rectangle(128, 192, 64, 64), 0.05f)
            .AddFrame(new Rectangle(192, 192, 64, 64), 0.05f);

            Texture2D torchTexture = content.Load<Texture2D>("Textures/Torch");

            Torch = new SingleTextureAnimation(torchTexture) {
                Origin = new Vector2(16f, 16f),
                IsLooping = true
            }
            .AddFrame(new Rectangle(0, 0, 32, 32), 1f);
        }

        public IAnimation PlayerIdleRight { get; }
        public IAnimation PlayerIdleLeft { get; }
        public IAnimation PlayerRunRight { get; }
        public IAnimation PlayerRunLeft { get; }
        public IAnimation PlayerFallRight { get; }
        public IAnimation PlayerFallLeft { get; }
        public IAnimation PlayerKickRight { get; }
        public IAnimation PlayerKickLeft { get; }

        public IAnimation Torch { get; }
    }
}
