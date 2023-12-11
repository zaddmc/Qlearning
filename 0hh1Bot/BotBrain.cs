namespace _0hh1Bot {
    public class BotBrain {
        private int[]? layers;
        private float[][]? neurons;
        private float[][]? biases;
        private float[][][]? weights;

        private float? learningRate;

        static string FilePath { get; set; }

        public BotBrain(int[] layers, float learningRate = 0.001f) { // constroctur for new brain
            this.layers = layers;
            this.learningRate = learningRate;

            Random rnd = new Random();

            // allocates all needed memory space for neurons and intializes the value
            neurons = new float[layers.Length][];
            for (int i = 0; i < layers.Length; i++) {
                neurons[i] = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                    neurons[i][j] = 0;
            }

            // allocates all needed memory space for biases and intializes the value
            biases = new float[layers.Length - 1][];
            for (int i = 0; i < biases.Length; i++) {
                biases[i] = new float[layers[i]];
                for (int j = 0; j < biases[i].Length; j++)
                    biases[i][j] = (rnd.NextSingle());
            }

            // allocates all needed memory space for weights and intializes the value
            weights = new float[layers.Length - 1][][];
            for (int i = 0; i < weights.Length; i++) {
                weights[i] = new float[layers[i]][];
                for (int j = 0; j < weights[i].Length; j++) {
                    weights[i][j] = new float[layers[i]];
                    for (int k = 0; k < weights[i][j].Length; k++)
                        weights[i][j][k] = (rnd.NextSingle());
                }
            }
        } // constructor with starting values
        public BotBrain(string filePath, float learningRate = 0.001f) { // constroctur for old brain
            this.learningRate = learningRate;
            FilePath = filePath;

            StreamReader reader = new StreamReader(filePath);

            // check if missing values
            if (reader.ReadLine() != "layers") throw new ArgumentException("missing layers tag");
            // intializing layers
            string[] layersRead = reader.ReadLine().Split(",");
            layers = new int[layersRead.Length];
            for (int i = 0; i < layersRead.Length; i++)
                layers[i] = int.Parse(layersRead[i]);


            // allocates all needed memory space for neurons and intializes the value
            neurons = new float[layers.Length][];
            for (int i = 0; i < layers.Length; i++) {
                neurons[i] = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                    neurons[i][j] = 0;
            }

            // allocates all needed memory space for biases and intializes the value
            if (reader.ReadLine() != "biases") throw new ArgumentException("missing biases tag");
            biases = new float[layers.Length][];
            for (int i = 0; i < layers.Length; i++) {
                biases[i] = new float[layers[i]];
                string[] biasRead = reader.ReadLine().Split("|");
                for (int j = 0; j < layers[i]; j++)
                    biases[i][j] = float.Parse(biasRead[j]);
            }

            // allocates all needed memory space for weights and intializes the value
            if (reader.ReadLine() != "weights") throw new ArgumentException("missing weights tag");
            weights = new float[layers.Length - 1][][];
            for (int i = 0; i < weights.Length; i++) {
                weights[i] = new float[layers[i + 1]][];
                for (int j = 0; j < layers[i + 1]; j++) {
                    weights[i][j] = new float[layers[i]];
                    string[] weightsRead = reader.ReadLine().Split("|");
                    for (int k = 0; k < weights[i][j].Length; k++)
                        weights[i][j][k] = float.Parse(weightsRead[k]);
                }
            }
        } // constructor with starting values
        public void Save() {

        }
        public float[] EvaluateStep(float[] input) {
            // quickly check if there is any values in the system
            if (neurons == null || biases == null || weights == null) throw new ArgumentNullException("not properly intiated");

            neurons[0] = input;

            for (int i = 0; i < neurons.Length; i++)
                for (int j = 0; j < neurons[i].Length - 1; j++) {
                    float value = biases[i][j];
                    for (int k = 0; k < neurons[i + 1].Length; k++) {
                        value += weights[i][j][k] * neurons[i][j];

                    }
                    neurons[i][j] = Sigmoid(value);
                }

            return neurons[^1];
        }
        private float Sigmoid(float x) {
            return 1 / (1 + MathF.Pow(MathF.E, -x));
        }
        private float SigmoidDerivative(float x) {
            return Sigmoid(x) * (1 - Sigmoid(x));
        }
        public static void SetFilePath(string filePath) {
            FilePath = filePath;
        }
    }
}
