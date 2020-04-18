using LD46.Levels;

namespace LD46.Entities {
    public interface IBrain {
        void Update(Entity entity, Level level, float deltaTime);
    }
}
