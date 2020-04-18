using Floppy.Physics;
using LD46.Physics;
using System;

namespace LD46.Levels {
    public class Level {
        public Level() {
            TileMap = new TileMap(32, 32, PhysicsConstants.TileSize);

            var random = new Random();
            for (int y = 0; y < TileMap.Height; y++) {
                for (int x = 0; x < TileMap.Width; x++) {
                    TileMap[x, y].CollisionType = (TileCollisionType)random.Next(3);
                }
            }
        }

        public TileMap TileMap { get; }
    }
}
