using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;

namespace Leauge_Auto_Accept
{
    class Program
    {
        private static void Main()
        {
            // Show initializing message
            UI.initializingWindow();

            // Initlize console size related stuff
            SizeHandler.initialize();

            // Set console title
            Console.Title = "League Auto Accept";

            // Attempt to load existing settings
            Settings.loadSettings();

            Updater.initialize();

            //Settings.chatMessagesEnabled = true;
            //Settings.chatMessages.Add("Support");
            //Settings.chatMessages.Add("Nami");

            // Start a bunch of task
            var taskKeys = new Task(Navigation.ReadKeys);
            taskKeys.Start();
            var taskQueue = new Task(MainLogic.acceptQueue);
            taskQueue.Start();
            var taskLeagueAlive = new Task(LCU.CheckIfLeagueClientIsOpenTask);
            taskLeagueAlive.Start();
            var taskResizeHandler = new Task(SizeHandler.SizeReader);
            taskResizeHandler.Start();

            // Indefinitely await them
            var tasks = new[] { taskKeys, taskQueue, taskLeagueAlive, taskResizeHandler };
            Task.WaitAll(tasks);

            Console.ReadKey();
        }
    }
}
