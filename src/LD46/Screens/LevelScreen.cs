using Floppy.Input;
using Floppy.Screens;
using LD46.Input;
using LD46.Levels;
using LD46.Views;
using Microsoft.Xna.Framework;

namespace LD46.Screens {
    public class LevelScreen : IScreen {
        private readonly ScreenManager _screens;
        private readonly InputBindings _bindings;
        private readonly LevelView _levelView;

        public LevelScreen(ScreenManager screens, InputBindings bindings, Level level, LevelView levelView) {
            _screens = screens;
            _bindings = bindings;
            _levelView = levelView;

            Level = level;
        }

        public Level Level { get; }

        public void Update(GameTime gameTime) {
            if (_bindings.IsPressed(Bindings.Restart)) {
                _screens.TransitionTo<LevelScreen>();
            }

            Level.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(GameTime gameTime) {
            _levelView.Draw(Level);
        }
    }
}
