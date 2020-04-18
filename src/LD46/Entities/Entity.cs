namespace LD46.Entities {
    public class Entity {
        public Entity(int id) {
            ID = id;
        }

        public int ID { get; }

        public int BodyID { get; set; }

        public IBrain? Brain { get; set; }
    }
}
