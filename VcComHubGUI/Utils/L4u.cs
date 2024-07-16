using System;
using System.IO;
using VcComHub.ConfigParser;

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
    /// Třída která se stará o zapisování událostí do souboru.
    /// </summary>
    internal class L4u
    {
        public enum LogType
        {
            ERROR,
            WARNING,
            INFO,
            EVENT,
        };

        /// <summary>
        /// Vytvoří log soubor.
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateLog(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, "");
            }
        }

        /// <summary>
        /// Zapíše událost do souboru.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        public static void WriteLog(string fileName, string message, LogType logType)
        {
            try
            {
                if (Config.IssetKey("debugging.level"))
                {
                    string debuggingLevel = Config.GetConfig("debugging.level");

                    if (debuggingLevel == "0")
                    {
                        return;
                    }
                }

                if (Config.IssetKey("debugging.filter"))
                {
                    string debuggingFilters = Config.GetConfig("debugging.filter");

                    if (!debuggingFilters.Contains(logType.ToString()))
                    {
                        return;
                    }
                }

                string dateTime = DateTime.Now.ToString();

                int lvl = (int)logType;

                string content = "";

                if (File.Exists(fileName))
                    content = File.ReadAllText(fileName);

                if (lvl == 0)
                    message = "    [" + dateTime + "][ERROR]: " + message;

                if (lvl == 1)
                    message = "    [" + dateTime + "][WARNING]: " + message;

                if (lvl == 2)
                    message = "    [" + dateTime + "][INFO]: " + message;

                if (lvl == 3)
                    message = "    [" + dateTime + "][EVENT]: " + message;

                content += message.TrimEnd() + "\n";

                File.WriteAllText(fileName, content);
            }
            catch { }
        }

        /// <summary>
        /// Vytvoří nový informace o budoucím logu.
        /// </summary>
        /// <param name="fileName"></param>
        public static void NewStage(string fileName)
        {
            if (Config.IssetKey("debugging.level"))
            {
                string debuggingLevel = Config.GetConfig("debugging.level");

                if (debuggingLevel == "0")
                {
                    return;
                }
            }

            try
            {
                string content = "";

                if (File.Exists(fileName))
                    content = File.ReadAllText(fileName);

                content += "\n\nNew stage created:\n Datetime: " + DateTime.Now.ToString() + "\n Username@domain: " + Environment.UserName + "@" + Environment.UserDomainName + "\n Computer name: " + Environment.MachineName + "\n Working directory: " + Environment.CurrentDirectory + "\n  Logging below:\n";

                File.WriteAllText(fileName, content);
            }
            catch { }
        }
    }
}
