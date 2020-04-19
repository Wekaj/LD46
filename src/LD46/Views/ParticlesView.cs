using Floppy.Graphics;
using LD46.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LD46.Views {
    public class ParticlesView {
        private readonly SpriteBatch _spriteBatch;

        private readonly List<Particle> _particles = new List<Particle>();

        public ParticlesView(SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;
        }

        public void Add(Particle particle) {
            _particles.Add(particle);
        }

        public void Update(float deltaTime) {
            foreach (Particle particle in _particles) {
                particle.Update(deltaTime);
            }

            _particles.RemoveAll(p => p.IsDead);
        }

        public void Draw(Camera2D camera) {
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransformMatrix());

            foreach (Particle particle in _particles) {
                particle.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }
    }
}
