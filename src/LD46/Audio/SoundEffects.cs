using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;

namespace LD46.Audio {
    public class SoundEffects {
        private readonly Random _random = new Random();

        public SoundEffects(ContentManager content) {
            Click1 = content.Load<SoundEffect>("Sounds/Click1");
            Click2 = content.Load<SoundEffect>("Sounds/Click2");
            Click3 = content.Load<SoundEffect>("Sounds/Click3");

            Thud1 = content.Load<SoundEffect>("Sounds/Thud1");
            Thud2 = content.Load<SoundEffect>("Sounds/Thud2");
            Thud3 = content.Load<SoundEffect>("Sounds/Thud3");
        }

        public SoundEffect Click1 { get; }
        public SoundEffect Click2 { get; }
        public SoundEffect Click3 { get; }

        public SoundEffect Thud1 { get; }
        public SoundEffect Thud2 { get; }
        public SoundEffect Thud3 { get; }

        public SoundEffect GetRandom(params SoundEffect[] soundEffects) {
            return soundEffects[_random.Next(soundEffects.Length)];
        }
    }
}
