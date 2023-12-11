using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace _0hh1 {
    internal class Program {
        public static int gridSize = 4;
        public static string fileName = "smolData.csv";

        public static Point upperLeft = new Point(341, 451); //upper left of the game screen. in the middle
        public static Point lowerRight = new Point(955, 1076);

        public static TileInfo[][]? Tiles;


        static void Main(string[] args) {
            string filePath = GetFilePath();

            WebDriver webDriver = new ChromeDriver();
            bool endless = true;

            webDriver.Navigate().GoToUrl("https://0hh1.com/");

            int runs = 0;
            bool isRunning = true;
            while (isRunning) {
                webDriver.ExecuteScript($"Game.startGame({gridSize},0)");

                Thread.Sleep(1000);
                Console.WriteLine($"[{DateTime.Now}] has begun reading");
                //ReadElements(webDriver);
                ReadScreen();
                string seed = TilesToString();

                DateTime beforeAlgo = DateTime.Now;
                Console.WriteLine($"[{DateTime.Now}] algoritmim has begun");
                Algorithim();
                Console.WriteLine($"[{DateTime.Now}] time diff: {(DateTime.Now - beforeAlgo).Milliseconds} miliseconds, algoritmim has finished");


                //ClickResult(webDriver);
                SaveResult(filePath, seed);


                isRunning = endless;
                if (endless) { // if you want to manually begin it
                    if (runs == 1000) isRunning = false;
                    
                    Console.WriteLine("Press and key other than q to continue");
                    if (Console.KeyAvailable) {
                        ConsoleKey key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Q) isRunning = false;
                    }
                }

                //Thread.Sleep(2000);
                runs++;
            }

            webDriver.Quit();
        }
        static string TilesToString() {
            string toBeSaved = "";
            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    toBeSaved += Tiles[i][j].TileState.ToString() + ",";
                }
            }
            return toBeSaved;
        }
        static void SaveResult(string filePath, string seed) {
            StreamWriter sr = new StreamWriter(filePath + fileName, true);

            string result = seed + "|" + TilesToString();

            sr.WriteLine(result);
            sr.Close();
        }
        static string GetFilePath() {
            string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string[] strings = exeFilePath.Split("\\");

            string newFilePath = "";
            for (int i = 0; i < strings.Length - 5; i++) {
                newFilePath += strings[i] + "\\";
            }
            newFilePath += "0hh1Solves\\";
            return newFilePath;
        }
        static void Algorithim() {
            if (Tiles == null) throw new ArgumentNullException("faggot");
            bool isRunning = true;
            while (isRunning) {
                TwoInSuccesionController();
                SingleSpaceController();
                LineCountController();
                CompareLinesController();

                if (IsDone()) isRunning = false;
            }
        }
        static void ReadScreen() {
            int unitLength = (int)((lowerRight.X - upperLeft.X) / (gridSize - 1)); //unit length is the distance between each box.

            Size size = new(lowerRight.X - upperLeft.X, lowerRight.Y - upperLeft.Y + 1);
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(upperLeft, new(0, 0), size);

            Tiles = new TileInfo[gridSize][];
            for (int i = 0; i < gridSize; i++) {
                Tiles[i] = new TileInfo[gridSize];
                for (int j = 0; j < gridSize; j++) {
                    Point tempPoint = new Point(upperLeft.X + unitLength * i, upperLeft.Y + unitLength * j);
                    Color color = bmp.GetPixel(tempPoint.X - upperLeft.X, tempPoint.Y - upperLeft.Y);
                    if (color.B >= 180) Tiles[i][j] = new(TileState.blue, tempPoint, i, j);
                    else if (color.G >= 200) Tiles[i][j] = new(TileState.yellow, tempPoint, i, j);
                    else Tiles[i][j] = new(TileState.empty, tempPoint, i, j);
                }
            }
        }
        static Point Scalluing(Point point) {
            float xRes = 1920, yRes = 1080;

            Point newPoint = new Point();

            newPoint.X = (int)((point.X / xRes) * 65535);
            newPoint.Y = (int)((point.Y / yRes) * 65535);

            return newPoint;
        }//Converter for simulating input screen size and pixel to use for the mouse ONLY!!!!!

        static bool CompareLinesController() {
            bool returnState = false;

            bool[] bools = new bool[2];
            for (int i = 0; i < gridSize - 1; i++) {
                for (int j = i + 1; j < gridSize; j++) {
                    bools[0] = CompareTwoLines(Tiles[i], Tiles[j]);
                    bools[1] = CompareTwoLines(Oreientationconverter(Tiles, i), Oreientationconverter(Tiles, j));
                }
            }
            if (bools[0] == true || bools[1] == true) returnState = true;

            return returnState;
        }
        static bool CompareTwoLines(TileInfo[] tiles1, TileInfo[] tiles2) {
            int checkCount = gridSize / 2 - 1; // the count that should be in every line minus one to make a change
            var set1 = CountLine(tiles1);
            var set2 = CountLine(tiles2);
            if (set1.isFinished == false && set2.isFinished == false) return false;
            if (set1.isFinished == true && set2.isFinished == true) return false;
            if (set1.isFinished && set2.blueCount == checkCount && set2.yellowCount == checkCount)
                return LineWorker(tiles1, tiles2);
            if (set2.isFinished && set1.blueCount == checkCount && set1.yellowCount == checkCount)
                return LineWorker(tiles2, tiles1);

            return false;
        }
        static bool LineWorker(TileInfo[] finished, TileInfo[] bad) {
            int diffs = 0;
            for (int i = 0; i < gridSize; i++) {
                if (finished[i].TileState != bad[i].TileState) diffs++;
            }
            if (diffs != 2) return false;

            for (int i = 0; i < gridSize; i++) {
                if (bad[i].TileState == TileState.empty) {
                    switch (finished[i].TileState) {
                        case TileState.yellow: bad[i].TileState = TileState.blue; break;
                        case TileState.blue: bad[i].TileState = TileState.yellow; break;
                        default: break;
                    }
                }
            }
            return true;
        }
        static (bool isFinished, int blueCount, int yellowCount) CountLine(TileInfo[] tiles) {
            int blueCount = 0, yellowCount = 0;
            for (int i = 0; i < tiles.Length; i++) {
                switch (tiles[i].TileState) {
                    case TileState.yellow: yellowCount++; break;
                    case TileState.blue: blueCount++; break;
                    default: break;
                }
            }
            bool isFinished = false;
            if (blueCount + yellowCount == gridSize) isFinished = true;
            return (isFinished, blueCount, yellowCount);
        }
        static bool LineCountController() {
            bool returnState = false;
            bool[] bools = new bool[2];
            for (int i = 0; i < gridSize; i++) {
                bools[0] = LineCountFinish(Oreientationconverter(Tiles, i));
                bools[1] = LineCountFinish(Tiles[i]);
            }
            if (bools[0] == true || bools[1] == true) returnState = true;
            return returnState;
        }
        static bool LineCountFinish(TileInfo[] tiles) {
            var (isFinished, blueCount, yellowCount) = CountLine(tiles);
            // check if line is finished
            if (isFinished) return false;
            // check if single color can finish
            if (blueCount == gridSize / 2 || yellowCount == gridSize / 2) {
                TileState filler = TileState.blue;
                if (blueCount == gridSize / 2) filler = TileState.yellow;
                for (int i = 0; i < gridSize; i++) {
                    if (tiles[i].TileState == TileState.empty)
                        tiles[i].TileState = filler;
                }
                return true;
            }
            return false;
        }
        static bool SingleSpaceController() {
            bool returnState = false;
            bool[] bools = new bool[2];
            for (int i = 0; i < gridSize; i++) {
                bools[0] = SingleSpaceFiller(Oreientationconverter(Tiles, i));
                bools[1] = SingleSpaceFiller(Tiles[i]);
            }
            if (bools[0] == true || bools[1] == true) returnState = true;
            return returnState;
        }
        static bool SingleSpaceFiller(TileInfo[] tiles) {
            bool returnState = false;
            Parallel.For(0, gridSize - 2, i => {
                if (tiles[i].TileState == TileState.empty) return;
                if (tiles[i].TileState == tiles[i + 2].TileState) {
                    // choose next new tilestate
                    TileState newTileState = TileState.yellow;
                    if (tiles[i].TileState == TileState.yellow) newTileState = TileState.blue;
                    // out of bounds check for before setup and if there is need to do change
                    if (i != 10 && tiles[i + 1].IsLocked == false && tiles[i + 1].TileState == TileState.empty) {
                        tiles[i + 1].TileState = newTileState;
                        returnState = true;
                    }
                }
            });
            return returnState;
        }
        static bool TwoInSuccesionController() {
            bool returnState = false;
            bool[] bools = new bool[2];
            for (int i = 0; i < gridSize; i++) {
                bools[0] = CheckFor2InSuccesion(Oreientationconverter(Tiles, i));
                bools[1] = CheckFor2InSuccesion(Tiles[i]);
            }
            if (bools[0] == true || bools[1] == true) returnState = true;
            return returnState;
        }
        static TileInfo[] Oreientationconverter(TileInfo[][] tiles, int index) {
            TileInfo[] newTiles = new TileInfo[gridSize];
            for (int j = 0; j < gridSize; j++) {
                newTiles[j] = tiles[j][index];
            }
            return newTiles;
        }
        static bool CheckFor2InSuccesion(TileInfo[] tiles) {
            bool returnState = false;
            Parallel.For(0, gridSize - 1, i => {
                if (tiles[i].TileState == TileState.empty) return;
                if (tiles[i].TileState == tiles[i + 1].TileState) {
                    // choose next new tilestate
                    TileState newTileState = TileState.yellow;
                    if (tiles[i].TileState == TileState.yellow) newTileState = TileState.blue;
                    // out of bounds check for before setup and if there is need to do change
                    if (i != 0 && tiles[i - 1].IsLocked == false && tiles[i - 1].TileState == TileState.empty) {
                        tiles[i - 1].TileState = newTileState;
                        returnState = true;
                    }
                    if (i != gridSize - 2 && tiles[i + 2].IsLocked == false && tiles[i + 2].TileState == TileState.empty) {
                        tiles[i + 2].TileState = newTileState;
                        returnState = true;
                    }
                }
            });
            return returnState;
        }
        static bool IsDone() {
            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    if (Tiles[i][j].TileState == TileState.empty) return false;
                }
            }
            return true;
        }
        static void ReadElements(WebDriver webDriver) {
            Tiles = new TileInfo[gridSize][];
            for (int i = 0; i < gridSize; i++) {
                Tiles[i] = new TileInfo[gridSize];
                //Parallel.For(0, gridSize, j => {
                //    Tiles[i][j] = new(webDriver.FindElement(By.Id($"tile-{i}-{j}")));
                //});
                for (int j = 0; j < gridSize; j++) {
                    Tiles[i][j] = new(webDriver.FindElement(By.Id($"tile-{i}-{j}")));
                    if (Tiles[i][j].IsLocked == false)
                        Tiles[i][j].WebElement.Click();
                }
            }
        }
        static void ClickResult(WebDriver webDriver) {
            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    TileInfo theTile = Tiles[i][j];
                    if (theTile.IsLocked) continue;
                    switch (theTile.TileState) {
                        case TileState.empty:
                            break;
                        case TileState.yellow:
                            //theTile.WebElement.Click();
                            break;
                        case TileState.blue:
                            //theTile.WebElement.Click();
                            theTile.WebElement.Click();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        static void ClickFancyResult(WebDriver webDriver) {
            List<Fancy> list = new List<Fancy>();

            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    TileInfo theTile = Tiles[i][j];
                    if (theTile.IsLocked) continue;
                    switch (theTile.TileState) {
                        case TileState.empty:
                            break;
                        case TileState.yellow:
                            list.Add(new(i, j));
                            break;
                        case TileState.blue:
                            list.Add(new(i, j));
                            list.Add(new(i, j));
                            break;
                        default:
                            break;
                    }
                }
            }
            Random random = new Random();
            while (list.Count > 0) {

                Fancy fancy = list[random.Next(list.Count)];
                Tiles[fancy.i][fancy.j].WebElement.Click();
                list.Remove(fancy);
            }

        }
    }
    public struct Fancy {
        public int i { get; set; }
        public int j { get; set; }

        public Fancy(int i, int j) {
            this.i = i;
            this.j = j;
        }
    }
    public enum TileState {
        empty,
        yellow,
        blue,
    }
    public class TileInfo {
        public Point Point { get; set; }
        public TileState TileState { get; set; }
        public bool IsLocked { get; private set; }
        public IWebElement WebElement { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public TileInfo(IWebElement element) {
            switch (element.GetAttribute("class")) {
                case "tile tile-":
                    TileState = TileState.empty;
                    IsLocked = false;
                    break;
                case "tile tile-1":
                    TileState = TileState.yellow;
                    IsLocked = true;
                    break;
                case "tile tile-2":
                    TileState = TileState.blue;
                    IsLocked = true;
                    break;
                default:
                    break;
            }
            WebElement = element;
        }
        public TileInfo(TileState tileState, Point point, int x, int y) {
            if (tileState != TileState.empty) IsLocked = true;
            TileState = tileState;
            Point = point;
            X = x;
            Y = y;
        }
    }
}