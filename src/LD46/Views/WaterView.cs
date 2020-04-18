using Floppy.Graphics;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD46.Views {
    public class WaterView {
        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _pixelTexture;

        public WaterView(ContentManager content, SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;

            _pixelTexture = content.Load<Texture2D>("Textures/Pixel");
        }

        public void Draw(Level level, Camera2D camera) {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            int top = level.TileMap.Height * GraphicsConstants.TileSize - (int)Math.Round(GraphicsConstants.PhysicsToView(level.WaterLevel));
            int bottom = level.TileMap.Height * GraphicsConstants.TileSize + 100;

            _spriteBatch.Draw(_pixelTexture, new Rectangle(-1000, top, 2000, bottom - top), Color.SeaGreen);

            _spriteBatch.End();
        }
    }
}
