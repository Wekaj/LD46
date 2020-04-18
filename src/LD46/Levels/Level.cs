using Floppy.Input;
using Floppy.Physics;
using Floppy.Utilities;
using LD46.Entities;
using LD46.Physics;
using Microsoft.Xna.Framework;

namespace LD46.Levels {
    public class Level {
        private readonly Entity _playerEntity;
        private readonly Entity _torchEntity;

        public Level(InputBindings bindings) {
            TileMap = new TileMap(32, 32, PhysicsConstants.TileSize);
            PhysicsWorld = new PhysicsWorld();
            EntityWorld = new EntityWorld();

            for (int y = 0; y < TileMap.Height; y++) {
                for (int x = 0; x < TileMap.Width; x++) {
                    if (x == 0 || x == TileMap.Width - 1 || y == TileMap.Height - 1) {
                        TileMap[x, y].CollisionType = TileCollisionType.Solid;
                    }
                }
            }

            _playerEntity = EntityWorld.CreateEntity();
            _torchEntity = EntityWorld.CreateEntity();

            SetupPlayer(bindings);
            SetupTorch();
        }

        public TileMap TileMap { get; }
        public PhysicsWorld PhysicsWorld { get; }
        public EntityWorld EntityWorld { get; }

        public Vector2 CameraCenter { get; private set; }

        public void Update(float deltaTime) {
            foreach (Entity entity in EntityWorld.Entities) {
                entity.Brain?.Update(entity, this, deltaTime);
            }

            foreach (Body body in PhysicsWorld.Bodies) {
                BodyPhysics.UpdateBody(body, deltaTime, TileMap);
            }

            if (PhysicsWorld.TryGetBody(_playerEntity.BodyID, out Body? playerBody)) {
                CameraCenter = playerBody.Position + playerBody.Bounds.Center;
            }
        }

        private void SetupPlayer(InputBindings bindings) {
            Body playerBody = PhysicsWorld.CreateBody();
            playerBody.Position = new Vector2(48f, 0f);
            playerBody.Bounds = new RectangleF(2f, 2f, 12f, 14f);
            playerBody.Gravity = new Vector2(0f, 500f);

            _playerEntity.BodyID = playerBody.ID;

            _playerEntity.Brain = new PlayerBrain(bindings, _torchEntity.ID);
        }

        private void SetupTorch() {
            Body torchBody = PhysicsWorld.CreateBody();
            torchBody.Position = new Vector2(64f, 0f);
            torchBody.Bounds = new RectangleF(2f, 2f, 12f, 14f);
            torchBody.Gravity = new Vector2(0f, 500f);

            _torchEntity.BodyID = torchBody.ID;
        }
    }
}
