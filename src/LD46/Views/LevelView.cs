using Floppy.Graphics;
using LD46.Graphics;
using LD46.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        private readonly Texture2D _flowMapTexture;

        private RenderTarget2D _worldTarget, _waterTarget;

        private readonly Camera2D _camera = new Camera2D();

        private int _extraSize = 128;

        private float _shaderTimer = 0f;

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
            _flowMapTexture = content.Load<Texture2D>("Textures/FlowMap");

            _worldTarget = CreateRenderTarget();
            _waterTarget = CreateRenderTarget();

            _waterEffect.Parameters["WaterMaskSampler+WaterMask"].SetValue(_waterTarget);
            _waterEffect.Parameters["FlowMapSampler+FlowMap"].SetValue(_flowMapTexture);
            _waterEffect.Parameters["WaterColor"].SetValue(new Color(152, 163, 152).ToVector4());
        }

        public void Update(float deltaTime) {
            _shaderTimer += deltaTime;

            _waterView.Update(deltaTime);
        }

        public void Draw(Level level) {
            if (RenderTargetsAreOutdated()) {
                UpdateRenderTargets();
            }

            _camera.Position = Vector2.Round(GraphicsConstants.PhysicsToView(level.CameraCenter) - _graphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);

            _renderTargetStack.Push(_worldTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _backgroundView.Draw(_camera);
                _tileMapView.Draw(level, _camera);
                _entitiesView.Draw(level, _camera);
            _renderTargetStack.Pop();

            _renderTargetStack.Push(_waterTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _waterView.DrawMask(level, _camera);
            _renderTargetStack.Pop();

            _waterEffect.Parameters["Time"].SetValue(_shaderTimer);

            Vector2 camera = _camera.Position / _graphicsDevice.Viewport.Bounds.Size.ToVector2();
            _waterEffect.Parameters["Camera"].SetValue(camera);

            _spriteBatch.Begin(effect: _waterEffect);
            _spriteBatch.Draw(_worldTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _waterView.Draw(level, _camera);
        }

        private bool RenderTargetsAreOutdated() {
            return _worldTarget.Width != _graphicsDevice.Viewport.Width + _extraSize
                || _worldTarget.Height != _graphicsDevice.Viewport.Height + _extraSize;
        }

        private void UpdateRenderTargets() {
            _worldTarget.Dispose();
            _waterTarget.Dispose();

            _worldTarget = CreateRenderTarget();
            _waterTarget = CreateRenderTarget();
        }

        private RenderTarget2D CreateRenderTarget() {
            return new RenderTarget2D(_graphicsDevice,
                _graphicsDevice.Viewport.Width + _extraSize,
                _graphicsDevice.Viewport.Height + _extraSize,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PreserveContents);
        }
    }
}
