using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept
{
    internal class Print
    {
        public static bool canMovePos = false;
        public static bool isMovingPos = false;
        private static int lastRow = 0;

        public static void printWhenPossible(string text, int top = -1, int left = -1, bool newLine = true, bool updateCursor = false)
        {
            if (isMovingPos)
            {
                Thread.Sleep(2);
            }
            canMovePos = false;
            if (left >= 0 || top >= 0)
            {
                if (left < 0)
                {
                    left = 0;
                }
                if (top < 0)
                {
                    top = lastRow;
                }
                Console.SetCursorPosition(left, top);
            }
            if (newLine)
            {
                Console.WriteLine(text);
                lastRow = top + 1;
            }
            else
            {
                Console.Write(text);
            }
            if (updateCursor)
            {
                UI.cursorPosition = new int[] { left, top };
            }
            canMovePos = true;
        }

        public static void printCentered(string text, int row = -1, bool newLine = true, bool updateCursor = false)
        {
            string[] paddedText = centerString(text);
            printWhenPossible(paddedText[0], row, 0, newLine);

            if (updateCursor)
            {
                int posLeft = Int32.Parse(paddedText[1]) + Int32.Parse(paddedText[2]);
                UI.cursorPosition = new int[] { posLeft, row };
            }
        }

        public static string[] centerString(string text)
        {
            int windowWidth = Console.WindowWidth;
            int textLength = text.Length;

            int leftPadding = (windowWidth - textLength) / 2;
            int rightPadding = windowWidth - textLength - leftPadding;

            string paddedText = text.PadLeft(leftPadding + textLength).PadRight(rightPadding + leftPadding + textLength);

            // return paddedText;
            return new string[] { paddedText, leftPadding.ToString(), textLength.ToString() };
        }

        public static string replaceAt(string text, string replacement, int replaceIndex)
        {
            if (text.Length >= replaceIndex)
            {
                return text.Substring(0, replaceIndex) + replacement + text.Substring(replaceIndex + replacement.Length);
            }
            else
            {
                return "";
            }
        }
    }
}
