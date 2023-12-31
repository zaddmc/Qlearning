﻿using System;

namespace ShortAdventure {
    internal class Program {
        static void Main(string[] args) {
            Game.Intialize(10, 20);

            Console.CursorVisible = false;

            //Start();
            Automation.RandomPlay();
        }
        public static void Start() {
            Draw.NextFrame();
            bool isRunning = true;
            while (isRunning) {
                GetDirectionAndMove();
                Draw.NextFrame();
                Console.WriteLine(Position.Parse(Game.PlayerPos.ToString()) + "          ");
                if (Game.ReachedGoal()) {
                    Console.Write("You reached the goal, woohoo");
                    isRunning = false;
                }
                Console.WriteLine();



            }
        }
        private static void GetDirectionAndMove() {
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
                case ConsoleKey.Q:
                    Game.AddObstruction(GetPositionFromConsole("obstruction"));
                    break;
            }

        }
        private static Position GetPositionFromConsole(string type) {
            while (true) {

                Console.WriteLine($"write x and y cordinates to add {type}: ");
                string consoleOut = Console.ReadLine();

                if (consoleOut == null) {
                    Console.WriteLine("bad input");
                    continue;
                }
                string[]? ss = null;
                if (consoleOut.Contains(' ') && consoleOut.Contains(',')) {
                    consoleOut = consoleOut.Replace(" ", "");
                    ss = consoleOut.Split(",");
                }
                if (consoleOut.Contains(' ')) {
                    ss = consoleOut.Split(" ");
                }
                if (consoleOut.Contains(',')) {
                    ss = consoleOut.Split(",");
                }
                if (ss == null) continue;
                return new Position(int.Parse(ss[0]), int.Parse(ss[1]));
            }
        }
    }
    public class Draw {
        static public void NextFrame() {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < Game.MapSize.X; i++) {
                for (int j = 0; j < Game.MapSize.Y; j++) {
                    Console.Write(GetVisualBlock(i, j));
                }
                Console.WriteLine();
            }

        }
        private static string GetVisualBlock(int x, int y) {
            switch (Movement.GetPosTile(new(x, y))) {
                case TileType.empty: return " ";
                case TileType.player: return "@";
                case TileType.goal: return "?";
                case TileType.obstruction: return "█";
                default: return " ";
            }
        }
    }
}