using Floppy.Graphics;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD46.Views {
    public class LevelView {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BackgroundView _backgroundView;
        private readonly TileMapView _tileMapView;
        private readonly EntitiesView _entitiesView;
        private readonly WaterView _waterView;

        private readonly Camera2D _camera = new Camera2D();

        public LevelView(GraphicsDevice graphicsDevice, BackgroundView backgroundView, TileMapView tileMapView, EntitiesView entitiesView, WaterView waterView) {
            _graphicsDevice = graphicsDevice;
            _backgroundView = backgroundView;
            _tileMapView = tileMapView;
            _entitiesView = entitiesView;
            _waterView = waterView;
        }

        public void Draw(Level level) {
            _camera.Position = Vector2.Round(GraphicsConstants.PhysicsToView(level.CameraCenter) - _graphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);

            _backgroundView.Draw(_camera);
            _tileMapView.Draw(level, _camera);
            _entitiesView.Draw(level, _camera);
            _waterView.Draw(level, _camera);
        }
    }
}
