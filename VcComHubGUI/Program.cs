using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VcComHub.ConfigParser;
using VcComHub.FileSystem;
using VcComHubGUI.GUIs;
using VcComHubGUI.Utils;

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
    /// Spouštěcí třída programu.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Hlavní vstupní bod aplikace.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Config.ConfigExists())
            {
                Config.CreateConfig(Constants.config);
            }

            string[] requiredLibs =
            {
                "VcComHub.ConfigParser.dll",
                "VcComHub.FileSystem.dll",
                "VcComHub.LanguageParser.dll",
                "VcComHub.Loader.dll",
                "VcComHub.Memory.dll",
                "VcComHub.Network.dll",
                "VcComHub.Automation.dll",
                "logs.dll",
                "game.dll"
            };

            LibraryChecker.Init(requiredLibs);

            if (!LibraryChecker.LibExists("VcComHub.EngineInjector.dll"))
            {
                Logger.LogError("DLL Library VcComHub.EngineInjector.dll does not exists!", "VcComHub - Error");
                Environment.Exit(0);
            }

            Config.LoadConfig();

            foreach (var defaultConfig in Constants.config)
            {
                if (defaultConfig.Contains("=") && !defaultConfig.Contains("]") && !defaultConfig.Contains("]"))
                {
                    string defKey = defaultConfig.Split('=')[0].Trim();

                    bool found = false;
                    foreach (var config in Config.GetConfig())
                    {
                        string key = config.Key.Trim();

                        if (key == defKey)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Logger.LogError("Missing configuration options (" + defKey + ")\n\nplease, delete vccomhub.ini and regenerate again if you are not sure with a key of value or edit it manually!", "VcComHub - Error");
                        Environment.Exit(0);
                    }
                }
            }

            if (!FileSystem.DirectoryExists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            L4u.CreateLog("logs\\errors.txt");
            L4u.CreateLog("logs\\warnings.txt");
            L4u.CreateLog("logs\\infos.txt");
            L4u.CreateLog("logs\\events.txt");

            L4u.NewStage("logs\\errors.txt");
            L4u.NewStage("logs\\warnings.txt");
            L4u.NewStage("logs\\infos.txt");
            L4u.NewStage("logs\\events.txt");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadingWindow loadWindow = new LoadingWindow();
            MainWindow mainWindow = new MainWindow();

            loadWindow.ShowDialog();

            Application.Run(mainWindow);            
        }
    }
}
