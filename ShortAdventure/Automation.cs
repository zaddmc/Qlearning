using System;

namespace ShortAdventure {
    public class Automation {
        public static void RandomPlay() {
            bool isRunning = true;
            while (isRunning) {
                MakeRandomMove();
                Draw.NextFrame();
                Console.WriteLine(Position.Parse(Game.PlayerPos.ToString()) + "          ");
                if (Game.ReachedGoal()) {
                    Console.Write("You reached the goal, woohoo");
                    isRunning = false;
                }
                Console.WriteLine();


            }
        }
        private static void MakeRandomMove() {
            Random rnd = new Random();

            switch (rnd.Next(4)) {
                case 0:
                    Movement.Move(Movement.MoveDirection.north);
                    break;
                case 1:
                    Movement.Move(Movement.MoveDirection.east);
                    break;
                case 2:
                    Movement.Move(Movement.MoveDirection.south);
                    break;
                case 3:
                    Movement.Move(Movement.MoveDirection.west);
                    break;
                default:
                    break;
            }
        }
    }
}
