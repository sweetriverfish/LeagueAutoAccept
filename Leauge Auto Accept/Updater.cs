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
            Print.printCentered(Strings.Get("checking_for_update"), SizeHandler.HeightCenter);
            var releaseResp = webRequest("https://api.github.com/repos/sweetriverfish/LeagueAutoAccept/releases/latest");
            if (releaseResp == null || releaseResp.IsSuccessStatusCode == false)
            {
                // Network error
                Console.Clear();
                Print.printCentered(Strings.Get("failed_check_update"), SizeHandler.HeightCenter - 1);
                Print.printCentered(Strings.Get("disable_check_in_settings"));
                Print.printCentered(Strings.Get("app_launch_shortly"));
                Thread.Sleep(1500);
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
                        Print.printCentered(Strings.Get("no_update_found"), SizeHandler.HeightCenter);
                        Thread.Sleep(178);
                        return;
                    }
                    else
                    {
                        // Running an different version than the latest release, suggest an update
                        Console.Clear();
                        Print.printCentered(Strings.Get("update_found"), SizeHandler.HeightCenter - 3);
                        Print.printCentered(string.Format(Strings.Get("current_latest_version_format"), appVersion, latestTag));

                        Print.printCentered(Strings.Get("latest_version_found_at"), SizeHandler.HeightCenter);
                        Print.printCentered("github.com/sweetriverfish/LeagueAutoAccept/releases/latest");

                        Print.printCentered(Strings.Get("disable_check_in_settings"), SizeHandler.HeightCenter + 3);
                        Print.printCentered(Strings.Get("app_launch_5_seconds"));

                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                    // Default case, in case github changes the json format or something idk
                    Console.Clear();
                    Print.printCentered(Strings.Get("failed_check_update"), SizeHandler.HeightCenter - 1);
                    Print.printCentered(Strings.Get("disable_check_in_settings"));
                    Print.printCentered(Strings.Get("app_launch_5_seconds"));
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

                    var request = new RestRequest(url)
                    {
                        Timeout = TimeSpan.FromMilliseconds(5000)
                    };

                    // Get the response
                    resp = client.ExecuteGet(request);
                    resp.ThrowIfError();
                    return resp;
                }
            }
            catch
            {
                return resp;
            }
        } 

    }

}
