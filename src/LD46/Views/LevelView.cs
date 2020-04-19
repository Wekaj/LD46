using Floppy.Extensions;
using Floppy.Graphics;
using Floppy.Physics;
using Floppy.Utilities;
using LD46.Entities;
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
        private readonly WaterView _waterView;

        private readonly Effect _waterEffect;
        private readonly Texture2D _flowMapTexture, _pixelTexture, _arrowTexture, _finishTexture;
        private readonly SpriteFont _regularFont;

        private RenderTarget2D _worldTarget, _waterTarget;

        private readonly Camera2D _camera = new Camera2D();

        private int _extraSize = 128;

        private float _shaderTimer = 0f;

        private bool _showLoseScreen = false;
        private float _loseScreenOpacity = 0f;

        private bool _fadeOut = false;
        private float _fadeOutOpacity = 0f;

        private readonly Sprite _arrowSprite;

        private readonly Random _random = new Random();

        private float _lightRadius = 0f;
        private float _flickerTimer = 0f;
        private const float _flickerTime = 0.075f;

        public LevelView(GraphicsDevice graphicsDevice, ContentManager content,
            SpriteBatch spriteBatch, IRenderTargetStack renderTargetStack, 
            BackgroundView backgroundView, TileMapView tileMapView, EntitiesView entitiesView, 
            WaterView waterView, ParticlesView particlesView) {

            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _renderTargetStack = renderTargetStack;
            Background = backgroundView;
            TileMap = tileMapView;
            Entities = entitiesView;
            Particles = particlesView;
            _waterView = waterView;

            _waterEffect = content.Load<Effect>("Effects/Water");
            _flowMapTexture = content.Load<Texture2D>("Textures/FlowMap");
            _pixelTexture = content.Load<Texture2D>("Textures/Pixel");
            _arrowTexture = content.Load<Texture2D>("Textures/TorchArrow");
            _finishTexture = content.Load<Texture2D>("Textures/Finish");
            _regularFont = content.Load<SpriteFont>("Fonts/Regular");

            _worldTarget = CreateRenderTarget();
            _waterTarget = CreateRenderTarget();

            _waterEffect.Parameters["WaterMaskSampler+WaterMask"].SetValue(_waterTarget);
            _waterEffect.Parameters["FlowMapSampler+FlowMap"].SetValue(_flowMapTexture);
            _waterEffect.Parameters["WaterColor"].SetValue(new Color(152, 163, 152).ToVector4());

            _arrowSprite = new Sprite(_arrowTexture) {
                Origin = _arrowTexture.Bounds.Center.ToVector2()
            };

            Entities.Particles = Particles;
        }

        public int TorchEntityID { get; set; }
        public int PlayerEntityID { get; set; }

        public EntitiesView Entities { get; }
        public ParticlesView Particles { get; }
        public TileMapView TileMap { get; }
        public BackgroundView Background { get; }

        public bool HasFadedOut => _fadeOutOpacity >= 1f;

        public void ShowLoseScreen() {
            _showLoseScreen = true;
        }

        public void FadeOut() {
            _fadeOut = true;
        }

        public void Update(Level level, float deltaTime) {
            _shaderTimer += deltaTime;

            Entities.Update(level, deltaTime);
            _waterView.Update(deltaTime);
            Particles.Update(deltaTime);

            if (_showLoseScreen) {
                _loseScreenOpacity += 0.3f * deltaTime;
                _loseScreenOpacity = MathHelper.Min(_loseScreenOpacity, 1f);
            }

            if (_fadeOut) {
                _fadeOutOpacity += 1f * deltaTime;
                _fadeOutOpacity = MathHelper.Min(_fadeOutOpacity, 1f);
            }

            _flickerTimer += deltaTime;
            if (_flickerTimer >= _flickerTime) {
                _flickerTimer -= _flickerTime;
                _lightRadius = 400f + (float)_random.NextDouble() * 20f;
            }
        }

        public void Draw(Level level) {
            if (RenderTargetsAreOutdated()) {
                UpdateRenderTargets();
            }

            _camera.Position = Vector2.Round(GraphicsConstants.PhysicsToView(level.CameraCenter) - _graphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);

            _renderTargetStack.Push(_worldTarget);
            {
                _graphicsDevice.Clear(Color.Transparent);
                Background.Draw(_camera);
                TileMap.Draw(level, _camera);

                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetTransformMatrix());

                int startX = (int)Math.Floor(_camera.Position.X / _finishTexture.Width);
                int endX = (int)Math.Floor((_camera.Position.X + _spriteBatch.GraphicsDevice.Viewport.Width) / _finishTexture.Width);

                float y = GraphicsConstants.PhysicsToView(level.FinishHeight);

                for (int x = startX; x <= endX; x++) {
                    _spriteBatch.Draw(_finishTexture, new Vector2(x * _finishTexture.Width, y - _finishTexture.Height / 2f), Color.White);
                }

                _spriteBatch.End();

                Particles.Draw(_camera);
                Entities.Draw(level, _camera);
            }
            _renderTargetStack.Pop();

            _renderTargetStack.Push(_waterTarget);
            {
                _graphicsDevice.Clear(Color.Transparent);
                _waterView.DrawMask(level, _camera);
            }
            _renderTargetStack.Pop();

            _waterEffect.Parameters["Time"].SetValue(_shaderTimer);

            Vector2 camera = _camera.Position / _graphicsDevice.Viewport.Bounds.Size.ToVector2();
            _waterEffect.Parameters["Camera"].SetValue(camera);

            _waterEffect.Parameters["Position"].SetValue(_camera.Position);
            _waterEffect.Parameters["Dimensions"].SetValue(_graphicsDevice.Viewport.Bounds.Size.ToVector2() + new Vector2(128f));

            if (level.EntityWorld.TryGetEntity(TorchEntityID, out Entity? te)) {
                if (!te.IsPutOut && level.PhysicsWorld.TryGetBody(te.BodyID, out Body? torchBody)) {
                    _waterEffect.Parameters["Light1"].SetValue(GraphicsConstants.PhysicsToView(torchBody.Position + torchBody.Bounds.Center));
                }
                else {
                    _waterEffect.Parameters["Light1"].SetValue(new Vector2(-1000f));
                }
            }

            _waterEffect.Parameters["Radius"].SetValue(_lightRadius);

            _spriteBatch.Begin(effect: _waterEffect);
            _spriteBatch.Draw(_worldTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _waterView.Draw(level, _camera);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (level.EntityWorld.TryGetEntity(TorchEntityID, out Entity? torchEntity)) {
                if (!torchEntity.IsPutOut && level.PhysicsWorld.TryGetBody(torchEntity.BodyID, out Body? torchBody)) {
                    Vector2 torchPosition = GraphicsConstants.PhysicsToView(torchBody.Position + torchBody.Bounds.Center);
                    Vector2 arrowPosition = torchPosition;

                    arrowPosition.X = MathHelper.Clamp(arrowPosition.X, _camera.Position.X + 32f, _camera.Position.X + _graphicsDevice.Viewport.Width - 32f);
                    arrowPosition.Y = MathHelper.Clamp(arrowPosition.Y, _camera.Position.Y + 32f, _camera.Position.Y + _graphicsDevice.Viewport.Height - 32f);

                    _arrowSprite.Rotation = (torchPosition - arrowPosition).GetAngle();

                    _arrowSprite.Draw(_spriteBatch, arrowPosition - _camera.Position);
                }
            }

            _spriteBatch.Draw(_pixelTexture, _graphicsDevice.Viewport.Bounds, Color.Black * 0.5f * _loseScreenOpacity);

            _spriteBatch.DrawString(_regularFont, "Press R to restart.",
                _graphicsDevice.Viewport.Bounds.Center.ToVector2() - _regularFont.MeasureString("Press R to restart.") / 2f, Color.White * _loseScreenOpacity);

            _spriteBatch.Draw(_pixelTexture, _graphicsDevice.Viewport.Bounds, Color.Black * _fadeOutOpacity);

            _spriteBatch.End();
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
