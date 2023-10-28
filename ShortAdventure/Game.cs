using System.Collections.Generic;

namespace ShortAdventure {
    public class Game {
        public static Dictionary<string, TileType> PositionMap { get; private set; }
        public static Position MapSize { get; private set; }
        public static Position PlayerPos { get; private set; }
        public static Position GoalPos { get; private set; }
        public static void Intialize(int width, int height) {
            MapSize = new Position(width, height);
            PlayerPos = new Position(1, 1);
            GoalPos = new Position(MapSize.X - 2, MapSize.Y - 2);
            MakeMap();


        }
        private static void MakeMap() {
            PositionMap = new Dictionary<string, TileType>();
            for (int i = 0; i < MapSize.X; i++) {
                for (int j = 0; j < MapSize.Y; j++) {
                    TileType toBeAdded = TileType.empty;
                    if (i == PlayerPos.X && j == PlayerPos.Y) toBeAdded = TileType.player;
                    if (i == GoalPos.X && j == GoalPos.Y) toBeAdded = TileType.goal;
                    PositionMap.Add(new Position(i, j).ToString(), toBeAdded);
                }
            }
        }



        // not ment to be called from this class but from the "Movement" class
        public static void MovePlayer(Position position) {
            PositionMap.Remove(position.ToString());
            PositionMap.Remove(PlayerPos.ToString());
            PositionMap.Add(PlayerPos.ToString(), TileType.empty);
            PositionMap.Add(position.ToString(), TileType.player);

            PlayerPos = position; 
        }
        public static bool ReachedGoal() {
            if (PlayerPos.ToString() == GoalPos.ToString()) return true;
            return false;
        }

        
    }
    public class Movement {
        public static bool Move(MoveDirection direction) {
            Position newPos = new(Game.PlayerPos);

            switch (direction) {
                case MoveDirection.north:
                    newPos.X -= 1;
                    if (newPos.X < 1) return false;
                    break;
                case MoveDirection.east:
                    newPos.Y += 1;
                    if (newPos.Y > Game.MapSize.Y - 2) return false;
                    break;
                case MoveDirection.south:
                    newPos.X += 1;
                    if (newPos.X > Game.MapSize.X - 2) return false;
                    break;
                case MoveDirection.west:
                    newPos.Y -= 1;
                    if (newPos.Y < 1) return false;
                    break;
                default:
                    return false;
            }
            if (GetPosTile(newPos) == TileType.obstruction) return false;
            Game.MovePlayer(newPos);
            return true;

        }
        public static TileType GetPosTile(Position pos) {
            return Game.PositionMap[pos.ToString()];
        }

        public enum MoveDirection {
            north,
            east,
            south,
            west,
        }
    }
    public enum TileType {
        empty,
        player,
        goal,
        obstruction,
    }
    public class Position {
        /// <summary>
        ///  X aka width
        ///  Y aka height
        /// </summary>
        /**
         * @return fish
         * 
         */

        public int X; // aka width
        public int Y; // aka height
        public Position(int x, int y) {
            /**
             * @return fish
             * 
             */
            X = x;
            Y = y;
        }
        public Position(Position position) {
            X = position.X;
            Y = position.Y;
        }
        public override string ToString() {
            return string.Format($"x:{X}|y:{Y}");
        }
        public static Position Parse(string s) {
            string[] ss = s.Split("|");
            string[] sx = ss[0].Split(":");
            string[] sy = ss[1].Split(":");

            return new Position(int.Parse(sx[1]), int.Parse(sy[1]));
        }
    }

}
