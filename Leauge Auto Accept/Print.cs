using System;
using System.Collections.Generic;
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

        public static void printWhenPossible(string text, int top = -1, int left = -1, bool newLine = true)
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
            canMovePos = true;
        }

        public static void printCentered(string text, int row = -1, bool newLine = true)
        {
            string paddedText = centerString(text);
            printWhenPossible(paddedText, row, 0, newLine);
        }

        public static string centerString(string text)
        {
            int windowWidth = Console.WindowWidth;
            int textLength = text.Length;

            int leftPadding = (windowWidth - textLength) / 2;
            int rightPadding = windowWidth - textLength - leftPadding;

            string paddedText = text.PadLeft(leftPadding + textLength).PadRight(rightPadding + leftPadding + textLength);

            return paddedText;
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
