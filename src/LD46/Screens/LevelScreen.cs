using Floppy.Input;
using Floppy.Physics;
using Floppy.Screens;
using Floppy.Utilities;
using LD46.Audio;
using LD46.Entities;
using LD46.Input;
using LD46.Levels;
using LD46.Physics;
using LD46.Views;
using Microsoft.Xna.Framework;
using System;

namespace LD46.Screens {
    public class LevelScreen : IScreen<LevelSettings> {
        private readonly ScreenManager _screens;
        private readonly InputBindings _bindings;
        private readonly SoundEffects _soundEffects;
        private readonly LevelView _levelView;

        private LevelSettings _settings;

        private Entity _playerEntity;
        private Entity _torchEntity;

        private bool _hasWon = false;
        private float _winTimer = 0f;

        public LevelScreen(ScreenManager screens, InputBindings bindings, SoundEffects soundEffects, LevelView levelView) {
            _screens = screens;
            _bindings = bindings;
            _soundEffects = soundEffects;
            _levelView = levelView;
        }

        public Level Level { get; private set; }

        public void Initialize(LevelSettings args) {
            _settings = args;

            Level = new Level(args);

            _torchEntity = Level.EntityWorld.CreateEntity();
            _playerEntity = Level.EntityWorld.CreateEntity();

            SetupTorch();
            SetupPlayer(_bindings);

            _levelView.TorchEntityID = _torchEntity.ID;
            _levelView.PlayerEntityID = _playerEntity.ID;

            _levelView.TileMap.TextureOffset = args.TextureOffset;
        }

        public void Update(GameTime gameTime) {
            if (_bindings.JustPressed(Bindings.Restart)) {
                _screens.TransitionTo<LevelScreen, LevelSettings>(_settings);
            }

            if (_levelView.HasFadedOut && _settings.NextLevel != null) {
                _screens.TransitionTo<LevelScreen, LevelSettings>(_settings.NextLevel);
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Level.SlowMoTimer > 0f) {
                deltaTime /= 2f;
            }

            Level.Update(deltaTime);

            if (Level.PhysicsWorld.TryGetBody(_torchEntity.BodyID, out Body? torchBody)) {
                if (!_hasWon && torchBody.Position.Y + torchBody.Bounds.Center.Y <= Level.FinishHeight) {
                    _hasWon = true;

                    Level.SlowMoTimer = float.MaxValue;
                }

                if (_hasWon) {
                    Level.CameraCenter += (torchBody.Position + torchBody.Bounds.Center - Level.CameraCenter) * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (_winTimer < 1f) {
                        _winTimer += deltaTime;
                        if (_winTimer >= 1f) {
                            _levelView.FadeOut();
                        }
                    }
                }
            }

            float waterSpeedModifier = 1f;

            if (Level.PhysicsWorld.TryGetBody(_playerEntity.BodyID, out Body? playerBody)) {
                if (!_hasWon) {
                    Level.CameraCenter = playerBody.Position + playerBody.Bounds.Center;
                }

                float yDistance = Math.Abs(playerBody.Position.Y - (Level.TileMap.Height * PhysicsConstants.TileSize - Level.WaterLevel));

                waterSpeedModifier *= Math.Max(yDistance * yDistance / 100000f, 1f);
            }

            Level.WaterLevel += _settings.WaterSpeed * waterSpeedModifier * deltaTime;

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
            playerBody.Position = new Vector2(Level.TileMap.Width / 2f, Level.TileMap.Height - 22);
            playerBody.Bounds = new RectangleF(18f / 32f, 18f / 32f, 28f / 32f, 30f / 32f);
            playerBody.Gravity = new Vector2(0f, 37.5f);

            _playerEntity.BodyID = playerBody.ID;

            _playerEntity.Brain = new PlayerBrain(bindings, _torchEntity.ID, _soundEffects);

            _levelView.Entities.Add(_playerEntity.ID, EntityViewProfile.Player);
        }

        private void SetupTorch() {
            Body torchBody = Level.PhysicsWorld.CreateBody();
            torchBody.Position = new Vector2(Level.TileMap.Width / 2f + 1f, Level.TileMap.Height - 21);
            torchBody.Bounds = new RectangleF(2f / 32f, 2f / 32f, 28f / 32f, 28f / 32f);
            torchBody.Gravity = new Vector2(0f, 18.8f);
            torchBody.BounceFactor = 0.5f;

            _torchEntity.BodyID = torchBody.ID;

            _torchEntity.GroundFriction = 2f;
            _torchEntity.AirFriction = 0.1f;

            _torchEntity.CanRotate = true;

            _torchEntity.IsBlowable = true;

            _levelView.Entities.Add(_torchEntity.ID, EntityViewProfile.Torch);
        }
    }
}
