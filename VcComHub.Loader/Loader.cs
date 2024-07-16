using System.Diagnostics;
using System.Linq;

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

namespace VcComHub.Loader
{
    /// <summary>
    /// Třída pro spouštěcí manipulaci s hrou.
    /// </summary>
    public class Loader
    {
        /// <summary>
        /// Spustí proces v pracovní složce s addonem a případnýma dalšíma vstupníma argumentama
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="addon"></param>
        /// <param name="additionalArgs"></param>
        public static void StartProcess(string moduleName, string workingDirectory, string addon, string[] additionalArgs)
        {
            string otherArguments = "";

            if (additionalArgs != null)
            {
                foreach (var args in additionalArgs)
                {
                    otherArguments += " " + args;
                }
            }

            otherArguments = otherArguments.Trim();

            // Vytvoření struktury procesu
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            if (addon != null)
            {
                otherArguments = "-addon \"" + addon + "\" " + otherArguments;
            }

            // Skočí na disk kde je hra, skočí do složky a spustí hru s případnýma parametrama.
            process.Start();
            process.StandardInput.WriteLine("" + workingDirectory.First() + ":");
            process.StandardInput.WriteLine("cd \"" + workingDirectory + "\"");
            process.StandardInput.WriteLine("start \"\" \"" + moduleName + "\" " + otherArguments);
            process.Close();
        }
    }
}
