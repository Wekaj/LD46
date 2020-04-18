using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LD46.Entities {
    public class EntityWorld {
        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();

        private int _nextEntityID = 1;

        public IEnumerable<Entity> Entities => _entities.Values;

        public Entity CreateEntity() {
            var entity = new Entity(_nextEntityID++);

            _entities.Add(entity.ID, entity);

            return entity;
        }

        public bool TryGetEntity(int id, [NotNullWhen(true)] out Entity? entity) {
            return _entities.TryGetValue(id, out entity);
        }
    }
}
