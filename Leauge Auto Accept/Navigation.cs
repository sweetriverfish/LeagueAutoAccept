using System;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace Leauge_Auto_Accept
{
    internal class Navigation
    {
        public static int currentPos = 0;
        public static int consolePosLast = 0;
        public static int searchPos = 0;
        public static int lastPosMainNav = 0;

        public static string filterKeyword = "";

        public static void ReadKeys()
        {
            while (true)
            {
                if (UI.currentWindow == "consoleTooSmallMessage")
                {
                    return;
                }

                bool movePointer = false;

                ConsoleKeyInfo key = new ConsoleKeyInfo();
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentPos--;
                        movePointer = true;
                        break;

                    case ConsoleKey.DownArrow:
                        currentPos++;
                        movePointer = true;
                        break;

                    case ConsoleKey.RightArrow:
                        currentPos += UI.totalRows;
                        movePointer = true;
                        break;

                    case ConsoleKey.LeftArrow:
                        currentPos -= UI.totalRows;
                        movePointer = true;
                        break;

                    case ConsoleKey.Escape:
                        handleNavEscape();
                        break;

                    case ConsoleKey.Enter:
                        handleNavEnter();
                        break;

                    case ConsoleKey.Backspace:
                        handleInputBackspace();
                        break;

                    default:
                        handleInput(key.KeyChar);
                        break;
                }

                if (filterKeyword == "Lochel")
                {
                    UI.printHeart();
                }
                else if (movePointer && !new[] { "infoMenu", "leagueClientIsClosedMessage", "initializing" }.Contains(UI.currentWindow))
                {
                    Print.isMovingPos = true;
                    handlePointerMovement();
                    if (UI.currentWindow == "settingsMenu")
                    {
                        UI.settingsMenuDesc(currentPos);
                    }
                    Print.isMovingPos = false;
                }
            }
        }

        private static void handleNavEscape()
        {
            switch (UI.currentWindow)
            {
                case "mainScreen":
                    UI.exitMenu();
                    break;
                case "exitMenu":
                    if (LCU.isLeagueOpen)
                    {
                        UI.mainScreen();
                    }
                    else
                    {
                        UI.leagueClientIsClosedMessage();
                    }
                    break;
                case "leagueClientIsClosedMessage":
                    UI.exitMenu();
                    break;
                case "initializing":
                    // do nothing
                    break;
                default:
                    if (filterKeyword == "Lochel")
                    {
                        filterKeyword = "";
                    }
                    UI.mainScreen();
                    break;
            }
        }

        private static void handleNavEnter()
        {
            switch (UI.currentWindow)
            {
                case "mainScreen":
                    mainMenuNav();
                    break;
                case "settingsMenu":
                    Settings.settingsModify(currentPos);
                    UI.settingsMenuUpdateUI(currentPos);
                    break;
                case "exitMenu":
                    exitMenuNav();
                    break;
                case "champSelector":
                case "spellSelector":
                    if (filterKeyword != "Lochel")
                    {
                        if (UI.currentWindow == "champSelector")
                        {
                            Settings.saveSelectedChamp();
                        }
                        else if (UI.currentWindow == "spellSelector")
                        {
                            Settings.saveSelectedSpell();
                        }
                        UI.mainScreen();
                    }
                    break;
            }
        }

        private static void handleInputBackspace()
        {
            if (UI.currentWindow == "champSelector" || UI.currentWindow == "spellSelector")
            {
                if (filterKeyword.Length > 0)
                {
                    filterKeyword = filterKeyword.Remove(filterKeyword.Length - 1);
                    UI.updateCurrentFilter();
                }
            }
            else if (UI.currentWindow == "settingsMenu")
            {
                if (currentPos == 3)
                {
                    if (Settings.lockDelayString.Length > 0)
                    {
                        Settings.lockDelayString = Settings.lockDelayString.Remove(Settings.lockDelayString.Length - 1);
                        Settings.settingsModify(currentPos);
                        UI.settingsMenuUpdateUI(currentPos);
                    }
                }
            }
        }

        private static void handleInput(char key)
        {
            if (UI.currentWindow == "champSelector" || UI.currentWindow == "spellSelector")
            {
                if (Functions.IsEnglishLetter(key) || key == '\'' || key == '.')
                {
                    if (filterKeyword.Length < 100)
                    {
                        filterKeyword += key;
                        UI.updateCurrentFilter();
                    }
                }
            }
            else if (UI.currentWindow == "settingsMenu")
            {
                if (currentPos == 3)
                {
                    if (Functions.IsNumeric(key))
                    {
                        if (Settings.lockDelayString.Length < 5)
                        {
                            Settings.lockDelayString += key;
                            Settings.settingsModify(currentPos);
                            UI.settingsMenuUpdateUI(currentPos);
                        }
                    }
                }
            }
        }

        public static void handlePointerMovement()
        {
            while (!Print.canMovePos)
            {
                Thread.Sleep(2);
            }

            if (UI.currentWindow == "mainScreen" && consolePosLast > 4)
            {
                // Handles the weird main menu navigation
                if (consolePosLast == 5)
                {
                    Console.SetCursorPosition(UI.leftPad, SizeHandler.HeightCenter + 5);
                }
                else if (consolePosLast == 6)
                {
                    Console.SetCursorPosition(UI.leftPad + 40, SizeHandler.HeightCenter + 5);
                }
            }
            else if (UI.currentWindow == "exitMenu" && consolePosLast == 1)
            {
                // Handles the weird exit menu navigation
                Console.SetCursorPosition(UI.leftPad + 30, UI.topPad);
            }
            else
            {
                int[] consolePosOld = getPositionOnConsole(consolePosLast);
                Console.SetCursorPosition(consolePosOld[1], consolePosOld[0]);
            }
            Print.printWhenPossible("  ");

            if (currentPos < 0)
            {
                currentPos = 0;
            }
            else if (currentPos >= UI.maxPos)
            {
                currentPos = UI.maxPos - 1;
            }
            consolePosLast = currentPos;
            if (UI.currentWindow == "mainScreen")
            {
                lastPosMainNav = currentPos;
            }

            if (UI.currentWindow == "mainScreen" && currentPos > 4)
            {
                // Handles the weird main menu navigation
                if (currentPos == 5)
                {
                    Console.SetCursorPosition(UI.leftPad, SizeHandler.HeightCenter + 5);
                }
                else if (currentPos == 6)
                {
                    Console.SetCursorPosition(UI.leftPad + 40, SizeHandler.HeightCenter + 5);
                }
            }
            else if (UI.currentWindow == "exitMenu" && currentPos == 1)
            {
                // Handles the weird exit menu navigation
                Console.SetCursorPosition(UI.leftPad + 30, UI.topPad);
            }
            else
            {
                int[] consolePos = getPositionOnConsole(currentPos);
                Console.SetCursorPosition(consolePos[1], consolePos[0]);
            }
            Print.printWhenPossible("->");
        }

        private static int[] getPositionOnConsole(int pos)
        {
            // Calculate the current row
            double row1 = pos / UI.totalRows;       //  1.111111111111111
            double row2 = Math.Floor(row1);         //  1

            // Calculate the current column
            double column1 = row2 * UI.totalRows;   //  27
            double column2 = pos - column1;         //  3

            // Convert to integer, caclulate column width
            int column = Convert.ToInt32(column2);
            int row = Convert.ToInt32(row2) * UI.columnWidth;

            if (column < 0)
                column = 0;
            if (row < 0)
                row = 0;

            return new int[] { column + UI.topPad, row + UI.leftPad };
        }

        private static void exitMenuNav()
        {
            // Swap sub menu
            if (currentPos == 0)
            {
                Environment.Exit(0);
            }
            else if (currentPos == 1)
            {
                if (LCU.isLeagueOpen)
                {
                    UI.mainScreen();
                }
                else
                {
                    UI.leagueClientIsClosedMessage();
                }
            }
        }

        private static void mainMenuNav()
        {
            switch (currentPos)
            {
                case 0:
                    UI.currentChampPicker = 0;
                    UI.champSelector();
                    break;
                case 1:
                    UI.currentChampPicker = 1;
                    UI.champSelector();
                    break;
                case 2:
                    UI.currentSpellSlot = 0;
                    UI.spellSelector();
                    break;
                case 3:
                    UI.currentSpellSlot = 1;
                    UI.spellSelector();
                    break;
                case 4:
                    Settings.toggleAutoAcceptSetting();
                    UI.toggleAutoAcceptSettingUI();
                    break;
                case 5:
                    UI.settingsMenu();
                    break;
                case 6:
                    UI.infoMenu();
                    break;
            }
        }
    }
}
