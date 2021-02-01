using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
//using Newtonsoft.Json;

namespace Leauge_Auto_Accept
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("##################################");
            Console.WriteLine("# Current state: Initializing    #");
            Console.WriteLine("##################################");
            // Find League client
            Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
            if (client == null)
            {
                // If League client cannot be found just kill the app
                // TODO: Maybe it should reattempt to find it?
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(" League client could not be found ");
                Console.ReadKey();
            } else
            {
                // Get the auth key and port for the LCU API
                string[] leagueAuth = getLeagueAuth(client);
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("# Current state: Initializing    #");

                // Start the loop
                while (true)
                {
                    // Get the current game session
                    string[] gameSession = clientRequest(leagueAuth, "GET", "lol-gameflow/v1/session", "");
                    if (gameSession[0] == "200")
                    {
                        /*// Originally I wanted to parse the JSON but then I decided against it to save on file size
                        var definition = new { phase = "" };
                        var result = JsonConvert.DeserializeAnonymousType(gameSession[1], definition);
                        var phase = result["phase"];*/

                        // Get the current game state
                        string phase = gameSession[1].Split(new string[] { "phase" }, StringSplitOptions.None)[2].Split('"')[2];

                        // Change the state on the console, accept the queue if need to
                        switch (phase)
                        {
                            case "Lobby":
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("# Current state: In lobby        #");
                                break;
                            case "Matchmaking":
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("# Current state: In queue        #");
                                break;
                            case "ReadyCheck":
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("# Current state: Accept ready    #");
                                // Accept queue if found
                                string[] matchAccept = clientRequest(leagueAuth, "POST", "lol-matchmaking/v1/ready-check/accept", "");
                                if (matchAccept[0] == "204")
                                {
                                    Console.SetCursorPosition(0, 1);
                                    Console.WriteLine("# Current state: Match accepted  #");
                                }
                                else
                                {
                                    // This probably shouldn't happen?
                                    Console.SetCursorPosition(0, 1);
                                    Console.WriteLine("# Current state: Accept failed   #");
                                }
                                break;
                            case "ChampSelect":
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("# Current state: In champ select #");
                                break;
                            case "InProgress":
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("# Current state: In game         #");
                                // No need to spam requests while in game
                                Thread.Sleep(9000);
                                break;
                            default:
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("# Current state: Not in game     #");
                                break;
                        }
                    } else if (gameSession[0] == "404")
                    {
                        // Chances are we are just not in game
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("# Current state: Not in game     #");
                    } else if (gameSession[0] == "999")
                    {
                        // League client was closed
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("# League client was closed       #");
                        break;
                    } else
                    {
                        // Probably shouldn't get here
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("# Something weird happen         #");
                        break;
                    }
                    Thread.Sleep(1000);
                }
                // Don't close the app until there's a button press
                Console.ReadKey();
            }
        }

        static string[] getLeagueAuth(Process client)
        {
            // Open a cmd window
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            // Get the command line for our process
            cmd.StandardInput.WriteLine("wmic process where 'Processid="+client.Id+"' get Commandline");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            // Parse the port and key into variables
            string Commandline = cmd.StandardOutput.ReadToEnd();
            string port = Commandline.Split(new string[] { "--app-port=" }, StringSplitOptions.None)[1].Split('"')[0];
            string key = Commandline.Split(new string[] { "--remoting-auth-token=" }, StringSplitOptions.None)[1].Split('"')[0];

            // Get the encoded auth key
            string auth = "riot:" + key;
            var authPlainText = System.Text.Encoding.UTF8.GetBytes(auth);
            string authBase64 = System.Convert.ToBase64String(authPlainText);

            // Return content
            string[] output = { authBase64, port };
            return output;
        }

        static string[] clientRequest(string[] leagueAuth, string method, string url, string body)
        {
            // Ignore invalid https
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            {
                // Set URL
                WebRequest request = WebRequest.Create("https://127.0.0.1:" + leagueAuth[1] + "/" + url);

                // Set headers
                request.Method = method;
                request.Headers.Add("Authorization", "Basic " + leagueAuth[0]);
                request.ContentType = "application/json";

                // Send POST data when doing a post request
                Stream dataStream;
                if (method == "POST")
                {
                    string postData = body;
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = byteArray.Length;
                    dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                // Get the response
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        // If the response is null (League client closed?)
                        if (((HttpWebResponse)response) == null)
                        {
                            string[] outputDef = { "999", "" };
                            return outputDef;
                        }

                        // Get the HTTP status code
                        int statusCode = (int)((HttpWebResponse)response).StatusCode;
                        string statusString = statusCode.ToString();

                        // Get the body
                        string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();

                        // Clean up the stream
                        response.Close();

                        // Return content
                        string[] output = { statusString, responseFromServer };
                        return output;
                    }
                }
                catch (WebException e)
                {
                    using (WebResponse response = e.Response)
                    {
                        // If the response is null (League client closed?)
                        if (((HttpWebResponse)response) == null)
                        {
                            string[] outputDef = { "999", "" };
                            return outputDef;
                        }

                        // Get the HTTP status code
                        int statusCode = (int)((HttpWebResponse)response).StatusCode;
                        string statusString = statusCode.ToString();

                        // Get the body
                        dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();

                        // Clean up the stream
                        reader.Close();
                        dataStream.Close();
                        response.Close();

                        // Return content
                        string[] output = { statusString, responseFromServer };
                        return output;
                    }
                }
            }
            catch
            {
                // If the URL is invalid (League client closed?)
                string[] output = { "999", "" };
                return output;
            }
        }
    }
}
