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

            LevelSettings level0Settings = new LevelSettings {
                Height = 64,
                TextureOffset = 0,
                BackgroundTile = 1,
                WaterSpeed = 0f,
                IsOpening = true,
            };

            LevelSettings level1Settings = new LevelSettings {
                Height = 100,
                SolidChance = 1f / 6f,
                MinPlatformWidth = 2,
                MaxPlatformWidth = 8,
                TextureOffset = 0,
                BackgroundTile = 1,
                WaterSpeed = 1f,
            };
            level0Settings.NextLevel = level1Settings;

            LevelSettings level2Settings = new LevelSettings {
                Height = 128,
                SolidChance = 1f / 4f,
                MinPlatformWidth = 1,
                MaxPlatformWidth = 7,
                TextureOffset = 2,
                BackgroundTile = 0,
                WaterSpeed = 2f,
                HasGrates = true,
            };
            level1Settings.NextLevel = level2Settings;

            LevelSettings level3Settings = new LevelSettings {
                Height = 128,
                SolidChance = 1f / 3f,
                MinPlatformWidth = 1,
                MaxPlatformWidth = 5,
                TextureOffset = 4,
                BackgroundTile = 2,
                WaterSpeed = 2.5f,
                HasWind = true,
            };
            level2Settings.NextLevel = level3Settings;

            _screens.TransitionTo<LevelScreen, LevelSettings>(level0Settings);
        }
    }
}
