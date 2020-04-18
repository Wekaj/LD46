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

                if (PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                    float speed = body.Velocity.Length();

                    if (speed >= entity.DangerSpeed && body.Contact.Y > 0f) {
                        body.Friction = entity.DangerFriction;
                    }
                    else {
                        body.Friction = body.Contact.Y > 0f ? entity.GroundFriction : entity.AirFriction;
                    }
                }
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
            playerBody.Gravity = new Vector2(0f, 600f);

            _playerEntity.BodyID = playerBody.ID;

            _playerEntity.Brain = new PlayerBrain(bindings, _torchEntity.ID);

            _playerEntity.DangerSpeed = 300f;
            _playerEntity.DangerFriction = 10f;
        }

        private void SetupTorch() {
            Body torchBody = PhysicsWorld.CreateBody();
            torchBody.Position = new Vector2(64f, 0f);
            torchBody.Bounds = new RectangleF(2f, 2f, 12f, 14f);
            torchBody.Gravity = new Vector2(0f, 500f);
            torchBody.Friction = 1f;
            torchBody.BounceFactor = 0.5f;

            _torchEntity.BodyID = torchBody.ID;

            _torchEntity.GroundFriction = 2f;
            _torchEntity.AirFriction = 0.5f;
        }
    }
}
