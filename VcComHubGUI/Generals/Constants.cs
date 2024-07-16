using System;
using System.Collections.Generic;
using System.Drawing;

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
    /// Tato třída slouží pro nastavení různých výchozích konstant a nastavení.
    /// </summary>
    internal class Constants
    {
        public static readonly string workingDirectory = Environment.CurrentDirectory;

        public static readonly Color darkThemeBackground2 = Color.FromArgb(120, 120, 120);
        public static readonly Color darkThemeBackground = Color.FromArgb(90, 90, 100);
        public static readonly Color darkThemeText = Color.FromArgb(230, 230, 230);

        public static readonly string vcModuleName = "vietcong.exe";
        public static readonly string version = "1.65";
        public static readonly string build = " (build: 1.65.3)";

        public static List<string> config = new List<string>()
        {
            "[Game]",
            "vc.path = ",
            "vc.path.addons = ",
            "",
            "",
            "[VcComHub]",
            "settings.dark_mode = false",
            "settings.auto_master_fetch = true",
            "settings.language = EN",
            "settings.player_name = Change me",
            "settings.start_without_asking = false",
            "settings.show_fps_at_start = false",
            "settings.show_prof_at_start = false",
            "",
            "",
            "[Debugging]",
            "debugging.level = 0",
            "debugging.filter = ERROR | WARNING | INFO | EVENTS",
        };
    }
}
