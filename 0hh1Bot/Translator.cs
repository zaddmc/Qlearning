namespace _0hh1Bot {
    public class Translator {
        static BotBrain Bot { get; set; }
        static int[] layers = { 144, 8, 8, 144 };
        public static void MakeBot() {
            Bot = new BotBrain(layers);


        } // MakeBot
        public static float[] EvaluateStep(Tile[][] inputTiles) {

            float[] inputFloats = new float[Program.gridSize * Program.gridSize];
            for (int i = 0; i < Program.gridSize; i++) {
                for (int j = 0; j < Program.gridSize; j++) {
                    inputFloats[Program.gridSize * i + j] = TileToFloat(inputTiles[i][j]);
                }
            }

            return Bot.EvaluateStep(inputFloats);
        } // EvaluateStep
        static float TileToFloat(Tile tile) {
            switch (tile.TileState) {
                case TileState.empty: return 0f;
                case TileState.yellow: return 1f;
                case TileState.blue: return -1f;
                default: return 0f;
            }
        } // TileToFloat
    }
}
