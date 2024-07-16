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

namespace VcComHub.FileSystem
{
    /// <summary>
    /// Tato třída slouží pro manipulaci se souborama a složkama.
    /// </summary>
    public class FileSystem
    {
        /// <summary>
        /// Zkontroluje zda soubor existuje v cestě.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// Vytvoří nový soubor a zapíše do něj obsah.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public static void CreateNewFile(string filePath, string fileContent)
        {
            File.WriteAllText(filePath, fileContent);
        }

        /// <summary>
        /// Vytvoří nový soubor a zapíše do něj obsah z Listu.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public static void CreateNewFile(string filePath, List<string> fileContent)
        {
            File.WriteAllLines(filePath, fileContent);
        }

        /// <summary>
        /// Připíše na konec souboru data pokud soubor existuje.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public static void AppendToFile(string filePath, string fileContent)
        {
            if (FileExists(filePath))
            {
                string originContent = File.ReadAllText(filePath);
                originContent += fileContent + "\n";
                File.WriteAllText(filePath, originContent);
            }
        }

        /// <summary>
        /// Získá velikost souboru.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            if (FileExists(filePath))
            {
                return new FileInfo(filePath).Length;
            }

            return -1;
        }

        /// <summary>
        /// Získá array řádků ze souboru pokud soubor existuje.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] GetFileLines(string filePath)
        {
            if (FileExists(filePath))
            {
                return File.ReadAllLines(filePath);
            }

            return null;
        }

        /// <summary>
        /// Vytvoří novou složku v cestě.
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// Zkontroluje zda složka existuje v cestě.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// Smaže složku v cestě.
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteDirectory(string directoryPath, bool forceRecursive)
        {
            if (DirectoryExists(directoryPath))
            {
                Directory.Delete(directoryPath, forceRecursive);
            }
        }
    }
}
