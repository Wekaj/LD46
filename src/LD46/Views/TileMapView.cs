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
            Rectangle sourceRectangle = GetSourceRectangle(x, y, tileMap);

            _spriteBatch.Draw(_tilesTexture, new Vector2(x + 0.5f, y + 0.5f) * GraphicsConstants.TileSize, sourceRectangle, 
                Color.White, 0f, new Vector2(32f, 32f), Vector2.One, SpriteEffects.None, 0f);
        }

        private Rectangle GetSourceRectangle(int x, int y, TileMap tileMap) {
            TileCollisionType collisionType = tileMap[x, y].CollisionType;

            int i = 0;

            switch (collisionType) {
                case TileCollisionType.Platform: {
                    i = 1;
                    break;
                }
                case TileCollisionType.Solid: {
                    if (tileMap.IsSolid(x + 1, y)) {
                        if (tileMap.IsSolid(x - 1, y)) {
                            i = 4;
                        }
                        else {
                            i = 3;
                        }
                    }
                    else if (tileMap.IsSolid(x - 1, y)) {
                        i = 5;
                    }
                    else {
                        i = 2;
                    }
                    break;
                }
            }

            return new Rectangle(i * 64, 0, 64, 64);
        }
    }
}
