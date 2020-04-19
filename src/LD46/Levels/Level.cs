using Floppy.Physics;
using LD46.Entities;
using LD46.Physics;
using Microsoft.Xna.Framework;
using System;

namespace LD46.Levels {
    public class Level {
        public Level(LevelSettings settings) {
            TileMap = new TileMap(32, settings.Height, PhysicsConstants.TileSize);
            PhysicsWorld = new PhysicsWorld();
            EntityWorld = new EntityWorld();

            var random = new Random();
            for (int y = 0; y < TileMap.Height; y++) {
                int platforms = 0;
                bool solid = false;

                for (int x = 0; x < TileMap.Width; x++) {
                    if (y % 3 == 0 && random.Next(10) == 0) {
                        platforms = random.Next(settings.MinPlatformWidth, settings.MaxPlatformWidth + 1);
                        solid = random.NextDouble() <= settings.SolidChance;
                    }

                    if (x == 0 || x == TileMap.Width - 1) {
                        TileMap[x, y].CollisionType = TileCollisionType.Solid;
                    }
                    else if (y == TileMap.Height - 20) {
                        TileMap[x, y].CollisionType = TileCollisionType.Platform;
                    }
                    else if (platforms > 0) {
                        TileMap[x, y].CollisionType = solid ? TileCollisionType.Solid : TileCollisionType.Platform;
                        platforms--;
                    }
                }
            }
        }

        public TileMap TileMap { get; }
        public PhysicsWorld PhysicsWorld { get; }
        public EntityWorld EntityWorld { get; }

        public Vector2 CameraCenter { get; set; }

        public float FinishHeight { get; set; } = 16.5f;

        public float WaterLevel { get; set; } = 12f;
        public float WaterTop => TileMap.Height * PhysicsConstants.TileSize - WaterLevel;

        public float SlowMoTimer { get; set; } = 0f;

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

                    if (entity.CanRotate) {
                        entity.Rotation += body.Velocity.X * deltaTime;
                    }

                    if (body.Position.Y >= WaterTop) {
                        entity.WaterTimer += deltaTime;

                        if (entity.WaterTimer >= 1f) {
                            entity.IsPutOut = true;
                        }
                    }
                    else {
                        entity.WaterTimer = 0f;
                    }
                }
            }

            foreach (Body body in PhysicsWorld.Bodies) {
                BodyPhysics.UpdateBody(body, deltaTime, TileMap);
            }

            if (SlowMoTimer > 0f) {
                SlowMoTimer -= deltaTime;
            }
        }
    }
}
