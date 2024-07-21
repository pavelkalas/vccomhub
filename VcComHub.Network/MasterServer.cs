using System;
using System.Collections.Generic;
using System.Net;

// Copyright (c) 2024 Delta Devs. All rights reserved.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to use
// the Software for personal, educational, and research purposes, including the
// rights to use, copy, modify, merge, publish, distribute copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// The Software is provided "as is", without warranty of any kind, express or implied,
// including but not limited to the warranties of merchantability, fitness for a particular
// purpose and noninfringement. In no event shall the authors or copyright holders be liable
// for any claim, damages or other liability, whether in an action of contract, tort or
// otherwise, arising from, out of or in connection with the Software or the use or other
// dealings in the Software.
// 
// Distribution and/or publication of the Software, modified or unmodified, to the public
// is strictly prohibited.
// 
// Developed by Pavel Kalaš 2024-present.

namespace VcComHub.Network
{
    /// <summary>
    /// Třída sloužící pro práci s master serverem hry.
    /// </summary>
    public class MasterServer
    {
        public static List<NetworkStruct> FetchMasterserver()
        {
            WebClient webClient = new WebClient();

            string[] output = webClient.DownloadString("https://vietcong1.eu/en/game/#serversList-perPage=100").Split('\n');

            List<NetworkStruct> connInfo = new List<NetworkStruct>();

            int counter = 0;
            foreach (var context in output)
            {
                string line = context.Trim();

                if (line.Trim().Length == 0) continue;

                if (line.Contains("<a href=\"/en/game/server/"))
                {
                    counter++;

                    string name = line.Split(new[] { "\">" }, StringSplitOptions.None)[1].Trim();
                    name = name.Split(new[] { "</a>" }, StringSplitOptions.None)[0].Trim();

                    name = name.Replace("&lt;", "<");
                    name = name.Replace("&gt;", ">");

                    string ip = line.Split(new[] { "<a href=\"/en/game/server/" }, StringSplitOptions.None)[1].Trim();

                    if (ip.Contains(":"))
                    {
                        ip = ip.Split(':')[0].Trim();
                    }
                    else
                    {
                        ip = ip.Split(new[] { "\">" }, StringSplitOptions.None)[0].Trim();
                    }

                    string port = "5425";

                    if (line.Contains(ip + ":"))
                    {
                        port = line.Split(':')[1].Trim();
                        port = port.Split(new[] { "\">" }, StringSplitOptions.None)[0].Trim();
                    }

                    connInfo.Add(new NetworkStruct { Id = counter, IpAddress = ip, Port = int.Parse(port), ServerName = name });
                }
            }

            counter = 0;
            int server = 0;
            foreach (var line in output)
            {
                counter++;

                if (line.Contains("<td class=\"grid-cell-numplayers\">"))
                {
                    server++;

                    if (output[counter].Contains("/") && output[counter].Trim().Length < 6)
                    {
                        string[] playerArgs = output[counter].Trim().Split('/');

                        for (int i = 0; i < connInfo.Count; i++)
                        {
                            if (connInfo[i].Id == server)
                            {
                                connInfo[i].PlayersCount = int.Parse(playerArgs[0]);
                                connInfo[i].MaxPlayersCount = int.Parse(playerArgs[1]);
                                break;
                            }
                        }
                    }
                }
            }

            counter = 0;
            server = 0;
            foreach (var line in output)
            {
                counter++;

                if (line.Contains("<td class=\"grid-cell-mode\">"))
                {
                    server++;

                    for (int i = 0; i < connInfo.Count; i++)
                    {
                        if (connInfo[i].Id == server)
                        {
                            connInfo[i].Mode = output[counter].Trim();
                            break;
                        }
                    }
                }
            }

            counter = 0;
            server = 0;
            foreach (var line in output)
            {
                counter++;

                if (line.Contains("<td class=\"grid-cell-map\">"))
                {
                    server++;

                    for (int i = 0; i < connInfo.Count; i++)
                    {
                        if (connInfo[i].Id == server)
                        {
                            string map = output[counter].Trim();
                            map = map.Split(new[] { "\">" }, StringSplitOptions.None)[1].Trim();
                            map = map.Split('<')[0].Trim();
                            connInfo[i].MapName = map;

                            break;
                        }
                    }
                }
            }

            counter = 0;
            server = 0;
            foreach (var line in output)
            {
                counter++;

                if (line.Contains("<td class=\"grid-cell-password center center center center center center center center center center center center center center center center center center center center\">"))
                {
                    server++;

                    for (int i = 0; i < connInfo.Count; i++)
                    {
                        if (connInfo[i].Id == server)
                        {
                            string passworded = output[counter].Trim();
                            connInfo[i].Passworded = passworded.Contains("<i class=\"fa fa-lock\"></i>");
                            break;
                        }
                    }
                }
            }

            return connInfo;
        }
    }
}
