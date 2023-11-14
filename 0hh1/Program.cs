using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace _0hh1 {
    public enum TileState {
        empty,
        yellow,
        blue,
    }
    internal class Program {
        public static int gridSize = 12;

        public static TileInfo[][]? Tiles;
        public static TileInfo[][]? DebugTiles;
        public static bool debug = false;
        static void Main(string[] args) {
            if (debug) {

                MakeDebugValues();
                MakeDebugValuesFinished();

                WriteInConsole(false);
                Console.WriteLine();
                Algorithim();
                Console.WriteLine();
                WriteInConsole(true);
                Console.WriteLine();
            }

            // debug statement
            if (debug) return;

            WebDriver webDriver = new ChromeDriver();

            webDriver.Navigate().GoToUrl("https://0hh1.com/");

            bool isRunning = true;
            int gamesPlayed = 0;
            while (isRunning) {
                webDriver.ExecuteScript($"Game.startGame({gridSize},0)");

                Thread.Sleep(1000);

                ReadElements(webDriver);

                Algorithim();

                PrintResult(webDriver);
                if (false) { // if you want to manually begin it
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Q) isRunning = false;
                }

                Thread.Sleep(1000);
            }

            webDriver.Quit();
        }
        static bool BoolCheck(bool[] bools) {
            for (int i = 0; i < bools.Length; i++) {
                if (bools[i]) return true;
            }
            return false;
        }
        static void Algorithim() {
            if (Tiles == null) throw new ArgumentNullException("faggot");
            bool[] checks = new bool[4];
            bool isRunning = true;
            int countDown = 8;
            while (isRunning) {
                checks[0] = TwoInSuccesionController();
                if (debug) { WriteInConsole(true); Console.WriteLine("twoinsuccsion"); }
                checks[1] = SingleSpaceController();
                if (debug) { WriteInConsole(true); Console.WriteLine("twoinsuccsion"); }
                checks[2] = LineCountController();
                if (debug) { WriteInConsole(true); Console.WriteLine("twoinsuccsion"); }
                checks[3] = CompareLinesController();
                if (debug) { WriteInConsole(true); Console.WriteLine("twoinsuccsion"); }

                isRunning = BoolCheck(checks);
                // reset countdown
                if (isRunning) countDown = 8;
                // insurance
                if (!isRunning) {
                    if (countDown > 0)
                        isRunning = true;
                    countDown--;
                }
            }
        }
        static bool CompareLinesController() {
            bool returnState = false;
            bool isRunning = true;

            while (isRunning) {
                bool[] bools = new bool[2];
                for (int i = 0; i < gridSize - 1; i++) {
                    for (int j = i + 1; j < gridSize; j++) {
                        bools[0] = CompareTwoLines(Tiles[i], Tiles[j]);
                        bools[1] = CompareTwoLines(Oreientationconverter(Tiles, i), Oreientationconverter(Tiles, j));
                    }
                }
                if (bools[0] == true || bools[1] == true) returnState = true;
                if (bools[0] == false && bools[1] == false) isRunning = false;
            }
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
            bool isRunning = true;
            while (isRunning) {
                bool[] bools = new bool[2];
                for (int i = 0; i < gridSize; i++) {
                    bools[0] = LineCountFinish(Oreientationconverter(Tiles, i));
                    bools[1] = LineCountFinish(Tiles[i]);
                }
                if (bools[0] == true || bools[1] == true) returnState = true;
                if (bools[0] == false && bools[1] == false) isRunning = false;
            }
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
            bool isRunning = true;
            while (isRunning) {
                bool[] bools = new bool[2];
                for (int i = 0; i < gridSize; i++) {
                    bools[0] = SingleSpaceFiller(Oreientationconverter(Tiles, i));
                    bools[1] = SingleSpaceFiller(Tiles[i]);
                }
                if (bools[0] == true || bools[1] == true) returnState = true;
                if (bools[0] == false && bools[1] == false) isRunning = false;
            }
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
            bool isRunning = true;
            while (isRunning) {
                bool[] bools = new bool[2];
                for (int i = 0; i < gridSize; i++) {
                    bools[0] = CheckFor2InSuccesion(Oreientationconverter(Tiles, i));
                    bools[1] = CheckFor2InSuccesion(Tiles[i]);
                }
                if (bools[0] == true || bools[1] == true) returnState = true;
                if (bools[0] == false && bools[1] == false) isRunning = false;
            }
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
                    if (i != 10 && tiles[i + 2].IsLocked == false && tiles[i + 2].TileState == TileState.empty) {
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
                for (int j = 0; j < gridSize; j++) {
                    Tiles[i][j] = new(webDriver.FindElement(By.Id($"tile-{i}-{j}")));
                }
            }


        }
        static void PrintResult(WebDriver webDriver) {
            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    TileInfo theTile = Tiles[i][j];
                    if (theTile.IsLocked) continue;
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
                }
            }
        }
        static int ConsoleCallTimes = 0;
        static void WriteInConsole(bool newLine) {
            int indent = (gridSize + 2) * ConsoleCallTimes;

            if (Tiles == null) throw new ArgumentNullException("fuck you");
            if (newLine) ConsoleCallTimes = 0;
            for (int i = 0; i < gridSize; i++) {
                if (!newLine) Console.SetCursorPosition(indent, i);
                for (int j = 0; j < gridSize; j++) {
                    TileInfo selectedTile = Tiles[i][j];
                    Console.ForegroundColor = GetTileStateColor(selectedTile);
                    Console.Write("█");
                }
                if (debug) {

                    Console.Write("  ");
                    for (int j = 0; j < gridSize; j++) {
                        TileInfo selectedTile = Tiles[i][j];
                        TileInfo selectedTruth = DebugTiles[i][j];

                        if (selectedTile.TileState == selectedTruth.TileState)
                            Console.ForegroundColor = GetTileStateColor(selectedTruth);
                        else if (selectedTile.TileState == TileState.empty) Console.ForegroundColor = GetTileStateColor(selectedTruth);
                        else if (selectedTile.TileState != selectedTruth.TileState) {
                            if (selectedTile.TileState == TileState.blue) Console.ForegroundColor = ConsoleColor.DarkGreen;
                            else if (selectedTile.TileState == TileState.yellow) Console.ForegroundColor = ConsoleColor.DarkRed;
                            else Console.ForegroundColor = ConsoleColor.DarkGray;
                        }


                        Console.Write("█");
                    }
                    Console.WriteLine();
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            ConsoleCallTimes++;
        }
        static ConsoleColor GetTileStateColor(TileInfo tile) {
            switch (tile.TileState) {
                case TileState.empty: return ConsoleColor.DarkGray;
                case TileState.yellow: return ConsoleColor.DarkYellow;
                case TileState.blue: return ConsoleColor.Blue;
                default: break;
            }
            return ConsoleColor.Gray;
        }
        static void MakeDebugValues() {
            Tiles = new TileInfo[][] {
            new TileInfo[] { // first layer
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
            },
            new TileInfo[] { // 2. layer
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(false, TileState.empty),
            },
                        new TileInfo[] { // 3. layer
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
            },
                                    new TileInfo[] { // 4. layer
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
            },
new TileInfo[] { // 5. layer
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
            },
            new TileInfo[] { // 6. layer
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
            },
                        new TileInfo[] { // 7. layer
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
            },
                                    new TileInfo[] { // 8. layer
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
            },
                                                new TileInfo[] { // 9. layer
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
            },
                                                            new TileInfo[] { // 10. layer
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
            },
                                                                        new TileInfo[] { // 11. layer
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
            },
                        new TileInfo[] { // 12. layer
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(false, TileState.empty),
                new(true, TileState.yellow),
                new(false, TileState.empty),
                new(false, TileState.empty),
            },
            };


        }
        static void MakeDebugValuesFinished() {
            DebugTiles = new TileInfo[][] {
            new TileInfo[] { // first layer
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
            },
            new TileInfo[] { // 2. layer
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
            },
                        new TileInfo[] { // 3. layer
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
            },
                                    new TileInfo[] { // 4. layer
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
            },
new TileInfo[] { // 5. layer
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
            },
            new TileInfo[] { // 6. layer
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
            },
                        new TileInfo[] { // 7. layer
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
            },
                                    new TileInfo[] { // 8. layer
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
            },
                                                new TileInfo[] { // 9. layer
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
            },
                                                            new TileInfo[] { // 10. layer
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
            },
                                                                        new TileInfo[] { // 11. layer
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
            },
                        new TileInfo[] { // 12. layer
                new(true, TileState.yellow),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.blue),
                new(true, TileState.yellow),
                new(true, TileState.blue),
                new(true, TileState.yellow),
            },
            };


        }

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
        public TileInfo(bool islocked, TileState tileState) {
            this.TileState = tileState;
            this.IsLocked = islocked;
        }
    }
}