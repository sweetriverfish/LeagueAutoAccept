using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using System.Text.Json.Nodes;

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
            var releaseResp = webRequest("https://api.github.com/repos/sweetriverfish/LeagueAutoAccept/releases/latest");
            if (releaseResp.IsSuccessStatusCode == false)
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
                    var latestRelease = JsonNode.Parse(releaseResp.Content);

                    var latestTag = (string)latestRelease["tag_name"];

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

        public static RestResponse webRequest(string url)
        {
            RestResponse resp = null;
            
            try
            {
                using (var client = new RestClient(configureDefaultHeaders: headers => {
                    headers.Add("User-Agent", "League_Auto_Accept/" + appVersion);
                }))
                {

                    // Get the response
                    resp = client.ExecuteGet(new RestRequest(url));
                    resp.ThrowIfError();
                    return resp;
                }
            }
            catch(Exception ex)
            {
                return resp;
            }
        } 

    }

}
