using Floppy.Input;
using Floppy.Physics;
using Floppy.Screens;
using Floppy.Utilities;
using LD46.Entities;
using LD46.Input;
using LD46.Levels;
using LD46.Physics;
using LD46.Views;
using Microsoft.Xna.Framework;
using System;

namespace LD46.Screens {
    public class LevelScreen : IScreen {
        private readonly ScreenManager _screens;
        private readonly InputBindings _bindings;
        private readonly LevelView _levelView;

        private readonly Entity _playerEntity;
        private readonly Entity _torchEntity;

        public LevelScreen(ScreenManager screens, InputBindings bindings, Level level, LevelView levelView) {
            _screens = screens;
            _bindings = bindings;
            _levelView = levelView;

            Level = level;

            _torchEntity = Level.EntityWorld.CreateEntity();
            _playerEntity = Level.EntityWorld.CreateEntity();

            SetupTorch();
            SetupPlayer(bindings);

            _levelView.TorchEntityID = _torchEntity.ID;
        }

        public Level Level { get; }

        public void Update(GameTime gameTime) {
            if (_bindings.JustPressed(Bindings.Restart)) {
                _screens.TransitionTo<LevelScreen>();
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Level.SlowMoTimer > 0f) {
                deltaTime /= 2f;
            }

            Level.Update(deltaTime);

            float waterSpeedModifier = 1f;

            if (Level.PhysicsWorld.TryGetBody(_playerEntity.BodyID, out Body? playerBody)) {
                Level.CameraCenter = playerBody.Position + playerBody.Bounds.Center;

                float yDistance = Math.Abs(playerBody.Position.Y - (Level.TileMap.Height * PhysicsConstants.TileSize - Level.WaterLevel));

                waterSpeedModifier *= Math.Max(yDistance * yDistance / 100000f, 1f);
            }

            Level.WaterLevel += 1f * waterSpeedModifier * deltaTime;

            if (_playerEntity.HasLostAllHope) {
                _levelView.ShowLoseScreen();
            }

            _levelView.Update(Level, deltaTime);
        }

        public void Draw(GameTime gameTime) {
            _levelView.Draw(Level);
        }

        private void SetupPlayer(InputBindings bindings) {
            Body playerBody = Level.PhysicsWorld.CreateBody();
            playerBody.Position = new Vector2(3f, Level.TileMap.Height - 2);
            playerBody.Bounds = new RectangleF(18f / 32f, 18f / 32f, 28f / 32f, 30f / 32f);
            playerBody.Gravity = new Vector2(0f, 37.5f);

            _playerEntity.BodyID = playerBody.ID;

            _playerEntity.Brain = new PlayerBrain(bindings, _torchEntity.ID);

            _playerEntity.DangerSpeed = 18.8f;
            _playerEntity.DangerFriction = 0.6f;

            _levelView.Entities.Add(_playerEntity.ID, EntityViewProfile.Player);
        }

        private void SetupTorch() {
            Body torchBody = Level.PhysicsWorld.CreateBody();
            torchBody.Position = new Vector2(4, (Level.TileMap.Height - 2) * PhysicsConstants.TileSize);
            torchBody.Bounds = new RectangleF(2f / 32f, 2f / 32f, 28f / 32f, 28f / 32f);
            torchBody.Gravity = new Vector2(0f, 18.8f);
            torchBody.BounceFactor = 0.5f;

            _torchEntity.BodyID = torchBody.ID;

            _torchEntity.GroundFriction = 2f;
            _torchEntity.AirFriction = 0.1f;

            _torchEntity.CanRotate = true;

            _levelView.Entities.Add(_torchEntity.ID, EntityViewProfile.Torch);
        }
    }
}
