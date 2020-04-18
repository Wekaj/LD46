using Floppy.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD46.Views {
    public class BackgroundView {
        private const int _tileSize = 64;

        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _backgroundTilesTexture;

        public BackgroundView(ContentManager content, SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;

            _backgroundTilesTexture = content.Load<Texture2D>("Textures/BackgroundTiles");
        }

        public void Draw(Camera2D camera) {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            int startX = (int)Math.Floor(camera.Position.X / _tileSize);
            int startY = (int)Math.Floor(camera.Position.Y / _tileSize);
            int endX = (int)Math.Floor((camera.Position.X + _spriteBatch.GraphicsDevice.Viewport.Width) / _tileSize);
            int endY = (int)Math.Floor((camera.Position.Y + _spriteBatch.GraphicsDevice.Viewport.Height) / _tileSize);

            for (int y = startY; y <= endY; y++) {
                for (int x = startX; x <= endX; x++) {
                    DrawTile(x, y);
                }
            }

            _spriteBatch.End();
        }

        private void DrawTile(int x, int y) {
            Rectangle sourceRectangle = GetSourceRectangle();

            _spriteBatch.Draw(_backgroundTilesTexture, new Vector2(x * _tileSize, y * _tileSize), sourceRectangle, Color.White);
        }

        private Rectangle GetSourceRectangle() {
            return new Rectangle(0, 0, 64, 64);
        }
    }
}
