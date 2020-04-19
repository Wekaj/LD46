using Floppy.Graphics;
using Floppy.Physics;
using LD46.Entities;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LD46.Views {
    public class EntitiesView {
        private readonly SpriteBatch _spriteBatch;

        private readonly EntityAnimations _animations;

        private readonly List<EntityView> _entityViews = new List<EntityView>();

        private readonly Texture2D _defaultTexture;

        public EntitiesView(ContentManager content, SpriteBatch spriteBatch, EntityAnimations animations) {
            _spriteBatch = spriteBatch;

            _animations = animations;

            _defaultTexture = content.Load<Texture2D>("Textures/Torch");
        }

        public void Add(int entityID, EntityViewProfile profile) {
            _entityViews.Add(new EntityView(entityID, profile, _defaultTexture, _animations));
        }

        public void Update(Level level, float deltaTime) {
            foreach (EntityView entityView in _entityViews) {
                entityView.Update(level, deltaTime);
            }
        }

        public void Draw(Level level, Camera2D camera) {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            foreach (EntityView entityView in _entityViews) {
                entityView.Draw(level, _spriteBatch);
            }

            _spriteBatch.End();
        }
    }
}
