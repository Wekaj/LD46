using Floppy.Graphics;
using Floppy.Input;
using Floppy.Physics;
using Floppy.Utilities;
using LD46.Entities;
using LD46.Physics;
using Microsoft.Xna.Framework;
using System;

namespace LD46.Levels {
    public class Level {
        private readonly Entity _playerEntity;

        public Level(InputBindings bindings) {
            TileMap = new TileMap(32, 32, PhysicsConstants.TileSize);
            PhysicsWorld = new PhysicsWorld();
            EntityWorld = new EntityWorld();

            var random = new Random();
            for (int y = 0; y < TileMap.Height; y++) {
                for (int x = 0; x < TileMap.Width; x++) {
                    TileMap[x, y].CollisionType = (TileCollisionType)random.Next(3);
                }
            }

            _playerEntity = EntityWorld.CreateEntity();

            SetupPlayer(bindings);
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
            playerBody.Bounds = new RectangleF(2f, 2f, 12f, 14f);
            playerBody.Gravity = new Vector2(0f, 500f);

            _playerEntity.BodyID = playerBody.ID;

            _playerEntity.Brain = new PlayerBrain(bindings);
        }
    }
}
