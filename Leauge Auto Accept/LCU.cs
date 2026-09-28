using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text.Json;
using RestSharp;
using System.Collections.Generic;

namespace Leauge_Auto_Accept
{
    internal class LCU
    {
        private static readonly NLog.ILogger Log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NLog.ILogger JsonLog = NLog.LogManager.GetLogger("JsonLog");

        private static string[] leagueAuth;
        private static int lcuPid = 0;
        private const int RequestTimeoutMilliseconds = 5000;
        private const int MaxRetryAttempts = 5;
        public static bool isLeagueOpen = false;

        public static void CheckIfLeagueClientIsOpenTask()
        {
            while (true)
            {
                Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
                if (client != null)
                {
                    isLeagueOpen = true;
                    if (lcuPid != client.Id)
                    {
                        lcuPid = client.Id;

                        //get token and port
                        leagueAuth = getLeagueAuth(client);
                        if (leagueAuth == null)
                        {
                            isLeagueOpen = false;
                            MainLogic.isAutoAcceptOn = false;
                            Log.Warn("League client was found, but LCU auth data could not be read.");
                            UI.leagueClientIsClosedMessage();
                            Thread.Sleep(2000);
                            continue;
                        }

                        //reset restclient
                        S_restClient?.Dispose(); S_restClient = null;

                        TryApplyClientLanguage();

                        // Check if preload data was enabled last time
                        if (Settings.preloadData)
                        {
                            bool championsLoaded = Data.loadChampionsList();
                            bool spellsLoaded = Data.loadSpellsList();
                            if (!championsLoaded || !spellsLoaded)
                            {
                                Console.Clear();
                                Print.printCentered(Strings.Get("some_league_data_failed"), SizeHandler.HeightCenter - 1);
                                Print.printCentered(Strings.Get("app_keep_running_retry"));
                                Thread.Sleep(2500);
                            }
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
                    Data.currentSummonerId = 0;
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
                Log.Info($"LeagueClientUx process info: id={client.Id}");
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
            var portMatch = Regex.Match(commandLine ?? "", @"--app-port=""?(\d+)""?");
            var authTokenMatch = Regex.Match(commandLine ?? "", @"--remoting-auth-token=([a-zA-Z0-9_-]+)");
            if (!portMatch.Success || !authTokenMatch.Success)
            {
                Log.Warn("Failed to parse LCU auth data. portFound={0} tokenFound={1}", portMatch.Success, authTokenMatch.Success);
                return null;
            }

            string port = portMatch.Groups[1].Value;
            string authToken = authTokenMatch.Groups[1].Value;

            // Compute the encoded key
            string auth = "riot:" + authToken;
            string authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

            // Return content
            return new string[] { authBase64, port };
        }

        private static RestClient S_restClient;

        public static RestResponse clientRequest(string method, string url, object json)
        {
            var methodEnum = Enum.Parse<Method>(method, true);

            var req = new RestRequest(url, methodEnum);
            if (json != null)
                req.AddJsonBody(json, ContentType.Json);

            return clientRequest(req);
        }

        public static RestResponse clientRequest(string method, string url, string body = null)
        {
            var methodEnum = Enum.Parse<Method>(method, true);

            var req = new RestRequest(url, methodEnum);
            if (body != null)
            {
                //if (body.StartsWith("{"))
                req.AddJsonBody(body);
                //else
                //    req.AddStringBody(body, ContentType.Plain);
            }

             return clientRequest(req);
        }

        public static RestResponse clientRequest(RestRequest req)
        {
            if (!EnsureClient())
            {
                return new RestResponse(req);
            }

            Log.Debug("Initiating request {0} {1}", req.Method, req.Resource);

            //execute request
            RestResponse restResp;
            try
            {
                req.Timeout = TimeSpan.FromMilliseconds(RequestTimeoutMilliseconds);
                restResp = S_restClient.Execute(req);
            }
            catch (Exception ex)
            {
                Log.Warn(ex, "LCU request failed before response: {0} {1}", req.Method, req.Resource);
                return new RestResponse(req)
                {
                    ErrorException = ex,
                    ErrorMessage = ex.Message,
                    ResponseStatus = ResponseStatus.Error
                };
            }

            if (Log.IsDebugEnabled)
            {
                 Log.Debug("statusCode={0}, responseStatus={1}, isSuccessful={2}", restResp?.StatusCode, restResp?.ResponseStatus, restResp?.IsSuccessful);
            }

            WriteToJsonLog(req, restResp);

            return restResp ?? new RestResponse(new RestRequest());
        }

        private static Dictionary<string, string> S_JsonLog_Values = new Dictionary<string, string>();

        public static RestResponse clientRequestUntilSuccess(string method, string url, string body = null)
        {
            RestResponse request;
            int attempts = 0;
            do
            {
                request = clientRequest(method, url, body);
                if (request.IsSuccessStatusCode == true)
                {
                    return request;
                }
                else
                {
                    attempts++;
                    if (attempts < MaxRetryAttempts && CheckIfLeagueClientIsOpen())
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Log.Warn("LCU request did not succeed after {0} attempts: {1} {2} statusCode={3} responseStatus={4}",
                            attempts, method, url, request?.StatusCode, request?.ResponseStatus);
                        return request;
                    }
                }
            } while (request.IsSuccessStatusCode == false && attempts < MaxRetryAttempts);

            return request;
        }

        public static RestResponse clientRequest<TResponse>(string method, string url, object json)
        {
            var methodEnum = Enum.Parse<Method>(method, true);

            var req = new RestRequest(url, methodEnum);
            if (json != null)
                req.AddJsonBody(json, ContentType.Json);

            return clientRequest<TResponse>(req);
        }

        public static RestResponse<TResponse> clientRequest<TResponse>(string method, string url, string body = null)
        {
            var methodEnum = Enum.Parse<Method>(method, true);

            var req = new RestRequest(url, methodEnum);
            if (body != null)
            {
                //if (body.StartsWith("{"))
                req.AddJsonBody(body);
                //else
                //    req.AddStringBody(body, ContentType.Plain);
            }

            return clientRequest<TResponse>(req);
        }

        public static RestResponse<TResponse> clientRequest<TResponse>(RestRequest req)
        {
            if (!EnsureClient())
            {
                return new RestResponse<TResponse>(req);
            }

            Log.Debug("Initiating request {0} {1}", req.Method, req.Resource);

            RestResponse<TResponse> restResp;
            try
            {
                req.Timeout = TimeSpan.FromMilliseconds(RequestTimeoutMilliseconds);
                restResp = S_restClient.Execute<TResponse>(req);
            }
            catch (Exception ex)
            {
                Log.Warn(ex, "LCU request failed before response: {0} {1}", req.Method, req.Resource);
                return new RestResponse<TResponse>(req)
                {
                    ErrorException = ex,
                    ErrorMessage = ex.Message,
                    ResponseStatus = ResponseStatus.Error
                };
            }


            if (Log.IsDebugEnabled)
            {
                Log.Debug("statusCode={0}, responseStatus={1}, isSuccessful={2}", restResp.StatusCode, restResp.ResponseStatus, restResp.IsSuccessful);
                if (restResp.IsSuccessful==false)
                {
                    Log.Debug("statusDescription={0}, errorMessage={1}", restResp.StatusDescription, restResp.ErrorMessage);
                }
            }

            WriteToJsonLog(req, restResp);

            return restResp ?? new RestResponse<TResponse>(new RestRequest());
        }

        public static RestResponse<TResponse> clientRequestUntilSuccess<TResponse>(string method, string url, string body = null)
        {
            RestResponse<TResponse> request;
            int attempts = 0;
            do
            {
                request = clientRequest<TResponse>(method, url, body);
                if (request.IsSuccessStatusCode == true && request.Data != null)
                {
                    return request;
                }
                else
                {
                    attempts++;
                    if (attempts < MaxRetryAttempts && CheckIfLeagueClientIsOpen())
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Log.Warn("LCU typed request did not succeed after {0} attempts: {1} {2} statusCode={3} responseStatus={4} hasData={5}",
                            attempts, method, url, request?.StatusCode, request?.ResponseStatus, request != null && request.Data != null);
                        return request;
                    }
                }
            } while ((!request.IsSuccessStatusCode || request.Data == null) && attempts < MaxRetryAttempts);

            return request;
        }

        private static void TryApplyClientLanguage()
        {
            if (!Settings.languageAutoDetectPending)
                return;

            try
            {
                var resp = clientRequest("GET", "/riotclient/region-locale");
                if (resp == null || !resp.IsSuccessStatusCode || string.IsNullOrWhiteSpace(resp.Content))
                    return;

                using var doc = JsonDocument.Parse(resp.Content);
                if (!doc.RootElement.TryGetProperty("locale", out var localeProp))
                    return;

                string matched = Strings.MatchLocale(localeProp.GetString());
                if (matched == null)
                    return;

                Settings.currentLanguage = matched;
                Strings.CurrentLanguage = matched;
                Settings.languageAutoDetectPending = false;

                if (Settings.saveSettings)
                {
                    Settings.settingsSave();
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex, "Failed to detect League Client language from region-locale.");
            }
        }

        private static bool EnsureClient()
        {
            if (S_restClient != null)
            {
                return true;
            }

            if (leagueAuth == null || leagueAuth.Length < 2 || string.IsNullOrWhiteSpace(leagueAuth[0]) || string.IsNullOrWhiteSpace(leagueAuth[1]))
            {
                Log.Warn("LCU client cannot be created because auth data is missing.");
                return false;
            }

            S_restClient = new RestClient(c => {
                c.BaseUrl = new Uri("https://127.0.0.1:" + leagueAuth[1] + "/");

                //disable ssl checks for the local self-signed LCU certificate
                c.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicy) => true;

            }, h => {
                h.Authorization = new AuthenticationHeaderValue("Basic", leagueAuth[0]);
            });

            return true;
        }

        private static void WriteToJsonLog(RestRequest request, RestResponse restResp)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (JsonLog.IsDebugEnabled)
            {
                try
                {
                    string httprequest = $"{request.Method} {request.Resource}";
                    object body = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.RequestBody)?.Value;
                    if (body != null) httprequest = httprequest + " " + body.GetHashCode().ToStringInvariant();
                    if (S_JsonLog_Values.TryGetValue(httprequest, out string storedvalue))
                    {
                        if (storedvalue == restResp?.Content) goto skipwrite;
                    }

                    S_JsonLog_Values[httprequest] = restResp?.Content;
                    var jdoc = JsonDocument.Parse(restResp?.Content ?? "");
                    JsonLog.Debug("{0} {1}:\n{2}", request.Method, request.Resource, JsonSerializer.Serialize(jdoc, new JsonSerializerOptions() { WriteIndented = true }));

                skipwrite:
                    ;
                }
                catch { }
            }
        }

    }
}
