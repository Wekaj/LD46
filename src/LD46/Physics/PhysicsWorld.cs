using Floppy.Physics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LD46.Physics {
    public class PhysicsWorld {
        private readonly Dictionary<int, Body> _bodies = new Dictionary<int, Body>();

        private int _nextBodyID = 1;

        public IEnumerable<Body> Bodies => _bodies.Values;

        public Body CreateBody() {
            var body = new Body(_nextBodyID++);

            _bodies.Add(body.ID, body);

            return body;
        }

        public bool TryGetBody(int id, [NotNullWhen(true)] out Body? body) {
            return _bodies.TryGetValue(id, out body);
        }
    }
}
