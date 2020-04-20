namespace LD46.Levels {
    public class LevelSettings {
        public int Height { get; set; } = 64;
        public float SolidChance { get; set; } = 1f / 6f;
        public int MinPlatformWidth { get; set; } = 2;
        public int MaxPlatformWidth { get; set; } = 7;
        public LevelSettings? NextLevel { get; set; }
        public int TextureOffset { get; set; }
        public int BackgroundTile { get; set; } = 0;
        public float WaterSpeed { get; set; } = 1f;
        public bool HasWind { get; set; } = false;
        public bool HasGrates { get; set; } = false;
        public bool IsOpening { get; set; } = false;
        public bool IsVictory { get; set; } = false;
        public string? Name { get; set; }
    }
}
