using Floppy.Physics;
using LD46.Physics;

namespace LD46.Levels {
    public class Level {
        public Level() {
            TileMap = new TileMap(32, 32, PhysicsConstants.TileSize);
        }

        public TileMap TileMap { get; }
    }
}
