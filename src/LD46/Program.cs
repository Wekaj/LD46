using System;

namespace LD46 {
    public static class Program {
        [STAThread]
        static void Main() {
            using var game = new LD46Game();

            game.Run();
        }
    }
}
