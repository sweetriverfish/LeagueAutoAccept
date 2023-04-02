using System;
using System.Threading;

namespace Leauge_Auto_Accept
{
    internal class SizeHandler
    {
        public static int minWidth = 120;
        public static int minHeight = 30;

        public static int WindowWidth = minWidth;
        public static int WindowHeight = minHeight;
        public static int WidthCenter = WindowWidth / 2;
        public static int HeightCenter = WindowHeight / 2;

        public static void initialize()
        {
            // Hide cursor/caret
            Console.CursorVisible = false;

            // Set the console size
            Console.SetWindowSize(minWidth, minHeight);
        }

        public static void SizeReader()
        {
            while (true)
            {
                int currentWidth = Console.WindowWidth;
                int currentHeight = Console.WindowHeight;
                if (WindowWidth != currentWidth || WindowHeight != currentHeight)
                {
                    WindowWidth = currentWidth;
                    WindowHeight = currentHeight;
                    CalculateCenter();
                    handleResize();
                }
                Thread.Sleep(1000);
            }
        }

        public static void handleResize()
        {
            // Hide cursor because for some reason it shows up every single time
            Console.CursorVisible = false;

            // Adapt to new size
            UI.totalRows = WindowHeight - 2;

            // Handle the console being too small
            if (WindowWidth < minWidth)
            {
                UI.consoleTooSmallMessage("width");
            }
            else if (WindowHeight < minHeight)
            {
                UI.consoleTooSmallMessage("height");
            }
            else if (UI.currentWindow == "consoleTooSmallMessage")
            {
                UI.reloadWindow("previous");
            }
            else
            {
                UI.reloadWindow("current");
            }
        }

        public static void CalculateCenter()
        {
            WidthCenter = WindowWidth / 2;
            HeightCenter = WindowHeight / 2;
        }
    }
}
