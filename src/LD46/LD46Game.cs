using Floppy.Graphics;
using Floppy.Input;
using Floppy.Screens;
using LD46.Input;
using LD46.Levels;
using LD46.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleInjector;
using System;

namespace LD46 {
    public class LD46Game : Game {
        private readonly GraphicsDeviceManager _graphics;

        private readonly ScreenManager _screens = new ScreenManager();

        private readonly InputBindings _bindings = new InputBindings();

        private RenderTargetStack? _renderTargetStack;
        private SpriteBatch? _spriteBatch;

        private PixelScaler? _pixelScaler;

        public LD46Game() {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            base.Initialize();

            SetupBindings();

            _renderTargetStack = new RenderTargetStack(GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Container container = CreateContainer();

            _pixelScaler = container.GetInstance<PixelScaler>();
            _pixelScaler.Scale = 1;

            InitializeScreens(container);
        }

        protected override void LoadContent() {
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            _bindings.Update();

            _screens.CurrentScreen?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Transparent);

            _pixelScaler!.Begin();

            _screens.CurrentScreen?.Draw(gameTime);

            _pixelScaler!.End();

            base.Draw(gameTime);
        }

        private void SetupBindings() {
            _bindings.Set(Bindings.Restart, new KeyboardBinding(Keys.R));
            _bindings.Set(Bindings.MoveRight, new KeyboardBinding(Keys.Right), new KeyboardBinding(Keys.D));
            _bindings.Set(Bindings.MoveLeft, new KeyboardBinding(Keys.Left), new KeyboardBinding(Keys.A));
            _bindings.Set(Bindings.Drop, new KeyboardBinding(Keys.Down), new KeyboardBinding(Keys.S));
            _bindings.Set(Bindings.Jump, new KeyboardBinding(Keys.Space));
            _bindings.Set(Bindings.Dash, new KeyboardBinding(Keys.LeftShift), new KeyboardBinding(Keys.RightShift));
            _bindings.Set(Bindings.Skip, new KeyboardBinding(Keys.P));
        }

        private Container CreateContainer() {
            var container = new Container();

            container.RegisterInstance(GraphicsDevice);
            container.RegisterInstance(Content);
            container.RegisterInstance(_screens);
            container.RegisterInstance(_bindings);
            container.RegisterInstance<IRenderTargetStack>(_renderTargetStack!);
            container.RegisterInstance(_spriteBatch!);

            return container;
        }

        private void InitializeScreens(Container container) {
            _screens.AddScreenType(() => container.GetInstance<LevelScreen>());

            int smallHeight = 80;
            int largeHeight = 120;

            Level0Settings = new LevelSettings {
                Height = 64,
                TextureOffset = 0,
                BackgroundTile = 1,
                WaterSpeed = 0f,
                IsOpening = true,
            };

            LevelSettings depths1 = new LevelSettings {
                Height = smallHeight,
                SolidChance = 1f / 12f,
                MinPlatformWidth = 3,
                MaxPlatformWidth = 8,
                TextureOffset = 0,
                BackgroundTile = 1,
                WaterSpeed = 1f,
                Name = "The Depths, Part 1",
            };
            Level0Settings.NextLevel = depths1;

            LevelSettings depths2 = new LevelSettings {
                Height = largeHeight,
                SolidChance = 1f / 6f,
                MinPlatformWidth = 2,
                MaxPlatformWidth = 8,
                TextureOffset = 0,
                BackgroundTile = 1,
                WaterSpeed = 2f,
                Name = "The Depths, Part 2",
            };
            depths1.NextLevel = depths2;

            LevelSettings furnace1 = new LevelSettings {
                Height = smallHeight,
                SolidChance = 1f / 6f,
                MinPlatformWidth = 2,
                MaxPlatformWidth = 7,
                TextureOffset = 2,
                BackgroundTile = 0,
                WaterSpeed = 1.25f,
                HasGrates = true,
                Name = "The Furnace, Part 1",
            };
            depths2.NextLevel = furnace1;

            LevelSettings furnace2 = new LevelSettings {
                Height = largeHeight,
                SolidChance = 1f / 4f,
                MinPlatformWidth = 1,
                MaxPlatformWidth = 7,
                TextureOffset = 2,
                BackgroundTile = 0,
                WaterSpeed = 2.25f,
                HasGrates = true,
                Name = "The Furnace, Part 2",
            };
            furnace1.NextLevel = furnace2;

            LevelSettings freezer1 = new LevelSettings {
                Height = smallHeight,
                SolidChance = 1f / 4f,
                MinPlatformWidth = 2,
                MaxPlatformWidth = 6,
                TextureOffset = 4,
                BackgroundTile = 2,
                WaterSpeed = 1.5f,
                HasWind = true,
                Name = "The Freezer, Part 1",
            };
            furnace2.NextLevel = freezer1;

            LevelSettings freezer2 = new LevelSettings {
                Height = largeHeight,
                SolidChance = 1f / 3f,
                MinPlatformWidth = 1,
                MaxPlatformWidth = 6,
                TextureOffset = 4,
                BackgroundTile = 2,
                WaterSpeed = 2.75f,
                HasWind = true,
                Name = "The Freezer, Part 2",
            };
            freezer1.NextLevel = freezer2;

            LevelSettings victorySettings = new LevelSettings {
                Height = 8,
                TextureOffset = 2,
                BackgroundTile = 0,
                IsVictory = true
            };
            freezer2.NextLevel = victorySettings;

            _screens.TransitionTo<LevelScreen, LevelSettings>(Level0Settings);
        }

        public static LevelSettings? Level0Settings { get; private set; }
    }
}
