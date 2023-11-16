using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace _0hh1 {
    internal class Program {
        public static int gridSize = 12;

        public static TileInfo[][]? Tiles;
        static void Main(string[] args) {
            WebDriver webDriver = new ChromeDriver();

            webDriver.Navigate().GoToUrl("https://0hh1.com/");
            bool isRunning = true;
            while (isRunning) {
                webDriver.ExecuteScript($"Game.startGame({gridSize},0)");

                Thread.Sleep(1000);
                Console.WriteLine($"[{DateTime.Now}] has begun reading");
                ReadElements(webDriver);

                DateTime beforeAlgo = DateTime.Now;
                Console.WriteLine($"[{DateTime.Now}] algoritmim has begun");
                Algorithim();
                Console.WriteLine($"[{DateTime.Now}] time diff: {(DateTime.Now - beforeAlgo).Milliseconds} miliseconds, algoritmim has finished");


                ClickResult(webDriver);

                isRunning = false;
                if (false) { // if you want to manually begin it
                    Console.WriteLine("Press and key other than q to continue");
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Q) isRunning = false;
                }

                Thread.Sleep(1000);
            }

            webDriver.Quit();
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
                Parallel.For(0, gridSize, j => {
                    Tiles[i][j] = new(webDriver.FindElement(By.Id($"tile-{i}-{j}")));
                });
                //for (int j = 0; j < gridSize; j++) {
                //    Tiles[i][j] = new(webDriver.FindElement(By.Id($"tile-{i}-{j}")));
                //}
            }
        }
        static void ClickResult(WebDriver webDriver) {

            for (int i = 0; i < gridSize; i++) {
                Parallel.For(0, gridSize, j => {
                    TileInfo theTile = Tiles[i][j];
                    if (theTile.IsLocked) return;
                    switch (theTile.TileState) {
                        case TileState.empty:
                            break;
                        case TileState.yellow:
                            theTile.WebElement.Click();
                            break;
                        case TileState.blue:
                            theTile.WebElement.Click();
                            theTile.WebElement.Click();
                            break;
                        default:
                            break;
                    }

                });
                //for (int j = 0; j < gridSize; j++) {
                //    TileInfo theTile = Tiles[i][j];
                //    if (theTile.IsLocked) continue;
                //    switch (theTile.TileState) {
                //        case TileState.empty:
                //            break;
                //        case TileState.yellow:
                //            theTile.WebElement.Click();
                //            break;
                //        case TileState.blue:
                //            theTile.WebElement.Click();
                //            theTile.WebElement.Click();
                //            break;
                //        default:
                //            break;
                //    }
                //}
            }
        }
    }
    public enum TileState {
        empty,
        yellow,
        blue,
    }
    public class TileInfo {
        //public Point Point { get; private set; }
        public TileState TileState { get; set; }
        public bool IsLocked { get; private set; }
        public IWebElement WebElement { get; private set; }
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
    }
}