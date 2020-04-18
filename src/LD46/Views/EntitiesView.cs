using Floppy.Graphics;
using Floppy.Physics;
using LD46.Entities;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD46.Views {
    public class EntitiesView {
        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _playerTexture;

        public EntitiesView(ContentManager content, SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;

            _playerTexture = content.Load<Texture2D>("Textures/Player");
        }

        public void Draw(Level level, Camera2D camera) {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            foreach (Entity entity in level.EntityWorld.Entities) {
                DrawEntity(entity, level);
            }

            _spriteBatch.End();
        }

        private void DrawEntity(Entity entity, Level level) {
            if (level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                _spriteBatch.Draw(_playerTexture, Vector2.Floor(body.Position), Color.White);
            }
        }
    }
}
