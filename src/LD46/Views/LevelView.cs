using LD46.Levels;

namespace LD46.Views {
    public class LevelView {
        private readonly TileMapView _tileMapView;

        public LevelView(TileMapView tileMapView) {
            _tileMapView = tileMapView;
        }

        public void Draw(Level level) {
            _tileMapView.Draw(level.TileMap);
        }
    }
}
