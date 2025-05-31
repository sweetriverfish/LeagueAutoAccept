using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leauge_Auto_Accept
{
    internal class Updater
    {
        public static string appVersion;

        public static void initialize()
        {
            Version appVersionTmp = Assembly.GetExecutingAssembly().GetName().Version;
            appVersion = appVersionTmp.Major + "." + appVersionTmp.Minor;
            
            if (!Debugger.IsAttached)
            {
                if (!Settings.disableUpdateCheck)
                {
                    versionCheck();
                }
            }
        }

        private static void versionCheck()
        {
            Console.Clear();
            Print.printCentered("Checking for an update...", SizeHandler.HeightCenter);
            string[] latestRelease = webRequest("https://api.github.com/repos/sweetriverfish/LeagueAutoAccept/releases/latest");
            if (latestRelease[0] != "200")
            {
                // Network error
                Console.Clear();
                Print.printCentered("Failed to check for an update.", SizeHandler.HeightCenter - 1);
                Print.printCentered("You can disable this check in the settings.");
                Print.printCentered("App will launch in 5 seconds.");
                Thread.Sleep(5000);
            }
            else
            {
                try
                {
                    string latestTag = latestRelease[1].Split("tag_name\": \"")[1].Split("\"")[0];
                    if ('v' + appVersion == latestTag)
                    {
                        // Running latest version, no update found/needed
                        Console.Clear();
                        Print.printCentered("No update found. Already using the latest version.", SizeHandler.HeightCenter);
                        Thread.Sleep(178);
                        return;
                    }
                    else
                    {
                        // Running an different version than the latest release, suggest an update
                        Console.Clear();
                        Print.printCentered("An update has been found, consider updating.", SizeHandler.HeightCenter - 3);
                        Print.printCentered("Current version is v" + appVersion + ", latest version is " + latestTag);

                        Print.printCentered("Latest version can be found at:", SizeHandler.HeightCenter);
                        Print.printCentered("github.com/sweetriverfish/LeagueAutoAccept/releases/latest");

                        Print.printCentered("You can disable this check in the settings.", SizeHandler.HeightCenter + 3);
                        Print.printCentered("App will launch in 5 seconds.");

                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                    // Default case, in case github changes the json format or something idk
                    Console.Clear();
                    Print.printCentered("Failed to check for an update.", SizeHandler.HeightCenter - 1);
                    Print.printCentered("You can disable this check in the settings.");
                    Print.printCentered("App will launch in 5 seconds.");
                    Thread.Sleep(5000);
                }
            }
        }

        public static string[] webRequest(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set URL, User-Agent
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");

                    // Get the response
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string content = response.Content.ReadAsStringAsync().Result;

                        return new[] { ((int)response.StatusCode).ToString(), content };
                    }
                }
            }
            catch
            {
                // If the URL is invalid
                string[] output = { "999", "" };
                return output;
            }
        }
    }
}
