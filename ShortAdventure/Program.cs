using System;

namespace ShortAdventure {
    internal class Program {
        static void Main(string[] args) {
            Game.Intialize(10, 20);

                Console.WriteLine(Game.PlayerPos.ToString());
            Start();

        }
        public static void Start() {
            bool isRunning = true;
            while (isRunning) {
                GetDirectionAndGo();
                Console.Write(Game.PlayerPos.ToString());
                if (Game.ReachedGoal()) Console.Write(" You reached the goal, woohoo");
                Console.WriteLine();
            }
        }
        private static void GetDirectionAndGo() {
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key) {
                case ConsoleKey.W or ConsoleKey.UpArrow:
                    Movement.Move(Movement.MoveDirection.north); break;
                case ConsoleKey.D or ConsoleKey.RightArrow:
                    Movement.Move(Movement.MoveDirection.east); break;
                case ConsoleKey.S or ConsoleKey.DownArrow:
                    Movement.Move(Movement.MoveDirection.south); break;
                case ConsoleKey.A or ConsoleKey.LeftArrow:
                    Movement.Move(Movement.MoveDirection.west); break;
            }

        }
    }
}