namespace LD46.Entities {
    public enum EntityAnimations {
        Player,
        Torch,
    }

    public class Entity {
        public Entity(int id) {
            ID = id;
        }

        public int ID { get; }

        public int BodyID { get; set; }

        public IBrain? Brain { get; set; }

        public float GroundFriction { get; set; }
        public float AirFriction { get; set; }

        public float DangerSpeed { get; set; } = float.MaxValue;
        public float DangerFriction { get; set; }

        public EntityAnimations Animations { get; set; }

        public bool CanRotate { get; set; }
        public float Rotation { get; set; }
    }
}
