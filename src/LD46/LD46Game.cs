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

        private SpriteBatch? _spriteBatch;

        public LD46Game() {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            base.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Container container = CreateContainer();

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

            _screens.CurrentScreen?.Draw(gameTime);

            base.Draw(gameTime);
        }

        private Container CreateContainer() {
            var container = new Container();

            container.RegisterInstance(GraphicsDevice);
            container.RegisterInstance(Content);
            container.RegisterInstance(_screens);
            container.RegisterInstance(_spriteBatch!);

            return container;
        }

        private void InitializeScreens(Container container) {
            _screens.AddScreenType(() => container.GetInstance<LevelScreen>());

            _screens.TransitionTo<LevelScreen>();
        }
    }
}
