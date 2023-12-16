namespace _0hh1Search;

internal class Program {
    static string filepath = "C:\\Users\\david\\source\\repos\\zaddmc\\Qlearning\\0hh1Solves\\smolData.csv";
    static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        StreamReader sr = new StreamReader(filepath);
        List<string> items = new List<string>();

        while (!sr.EndOfStream) {
            string[] item = sr.ReadLine().Split('|');
            items.Add(item[1]);
        }

        //Similarities(items);
        //SaveUniques(items);

        var temp = HashTest(items);
        SaveHash(temp);
    }
    static void SaveUniques(List<string> items) {

        for (int i = 0; i < items.Count; i++) {
            StreamWriter sw = new StreamWriter(filepath.Replace(".csv", "Uniques.csv"), true);
            sw.WriteLine(items[i]);
            sw.Close();
        }
    }
    static void Similarities(List<string> items) {
        int[] results = new int[items.Count];
        for (int i = 0; i < items.Count; i++) {
            for (int j = 0; j < items.Count; j++) {
                if (i == j) continue;
                if (items[i] == items[j]) {
                    results[i]++;
                    items.RemoveAt(j);
                }
            }
        }

        Console.WriteLine("Reuslts:");

        for (int i = 0; i < results.Length; i++) {
            if (results[i] > 0) Console.WriteLine($"Index {i} has {results[i]} similarities");
        }

    }
    static HashSet<string> HashTest(List<string> items) {
        HashSet<string> keys = new HashSet<string>();
        while (items.Count > 0) {
            try {
                keys.Add(items[0]);
                items.RemoveAt(0);
            }
            catch (Exception) {
                items.RemoveAt(0);
            }
        }
        return keys;
    }
    static void SaveHash(HashSet<string> keys) {
        List<string> items = new List<string>();
        items = keys.ToList();
        for (int i = 0; i < items.Count; i++) {
            StreamWriter sw = new StreamWriter(filepath.Replace(".csv", "Uniques.csv"), true);
            sw.WriteLine(items[i]);
            sw.Close();
        }

    }
}