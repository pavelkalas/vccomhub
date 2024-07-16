using System;
using System.IO;
using System.Reflection;

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
    /// Třída pro kontrolu a ověření platnosti COM knihoven.
    /// </summary>
    internal class LibraryChecker
    {
        /// <summary>
        /// Spustí proces kdy se ověří zda knihovna existuje a zda je validní pro VcComHub.
        /// </summary>
        /// <param name="libs"></param>
        public static void Init(string[] libs)
        {
            foreach (var lib in libs)
            {
                bool exists = LibExists(lib);

                if (!exists)
                {
                    Logger.LogError("DLL Library " + lib + " does not exists!", "VcComHub - Error");
                    Environment.Exit(0);
                }
                else
                {
                    bool isValid = IsValidAssembly(lib);

                    if (!isValid && lib.StartsWith("VcComHub."))
                    {
                        Logger.LogError("DLL Library " + lib + " is not valid COM assembly!", "VcComHub - Error");
                        Environment.Exit(0);
                    }
                }
            }
        }

        /// <summary>
        /// Zkontroluje zda existuje knihovna.
        /// </summary>
        /// <param name="libName"></param>
        /// <returns></returns>
        public static bool LibExists(string libName)
        {
            return File.Exists(Constants.workingDirectory + "\\" + libName);
        }

        /// <summary>
        /// Zkontroluje zda je knihovna validní a platná pro COM.
        /// </summary>
        /// <param name="libName"></param>
        /// <returns></returns>
        public static bool IsValidAssembly(string libName)
        {
            try
            {
                // Načtení obsahu souboru jako byte array
                byte[] assemblyBytes = System.IO.File.ReadAllBytes(Constants.workingDirectory + "\\" + libName);

                // Načtení Assembly z pole bytů
                Assembly assembly = Assembly.Load(assemblyBytes);

                // Získání atributů z Assembly
                object[] attributes = assembly.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    // Kontrola potřebných atributů
                    if (attribute is AssemblyTitleAttribute ||
                        attribute is AssemblyDescriptionAttribute ||
                        attribute is AssemblyCompanyAttribute ||
                        attribute is AssemblyProductAttribute)
                    {
                        return true; // Nalezen alespoň jeden požadovaný atribut
                    }
                }

                return false; // Žádný z požadovaných atributů nebyl nalezen
            }
            catch
            {
                // V případě jakékoliv chyby automaticky považujeme soubor za neplatný
                return false;
            }
        }
    }
}
