using System;
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

namespace VcComHub.LanguageParser
{
    public class Language
    {
        private static readonly List<LanguageStruct> languageList = new List<LanguageStruct>();

        /// <summary>
        /// Načte do paměti jazykový soubor.
        /// </summary>
        /// <param name="languageFile"></param>
        public static void LoadFile(string languageFile)
        {
            if (File.Exists(languageFile))
            {
                string content = File.ReadAllText(languageFile);

                foreach (var context in content.Split('\n'))
                {
                    if (context.StartsWith("#"))
                    {
                        string indexStr = context.Remove(0, 1);
                        indexStr = indexStr.Substring(0, 4);

                        if (int.TryParse(indexStr, out int index))
                        {
                            string text = context.Split(new[] { "#" + index }, System.StringSplitOptions.None)[1].Trim();
                            languageList.Add(new LanguageStruct { Index = index, Text = text });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Získá jméno jazyka z jazykového souboru.
        /// </summary>
        /// <param name="languageFile"></param>
        /// <returns></returns>
        public static string GetName(string languageFile)
        {
            return GetProperty(languageFile, "lang_name");
        }

        /// <summary>
        /// Získá kód jazyka z jazykového souboru.
        /// </summary>
        /// <param name="languageFile"></param>
        /// <returns></returns>
        public static string GetCode(string languageFile)
        {
            return GetProperty(languageFile, "lang_code");
        }

        /// <summary>
        /// Získá vlastnost z jazykového souboru.
        /// </summary>
        /// <param name="languageFile"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetProperty(string languageFile, string property)
        {
            if (File.Exists(languageFile))
            {
                string content = File.ReadAllText(languageFile);

                foreach (var context in content.Split('\n'))
                {
                    if (context.Contains(":"))
                    {
                        string key = context.Split(':')[0].Trim();
                        string val = context.Split(':')[1].Trim();

                        if (key == property)
                        {
                            return val;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Získá string na danném indexu a vrátí jej pokud exisstuje.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetStr(int index)
        {
            foreach (var context in languageList)
            {
                if (context.Index == index)
                {
                    string outputStr = context.Text;

                    outputStr = outputStr.Replace("\\", Environment.NewLine).Trim();

                    return outputStr;
                }
            }

            return null;
        }

    }
}
