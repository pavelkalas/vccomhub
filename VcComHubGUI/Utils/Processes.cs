using System.Diagnostics;

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

namespace VcComHubGUI
{
    /// <summary>
    /// Tato třída slouží pro práci s procesama.
    /// </summary>
    internal class Processes
    {
        /// <summary>
        /// Zkontroluje zda process ze zadaným jménem existuje a běží
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsProcessRunning(string name)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Zkontroluje zda process se zadaným ID existuje a běží
        /// </summary>
        /// <param name="pID"></param>
        /// <returns></returns>
        public static bool IsProcessRunning(int pID)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.Id == pID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Ukončí všechny procesy s dannym jménem.
        /// </summary>
        /// <param name="name"></param>
        public static void KillAllProcesses(string name)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == name)
                {
                    process.Kill();
                }
            }
        }

        /// <summary>
        /// Ukončí všechny procesy s dannym ID.
        /// </summary>
        /// <param name="name"></param>
        public static void KillAllProcesses(int pID)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.Id == pID)
                {
                    process.Kill();
                }
            }
        }

        /// <summary>
        /// Zkontroluje zda existuje process se specifickým nadpisem podle ID procesu.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static bool WindowExists(string title, int processId)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.Id == processId && process.MainWindowTitle == title)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
