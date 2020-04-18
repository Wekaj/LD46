using Floppy.Graphics;
using Floppy.Screens;
using LD46.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleInjector;

namespace LD46 {
    public class LD46Game : Game {
        private readonly GraphicsDeviceManager _graphics;

        private readonly ScreenManager _screens = new ScreenManager();

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

            _renderTargetStack = new RenderTargetStack(GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Container container = CreateContainer();

            _pixelScaler = container.GetInstance<PixelScaler>();
            _pixelScaler.Scale = 3;

            InitializeScreens(container);
        }

        protected override void LoadContent() {
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

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

        private Container CreateContainer() {
            var container = new Container();

            container.RegisterInstance(GraphicsDevice);
            container.RegisterInstance(Content);
            container.RegisterInstance(_screens);
            container.RegisterInstance<IRenderTargetStack>(_renderTargetStack!);
            container.RegisterInstance(_spriteBatch!);

            return container;
        }

        private void InitializeScreens(Container container) {
            _screens.AddScreenType(() => container.GetInstance<LevelScreen>());

            _screens.TransitionTo<LevelScreen>();
        }
    }
}
