using DiscordRPC;
using DiscordRPC.Logging;
using System.Threading;
using System.Windows.Forms.VisualStyles;

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

namespace VcComHubGUI.Utils
{
    /// <summary>
    /// Třída sloužící pro správu rich presance pro VcComHub.
    /// </summary>
    internal class DiscordRichPresence
    {
        public static DiscordRpcClient client;

        private static bool isRunning = false;

        /// <summary>
        /// Spustí vlákno s discord rich presence
        /// </summary>
        public static void StartTask(string state, string details)
        {
            if (state == null || details == null) return;

            isRunning = true;

            client = new DiscordRpcClient("1259775254679523388")
            {
                Logger = new ConsoleLogger() { Level = LogLevel.None }
            };

            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "large_icon",
                    LargeImageText = "My large icon",
                    SmallImageKey = "small_icon"
                }
            });

            while (isRunning)
            {
                client.Invoke();
                Thread.Sleep(500);

                if (!isRunning)
                {
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// Zastaví vlákno discord rich presence
        /// </summary>
        public static void StopTask()
        {
            isRunning = false;
        }
    }
}
