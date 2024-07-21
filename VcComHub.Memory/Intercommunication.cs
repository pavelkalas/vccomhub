using System.IO;
using System.IO.Pipes;

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

namespace VcComHub.Memory
{
    /// <summary>
    /// Třída pro správu interkomunikace mezi VcComHub a hrou.
    /// </summary>
    public class Intercommunication
    {
        private static int pid;

        /// <summary>
        /// Nastavit pID pro proces kde má probíhat interkomunikace.
        /// </summary>
        /// <param name="pID"></param>
        public static void Setup(int pID)
        {
            pid = pID;
        }

        /// <summary>
        /// Odešle data na předdefinované namedpipes.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pID"></param>
        public static void SendDataToMemory(string text)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pid.ToString(), PipeDirection.Out))
            {
                pipeClient.Connect();

                using (StreamWriter sw = new StreamWriter(pipeClient))
                {
                    sw.WriteLine(text);
                    sw.Flush();
                }
            }
        }

        /// <summary>
        /// Pošle string do konzole hry.
        /// </summary>
        /// <param name="str"></param>
        public static void SendString(string str)
        {
            SendDataToMemory(str);
        }

        /// <summary>
        /// Nastaví status konzole, TRUE = otevřená | FALSE = zavřená.
        /// </summary>
        /// <param name="isOpen"></param>
        public static void ConsoleOpened(bool isOpen)
        {
            if (isOpen)
            {
                SendDataToMemory("open+");
            }
            else
            {
                SendDataToMemory("close+");
            }
        }

        /// <summary>
        /// Funkce přes kterou mohu odeslat zprávu na serveru jako hráč.
        /// </summary>
        /// <param name="str"></param>
        public static void SendMessageToServerAsPlayer(string str, bool publicChat)
        {
            if (publicChat)
            {
                SendDataToMemory("cmd+say \"" + str.Trim().Replace("\"", "''") + "\"");
            }
            else
            {
                SendDataToMemory("cmd+sayteam \"" + str.Trim().Replace("\"", "''") + "\"");
            }
        }

        /// <summary>
        /// Odešle příkaz do hry do konzole.
        /// </summary>
        /// <param name="str"></param>
        public static void ConsoleCommand(string str)
        {
            SendDataToMemory("cmd+" + str);
        }

        /// <summary>
        /// Vymaže konzoli ve hře.
        /// </summary>
        public static void ConsoleClear()
        {
            SendDataToMemory("clear+");
        }
    }
}
