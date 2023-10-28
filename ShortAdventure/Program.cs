using System;

namespace ShortAdventure {
    internal class Program {
        static void Main(string[] args) {
            Game.Intialize(10, 20);

            Start();

        }
        public static void Start() {
            bool isRunning = true;
            while (isRunning) {
                GetDirectionAndGo();
                Console.WriteLine(Game.PlayerPos.ToString());


            }
        }
        private static void GetDirectionAndGo() {
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key) {
                case ConsoleKey.W:
                    Movement.Move(Movement.MoveDirection.north); break;
                case ConsoleKey.D:
                    Movement.Move(Movement.MoveDirection.east); break;
                case ConsoleKey.S:
                    Movement.Move(Movement.MoveDirection.south); break;
                case ConsoleKey.A:
                    Movement.Move(Movement.MoveDirection.west); break;
            }

        }
    }
}