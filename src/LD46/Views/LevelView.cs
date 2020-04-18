using LD46.Levels;

namespace LD46.Views {
    public class LevelView {
        private readonly TileMapView _tileMapView;
        private readonly EntitiesView _entitiesView;

        public LevelView(TileMapView tileMapView, EntitiesView entitiesView) {
            _tileMapView = tileMapView;
            _entitiesView = entitiesView;
        }

        public void Draw(Level level) {
            _tileMapView.Draw(level.TileMap);
            _entitiesView.Draw(level);
        }
    }
}
