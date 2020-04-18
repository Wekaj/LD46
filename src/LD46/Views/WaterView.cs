using Floppy.Graphics;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD46.Views {
    public class WaterView {
        private const int _waterSize = 128;

        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _waterOutlineTexture, _waterMaskTexture, _pixelTexture;

        private readonly Sprite _waterOutlineSprite, _waterMaskSprite;

        private readonly IAnimation _waterOutlineAnimation, _waterMaskAnimation;

        private float _animationTimer = 0f;

        public WaterView(ContentManager content, SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;

            _waterOutlineTexture = content.Load<Texture2D>("Textures/WaterOutline");
            _waterMaskTexture = content.Load<Texture2D>("Textures/WaterMask");
            _pixelTexture = content.Load<Texture2D>("Textures/Pixel");

            _waterOutlineSprite = new Sprite(_waterOutlineTexture);
            _waterMaskSprite = new Sprite(_waterMaskTexture) { Color = Color.SeaGreen * 0.5f };

            _waterOutlineAnimation = CreateWaterAnimation(_waterOutlineTexture);
            _waterMaskAnimation = CreateWaterAnimation(_waterMaskTexture);
        }

        public void Update(float deltaTime) {
            _animationTimer += deltaTime;
        }

        public void Draw(Level level, Camera2D camera) {
            _waterOutlineAnimation.Apply(_waterOutlineSprite, _animationTimer);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            int startX = (int)Math.Floor(camera.Position.X / _waterSize);
            int endX = (int)Math.Floor((camera.Position.X + _spriteBatch.GraphicsDevice.Viewport.Width) / _waterSize);

            int top = level.TileMap.Height * GraphicsConstants.TileSize - (int)Math.Round(GraphicsConstants.PhysicsToView(level.WaterLevel));

            for (int x = startX; x <= endX; x++) {
                _waterOutlineSprite.Draw(_spriteBatch, new Vector2(x * _waterSize, top - _waterSize / 2f));
            }

            _spriteBatch.End();
        }

        public void DrawMask(Level level, Camera2D camera) {
            _waterMaskAnimation.Apply(_waterMaskSprite, _animationTimer);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            int startX = (int)Math.Floor(camera.Position.X / _waterSize);
            int endX = (int)Math.Floor((camera.Position.X + _spriteBatch.GraphicsDevice.Viewport.Width) / _waterSize);

            int top = level.TileMap.Height * GraphicsConstants.TileSize - (int)Math.Round(GraphicsConstants.PhysicsToView(level.WaterLevel));
            int bottom = level.TileMap.Height * GraphicsConstants.TileSize + 100;

            for (int x = startX; x <= endX; x++) {
                _waterMaskSprite.Draw(_spriteBatch, new Vector2(x * _waterSize, top - _waterSize / 2f));
            }
            _spriteBatch.Draw(_pixelTexture, new Rectangle(startX * _waterSize, top + _waterSize / 2, (endX - startX + 1) * _waterSize, bottom - top), Color.White);

            _spriteBatch.End();
        }

        private IAnimation CreateWaterAnimation(Texture2D texture) {
            var animation = new SingleTextureAnimation(texture)
                .AddFrame(new Rectangle(0, 0, 128, 128), 0.2f)
                .AddFrame(new Rectangle(128, 0, 128, 128), 0.2f)
                .AddFrame(new Rectangle(256, 0, 128, 128), 0.2f)
                .AddFrame(new Rectangle(384, 0, 128, 128), 0.2f);

            animation.IsLooping = true;

            return animation;
        }
    }
}
