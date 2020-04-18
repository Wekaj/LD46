namespace LD46.Entities {
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
    }
}
