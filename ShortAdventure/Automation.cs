using System;

namespace ShortAdventure {
    public class Automation {
        public static void RandomPlay() {
            bool isRunning = true;
            int movesLine = 0;
            while (isRunning) {
                MakeRandomMove();
                Draw.NextFrame();
                Console.WriteLine(Position.Parse(Game.PlayerPos.ToString()) + "          ");
                if (Game.ReachedGoal()) {
                    Console.Write("You reached the goal, woohoo");
                    //isRunning = false;
                    movesLine++;
                    Game.ResetMoves();
                }
                Console.WriteLine();
                for (int i = 0; i < movesLine; i++) {
                    Console.WriteLine();
                }
                Console.WriteLine($"Has made: {Game.Moves}    ");


            }
        }
        private static bool MakeRandomMove() {
            Random rnd = new Random();

            switch (rnd.Next(4)) {
                case 0: return Movement.Move(Movement.MoveDirection.north);
                case 1: return Movement.Move(Movement.MoveDirection.east);
                case 2: return Movement.Move(Movement.MoveDirection.south);
                case 3: return Movement.Move(Movement.MoveDirection.west);
                default: return false;
            }
        }
    }

    public class QLearning {
        public static Movement.MoveDirection NextStep(float[] input, BotBrain bot) {
            float[] output = bot.EvaluateStep(input);
            int maxIndex = 0;
            float maxValue = output[0];
            for (int i = 0; i < output.Length; i++) {
                if (output[i] > maxValue) {
                    maxIndex = i;
                    maxValue = output[i];
                }
            }
            if (maxIndex >= 4) throw new ArgumentException("not valid");
            return (Movement.MoveDirection)maxValue;
        }





    }
}
