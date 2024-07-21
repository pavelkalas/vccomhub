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
    /// Třída pro kontrolu textů.
    /// </summary>
    internal class TextManager
    {
        private static readonly string normalAlphabetStr = "qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
        private static readonly string normalNumbersStr = "1234567890";
        private static readonly string diacriticsStr = "ěščřžýáíéúůťďíüäïł";

        /// <summary>
        /// Zkontroluje zda string obsahuje nějaké standartní písmena.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasLetters(string text)
        {
            foreach (var letter in text)
            {
                if (normalAlphabetStr.Contains(letter.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Zkontroluje string zda obsahuje čísla.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasNumber(string text)
        {
            foreach (var number in text)
            {
                if (normalNumbersStr.Contains(number.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Zkontroluje zda má string obsaženou diakritiku.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasDiacritics(string text)
        {
            foreach (var letter in text)
            {
                if (diacriticsStr.ToLower().Contains(letter.ToString().ToLower()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
