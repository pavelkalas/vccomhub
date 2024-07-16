using System.Collections.Generic;
using System.IO;

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

namespace VcComHub.ConfigParser
{
    /// <summary>
    /// Třída pro správu konfigurací.
    /// </summary>
    public class Config
    {
        private static readonly List<ConfigStruct> configList = new List<ConfigStruct>();

        /// <summary>
        /// Vytvoří konfigurační soubor.
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="configList"></param>
        public static void CreateConfig(List<string> configList, string configPath = "vccomhub.ini")
        {
            File.WriteAllLines(configPath, configList);
        }

        /// <summary>
        /// Zkontroluj jestli konfigurační soubor existuje.
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static bool ConfigExists(string configPath = "vccomhub.ini")
        {
            return File.Exists(configPath);
        }

        /// <summary>
        /// Načte konfiguraci ze souboru do paměti.
        /// </summary>
        /// <param name="configPath"></param>
        public static void LoadConfig(string configPath = "vccomhub.ini")
        {
            if (configList.Count > 0)
            {
                configList.Clear();
            }

            string[] configArray = File.ReadAllText(configPath).Split('\n');

            foreach (var configLine in configArray)
            {
                if (configLine.Contains("="))
                {
                    string key = configLine.Split('=')[0].Trim();
                    string val = configLine.Split('=')[1].Trim();

                    configList.Add(new ConfigStruct { Key = key, Value = val });
                }
            }
        }

        /// <summary>
        /// Získá hodnotu podle klíče konfigurace.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfig(string key)
        {
            foreach (var configLine in configList)
            {
                if (key == configLine.Key)
                {
                    return configLine.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Získá hodnotu podle klíče konfigurace.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<ConfigStruct> GetConfig()
        {
            return configList;
        }

        /// <summary>
        /// Změní hodnotu klíče v konfiguračním souboru.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="configPath"></param>
        public static void ChangeConfig(string key, string newValue, string configPath = "vccomhub.ini")
        {
            string fileContent = File.ReadAllText(configPath);

            string[] configArray = fileContent.Split('\n');
            string selectedConfigLine = null;

            foreach (var configLine in configArray)
            {
                string cKey = configLine.Split('=')[0].Trim();

                if (cKey == key)
                {
                    selectedConfigLine = configLine;
                    break;
                }
            }

            if (selectedConfigLine != null)
            {
                File.WriteAllText(configPath, fileContent.Replace(selectedConfigLine, key.Trim() + " = " + newValue.Trim()));
            }
        }

        /// <summary>
        /// Vymaže konfiguraci z paměti a načte jí znova ze souboru.
        /// </summary>
        /// <param name="configPath"></param>
        public static void ReloadConfig(string configPath = "vccomhub.ini")
        {
            if (configList.Count > 0)
            {
                configList.Clear();
            }

            LoadConfig(configPath);
        }

        /// <summary>
        /// Zkontroluje zda v konfiguračním souboru existuje klíč.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IssetKey(string key)
        {
            foreach (var configLine in configList)
            {
                if (configLine.Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Zkontroluje zda je konfigurace nulová nebo prázdná.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IssetValue(string key)
        {
            foreach (var configLine in configList)
            {
                if (configLine.Key == key)
                {
                    if (configLine.Value.Trim().Length != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
