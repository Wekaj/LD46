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

        public bool CanRotate { get; set; }
        public float Rotation { get; set; }

        public bool Kick { get; set; } = false;

        public float WaterTimer { get; set; } = 0f;
        public bool IsPutOut { get; set; } = false;

        public bool HasLostAllHope { get; set; } = false;

        public float KickTimer { get; set; } = 0f;
        public float DashTimer { get; set; } = 0f;

        public bool IsBlowable { get; set; } = false;
        public bool Sleeping { get; set; } = false;

        public bool PlayedHiss { get; set; } = false;
    }
}
