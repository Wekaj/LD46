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
        private readonly Entity _torchEntity;

        public Level(InputBindings bindings) {
            TileMap = new TileMap(32, 128, PhysicsConstants.TileSize);
            PhysicsWorld = new PhysicsWorld();
            EntityWorld = new EntityWorld();

            var random = new Random();
            for (int y = 0; y < TileMap.Height; y++) {
                int platforms = 0;
                bool solid = false;

                for (int x = 0; x < TileMap.Width; x++) {
                    if (y % 3 == 0 && random.Next(10) == 0) {
                        platforms = random.Next(2, 7);
                        solid = random.Next(3) == 0;
                    }

                    if (x == 0 || x == TileMap.Width - 1 || y == TileMap.Height - 1) {
                        TileMap[x, y].CollisionType = TileCollisionType.Solid;
                    }
                    else if (platforms > 0) {
                        TileMap[x, y].CollisionType = solid ? TileCollisionType.Solid : TileCollisionType.Platform;
                        platforms--;
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

        public float WaterLevel { get; private set; } = -64f;

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

            float waterSpeedModifier = 1f;

            if (PhysicsWorld.TryGetBody(_playerEntity.BodyID, out Body? playerBody)) {
                CameraCenter = playerBody.Position + playerBody.Bounds.Center;

                float yDistance = Math.Abs(playerBody.Position.Y - (TileMap.Height * PhysicsConstants.TileSize - WaterLevel));

                waterSpeedModifier *= Math.Max(yDistance * yDistance / 100000f, 1f);
            }

            WaterLevel += 32f * waterSpeedModifier * deltaTime;
        }

        private void SetupPlayer(InputBindings bindings) {
            Body playerBody = PhysicsWorld.CreateBody();
            playerBody.Position = new Vector2(48f, (TileMap.Height - 2) * PhysicsConstants.TileSize);
            playerBody.Bounds = new RectangleF(2f, 2f, 12f, 14f);
            playerBody.Gravity = new Vector2(0f, 600f);

            _playerEntity.BodyID = playerBody.ID;

            _playerEntity.Brain = new PlayerBrain(bindings, _torchEntity.ID);

            _playerEntity.DangerSpeed = 300f;
            _playerEntity.DangerFriction = 10f;
        }

        private void SetupTorch() {
            Body torchBody = PhysicsWorld.CreateBody();
            torchBody.Position = new Vector2(64f, (TileMap.Height - 2) * PhysicsConstants.TileSize);
            torchBody.Bounds = new RectangleF(2f, 2f, 12f, 14f);
            torchBody.Gravity = new Vector2(0f, 300f);
            torchBody.Friction = 1f;
            torchBody.BounceFactor = 0.5f;

            _torchEntity.BodyID = torchBody.ID;

            _torchEntity.GroundFriction = 2f;
            _torchEntity.AirFriction = 0.5f;
        }
    }
}
