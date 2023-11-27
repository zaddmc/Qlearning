namespace _0hh1Bot {
    public class Translator {
        static BotBrain Bot { get; set; }
        static int[] layers = { 144, 8, 8, 144 };
        public static void MakeBot() {
            Bot = new BotBrain(layers);


        }
        public static Tile[][] EvaluateStep(Tile[][] inputTiles) {

            float[] inputFloats = new float[Program.gridSize * Program.gridSize];
            for (int i = 0; i < Program.gridSize; i++) {
                for (int j = 0; j < Program.gridSize; j++) {
                    inputFloats[Program.gridSize * i + j] = TileToFloat(inputTiles[i][j]);
                }
            }

            float[] outputFloats = Bot.EvaluateStep(inputFloats);

            Tile[][] outputTiles = new Tile[Program.gridSize][];
            for (int i = 0; i < Program.gridSize; i++) {
                outputTiles[i] = new Tile[Program.gridSize];
                for (int j = 0; j < Program.gridSize; j++) {
                    outputTiles[i][j] = new Tile((TileState)outputFloats[Program.gridSize * i + j]);
                }
            }
            return outputTiles;
        }
        static float TileToFloat(Tile tile) {
            switch (tile.TileState) {
                case TileState.empty: return 0f;
                case TileState.yellow: return 1f;
                case TileState.blue: return -1f;
                default: return 0f;
            }
        }
    }
}
