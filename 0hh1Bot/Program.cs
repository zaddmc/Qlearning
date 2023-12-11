namespace _0hh1Bot {
    internal class Program {
        public static Tile[][]? SeedTiles { get; set; }
        public static Tile[][]? TrueTiles { get; set; }

        static string file = "smalltest.csv";
        public static int gridSize; // the decompiler sets this value
        static void Main(string[] args) {

            BackPropProgram.Start(args);
            Console.ReadLine();

            return; // the rest is my own shit code and therfore useless

            Translator.MakeBot();

            bool isrunning = true;
            while (isrunning) {
                if (!Nextset()) break; // bit weird, nextset returns a bool for if it was succesful, so if its true nothing matters it can continue, but if false it will break, and that being called from a if statement, which is why its a bit weird

                EvaluateStep();
                Console.WriteLine(lineIndex + Compare().ToString());
            }

        }
        static float EvaluateStep() {
            float[] output = Translator.EvaluateStep(SeedTiles);

            //smoothing the output 
            for (int i = 0; i < output.Length; i++) {
                switch (output[i]) {
                    case var _ when output[i] < 0:
                        output[i] = -1f; break;
                    case var _ when output[i] > 0:
                        output[i] = 1f; break;
                    default:
                        output[i] = 0f; break;
                }
            }




            return 0f;
        }
        static bool Compare() {

            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    if (SeedTiles[i][j].TileState != TrueTiles[i][j].TileState) return false;
                }
            }

            return true;
        } // Compare
        public static string GetFilePath() {
            string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string[] strings = exeFilePath.Split("\\");

            string newFilePath = "";
            for (int i = 0; i < strings.Length - 5; i++) {
                newFilePath += strings[i] + "\\";
            }
            newFilePath += "0hh1Solves\\";
            newFilePath += file; // static string, may be dangerous
            return newFilePath;
        }
        static StreamReader sr;
        static int lineIndex = 0;
        static bool Nextset() {
            if (lineIndex == 0) sr = new StreamReader(GetFilePath());
            if (sr.EndOfStream) return false;

            string[] temp = sr.ReadLine().Split('|'); // should only return 2 strings, the first is the seed the second is the answer

            Decompiler(temp[0], "seed");
            Decompiler(temp[1], "truth");


            lineIndex++;
            return true;
        }
        static void Decompiler(string input, string set) {
            Tile[][]? tiles = null;
            string[] inputArr = input.Split(",");
            if (gridSize == 0) gridSize = (int)Math.Sqrt(inputArr.Length);

            tiles = new Tile[gridSize][];
            for (int i = 0; i < gridSize; i++) {
                tiles[i] = new Tile[gridSize];
                for (int j = 0; j < gridSize; j++) {
                    tiles[i][j] = new(inputArr[gridSize * i + j]);
                }
            }

            switch (set) { // have to do it this way do to it being dumb
                case "seed": SeedTiles = tiles; break;
                case "truth": TrueTiles = tiles; break;
                default: break;
            }
        } // Decompiler
    } // Program
    public class Tile {
        public bool IsLocked { get; private set; }
        public TileState TileState { get; set; }
        public Tile(string tileState) {
            switch (tileState) {
                case "yellow":
                    TileState = TileState.yellow;
                    IsLocked = true;
                    break;
                case "blue":
                    TileState = TileState.blue;
                    IsLocked = true;
                    break;
                default:
                    TileState = TileState.empty;
                    IsLocked = false;
                    break;
            }
        }
        public Tile(TileState tileState) {
            TileState = tileState;
            if (tileState == TileState.empty) IsLocked = false;
            else IsLocked = true;

        }
    }
    public enum TileState {
        empty,
        yellow,
        blue = -1,
    }
}