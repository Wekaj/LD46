using Floppy.Graphics;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD46.Views {
    public class LevelView {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly IRenderTargetStack _renderTargetStack;
        private readonly BackgroundView _backgroundView;
        private readonly TileMapView _tileMapView;
        private readonly EntitiesView _entitiesView;
        private readonly WaterView _waterView;

        private readonly Effect _waterEffect;

        private RenderTarget2D _worldTarget, _waterTarget;

        private readonly Camera2D _camera = new Camera2D();

        public LevelView(GraphicsDevice graphicsDevice, ContentManager content,
            SpriteBatch spriteBatch, IRenderTargetStack renderTargetStack, 
            BackgroundView backgroundView, TileMapView tileMapView, EntitiesView entitiesView, WaterView waterView) {

            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _renderTargetStack = renderTargetStack;
            _backgroundView = backgroundView;
            _tileMapView = tileMapView;
            _entitiesView = entitiesView;
            _waterView = waterView;

            _waterEffect = content.Load<Effect>("Effects/Water");

            _worldTarget = CreateRenderTarget();
            _waterTarget = CreateRenderTarget();
        }

        public void Update(float deltaTime) {
            _waterView.Update(deltaTime);
        }

        public void Draw(Level level) {
            if (RenderTargetsAreOutdated()) {
                UpdateRenderTargets();
            }

            _camera.Position = Vector2.Round(GraphicsConstants.PhysicsToView(level.CameraCenter) - _graphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);

            _renderTargetStack.Push(_worldTarget);
                _backgroundView.Draw(_camera);
                _tileMapView.Draw(level, _camera);
                _entitiesView.Draw(level, _camera);
                _waterView.Draw(level, _camera);
            _renderTargetStack.Pop();

            _renderTargetStack.Push(_waterTarget);
                _waterView.DrawMask(level, _camera);
            _renderTargetStack.Pop();

            //_waterEffect.Parameters["WaterMask"].SetValue(_waterTarget);

            _spriteBatch.Begin(effect: _waterEffect);
            _spriteBatch.Draw(_worldTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        private bool RenderTargetsAreOutdated() {
            return _worldTarget.Width != _graphicsDevice.Viewport.Width
                || _worldTarget.Height != _graphicsDevice.Viewport.Height;
        }

        private void UpdateRenderTargets() {
            _worldTarget.Dispose();
            _waterTarget.Dispose();

            _worldTarget = CreateRenderTarget();
            _waterTarget = CreateRenderTarget();
        }

        private RenderTarget2D CreateRenderTarget() {
            return new RenderTarget2D(_graphicsDevice,
                _graphicsDevice.Viewport.Width,
                _graphicsDevice.Viewport.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents);
        }
    }
}
