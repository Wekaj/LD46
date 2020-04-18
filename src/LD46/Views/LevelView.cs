using Floppy.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD46.Views {
    public class LevelView {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly TileMapView _tileMapView;
        private readonly EntitiesView _entitiesView;

        private readonly Camera2D _camera = new Camera2D();

        public LevelView(GraphicsDevice graphicsDevice, TileMapView tileMapView, EntitiesView entitiesView) {
            _graphicsDevice = graphicsDevice;
            _tileMapView = tileMapView;
            _entitiesView = entitiesView;
        }

        public void Draw(Level level) {
            _camera.Position = Vector2.Floor(level.CameraCenter - _graphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);

            _tileMapView.Draw(level, _camera);
            _entitiesView.Draw(level, _camera);
        }
    }
}
