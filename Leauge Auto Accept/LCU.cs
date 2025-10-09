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

						//reset restclient
						S_restClient?.Dispose(); S_restClient = null;

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

        private static RestClient S_restClient;

		public static RestResponse clientRequest(string method, string url, object json)
		{
			string body = null;
			
			if (json != null) body = JsonSerializer.Serialize(json);

			return clientRequest(method, url, body);
		}

		public static RestResponse clientRequest(string method, string url, string body = null)
		{
			if (S_restClient == null)
			{
				S_restClient = new RestClient(c => {
					c.BaseUrl = new Uri("https://127.0.0.1:" + leagueAuth[1] + "/");
					//c.Authenticator = new HttpBasicAuthenticator(leagueAuth[0], "");

					//disable ssl checks
					c.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicy) => true;

				}, h => {
					h.Authorization = new AuthenticationHeaderValue("Basic", leagueAuth[0]);
				});
			}

			Log.Debug("Initiating request {0} {1}", method, url);

			RestResponse restResp = null;
			if (method == "GET")
			{
				RestRequest req = new RestRequest(url, Method.Get);
				restResp = S_restClient.ExecuteGet(req);
			}

			if (method == "POST")
			{
				RestRequest req = new RestRequest(url, Method.Post);
				if (body != null) req.AddStringBody(body, ContentType.Plain);
				restResp = S_restClient.ExecutePost(req);
			}

			if (method == "PUT")
			{
				RestRequest req = new RestRequest(url, Method.Post);
				if (body != null) req.AddStringBody(body, ContentType.Json);
				restResp = S_restClient.ExecutePut(req);
			}


			if (method == "PATCH")
			{
				RestRequest req = new RestRequest(url, Method.Post);
				if (body != null) req.AddStringBody(body, ContentType.Json);
				restResp = S_restClient.ExecutePatch(req);
			}

			if (method == "DELETE")
			{
				RestRequest req = new RestRequest(url, Method.Delete);
				restResp = S_restClient.ExecuteDelete(req);
			}

			if (Log.IsDebugEnabled)
			{
				Log.Debug("statusCode={0}, isSuccessful={1}", restResp?.StatusCode, restResp?.IsSuccessful);
			}

			if (JsonLog.IsDebugEnabled)
			{
				try
				{
					string httprequest = $"{method} {url}";
					if (body != null) httprequest = string.Concat(httprequest, " ", body.GetHashCode().ToStringInvariant());
					if (S_JsonLog_Values.TryGetValue(httprequest, out string storedvalue))
					{
						if (storedvalue == restResp?.Content) goto skipwrite;
					}

					S_JsonLog_Values[httprequest] = restResp?.Content;
					var jdoc = JsonDocument.Parse(restResp?.Content ?? "");
					JsonLog.Debug("{0} {1}:\n{2}", method, url, JsonSerializer.Serialize(jdoc, new JsonSerializerOptions() { WriteIndented = true }));

				skipwrite:
					;
				}
				catch { }
			}

			return restResp ?? new RestResponse(new RestRequest());
		}


		private static Dictionary<string, string> S_JsonLog_Values = new Dictionary<string, string>();

        public static RestResponse clientRequestUntilSuccess(string method, string url, string body = null)
        {
            RestResponse request;
			do
			{
				request = clientRequest(method, url, body);
				if (request.IsSuccessStatusCode == true)
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
			} while (request.IsSuccessStatusCode == false);

			return request;
        }

		public static RestResponse<TResponse> clientRequest<TResponse>(string method, string url, string body = null)
		{
			if (S_restClient == null)
			{
				S_restClient = new RestClient(c => {
					c.BaseUrl = new Uri("https://127.0.0.1:" + leagueAuth[1] + "/");
					//c.Authenticator = new HttpBasicAuthenticator(leagueAuth[0], "");

					//disable ssl checks
					c.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicy) => true;

				}, h => {
					h.Authorization = new AuthenticationHeaderValue("Basic", leagueAuth[0]);
				});
			}

			Log.Debug("Initiating request {0} {1}", method, url);

			RestResponse<TResponse> restResp = null;
			if (method == "GET")
			{
				RestRequest req = new RestRequest(url, Method.Get);
				restResp = S_restClient.ExecuteGet<TResponse>(url);
			}

			if (method == "POST")
			{
				RestRequest req = new RestRequest(url, Method.Post);
				req.AddStringBody(body, ContentType.Plain);
				restResp = S_restClient.ExecutePost<TResponse>(req);
			}

			if (method == "PUT")
			{
				RestRequest req = new RestRequest(url, Method.Post);
				if (body != null) req.AddStringBody(body, ContentType.Json);
				restResp = S_restClient.ExecutePut<TResponse>(req);
			}

			if (method == "PATCH")
			{
				RestRequest req = new RestRequest(url, Method.Post);
				if (body != null) req.AddStringBody(body, ContentType.Json);
				restResp = S_restClient.ExecutePatch<TResponse>(req);
			}

			if (method == "DELETE")
			{
				restResp = S_restClient.ExecuteDelete<TResponse>(url);
			}

			if (Log.IsDebugEnabled)
			{
				Log.Debug("statusCode={0}, isSuccessful={1}", restResp.StatusCode, restResp.IsSuccessful);
			}

			if (JsonLog.IsDebugEnabled)
			{
				try
				{
					string httprequest = $"{method} {url}";
					if (body != null) httprequest = string.Concat(httprequest, " ", body.GetHashCode().ToStringInvariant());
					if (S_JsonLog_Values.TryGetValue(httprequest, out string storedvalue))
					{
						if (storedvalue == restResp?.Content) goto skipwrite;
					}

					S_JsonLog_Values[httprequest] = restResp?.Content;
					var jdoc = JsonDocument.Parse(restResp?.Content ?? "");
					JsonLog.Debug("{0} {1}:\n{2}", method, url, JsonSerializer.Serialize(jdoc, new JsonSerializerOptions() { WriteIndented = true }));

				skipwrite:
					;
				}
				catch { }
			}

			return restResp ?? new RestResponse<TResponse>(new RestRequest());
		}

		public static RestResponse<TResponse> clientRequestUntilSuccess<TResponse>(string method, string url, string body = null)
		{
			RestResponse<TResponse> request;
			do
			{
				request = clientRequest<TResponse>(method, url, body);
				if (request.IsSuccessStatusCode == true)
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
			} while (request.IsSuccessStatusCode == false);

			return request;
		}
	}
}
