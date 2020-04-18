using Floppy.Input;
using Floppy.Physics;
using LD46.Input;
using LD46.Levels;
using Microsoft.Xna.Framework;

namespace LD46.Entities {
    public class PlayerBrain : IBrain {
        private const float _movementForce = 200f;
        private const float _jumpImpulse = 200f;

        private readonly InputBindings _bindings;
        
        public PlayerBrain(InputBindings bindings) {
            _bindings = bindings;
        }

        public void Update(Entity entity, Level level, float deltaTime) {
            if (!level.PhysicsWorld.TryGetBody(entity.BodyID, out Body? body)) {
                return;
            }

            if (_bindings.IsPressed(Bindings.MoveRight)) {
                body.Force += new Vector2(_movementForce, 0f);
            }
            if (_bindings.IsPressed(Bindings.MoveLeft)) {
                body.Force -= new Vector2(_movementForce, 0f);
            }

            if (_bindings.JustPressed(Bindings.Jump)) {
                body.Impulse -= new Vector2(0f, _jumpImpulse);
            }
        }
    }
}
