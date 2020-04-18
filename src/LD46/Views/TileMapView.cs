using Floppy.Graphics;
using Floppy.Physics;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD46.Views {
    public class TileMapView {
        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _tilesTexture;

        public TileMapView(ContentManager content, SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;

            _tilesTexture = content.Load<Texture2D>("Textures/Tiles");
        }

        public void Draw(Level level, Camera2D camera) {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            for (int y = 0; y < level.TileMap.Height; y++) {
                for (int x = 0; x < level.TileMap.Width; x++) {
                    DrawTile(x, y, level.TileMap);
                }
            }

            _spriteBatch.End();
        }

        private void DrawTile(int x, int y, TileMap tileMap) {
            Rectangle sourceRectangle = GetSourceRectangle(tileMap[x, y].CollisionType);

            _spriteBatch.Draw(_tilesTexture, new Vector2(x, y) * GraphicsConstants.TileSize, sourceRectangle, Color.White);
        }

        private Rectangle GetSourceRectangle(TileCollisionType collisionType) {
            int i = (int)collisionType;

            return new Rectangle(i * GraphicsConstants.TileSize, 0, GraphicsConstants.TileSize, GraphicsConstants.TileSize);
        }
    }
}
