using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text.Json;

namespace Leauge_Auto_Accept
{
    internal class LCU
    {
        private static readonly NLog.ILogger Log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NLog.ILogger JsonLog = NLog.LogManager.GetLogger("JsonLog");

		private static string[] leagueAuth;
        private static int lcuPid = 0;
        public static bool isLeagueOpen = false;

        public static void CheckIfLeagueClientIsOpenTask()
        {
            while (true)
            {
                Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
                if (client != null)
                {
                    leagueAuth = getLeagueAuth(client);
                    isLeagueOpen = true;
                    if (lcuPid != client.Id)
                    {
                        lcuPid = client.Id;
                        // Check if preload data was enabled last time
                        if (Settings.preloadData)
                        {
                            Data.loadChampionsList();
                            Data.loadSpellsList();
                        }
                        if (Settings.shouldAutoAcceptbeOn)
                        {
                            MainLogic.isAutoAcceptOn = true;
                        }
                        if (UI.currentWindow != "exitMenu")
                        {
                            UI.mainScreen();
                        }
                    }
                }
                else
                {
                    isLeagueOpen = false;
                    MainLogic.isAutoAcceptOn = false;
                    Data.champsSorted.Clear();
                    Data.spellsSorted.Clear();
                    Data.currentSummonerId = "";
                    if (UI.currentWindow != "leagueClientIsClosedMessage" && UI.currentWindow != "exitMenu")
                    {
                        UI.leagueClientIsClosedMessage();
                    }
                }
                Thread.Sleep(2000);
            }
        }

        public static bool CheckIfLeagueClientIsOpen()
        {
            Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
            if (client != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string[] getLeagueAuth(Process client)
        {
            string query = $"SELECT CommandLine FROM Win32_Process where ProcessId = {client.Id}";
            string commandLine = string.Empty;

            using (var searcher = new System.Management.ManagementObjectSearcher(query))
            using (var results = searcher.Get())
            {
                foreach (var result in results)
                {
                    commandLine = result["CommandLine"]?.ToString();
                    break;
                }
            }

            // Parse the port and auth token into variables
            string port = Regex.Match(commandLine, @"--app-port=""?(\d+)""?").Groups[1].Value;
            string authToken = Regex.Match(commandLine, @"--remoting-auth-token=([a-zA-Z0-9_-]+)").Groups[1].Value;

            // Compute the encoded key
            string auth = "riot:" + authToken;
            string authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

            // Return content
            return new string[] { authBase64, port };
        }

        public static string[] clientRequest(string method, string url, string body = null)
        {
            // Ignore invalid https
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    // Set URL
                    client.BaseAddress = new Uri("https://127.0.0.1:" + leagueAuth[1] + "/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", leagueAuth[0]);

                    // Set headers
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), url);

                    // Send POST data when doing a post request
                    if (!string.IsNullOrEmpty(body))
                    {
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    }

                    // Get the response
                    Log.Debug("{0} {1}", request.Method, request.RequestUri.ToString());
                    HttpResponseMessage response = client.SendAsync(request).Result;

                    // If the response is null (League client closed?)
                    if (response == null)
                    {
                        return new string[] { "999", "" };
                    }

                    // Get the HTTP status code
                    int statusCode = (int)response.StatusCode;
                    string statusString = statusCode.ToString();

                    // Get the body
                    string responseFromServer = response.Content.ReadAsStringAsync().Result;
                    if (JsonLog.IsDebugEnabled)
                    {
                        var jsonpretty = JsonSerializer.Serialize(JsonDocument.Parse(responseFromServer), new JsonSerializerOptions() { WriteIndented = true });
                        JsonLog.Debug("Output from {0}:\n{1}", url, jsonpretty);
                    }

                    // Clean up the response
                    response.Dispose();

                    // Return content
                    return new string[] { statusString, responseFromServer };
                }
            }
            catch
            {
                // If the URL is invalid (League client closed?)
                return new string[] { "999", "" };
            }
        }

        public static string[] clientRequestUntilSuccess(string method, string url, string body = null)
        {
            string[] request = { "000", "" };
            while (request[0].Substring(0, 1) != "2")
            {
                request = clientRequest(method, url, body);
                if (request[0].Substring(0, 1) == "2")
                {
                    return request;
                }
                else
                {
                    if (CheckIfLeagueClientIsOpen())
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        return request;
                    }
                }
            }
            return request;
        }
    }
}
