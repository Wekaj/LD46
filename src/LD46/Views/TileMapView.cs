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

        public int TextureOffset { get; set; }

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

            int tx = 0;
            int ty = 0;

            switch (collisionType) {
                case TileCollisionType.Platform: {
                    if (tileMap.IsSolid(x + 1, y, true)) {
                        if (tileMap.IsSolid(x - 1, y, true)) {
                            tx = 3;
                        }
                        else {
                            tx = 2;
                        }
                    }
                    else if (tileMap.IsSolid(x - 1, y, true)) {
                        tx = 4;
                    }
                    else {
                        tx = 1;
                    }
                    break;
                }
                case TileCollisionType.Solid: {
                    ty = 1;

                    if (tileMap.IsSolid(x + 1, y)) {
                        if (tileMap.IsSolid(x - 1, y)) {
                            tx = 3;
                        }
                        else {
                            tx = 2;
                        }
                    }
                    else if (tileMap.IsSolid(x - 1, y)) {
                        tx = 4;
                    }
                    else {
                        tx = 1;
                    }
                    break;
                }
            }

            return new Rectangle(tx * 64, (ty + TextureOffset) * 64, 64, 64);
        }
    }
}
