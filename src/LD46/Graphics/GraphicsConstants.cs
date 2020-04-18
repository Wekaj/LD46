using LD46.Physics;
using Microsoft.Xna.Framework;

namespace LD46.Graphics {
    public static class GraphicsConstants {
        public const int TileSize = 32;

        public static float PhysicsToView(float coordinate) {
            return coordinate * TileSize / PhysicsConstants.TileSize;
        }

        public static Vector2 PhysicsToView(Vector2 position) {
            return position * TileSize / PhysicsConstants.TileSize;
        }
    }
}
