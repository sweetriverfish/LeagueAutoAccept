using System;
using System.Data.Common;
using System.Diagnostics;
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

        public static string currentInput = "";

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
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.LeftArrow:
                        movePointer = handlePointerMovement(key.Key);
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

                if (currentInput == "Lochel" && UI.windowType == "grid")
                {
                    UI.printHeart();
                }
                else if (movePointer)
                {
                    Print.isMovingPos = true;
                    handlePointerMovementPrint();
                    consolePosLast = currentPos;
                    if (UI.currentWindow == "settingsMenu")
                    {
                        UI.settingsMenuDesc(currentPos);
                    }
                    else if (UI.currentWindow == "delayMenu")
                    {
                        UI.delayMenuDesc(currentPos);
                    }
                    Print.isMovingPos = false;
                }
            }
        }

        private static bool handlePointerMovement(ConsoleKey key)
        {
            switch (UI.windowType)
            {
                case "normal":
                    return handlePointerMovementNormal(key);

                case "messageEdit":
                case "sideways":
                    return handlePointerMovementSideways(key);

                case "grid":
                    return handlePointerMovementGrid(key);

                case "nocursor":
                    return false;

                case "pages":
                    return handlePointerMovementPages(key);
            }
            return false;
        }

        private static bool handlePointerMovementNormal(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (currentPos == 0)
                    {
                        return false;
                    }
                    currentPos--;
                    return true;

                case ConsoleKey.DownArrow:
                    if (currentPos + 1 == UI.maxPos)
                    {
                        return false;
                    }
                    currentPos++;
                    return true;
            }
            return false;
        }

        private static bool handlePointerMovementSideways(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (currentPos == 0)
                    {
                        return false;
                    }
                    currentPos--;
                    return true;

                case ConsoleKey.UpArrow:
                    if (currentPos == 0)
                    {
                        return false;
                    }
                    currentPos--;
                    return true;

                case ConsoleKey.RightArrow:
                    if (currentPos + 1 == UI.maxPos)
                    {
                        return false;
                    }
                    currentPos++;
                    return true;

                case ConsoleKey.DownArrow:
                    if (currentPos + 1 == UI.maxPos)
                    {
                        return false;
                    }
                    currentPos++;
                    return true;
            }
            return false;
        }

        private static bool handlePointerMovementGrid(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (currentPos == 0)
                    {
                        return false;
                    }
                    currentPos--;
                    return true;

                case ConsoleKey.DownArrow:
                    if (currentPos + 1 == UI.maxPos)
                    {
                        return false;
                    }
                    currentPos++;
                    return true;

                case ConsoleKey.RightArrow:
                    if (currentPos + UI.totalRows >= UI.maxPos)
                    {
                        return false;
                    }
                    currentPos += UI.totalRows;
                    return true;

                case ConsoleKey.LeftArrow:
                    if (currentPos <= 0)
                    {
                        currentPos = 0;
                        return false;
                    }
                    currentPos -= UI.totalRows;
                    return true;
            }
            return false;
        }

        private static bool handlePointerMovementPages(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (currentPos == 0)
                    {
                        return false;
                    }
                    currentPos--;
                    return true;

                case ConsoleKey.DownArrow:
                    if (currentPos + 1 == UI.maxPos)
                    {
                        return false;
                    }
                    currentPos++;
                    return true;

                case ConsoleKey.RightArrow:
                    if (UI.currentPage + 1 == UI.totalPages)
                    {
                        return false;
                    }
                    // TODO: improve this somehow
                    UI.currentPage++;
                    currentPos = 0;
                    UI.chatMessagesWindow(UI.currentPage);
                    return true;

                case ConsoleKey.LeftArrow:
                    if (UI.currentPage == 0)
                    {
                        return false;
                    }
                    // TODO: improve this somehow
                    UI.currentPage--;
                    currentPos = 0;
                    UI.chatMessagesWindow(UI.currentPage);
                    return true;
            }
            return false;
        }

        private static void handleNavEscape()
        {
            switch (UI.currentWindow)
            {
                case "delayMenu":
                    UI.settingsMenu();
                    break;
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
                case "chatMessagesEdit":
                    UI.chatMessagesWindow();
                    break;
                default:
                    if (currentInput == "Lochel" && UI.windowType == "grid")
                    {
                        currentInput = "";
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
                    if (UI.currentWindow == "settingsMenu")
                    {
                        UI.settingsMenuUpdateUI(currentPos);
                    }
                    break;
                case "delayMenu":
                    //Settings.delayModify(currentPos);
                    //UI.delayMenuUpdateUI(currentPos);
                    break;
                case "exitMenu":
                    exitMenuNav();
                    break;
                case "champSelector":
                case "spellSelector":
                    if (currentInput != "Lochel")
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
                case "chatMessagesWindow":
                    UI.messageIndex = currentPos;
                    UI.chatMessagesEdit();
                    break;
                case "chatMessagesEdit":
                    chatMessagesEditNav();
                    break;
            }
        }

        private static void handleInputBackspace()
        {
            if (UI.currentWindow == "champSelector" || UI.currentWindow == "spellSelector")
            {
                if (currentInput.Length > 0)
                {
                    currentInput = currentInput.Remove(currentInput.Length - 1);
                    UI.updateCurrentFilter();
                }
            }
            else if (UI.currentWindow == "chatMessagesEdit")
            {
                if (currentInput.Length > 0)
                {
                    currentInput = currentInput.Remove(currentInput.Length - 1);
                    UI.updateMessageEdit();
                }
            }
            else if (UI.currentWindow == "delayMenu")
            {
                Settings.delayModify(currentPos, -1);
                UI.delayMenuUpdateUI(currentPos);
            }
        }

        private static void handleInput(char key)
        {
            if (UI.currentWindow == "champSelector" || UI.currentWindow == "spellSelector")
            {
                if (currentInput.Length < 100)
                {
                    currentInput += key;
                    UI.updateCurrentFilter();
                }
            }
            if (UI.currentWindow == "chatMessagesEdit")
            {
                if (currentInput.Length < 200)
                {
                    currentInput += key;
                    UI.updateMessageEdit();
                }
            }
            else if (UI.currentWindow == "delayMenu")
            {
                if (Functions.IsNumeric(key))
                {
                    Settings.delayModify(currentPos, Int32.Parse(key.ToString()));
                    UI.delayMenuUpdateUI(currentPos);
                }
            }
        }

        public static void handlePointerMovementPrint()
        {
            Console.CursorVisible = false;
            while (!Print.canMovePos)
            {
                Thread.Sleep(2);
            }

            {
                int positionLeft = 0;
                int positionTop = 0;
                if (UI.currentWindow == "mainScreen" && consolePosLast > 5)
                {
                    // Handles the weird main menu navigation
                    if (consolePosLast == 6)
                    {
                        positionLeft = UI.leftPad;
                        positionTop = SizeHandler.HeightCenter + 6;
                    }
                    else if (consolePosLast == 7)
                    {
                        positionLeft = UI.leftPad + 40;
                        positionTop = SizeHandler.HeightCenter + 6;
                    }
                }
                else if (UI.currentWindow == "exitMenu" && consolePosLast == 1)
                {
                    // Handles the weird exit menu navigation
                    positionLeft = UI.leftPad + 30;
                    positionTop = UI.topPad;
                }
                else if (UI.currentWindow == "chatMessagesEdit")
                {
                    positionTop = UI.topPad + 3;
                    switch (consolePosLast)
                    {
                        case 0:
                            positionLeft = UI.leftPad - 21;
                            break;
                        case 1:
                            positionLeft = UI.leftPad - 7;
                            break;
                        case 2:
                            positionLeft = UI.leftPad + 8;
                            break;
                    }
                }
                else
                {
                    int[] consolePos = getPositionOnConsole(consolePosLast);
                    positionLeft = consolePos[1];
                    positionTop = consolePos[0];
                }
                Print.printWhenPossible("  ", positionTop, positionLeft, false);
            }


            {
                int positionLeft = 0;
                int positionTop = 0;
                if (UI.currentWindow == "mainScreen")
                {
                    lastPosMainNav = currentPos;
                }

                if (UI.currentWindow == "mainScreen" && currentPos > 5)
                {
                    // Handles the weird main menu navigation
                    if (currentPos == 6)
                    {
                        positionLeft = UI.leftPad;
                        positionTop = SizeHandler.HeightCenter + 6;
                    }
                    else if (currentPos == 7)
                    {
                        positionLeft = UI.leftPad + 40;
                        positionTop = SizeHandler.HeightCenter + 6;
                    }
                }
                else if (UI.currentWindow == "exitMenu" && currentPos == 1)
                {
                    // Handles the weird exit menu navigation
                    positionLeft = UI.leftPad + 30;
                    positionTop = UI.topPad;
                }
                else if (UI.currentWindow == "chatMessagesEdit")
                {
                    positionTop = UI.topPad + 3;
                    switch (currentPos)
                    {
                        case 0:
                            positionLeft = UI.leftPad - 21;
                            break;
                        case 1:
                            positionLeft = UI.leftPad - 7;
                            break;
                        case 2:
                            positionLeft = UI.leftPad + 8;
                            break;
                    }
                }
                else
                {
                    int[] consolePos = getPositionOnConsole(currentPos);
                    positionLeft = consolePos[1];
                    positionTop = consolePos[0];
                }
                Print.printWhenPossible("->", positionTop, positionLeft, false);
            }
            if (UI.showCursor)
            {
                Console.CursorVisible = true;
                UI.updateCursorPosition();
            }
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
            int row = Convert.ToInt32(row2) * UI.columnSize;

            if (column < 0)
                column = 0;
            if (row < 0)
                row = 0;

            return new int[] { column + UI.topPad, row + UI.leftPad };
        }

        private static void chatMessagesEditNav()
        {
            if (currentPos == 0)
            {
                //save
                if (currentInput.Length == 0)
                {
                    Settings.deleteChatMessage();
                }
                else
                {
                    Settings.updateChatMessage();
                }
                UI.chatMessagesWindow();
            }
            else if (currentPos == 1)
            {
                //delete
                Settings.deleteChatMessage();
                UI.chatMessagesWindow();
            }
            else if (currentPos == 2)
            {
                //cancel
                UI.chatMessagesWindow();
            }
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
                    UI.chatMessagesWindow();
                    break;
                case 5:
                    Settings.toggleAutoAcceptSetting();
                    UI.toggleAutoAcceptSettingUI();
                    break;
                case 6:
                    UI.settingsMenu();
                    break;
                case 7:
                    UI.infoMenu();
                    break;
            }
        }
    }
}
