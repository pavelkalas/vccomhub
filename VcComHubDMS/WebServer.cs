using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VcComHub.Network;

namespace VcComHubDMS
{
    internal class WebServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        private static string responseData = "";
        private static string masterData = "";

        /// <summary>
        /// Handler pro webový server.
        /// </summary>
        /// <returns></returns>
        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if (req.HttpMethod == "GET")
                {
                    string route = req.Url.AbsolutePath.Remove(0, 1);

                    if (route == "msrequest")
                    {
                        if (masterData.Trim().Length == 0)
                        {
                            responseData = "<pre>no healthy upstream</pre>";
                        }
                        else
                        {
                            responseData = masterData;
                        }
                    }
                    else
                    {
                        responseData = "<h2>Unknown or empty request</h2>";
                    }
                }

                byte[] data = Encoding.UTF8.GetBytes(responseData);
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        /// <summary>
        /// Funkce která neustále obnovuje data z masterserveru.
        /// </summary>
        public static void GetData()
        {
            while (true)
            {
                Console.WriteLine("[VcComHubDMS] Fetching data from the masterserver..");
                var ms = MasterServer.FetchMasterserver();

                responseData = "<pre>[\n";
                foreach (var server in ms)
                {
                    responseData += "   {\"name\":\"" + server.ServerName + "\", \"players\": \"" + server.PlayersCount + "\", \"max-players\":\"" + server.MaxPlayersCount + "\", \"ip\":\"" + server.IpAddress + "\", \"port\":\"" + server.Port + "\", \"map\":\"" + server.MapName + "\", \"mode\":\"" + server.Mode + "\", \"is_password_protected\":\"" + server.Passworded.ToString().ToLower() + "\"},\n";
                }
                responseData += "],\n[\n";
                responseData += "   {\"last-fetch\":\"" + DateTime.Now + "\", \"response\": \"200 OK\", \"request-rayid\":\"" + new Random().Next(111111, 999999) + "\"}\n";
                responseData += "];</pre>\n";
                masterData = responseData;

                Console.WriteLine("[VcComHubDMS] Got data, " + ms.Count + " servers found! Sleeping 60 seconds before next fetch..\n");

                Thread.Sleep(1000 * 60 * 1);
            }
        }

        /// <summary>
        /// Spouští hlavní vláko které spustí funkci pro stahování dat masterserveru a spustí webserver.
        /// </summary>
        /// <param name="args"></param>
        public static void RunTask()
        {
            try
            {
                new Thread(GetData).Start();
                Console.WriteLine("[VcComHubDMS] Done! Running with 0 errors.\n\n");

                listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();
                Task listenTask = HandleIncomingConnections();
                listenTask.GetAwaiter().GetResult();
                listener.Close();
            }
            catch { Console.WriteLine("[VcComHubDMS] Chyba!\nPress any key to continue . . ."); Console.ReadKey(); Environment.Exit(0); }
        }
    }
}
