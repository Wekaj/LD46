using Floppy.Extensions;
using Floppy.Input;
using Floppy.Physics;
using LD46.Input;
using LD46.Levels;
using Microsoft.Xna.Framework;

namespace LD46.Entities {
    public class PlayerBrain : IBrain {
        private const float _movementForce = 200f;
        private const float _jumpImpulse = 200f;
        private const float _kickImpulse = 300f;
        private const float _kickHMultiplier = 5f;
        private const float _kickDistance = 20f;

        private readonly InputBindings _bindings;

        private readonly int _torchEntityID;

        public PlayerBrain(InputBindings bindings, int torchEntityID) {
            _bindings = bindings;

            _torchEntityID = torchEntityID;
        }

        public void Update(Entity entity, Level level, float deltaTime) {
            if (!level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                return;
            }

            if (_bindings.IsPressed(Bindings.MoveRight)) {
                body.Force += new Vector2(_movementForce, 0f);
            }
            if (_bindings.IsPressed(Bindings.MoveLeft)) {
                body.Force += new Vector2(-_movementForce, 0f);
            }

            if (_bindings.JustPressed(Bindings.Jump)) {
                body.Impulse += new Vector2(0f, -_jumpImpulse);
            }

            if (_bindings.JustPressed(Bindings.Kick)) {
                KickTorch(body, level);
            }
        }

        private void KickTorch(Body body, Level level) {
            if (!level.EntityWorld.TryGetEntity(_torchEntityID, out Entity? torchEntity)) {
                return;
            }

            if (!level.PhysicsWorld.TryGetBody(torchEntity.BodyID, out Body? torchBody)) {
                return;
            }

            float distance = Vector2.Distance(body.Position + body.Bounds.Center, torchBody.Position + torchBody.Bounds.Center);

            if (distance <= _kickDistance) {
                torchBody.Velocity = torchBody.Velocity.SetY(0f);

                var impulse = new Vector2((torchBody.Position.X - body.Position.X) * _kickHMultiplier, -_kickImpulse);

                torchBody.Impulse += impulse;
            }
        }
    }
}
