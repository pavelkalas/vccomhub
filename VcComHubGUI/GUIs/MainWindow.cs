using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VcComHub.Automation;
using VcComHub.ConfigParser;
using VcComHub.FileSystem;
using VcComHub.LanguageParser;
using VcComHub.Loader;
using VcComHub.Memory;
using VcComHub.Network;
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
    /// Třída hlavního vlákna a hlavního dialogového okna.
    /// </summary>
    public partial class MainWindow : Form
    {
        // obecné proměnné
        private string vcGamePath = null;
        private string vcAddonPath = null;
        private string language = null;
        private string playerName;
        private bool darkMode = false;
        private bool startWithoutAsking = false;
        private bool showFpsAtStart = false;
        private bool showProfAtStart = false;

        // proměnné pro "MULTIPLAYER"
        private Thread fetchThread;
        private string connectionString = null;
        private bool fetchAtStartup = false;
        private bool isFetching = false;

        private string newSelectedLanguageCode = null;

        private string selectedServerName = null;
        private string selectedServerIPandPort = null;

        public static int processId = 0;

        private DateTime playingSingleplayerSince;
        private DateTime playingMultiplayerSince;
        private DateTime gameRunSince;

        public static bool isDeveloper = false;

        public static bool ignoreUnfocusedGameWindow = false;

        public MainWindow()
        {
            L4u.WriteLog("logs\\events.txt", "MainWindow() : CONSTRUCTOR", L4u.LogType.EVENT);

            InitializeComponent();
        }

        /// <summary>
        /// Přeloží všechny prvky na dialogových oknech do vybraného jazyka.
        /// </summary>
        private void TranslateElements()
        {
            L4u.WriteLog("logs\\events.txt", "TranslateElements() : VOID", L4u.LogType.EVENT);

            Language.LoadFile(language);

            SetText(() => RunGame.Text = Language.GetStr(2020));

            SetText(() => WelcomeTab.Text = Language.GetStr(2010));
            SetText(() => ConfigurationTab.Text = Language.GetStr(2011));
            SetText(() => MultiPlayerTab.Text = Language.GetStr(2012));

            SetText(() => WelcomeLbl.Text = Language.GetStr(1000));
            SetText(() => WhatWeDoinLbl.Text = Language.GetStr(1001));
            SetText(() => WhatWeDoinTextLbl.Text = Language.GetStr(1002));

            SetText(() => SectionsLbl.Text = Language.GetStr(1086));
            SetText(() => SectionDescribeLbl.Text = Language.GetStr(1087));

            SetText(() => ConfigurationLbl.Text = Language.GetStr(1010));
            SetText(() => ConfigurationVcHubLbl.Text = Language.GetStr(1011));
            SetText(() => VcPathLbl.Text = Language.GetStr(1012));
            SetText(() => AddonsPathLbl.Text = Language.GetStr(1013));
            SetText(() => ConfigurationGeneralLbl.Text = Language.GetStr(1014));
            SetText(() => ConfigurationLangLbl.Text = Language.GetStr(1015));

            SetText(() => DarkModeCheck.Text = Language.GetStr(1020));
            SetText(() => AutoFetchCheck.Text = Language.GetStr(1021));

            SetText(() => SaveConfigBtn.Text = Language.GetStr(1030));
            SetText(() => BrowseAddonsDirBtn.Text = Language.GetStr(1031));
            SetText(() => BrowseVcDirBtn.Text = Language.GetStr(1031));

            SetText(() => MultiplayerLbl.Text = Language.GetStr(1040));
            SetText(() => MultiplayerDescriptionLbl.Text = Language.GetStr(1041));
            SetText(() => MultiplayerStatusLbl.Text = Language.GetStr(1042));
            SetText(() => SelectedServerLbl.Text = Language.GetStr(1043) + " " + Language.GetStr(2002));
            SetText(() => RefreshServersBtn.Text = Language.GetStr(1050));
            SetText(() => UnselectServerBtn.Text = Language.GetStr(1052));
            SetText(() => ConfigurationPlayerNameLbl.Text = Language.GetStr(1016));
            SetText(() => ConfiguationPlayerLbl.Text = Language.GetStr(1017));
            SetText(() => ShowFpsCheck.Text = Language.GetStr(1018));
            SetText(() => ShowProfCheck.Text = Language.GetStr(1019));
            SetText(() => StartGameWithoutAskCheck.Text = Language.GetStr(1022));
            SetText(() => InGameChatLbl.Text = Language.GetStr(1070));
            SetText(() => InGameChatDescriptionLbl.Text = Language.GetStr(1071));
            SetText(() => ToPublicChatRadio.Text = Language.GetStr(1072));
            SetText(() => ToYourTeamRadio.Text = Language.GetStr(1073));
            SetText(() => InGameChatConfigGBox.Text = Language.GetStr(1073));
            SetText(() => InGameChatTab.Text = Language.GetStr(2013));

            SetText(() => DeveloperTab.Text = Language.GetStr(1100));
            SetText(() => OpenAddonsFolderBtn.Text = Language.GetStr(1109));
            SetText(() => OpenGameFolderBtn.Text = Language.GetStr(1108));
            SetText(() => OpenLogsFolder.Text = Language.GetStr(1110));
            SetText(() => OpenDevFolderBtn.Text = Language.GetStr(1111));
            SetText(() => OpenMapsFolderBtn.Text = Language.GetStr(1112));
            SetText(() => OpenChatlogBtn.Text = Language.GetStr(1113));
            SetText(() => OpenConsoleBtn.Text = Language.GetStr(1114));
            SetText(() => RunGameBtn.Text = Language.GetStr(1115));
            SetText(() => RunServerBtn.Text = Language.GetStr(1116));
            SetText(() => OpenVcStarterBtn.Text = Language.GetStr(1117));
            SetText(() => OpenCommandPromptBtn.Text = Language.GetStr(1118));
            SetText(() => OpenNotepadBtn.Text = Language.GetStr(1119));
            SetText(() => IgnoreUnfocusedWindowCheck.Text = Language.GetStr(1120));
            SetText(() => CpuLbl.Text = Language.GetStr(1121));
            SetText(() => DeveloperOthersGrp.Text = Language.GetStr(1105));
            SetText(() => ProcessGrp.Text = Language.GetStr(1106));
            SetText(() => GameProfilingGrp.Text = Language.GetStr(1107));
            SetText(() => QuickAccessGrp.Text = Language.GetStr(1103));
            SetText(() => QuickAccessProgramsGrp.Text = Language.GetStr(1104));
            SetText(() => DeveloperToolsDescriptionLbl.Text = Language.GetStr(1102));
            SetText(() => DeveloperToolsLbl.Text = Language.GetStr(1100));

            SetText(() => columnHeader1.Text = Language.GetStr(1060));
            SetText(() => columnHeader2.Text = Language.GetStr(1061));
            SetText(() => columnHeader3.Text = Language.GetStr(1062));
            SetText(() => columnHeader4.Text = Language.GetStr(1063));
            SetText(() => columnHeader7.Text = Language.GetStr(1064));
            SetText(() => columnHeader8.Text = Language.GetStr(1065));
            SetText(() => columnHeader9.Text = Language.GetStr(1066));
        }

        /// <summary>
        /// Nastaví text prvku.
        /// </summary>
        /// <param name="action"></param>
        void SetText(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Vláko pro sledování chatu hry.
        /// </summary>
        public void LoadInGameChat()
        {
            L4u.WriteLog("logs\\events.txt", "LoadInGameChat() : VOID", L4u.LogType.EVENT);

            Thread chatWorker = new Thread(() =>
            {
                Thread.Sleep(2000);

                long lastChanges = 0;

                string file = Path.Combine(vcGamePath, "dev", "chatlog.txt");

                while (Processes.IsProcessRunning(processId) && processId != 0)
                {
                    long currentSize = FileSystem.GetFileSize(file);

                    if (currentSize > lastChanges)
                    {
                        string lastLine = ReadLastLine(file);

                        if (!string.IsNullOrEmpty(lastLine) && !lastLine.Contains("==="))
                        {
                            if (lastChanges == 0)
                            {
                                int readLastCount = 100;
                                int counter = 0;

                                string text = "";

                                foreach (var chatLine in File.ReadLines(vcGamePath + "\\dev\\chatlog.txt").Reverse())
                                {
                                    counter++;

                                    if (counter > readLastCount)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        string chat = chatLine;

                                        if (chat.Length > 30)
                                        {
                                            chat = chat.Substring(26).Trim();
                                        }

                                        text += chat.Trim() + "\n";
                                    }
                                }

                                AppendChatLine(text);
                            }
                            else
                            {
                                AppendChatLine(lastLine.Substring(26).Trim());
                            }
                        }
                    }
                    lastChanges = currentSize;

                    EnableChatControls(Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x777D28), "game.dll") == 1);

                    Thread.Sleep(500);
                }

                EnableChatControls(false);
            });

            chatWorker.Start();
        }

        /// <summary>
        /// Povolí nebo zakáže ovládací prvky chatu.
        /// </summary>
        /// <param name="enabled">true pro povolení, false pro zakázání.</param>
        private void EnableChatControls(bool enabled)
        {
            L4u.WriteLog("logs\\events.txt", "EnableChatControls(bool enabled) : VOID", L4u.LogType.EVENT);

            if (InGameChatInput.InvokeRequired)
            {
                InGameChatInput.Invoke(new Action(() => InGameChatInput.Enabled = enabled));
            }
            else
            {
                InGameChatInput.Enabled = enabled;
            }

            if (InGameChatOutputRich.InvokeRequired)
            {
                InGameChatOutputRich.Invoke(new Action(() => InGameChatOutputRich.Enabled = enabled));
            }
            else
            {
                InGameChatOutputRich.Enabled = enabled;
            }

            if (!enabled)
            {
                if (InGameChatOutputRich.InvokeRequired)
                {
                    InGameChatOutputRich.Invoke(new Action(() => InGameChatOutputRich.Clear()));
                }
                else
                {
                    InGameChatOutputRich.Clear();
                }
            }
        }

        /// <summary>
        /// Přečte poslední řádek ze souboru.
        /// </summary>
        /// <param name="filePath">Cesta k souboru.</param>
        /// <returns>Poslední řádek textu v souboru.</returns>
        private string ReadLastLine(string filePath)
        {
            L4u.WriteLog("logs\\events.txt", "ReadLastLine(string filePath) : STRING", L4u.LogType.EVENT);

            if (!FileSystem.FileExists(filePath))
            {
                try
                {
                    FileSystem.CreateNewFile(filePath, "");
                    L4u.WriteLog("logs\\infos.txt", "File " + filePath + " was not existed and was successfully created!", L4u.LogType.INFO);
                }
                catch
                {
                    L4u.WriteLog("logs\\infos.txt", "File " + filePath + " cannot be created!", L4u.LogType.ERROR);
                    Environment.Exit(0);
                }
            }

            string lastLine = string.Empty;

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        lastLine = sr.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                L4u.WriteLog("logs\\errors.txt", "Cannot read file " + filePath + " in function ReadLastLine(string filePath) : string - " + ex.Message, L4u.LogType.ERROR);
                Environment.Exit(0);
            }

            return lastLine?.Trim();
        }

        /// <summary>
        /// Přidá řádek do chatového výstupu.
        /// </summary>
        /// <param name="line">Řádek textu k přidání.</param>
        private void AppendChatLine(string line)
        {
            L4u.WriteLog("logs\\events.txt", "AppendChatLine(string line) : VOID", L4u.LogType.EVENT);

            if (InGameChatOutputRich.InvokeRequired)
            {
                InGameChatOutputRich.BeginInvoke(new Action(() =>
                {
                    InGameChatOutputRich.Text += line + "\n";
                    InGameChatOutputRich.SelectionStart = InGameChatOutputRich.Text.Length;
                    InGameChatOutputRich.ScrollToCaret();
                }));
            }
            else
            {
                InGameChatOutputRich.Text += line + "\n";
                InGameChatOutputRich.SelectionStart = InGameChatOutputRich.Text.Length;
                InGameChatOutputRich.ScrollToCaret();
            }
        }

        /// <summary>
        /// Při vyrenderování formuláře.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppLoad(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\events.txt", "ApppLoad(object sender, EventArgs e) : VOID", L4u.LogType.EVENT);

            if (!FileSystem.FileExists(Constants.workingDirectory + "\\lang"))
            {
                L4u.WriteLog("logs\\infos.txt", "Directory " + Constants.workingDirectory + "\\lang" + " was not existed and was successfully created!", L4u.LogType.INFO);
                FileSystem.CreateDirectory(Constants.workingDirectory + "\\lang");
            }

            bool configValidate = (Config.IssetKey("vc.path") && Config.IssetKey("vc.path.addons")) && Config.IssetKey("settings.dark_mode") && Config.IssetKey("settings.auto_master_fetch") && Config.IssetKey("settings.language") && Config.IssetKey("settings.player_name");

            if (!configValidate)
            {
                L4u.WriteLog("logs\\warnings.txt", "Invalid or missing configuration line, please, delete vccomhub.ini and run this application again!", L4u.LogType.WARNING);
                Environment.Exit(0);
            }

            vcGamePath = Config.GetConfig("vc.path");
            vcAddonPath = Config.GetConfig("vc.path.addons");
            darkMode = Config.GetConfig("settings.dark_mode").Contains("true");
            fetchAtStartup = Config.GetConfig("settings.auto_master_fetch").Contains("true");
            language = Config.GetConfig("settings.language");
            playerName = Config.GetConfig("settings.player_name");
            startWithoutAsking = Config.GetConfig("settings.start_without_asking").Contains("true");
            showFpsAtStart = Config.GetConfig("settings.show_fps_at_start").Contains("true");
            showProfAtStart = Config.GetConfig("settings.show_prof_at_start").Contains("true");

            if (playerName.Trim().Length > 20)
            {
                playerName = playerName.Substring(0, 20).Trim();
                L4u.WriteLog("logs\\warnings.txt", "Player name was truncated because 20 letters/numbers is maximum length.", L4u.LogType.WARNING);
            }

            else if (playerName.Length < 1)
            {
                Logger.LogError("Player name must be minimum one letter long!", Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Player name must be minimum one letter long!", L4u.LogType.WARNING);
                playerName = "Player";
            }

            if (TextManager.HasDiacritics(playerName))
            {
                Logger.LogError("Player name cannot contains diacritics!", Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Player name cannot contains diacritics!", L4u.LogType.WARNING);
                playerName = "Player";
            }

            if (!TextManager.HasLetters(playerName) && !TextManager.HasNumber(playerName))
            {
                Logger.LogError(Language.GetStr(3010), Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Player name must contains some letters!", L4u.LogType.WARNING);
                playerName = "Player";
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    ServerListView.GridLines = true;
                    ServerListView.FullRowSelect = true;
                }));
            }
            else
            {
                ServerListView.GridLines = true;
                ServerListView.FullRowSelect = true;
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    VcDirTxt.Text = vcGamePath;
                    VcAddonsDirTxt.Text = vcAddonPath;
                    PlayerNameTxt.Text = playerName;
                    StartGameWithoutAskCheck.Checked = startWithoutAsking;
                    ShowFpsCheck.Checked = showFpsAtStart;
                    ShowProfCheck.Checked = showProfAtStart;
                }));
            }
            else
            {
                VcDirTxt.Text = vcGamePath;
                VcAddonsDirTxt.Text = vcAddonPath;
                PlayerNameTxt.Text = playerName;
                StartGameWithoutAskCheck.Checked = startWithoutAsking;
                ShowFpsCheck.Checked = showFpsAtStart;
                ShowProfCheck.Checked = showProfAtStart;
                AutoFetchCheck.Checked = fetchAtStartup;
            }

            if (FileSystem.DirectoryExists(vcAddonPath))
            {
                foreach (var addon in Directory.GetDirectories(vcAddonPath))
                {
                    string file = addon + "\\.name";

                    if (FileSystem.FileExists(file))
                    {
                        string name = File.ReadAllText(file);
                        AddonListView.Items.Add(name);
                    }
                    else
                    {
                        AddonListView.Items.Add(new DirectoryInfo(addon).Name);
                    }
                }
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    Text = "VcComHub v" + Constants.version + " active (by Pavel Kalas)";
                }));
            }
            else
            {
                Text = "VcComHub v" + Constants.version + " active (by Pavel Kalas)";
            }

            if (darkMode)
            {
                //
                // hlavní okno (toto)
                //
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        BackColor = Constants.darkThemeBackground;
                        WelcomeTab.BackColor = Constants.darkThemeBackground;
                    }));
                }
                else
                {
                    BackColor = Constants.darkThemeBackground;
                    WelcomeTab.BackColor = Constants.darkThemeBackground;
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        RunGame.BackColor = Constants.darkThemeBackground;
                        RunGame.ForeColor = Constants.darkThemeText;
                    }));
                }
                else
                {
                    RunGame.BackColor = Constants.darkThemeBackground;
                    RunGame.ForeColor = Constants.darkThemeText;
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        DarkModeCheck.Checked = true;
                        ConfigurationTab.BackColor = Constants.darkThemeBackground;
                        AboutTab.BackColor = Constants.darkThemeBackground;
                        VcPathLbl.ForeColor = Constants.darkThemeText;
                        AddonsPathLbl.ForeColor = Constants.darkThemeText;
                        ConfigurationLbl.ForeColor = Constants.darkThemeText;
                        DarkModeCheck.ForeColor = Constants.darkThemeText;
                        ConfigurationVcHubLbl.ForeColor = Constants.darkThemeText;
                        SaveConfigBtn.ForeColor = Constants.darkThemeText;
                        BrowseAddonsDirBtn.ForeColor = Constants.darkThemeText;
                        BrowseVcDirBtn.ForeColor = Constants.darkThemeText;
                        AddonsListLbl.ForeColor = Constants.darkThemeText;
                        AddonListView.Enabled = false;
                        AutoFetchCheck.ForeColor = Constants.darkThemeText;
                        ConfigurationGeneralLbl.ForeColor = Constants.darkThemeText;
                        ConfigurationLangLbl.ForeColor = Constants.darkThemeText;
                        StartGameWithoutAskCheck.ForeColor = Constants.darkThemeText;
                        ShowFpsCheck.ForeColor = Constants.darkThemeText;
                        UnselectServerBtn.ForeColor = Constants.darkThemeText;
                        SectionDescribeLbl.ForeColor = Constants.darkThemeText;
                        SectionsLbl.ForeColor = Constants.darkThemeText;
                        label1.ForeColor = Constants.darkThemeText;
                        label3.ForeColor = Constants.darkThemeText;
                        label4.ForeColor = Constants.darkThemeText;
                        label5.ForeColor = Constants.darkThemeText;
                        label5.ForeColor = Constants.darkThemeText;
                        label6.ForeColor = Constants.darkThemeText;
                        label7.ForeColor = Constants.darkThemeText;
                        label8.ForeColor = Constants.darkThemeText;
                        label9.ForeColor = Constants.darkThemeText;
                        label10.ForeColor = Constants.darkThemeText;
                        label11.ForeColor = Constants.darkThemeText;
                        label12.ForeColor = Constants.darkThemeText;
                    }));
                }
                else
                {
                    DarkModeCheck.Checked = true;
                    ConfigurationTab.BackColor = Constants.darkThemeBackground;
                    VcPathLbl.ForeColor = Constants.darkThemeText;
                    AboutTab.BackColor = Constants.darkThemeBackground;
                    AddonsPathLbl.ForeColor = Constants.darkThemeText;
                    ConfigurationLbl.ForeColor = Constants.darkThemeText;
                    DarkModeCheck.ForeColor = Constants.darkThemeText;
                    ConfigurationVcHubLbl.ForeColor = Constants.darkThemeText;
                    SaveConfigBtn.ForeColor = Constants.darkThemeText;
                    BrowseAddonsDirBtn.ForeColor = Constants.darkThemeText;
                    BrowseVcDirBtn.ForeColor = Constants.darkThemeText;
                    AddonsListLbl.ForeColor = Constants.darkThemeText;
                    AddonListView.Enabled = false;
                    AutoFetchCheck.ForeColor = Constants.darkThemeText;
                    ConfigurationGeneralLbl.ForeColor = Constants.darkThemeText;
                    ConfigurationLangLbl.ForeColor = Constants.darkThemeText;
                    StartGameWithoutAskCheck.ForeColor = Constants.darkThemeText;
                    ShowFpsCheck.ForeColor = Constants.darkThemeText;
                    ShowProfCheck.ForeColor = Constants.darkThemeText;
                    SectionDescribeLbl.ForeColor = Constants.darkThemeText;
                    SectionsLbl.ForeColor = Constants.darkThemeText;
                    label1.ForeColor = Constants.darkThemeText;
                    label3.ForeColor = Constants.darkThemeText;
                    label4.ForeColor = Constants.darkThemeText;
                    label5.ForeColor = Constants.darkThemeText;
                    label5.ForeColor = Constants.darkThemeText;
                    label6.ForeColor = Constants.darkThemeText;
                    label7.ForeColor = Constants.darkThemeText;
                    label8.ForeColor = Constants.darkThemeText;
                    label9.ForeColor = Constants.darkThemeText;
                    label10.ForeColor = Constants.darkThemeText;
                    label11.ForeColor = Constants.darkThemeText;
                    label12.ForeColor = Constants.darkThemeText;
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        WelcomeLbl.ForeColor = Constants.darkThemeText;
                        WhatWeDoinLbl.ForeColor = Constants.darkThemeText;
                        WhatWeDoinTextLbl.ForeColor = Constants.darkThemeText;
                    }));
                }
                else
                {
                    WelcomeLbl.ForeColor = Constants.darkThemeText;
                    WhatWeDoinLbl.ForeColor = Constants.darkThemeText;
                    WhatWeDoinTextLbl.ForeColor = Constants.darkThemeText;
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        MultiPlayerTab.BackColor = Constants.darkThemeBackground;
                        MultiplayerLbl.ForeColor = Constants.darkThemeText;
                        MultiplayerDescriptionLbl.ForeColor = Constants.darkThemeText;
                        RefreshServersBtn.ForeColor = Constants.darkThemeText;
                        MultiplayerStatusLbl.ForeColor = Constants.darkThemeText;
                        SelectedServerLbl.ForeColor = Constants.darkThemeText;
                        UnselectServerBtn.ForeColor = Constants.darkThemeText;
                        ConfigurationPlayerNameLbl.ForeColor = Constants.darkThemeText;
                        ConfiguationPlayerLbl.ForeColor = Constants.darkThemeText;
                        DeveloperTab.BackColor = Constants.darkThemeBackground;
                        OpenAddonsFolderBtn.ForeColor = Constants.darkThemeText;
                        OpenGameFolderBtn.ForeColor = Constants.darkThemeText;
                        OpenLogsFolder.ForeColor = Constants.darkThemeText;
                        OpenDevFolderBtn.ForeColor = Constants.darkThemeText;
                        OpenMapsFolderBtn.ForeColor = Constants.darkThemeText;
                        OpenChatlogBtn.ForeColor = Constants.darkThemeText;
                        OpenConsoleBtn.ForeColor = Constants.darkThemeText;
                        RunGameBtn.ForeColor = Constants.darkThemeText;
                        RunServerBtn.ForeColor = Constants.darkThemeText;
                        OpenVcStarterBtn.ForeColor = Constants.darkThemeText;
                        OpenCommandPromptBtn.ForeColor = Constants.darkThemeText;
                        OpenNotepadBtn.ForeColor = Constants.darkThemeText;
                        IgnoreUnfocusedWindowCheck.ForeColor = Constants.darkThemeText;
                        CpuLbl.ForeColor = Constants.darkThemeText;
                        DeveloperOthersGrp.ForeColor = Constants.darkThemeText;
                        ProcessGrp.ForeColor = Constants.darkThemeText;
                        GameProfilingGrp.ForeColor = Constants.darkThemeText;
                        QuickAccessGrp.ForeColor = Constants.darkThemeText;
                        QuickAccessProgramsGrp.ForeColor = Constants.darkThemeText;
                        DeveloperToolsDescriptionLbl.ForeColor = Constants.darkThemeText;
                        DeveloperToolsLbl.ForeColor = Constants.darkThemeText;
                        OpenGitLinkLbl.LinkColor = Color.FromArgb(50, 180, 140);
                    }));
                }
                else
                {
                    MultiPlayerTab.BackColor = Constants.darkThemeBackground;
                    MultiplayerLbl.ForeColor = Constants.darkThemeText;
                    MultiplayerDescriptionLbl.ForeColor = Constants.darkThemeText;
                    RefreshServersBtn.ForeColor = Constants.darkThemeText;
                    MultiplayerStatusLbl.ForeColor = Constants.darkThemeText;
                    SelectedServerLbl.ForeColor = Constants.darkThemeText;
                    UnselectServerBtn.ForeColor = Constants.darkThemeText;
                    ConfigurationPlayerNameLbl.ForeColor = Constants.darkThemeText;
                    ConfiguationPlayerLbl.ForeColor = Constants.darkThemeText;
                    DeveloperTab.BackColor = Constants.darkThemeBackground;
                    DeveloperTab.BackColor = Constants.darkThemeBackground;
                    OpenAddonsFolderBtn.ForeColor = Constants.darkThemeText;
                    OpenGameFolderBtn.ForeColor = Constants.darkThemeText;
                    OpenLogsFolder.ForeColor = Constants.darkThemeText;
                    OpenDevFolderBtn.ForeColor = Constants.darkThemeText;
                    OpenMapsFolderBtn.ForeColor = Constants.darkThemeText;
                    OpenChatlogBtn.ForeColor = Constants.darkThemeText;
                    OpenConsoleBtn.ForeColor = Constants.darkThemeText;
                    RunGameBtn.ForeColor = Constants.darkThemeText;
                    RunServerBtn.ForeColor = Constants.darkThemeText;
                    OpenVcStarterBtn.ForeColor = Constants.darkThemeText;
                    OpenCommandPromptBtn.ForeColor = Constants.darkThemeText;
                    OpenNotepadBtn.ForeColor = Constants.darkThemeText;
                    IgnoreUnfocusedWindowCheck.ForeColor = Constants.darkThemeText;
                    CpuLbl.ForeColor = Constants.darkThemeText;
                    DeveloperOthersGrp.ForeColor = Constants.darkThemeText;
                    ProcessGrp.ForeColor = Constants.darkThemeText;
                    GameProfilingGrp.ForeColor = Constants.darkThemeText;
                    QuickAccessGrp.ForeColor = Constants.darkThemeText;
                    QuickAccessProgramsGrp.ForeColor = Constants.darkThemeText;
                    DeveloperToolsDescriptionLbl.ForeColor = Constants.darkThemeText;
                    DeveloperToolsLbl.ForeColor = Constants.darkThemeText;
                    OpenGitLinkLbl.LinkColor = Color.FromArgb(50, 180, 140);
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        InGameChatInput.Enabled = false;
                        InGameChatOutputRich.Enabled = false;
                        InGameChatTab.BackColor = Constants.darkThemeBackground;
                        InGameChatConfigGBox.BackColor = Constants.darkThemeBackground;
                        InGameChatConfigGBox.ForeColor = Constants.darkThemeText;
                        ToPublicChatRadio.ForeColor = Constants.darkThemeText;
                        ToYourTeamRadio.ForeColor = Constants.darkThemeText;
                        InGameChatLbl.ForeColor = Constants.darkThemeText;
                        InGameChatDescriptionLbl.ForeColor = Constants.darkThemeText;
                    }));
                }
                else
                {
                    InGameChatInput.Enabled = false;
                    InGameChatOutputRich.Enabled = false;
                    InGameChatTab.BackColor = Constants.darkThemeBackground;
                    InGameChatConfigGBox.BackColor = Constants.darkThemeBackground;
                    InGameChatConfigGBox.ForeColor = Constants.darkThemeText;
                    ToPublicChatRadio.ForeColor = Constants.darkThemeText;
                    ToYourTeamRadio.ForeColor = Constants.darkThemeText;
                    InGameChatLbl.ForeColor = Constants.darkThemeText;
                    InGameChatDescriptionLbl.ForeColor = Constants.darkThemeText;
                }
            }

            string selectedCode = null;

            if (language != null)
            {
                foreach (var languages in Directory.GetFiles(Constants.workingDirectory + "\\lang"))
                {
                    if (Language.GetCode(languages) == language)
                    {
                        language = languages;
                        selectedCode = Language.GetCode(languages);
                        L4u.WriteLog("logs\\infos.txt", "Language file " + languages + " was found and successfully loaded!", L4u.LogType.INFO);
                        break;
                    }
                }

                TranslateElements();
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    LanguageSelectorList.Items.Clear();

                    foreach (var languages in Directory.GetFiles(Constants.workingDirectory + "\\lang"))
                    {
                        if (selectedCode != null && selectedCode == Language.GetCode(languages))
                        {
                            LanguageSelectorList.Items.Add(Language.GetCode(languages) + "; " + Language.GetName(languages) + " <---");
                        }
                        else
                        {
                            LanguageSelectorList.Items.Add(Language.GetCode(languages) + "; " + Language.GetName(languages));
                        }
                    }
                }));
            }
            else
            {
                LanguageSelectorList.Items.Clear();

                foreach (var languages in Directory.GetFiles(Constants.workingDirectory + "\\lang"))
                {
                    if (selectedCode != null && selectedCode == Language.GetCode(languages))
                    {
                        LanguageSelectorList.Items.Add(Language.GetCode(languages) + "; " + Language.GetName(languages) + " <---");
                    }
                    else
                    {
                        LanguageSelectorList.Items.Add(Language.GetCode(languages) + "; " + Language.GetName(languages));
                    }
                }
            }

            if (fetchAtStartup)
            {
                FetchMaster();
            }
        }

        /// <summary>
        /// Urychleně spustí instanci hry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickLaunchBtn(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\infos.txt", "Trying to launch the game..", L4u.LogType.INFO);

            if (Processes.IsProcessRunning(processId) && processId != 0)
            {
                L4u.WriteLog("logs\\infos.txt", "Game is already running. You cannot run more than one instance of game at this moment!", L4u.LogType.INFO);

                var dialogResult = Logger.LogError(Language.GetStr(3000), Language.GetStr(2999), true);

                if (dialogResult == DialogResult.Yes)
                {
                    Process.GetProcessById(processId).Kill();
                }
                else
                {
                    return;
                }
            }

            bool validConfig = Config.IssetValue("vc.path") && Config.IssetKey("vc.path");

            if (!Directory.Exists(vcGamePath))
            {
                L4u.WriteLog("logs\\errors.txt", "Game directory " + vcGamePath + " not found, game was probably deleted or moved to another location.", L4u.LogType.ERROR);
                Logger.LogError(Language.GetStr(3002).Replace("%s", vcGamePath), Language.GetStr(2999));
                return;
            }
            else if (!File.Exists(vcGamePath + "\\" + Constants.vcModuleName))
            {
                L4u.WriteLog("logs\\errors.txt", Language.GetStr(3002).Replace("{0}", Constants.vcModuleName).Replace("{1}", vcGamePath), L4u.LogType.ERROR);
                Logger.LogError("Game module " + Constants.vcModuleName + " not found in " + vcGamePath, Language.GetStr(2999));
                return;
            }

            if (vcGamePath != null && validConfig)
            {
                string addonName = null;

                if (vcAddonPath != null && Directory.Exists(vcAddonPath) && Directory.GetDirectories(vcAddonPath).Count() > 0)
                {
                    new AddonSelector().ShowDialog();

                    addonName = (AddonSelector.addonName == "noaddon") ? null : AddonSelector.addonName;

                    if (addonName == "norun")
                    {
                        return;
                    }
                }

                List<int> processesList = new List<int>();

                string modName = Constants.vcModuleName;

                if (modName.Contains("."))
                {
                    modName = modName.Split('.')[0].Trim();
                }

                foreach (var processesNow in Process.GetProcesses())
                {
                    if (processesNow.ProcessName.ToLower() == modName)
                    {
                        processesList.Add(processesNow.Id);
                    }
                }

                L4u.WriteLog("logs\\infos.txt", "Trying to create and start process " + Constants.vcModuleName + " in " + vcGamePath + "..", L4u.LogType.INFO);

                try
                {
                    Loader.StartProcess(Constants.vcModuleName, vcGamePath, addonName, new string[] { (connectionString != null) ? ("-ip " + connectionString) : "" });
                    L4u.WriteLog("logs\\infos.txt", "Game successfully started!", L4u.LogType.INFO);
                }
                catch
                {
                    L4u.WriteLog("logs\\errors.txt", "Cannot create and start the process, please, check game path!", L4u.LogType.ERROR);
                }

                Thread.Sleep(100);

                new Thread(() =>
                {
                    processId = 0;

                    foreach (var processesNow in Process.GetProcesses())
                    {
                        if (processesNow.ProcessName.ToLower() == modName)
                        {
                            if (!processesList.Contains(processesNow.Id))
                            {
                                processId = processesNow.Id;
                                break;
                            }
                        }
                    }

                    if (startWithoutAsking)
                    {
                        L4u.WriteLog("logs\\infos.txt", "Skipping user click validation, run game IMMEDIATELLY!", L4u.LogType.INFO);

                        int attemptRunGame = 0;

                        while (!Processes.WindowExists("Ptero-Engine-II Setup", processId) && Processes.IsProcessRunning(processId))
                        {
                            Thread.Sleep(50);

                            attemptRunGame++;

                            if (attemptRunGame >= 1000)
                            {
                                break;
                            }
                        }

                        L4u.WriteLog("logs\\infos.txt", "Game setup/launch window successfully found! Trying to click \"OK\" button..", L4u.LogType.INFO);

                        if (Processes.WindowExists("Ptero-Engine-II Setup", processId))
                        {
                            bool clicked = Window.ClickButton(Process.GetProcessById(processId).MainWindowHandle, "OK");

                            if (clicked)
                            {
                                L4u.WriteLog("logs\\infos.txt", "Button \"OK\" successfully clicked!", L4u.LogType.INFO);
                            }
                            else
                            {
                                L4u.WriteLog("logs\\errors.txt", "Cannot click the button, waiting for user validation..", L4u.LogType.ERROR);
                            }
                        }
                    }

                    bool foundWindow = false;
                    int attemptToFindProcess = 0;

                    while (!foundWindow)
                    {
                        foreach (var processsesNow in Process.GetProcesses())
                        {
                            if (processsesNow.Id == processId && processsesNow.MainWindowTitle.Trim().ToLower() == "ptero-engine-ii : vietcong")
                            {
                                foundWindow = true;
                                break;
                            }
                        }

                        attemptToFindProcess++;

                        if (attemptToFindProcess > 1000)
                        {
                            break;
                        }

                        if (!Processes.IsProcessRunning(processId))
                        {
                            break;
                        }

                        Thread.Sleep(50);
                    }

                    Thread.Sleep(100);

                    if (foundWindow)
                    {
                        L4u.WriteLog("logs\\infos.txt", "Main game window found!", L4u.LogType.INFO);

                        Window.MoveWindowToCenter(Process.GetProcessById(processId).MainWindowHandle);

                        try
                        {
                            Memory.InjectDllToProcess(Process.GetProcessById(processId), Constants.workingDirectory + "\\VcComHub.EngineInjector.dll");
                            L4u.WriteLog("logs\\infos.txt", "Game with pID: " + processId + " was successfully injected!", L4u.LogType.INFO);
                        }
                        catch
                        {
                            L4u.WriteLog("logs\\infos.txt", "Cannot inject process with pID: " + processId + "!", L4u.LogType.ERROR);
                        }

                        L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl CNS_AddTxt(char*) : VOID", L4u.LogType.EVENT);

                        Intercommunication.Setup(processId);
                        Intercommunication.SendString("");
                        Intercommunication.SendString("> VcComHub v" + Constants.version + Constants.build + " active - https://github.com/pavelkalas/vccomhub.git / https://yungrixxxi.xyz");
                        Intercommunication.SendString("");

                        if (selectedServerIPandPort != null && selectedServerName != null)
                        {
                            L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl NET_JoinGameByAddress(void*, struct *t_NetGameInit, char*, ulong) : VOID", L4u.LogType.EVENT);
                            L4u.WriteLog("logs\\infos.txt", "Calling the game function NET_JoinGameByAddress(void *,t_NetGameInit *,char *,ulong) to join the server.", L4u.LogType.INFO);
                            L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl CNS_AddTxt(char*) : VOID", L4u.LogType.EVENT);
                            Intercommunication.SendDataToMemory("  -> Connecting to server: " + selectedServerName + " (" + selectedServerIPandPort + ") ..");
                            Intercommunication.SendDataToMemory("");
                        }

                        if (Processes.IsProcessRunning(processId))
                        {
                            L4u.WriteLog("logs\\infos.txt", "Temporarily writing playername to addresses 0x71C0D4 & 0x71C228 (size is player name length * 16 + 1 + 8, enought!).", L4u.LogType.INFO);

                            L4u.WriteLog("logs\\events.txt", "Memory.WriteStringToAddress(processId, moduleName, address, string, size) : VOID (0x71C0D4)", L4u.LogType.EVENT);
                            Memory.WriteStringToAddress(processId, "game.dll", 0x71C0D4, playerName, 16);

                            L4u.WriteLog("logs\\events.txt", "Memory.WriteStringToAddress(processId, moduleName, address, string, size) : VOID (0x71C228)", L4u.LogType.EVENT);
                            Memory.WriteStringToAddress(processId, "game.dll", 0x71C228, playerName, 16);

                            L4u.WriteLog("logs\\events.txt", "Memory.ReadFromMemoryInt(processs_handle, IntPtr address, moduleName) : VOID (0x779284)", L4u.LogType.EVENT);
                            if (Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x779284), "game.dll") == 0 && Processes.IsProcessRunning(processId))
                            {
                                L4u.WriteLog("logs\\infos.txt", "Writing \"ONE\" to memory address 0x779284 (to show fps on screen).", L4u.LogType.INFO);
                                L4u.WriteLog("logs\\events.txt", "Memory.WriteStringToAddress(processId, moduleName, address, string, size) : VOID (0x779284)", L4u.LogType.EVENT);

                                new Thread(() =>
                                {
                                    Memory.WriteIntToAddress(processId, "game.dll", 0x779284, 1);

                                    string[] messages = { "VcComHub v" + Constants.version + Constants.build + " by Pavel Kalas", "fps: %3.1f   coord: %2.1f / %2.1f" };

                                    while (Processes.IsProcessRunning(processId))
                                    {
                                        for (int i = 0; i < messages.Length; i++)
                                        {
                                            if (i == 1 && !showFpsAtStart)
                                            {
                                                continue;
                                            }

                                            Memory.WriteStringToAddress(processId, "game.dll", 0x20B3E4, messages[i], 64);

                                            Thread.Sleep(2000);
                                        }
                                    }

                                }).Start();

                                new Thread(() =>
                                {
                                    while (Processes.IsProcessRunning(processId) && processId != 0)
                                    {
                                        Memory.WriteIntToAddress(processId, "game.dll", 0x779284, 1);

                                        string pushedCommand = Memory.ReadFromMemoryStr(Process.GetProcessById(processId), new IntPtr(0x36D5E4), "logs.dll").Trim();
                                        string pushedArguments = Memory.ReadFromMemoryStr(Process.GetProcessById(processId), new IntPtr(0x36B4FC), "logs.dll").Trim();

                                        if (pushedArguments.Length >= 1)
                                        {
                                            pushedArguments = pushedArguments.Trim();

                                            if (pushedCommand.ToLower().Contains("showfps") && pushedArguments.Contains("0"))
                                            {
                                                Intercommunication.SendString("Command showfps is disabled, to change this please, use VcComHub configuration!");
                                                Memory.WriteIntToAddress(processId, "logs.dll", 0x36B4FC, 1);
                                            }
                                            else if (pushedArguments.ToLower().Contains("close"))
                                            {
                                                Processes.KillAllProcesses(processId);
                                                Environment.Exit(0);
                                            }
                                            else if (pushedArguments.ToLower().Contains("giftfrompterodon") || pushedArguments.ToLower().Contains("chthelp"))
                                            {
                                                Intercommunication.SendString("Cheating? Come on! Allright!");
                                                Intercommunication.SendString("+------------------------------- CHEAT COMMANDS -----------------------------");
                                                Intercommunication.SendString("| chthelp            ->  Shows this table of cheats!");
                                                Intercommunication.SendString("| chtweap [number]   ->  Gives player a weapon! (number range: 1 - 30)");
                                                Intercommunication.SendString("| chtkostej          ->  Unlimited life (not include explosions!)");
                                                Intercommunication.SendString("| cht3pv [1/0]       ->  You will see yourself from 3rd person view!");
                                                Intercommunication.SendString("| chtallqf           ->  Enables all quick fights!");
                                                Intercommunication.SendString("| chtammo            ->  Gives a player ammo!");
                                                Intercommunication.SendString("| chtheal            ->  Gives a full heal!");
                                                Intercommunication.SendString("| chthealteam        ->  Gives a full heal to you and your team!");
                                                Intercommunication.SendString("| chtgrenades        ->  Gives you a grenades!");
                                                Intercommunication.SendString("| chtcannibals       ->  Sets a indian masks to every character!");
                                                Intercommunication.SendString("+----------------------------------------------------------------------------");
                                                Intercommunication.SendString("+ Menu added by Pavel Kalas");
                                                Intercommunication.SendString("");
                                                Memory.WriteIntToAddress(processId, "logs.dll", 0x36B4FC, 1);
                                            }
                                        }

                                        Thread.Sleep(500);
                                    }
                                }).Start();
                            }

                            L4u.WriteLog("logs\\events.txt", "Memory.ReadFromMemoryInt(processs_handle, IntPtr address, moduleName) : VOID (0x54C434)", L4u.LogType.EVENT);
                            if (Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x54C434), "game.dll") == 0 && Processes.IsProcessRunning(processId) && showProfAtStart)
                            {
                                L4u.WriteLog("logs\\infos.txt", "Writing \"ONE\" to memory address 0x54C434 (to show proofs on screen).", L4u.LogType.INFO);
                                L4u.WriteLog("logs\\events.txt", "Memory.WriteStringToAddress(processId, moduleName, address, string, size) : VOID (0x54C434)", L4u.LogType.EVENT);

                                bool written = Memory.WriteIntToAddress(processId, "game.dll", 0x54C434, 1);

                                if (!written)
                                {
                                    L4u.WriteLog("logs\\erorrs.txt", "Writing failed to memory at address 0x54C434.", L4u.LogType.ERROR);
                                }
                                else
                                {
                                    L4u.WriteLog("logs\\infos.txt", "Writing successfull to 0x54C434.", L4u.LogType.INFO);
                                }
                            }
                        }

                        gameRunSince = DateTime.Now;

                        new Thread(() =>
                        {
                            bool doUpdates = false;

                            L4u.WriteLog("logs\\infos.txt", "Starting discord rich presence..", L4u.LogType.INFO);

                            try
                            {
                                new Thread(() => DiscordRichPresence.StartTask("Idling", "In menu")).Start();
                                L4u.WriteLog("logs\\infos.txt", "Starting discord rich presence was OK!", L4u.LogType.INFO);
                                doUpdates = true;

                            }
                            catch
                            {
                                L4u.WriteLog("logs\\errors.txt", "Cannot start discord rich presence module!", L4u.LogType.ERROR);
                            }

                            bool multiplayerState = false;
                            bool singleplayerState = false;

                            L4u.WriteLog("logs\\events.txt", "EventHandler registered : Memory.ReadFromMemoryInt(processs_handle, IntPtr address, moduleName) : VOID (0x777D28)", L4u.LogType.EVENT);
                            L4u.WriteLog("logs\\events.txt", "EventHandler registered : Memory.ReadFromMemoryInt(processs_handle, IntPtr address, moduleName) : VOID (0x106F9D0)", L4u.LogType.EVENT);

                            while (Processes.IsProcessRunning(processId) && doUpdates)
                            {
                                bool isPlayingMultiplayer = Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x777D28), "game.dll") == 1;

                                bool isPlayingSingleplayer = Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x106F9D0), "logs.dll") == 1;

                                bool isInMenu = Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x21E8F4), "game.dll") == 1;

                                string serverName = Memory.ReadFromMemoryStr(Process.GetProcessById(processId), new IntPtr(0x77809C), "game.dll").Trim();

                                serverName = serverName.Substring(0, 30).Trim();

                                if (multiplayerState != isPlayingMultiplayer)
                                {
                                    if (isPlayingMultiplayer)
                                    {
                                        playingMultiplayerSince = DateTime.Now;
                                    }
                                }
                                multiplayerState = isPlayingMultiplayer;

                                if (singleplayerState != isPlayingSingleplayer)
                                {
                                    if (isPlayingSingleplayer)
                                    {
                                        playingSingleplayerSince = DateTime.Now;
                                    }
                                }

                                singleplayerState = isPlayingSingleplayer;

                                if (isPlayingMultiplayer && !isInMenu)
                                {
                                    DiscordRichPresence.client.SetPresence(new RichPresence()
                                    {
                                        State = Language.GetStr(1084).Replace("%s", serverName),
                                        Details = Language.GetStr(1085),
                                        Timestamps = new Timestamps
                                        {
                                            Start = Timestamps.FromUnixMilliseconds(ulong.Parse(new DateTimeOffset(playingMultiplayerSince).ToUnixTimeMilliseconds().ToString())),
                                        },
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "large_icon",
                                            LargeImageText = ":-)",
                                            SmallImageKey = "small_icon"
                                        }
                                    });
                                }
                                else if (isPlayingSingleplayer && !isInMenu)
                                {
                                    int difficultyInteger = Memory.ReadFromMemoryInt(Process.GetProcessById(processId), new IntPtr(0x80D608), "game.dll");

                                    string difficultyString = null;

                                    if (difficultyInteger == 0)
                                        difficultyString = Language.GetStr(3070);

                                    else if (difficultyInteger == 1)
                                        difficultyString = Language.GetStr(3071);

                                    else if (difficultyInteger == 2)
                                        difficultyString = Language.GetStr(3072);

                                    else if (difficultyInteger == 3)
                                        difficultyString = Language.GetStr(3073);

                                    DiscordRichPresence.client.SetPresence(new RichPresence()
                                    {
                                        State = Language.GetStr(1080).Replace("%s", difficultyString),
                                        Details = Language.GetStr(1081),
                                        Timestamps = new Timestamps
                                        {
                                            Start = Timestamps.FromUnixMilliseconds(ulong.Parse(new DateTimeOffset(playingSingleplayerSince).ToUnixTimeMilliseconds().ToString())),
                                        },
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "large_icon",
                                            LargeImageText = ":-)",
                                            SmallImageKey = "small_icon"
                                        }
                                    });
                                }
                                else
                                {
                                    DiscordRichPresence.client.SetPresence(new RichPresence()
                                    {
                                        State = Language.GetStr(1082),
                                        Details = Language.GetStr(1083),
                                        Timestamps = new Timestamps
                                        {
                                            Start = Timestamps.FromUnixMilliseconds(ulong.Parse(new DateTimeOffset(gameRunSince).ToUnixTimeMilliseconds().ToString())),
                                        },
                                        Assets = new Assets()
                                        {
                                            LargeImageKey = "large_icon",
                                            LargeImageText = ":-)",
                                            SmallImageKey = "small_icon"
                                        }
                                    });
                                }

                                Thread.Sleep(5000);
                            }

                            DiscordRichPresence.StopTask();

                            L4u.WriteLog("logs\\infos.txt", "Discord rich presence disconnected!", L4u.LogType.INFO);
                        }).Start();

                        new Thread(() =>
                        {
                            new Thread(() => PerformanceMonitor.StartWatcher(processId)).Start();

                            while (Processes.IsProcessRunning(processId) && processId != 0)
                            {
                                Thread.Sleep(250);

                                if (CpuProgress.InvokeRequired)
                                {
                                    CpuProgress.Invoke(new Action(() => CpuProgress.Value = (int)PerformanceMonitor._cpuUsage));
                                }
                                else
                                {
                                    CpuProgress.Value = (int)PerformanceMonitor._cpuUsage;
                                }

                                if (ignoreUnfocusedGameWindow)
                                {
                                    Memory.WriteIntToAddress(processId, Constants.vcModuleName, 0x1B758, 1);
                                    Memory.WriteIntToAddress(processId, Constants.vcModuleName, 0x1B770, 1);
                                }
                            }

                            if (CpuProgress.InvokeRequired)
                            {
                                CpuProgress.Invoke(new Action(() => CpuProgress.Value = 0));
                            }
                            else
                            {
                                CpuProgress.Value = 0;
                            }
                        }).Start();

                        LoadInGameChat();
                    }
                }).Start();
            }
            else if (!validConfig)
            {
                Logger.LogError(Language.GetStr(3004), Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Please, configure game directory first!", L4u.LogType.WARNING);
            }
        }

        /// <summary>
        /// Tlačítko pro uložení veškeré konfigurace.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveConfigBtn_Click(object sender, EventArgs e)
        {
            string gamePath = VcDirTxt.Text.Trim();
            string addonsPath = VcAddonsDirTxt.Text.Trim();

            playerName = PlayerNameTxt.Text.Trim();

            if (playerName.Length > 20)
            {
                playerName = playerName.Substring(0, 20).Trim();
            }
            else if (playerName.Length < 1)
            {
                Logger.LogError(Language.GetStr(3005), Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Player name must be minimum one letter long!", L4u.LogType.WARNING);
                return;
            }

            if (TextManager.HasDiacritics(playerName))
            {
                Logger.LogError(Language.GetStr(3006), Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Player name cannot contains diacritics!", L4u.LogType.WARNING);
                return;
            }

            if (!TextManager.HasLetters(playerName) && !TextManager.HasNumber(playerName))
            {
                Logger.LogError(Language.GetStr(3007), Language.GetStr(2999));
                L4u.WriteLog("logs\\warnings.txt", "Player name must contains some letters!", L4u.LogType.WARNING);
                return;
            }

            if (gamePath.Length > 0)
            {
                Config.ChangeConfig("vc.path", gamePath);
            }

            if (addonsPath.Length > 0)
            {
                Config.ChangeConfig("vc.path.addons", addonsPath);
            }

            if (newSelectedLanguageCode != null)
            {
                Config.ChangeConfig("settings.language", newSelectedLanguageCode.ToUpper().Trim());
            }

            Config.ChangeConfig("settings.dark_mode", DarkModeCheck.Checked.ToString().ToLower());
            Config.ChangeConfig("settings.auto_master_fetch", AutoFetchCheck.Checked.ToString().ToLower());
            Config.ChangeConfig("settings.start_without_asking", StartGameWithoutAskCheck.Checked.ToString().ToLower());
            Config.ChangeConfig("settings.player_name", playerName);
            Config.ChangeConfig("settings.show_fps_at_start", ShowFpsCheck.Checked.ToString().ToLower());
            Config.ChangeConfig("settings.show_prof_at_start", ShowProfCheck.Checked.ToString().ToLower());

            try
            {
                L4u.WriteLog("logs\\infos.txt", "Changes writed to vccomhub.ini, reloading configuration file..", L4u.LogType.INFO);
                Config.ReloadConfig();
            }
            catch
            {
                L4u.WriteLog("logs\\errors.txt", "Cannot save the data to configuration file, please, try it change manually with closed VcComHub!", L4u.LogType.ERROR);
            }

            string currentProcessPath = Process.GetCurrentProcess().MainModule.FileName;

            if (File.Exists(currentProcessPath))
            {
                Process.Start(currentProcessPath);

                Environment.Exit(0);
            }
            else
            {
                Logger.LogError(Language.GetStr(3008), Language.GetStr(2999));
                L4u.WriteLog("logs\\errors.txt", "Cannot find VcComHubGUI.exe, please, restart application manually!", L4u.LogType.ERROR);
            }
        }

        /// <summary>
        /// Otevře složku pro vybrání složky s hrou.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseVcDirBtn_Click(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\infos.txt", "Opened dialog for choose vietcong directory path, waiting for user..", L4u.LogType.INFO);

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog
            {
                Description = Language.GetStr(3050)
            };

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string selectedDirectory = folderBrowser.SelectedPath;

                if (Directory.Exists(selectedDirectory))
                {
                    if (!File.Exists(Path.Combine(selectedDirectory, Constants.vcModuleName)))
                    {
                        Logger.LogError("Selected folder does not contain " + Constants.vcModuleName + " module!", Language.GetStr(2999));
                        L4u.WriteLog("logs\\errors.txt", "Selected folder does not contain " + Constants.vcModuleName + " module!", L4u.LogType.WARNING);
                    }
                    else
                    {
                        VcDirTxt.Text = selectedDirectory;
                        L4u.WriteLog("logs\\infos.txt", "Selected game folder is: " + selectedDirectory + ".", L4u.LogType.INFO);
                    }
                }
            }
            else
            {
                L4u.WriteLog("logs\\infos.txt", "Choosing was cancelled by the user.", L4u.LogType.INFO);
            }
        }

        /// <summary>
        /// Otevře složku pro vybrání složky s addonama.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseAddonsDirBtn_Click(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\infos.txt", "Opened dialog for choose vietcong addons path, waiting for user..", L4u.LogType.INFO);

            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog
            {
                Description = Language.GetStr(3051)
            })
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string selectedDirectory = folderBrowser.SelectedPath;

                    if (Directory.Exists(selectedDirectory))
                    {
                        VcAddonsDirTxt.Text = selectedDirectory;
                        L4u.WriteLog("logs\\infos.txt", "Selected addons folder is: " + selectedDirectory + ".", L4u.LogType.INFO);
                    }
                }
                else
                {
                    L4u.WriteLog("logs\\infos.txt", "Choosing was cancelled by the user.", L4u.LogType.INFO);
                }
            }
        }

        /// <summary>
        /// Tlačítko pro stažení dat z masterserveru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshServersBtn_Click(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\events.txt", "RefreshServersBtn() : VOID", L4u.LogType.EVENT);
            FetchMaster();
        }

        /// <summary>
        /// Při vybrání serverů z tabulky uloží informace do proměnných.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnServerSelection(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\events.txt", "OnServerSelection(object sender, EventArgs e) : VOID", L4u.LogType.EVENT);

            try
            {
                foreach (ListViewItem item in ServerListView.Items)
                {
                    if (item.Selected)
                    {
                        connectionString = item.SubItems[6].Text.Replace(":", " ");
                        SelectedServerLbl.Text = Language.GetStr(1043) + " " + item.SubItems[1].Text;
                        selectedServerIPandPort = item.SubItems[6].Text;
                        selectedServerName = item.SubItems[1].Text;
                        UnselectServerBtn.Enabled = true;
                    }
                }

                L4u.WriteLog("logs\\infos.txt", "Successfully selected server! name: " + selectedServerName + " - ip: " + selectedServerIPandPort, L4u.LogType.INFO);
            }
            catch
            {
                L4u.WriteLog("logs\\errors.txt", "Cannot select a server from list.", L4u.LogType.ERROR);
            }
        }

        /// <summary>
        /// Stáhne data z masterserveru a zobrazí v tabulce
        /// </summary>
        private void FetchMaster()
        {
            L4u.WriteLog("logs\\events.txt", "FetchMaster() : VOID", L4u.LogType.EVENT);
            
            if (!isFetching)
            {
                RefreshServersBtn.Text = Language.GetStr(1051);
                isFetching = true;
                ServerListView.Items.Clear();
                MultiplayerStatusLbl.Text = Language.GetStr(1044);

                fetchThread = new Thread(() =>
                {
                    try
                    {
                        var serverList = MasterServer.FetchMasterserver();
                        int playersTotal = 0;

                        foreach (var serverSelection in serverList)
                        {
                            if (!string.IsNullOrWhiteSpace(serverSelection.ServerName))
                            {
                                ListViewItem listViewItem = new ListViewItem(serverSelection.Id.ToString())
                                {
                                    SubItems = {
                                        serverSelection.ServerName,
                                        serverSelection.MapName,
                                        serverSelection.Mode,
                                        $"{serverSelection.PlayersCount} / {serverSelection.MaxPlayersCount}",
                                        serverSelection.Passworded ? Language.GetStr(2000) : Language.GetStr(2001),
                                        $"{serverSelection.IpAddress}:{serverSelection.Port}"
                                    }
                                };

                                if (ServerListView.InvokeRequired)
                                {
                                    ServerListView.Invoke(new Action(() => ServerListView.Items.Add(listViewItem)));
                                }
                                else
                                {
                                    ServerListView.Items.Add(listViewItem);
                                }

                                playersTotal += serverSelection.PlayersCount;
                            }
                        }

                        connectionString = null;
                        isFetching = false;

                        if (RefreshServersBtn.InvokeRequired)
                        {
                            RefreshServersBtn.Invoke(new Action(() => RefreshServersBtn.Text = Language.GetStr(1050)));
                        }
                        else
                        {
                            RefreshServersBtn.Text = Language.GetStr(1050);
                        }

                        if (MultiplayerStatusLbl.InvokeRequired)
                        {
                            MultiplayerStatusLbl.Invoke(new Action(() =>
                                MultiplayerStatusLbl.Text = Language.GetStr(1045)
                                    .Replace("{0}", serverList.Count.ToString())
                                    .Replace("{1}", playersTotal.ToString())
                            ));
                        }
                        else
                        {
                            MultiplayerStatusLbl.Text = Language.GetStr(1045)
                                .Replace("{0}", serverList.Count.ToString())
                                .Replace("{1}", playersTotal.ToString());
                        }

                        L4u.WriteLog("logs\\infos.txt", "Fetching masterserver was successfull. Got response 200 OK!", L4u.LogType.INFO);
                    }
                    catch
                    {
                        L4u.WriteLog("logs\\errors.txt", "Cannot fetch data from masterserver!", L4u.LogType.ERROR);
                    }
                });

                fetchThread.Start();
                L4u.WriteLog("logs\\infos.txt", "Trying to fetch masterserver..", L4u.LogType.INFO);
            }
            else
            {
                L4u.WriteLog("logs\\infos.txt", "Fetching serverlist was cancelled by the user.", L4u.LogType.INFO);

                fetchThread.Abort();
                isFetching = false;

                if (RefreshServersBtn.InvokeRequired)
                {
                    RefreshServersBtn.Invoke(new Action(() => RefreshServersBtn.Text = Language.GetStr(1050)));
                }
                else
                {
                    RefreshServersBtn.Text = Language.GetStr(1050);
                }

                if (MultiplayerStatusLbl.InvokeRequired)
                {
                    MultiplayerStatusLbl.Invoke(new Action(() => MultiplayerStatusLbl.Text = Language.GetStr(1046)));
                }
                else
                {
                    MultiplayerStatusLbl.Text = Language.GetStr(1046);
                }
            }
        }

        /// <summary>
        /// Nastaví do proměnné kód jazyka pro změnu při uložení a restartu aplikace.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeLanguage(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\events.txt", "ChangeLanguage(object sender, EventArgs e) : VOID", L4u.LogType.EVENT);

            try
            {
                newSelectedLanguageCode = LanguageSelectorList.SelectedItem.ToString().Trim();

                if (newSelectedLanguageCode.Contains(";"))
                {
                    newSelectedLanguageCode = newSelectedLanguageCode.Split(';')[0].Trim();
                }

                L4u.WriteLog("logs\\infos.txt", "Language successfully changed!", L4u.LogType.INFO);
            }
            catch
            {
                L4u.WriteLog("logs\\errors.txt", "Cannot change the language!", L4u.LogType.ERROR);
            }
        }

        /// <summary>
        /// Zruší vybrání serveru v tabulce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectServerBtn_Click(object sender, EventArgs e)
        {
            L4u.WriteLog("logs\\events.txt", "UnselectServerBtn(object sender, EventArgs e) : VOID", L4u.LogType.EVENT);

            try
            {
                connectionString = null;
                SelectedServerLbl.Text = $"{Language.GetStr(1043)} {Language.GetStr(2002)}";
                selectedServerIPandPort = null;
                selectedServerName = null;
                ServerListView.Refresh();
                UnselectServerBtn.Enabled = false;
                L4u.WriteLog("logs\\infos.txt", "Server successfully unselected!", L4u.LogType.INFO);
            }
            catch
            {
                L4u.WriteLog("logs\\errors.txt", "Cannot unselect server!", L4u.LogType.ERROR);
            }
        }

        /// <summary>
        /// Odeslání zprávy do hry (pokud je hráč na serveru)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InGameChatOnInput(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                L4u.WriteLog("logs\\events.txt", "InGameChatOnInput(object sender, KeyEventArgs e) : VOID", L4u.LogType.EVENT);

                string text = InGameChatInput.Text.Trim();
                InGameChatInput.Clear();

                try
                {
                    Intercommunication.ConsoleOpened(true);
                    L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl CNS_Open(int) : VOID", L4u.LogType.EVENT);

                    bool isPublic = ToPublicChatRadio.Checked;

                    L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl CNS_AddCmdTxt(char*) : VOID", L4u.LogType.EVENT);
                    Intercommunication.SendMessageToServerAsPlayer(text, isPublic);
                    Thread.Sleep(50);
                    L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl CNS_AddTxt(char*) : VOID", L4u.LogType.EVENT);
                    Intercommunication.SendDataToMemory("");

                    Intercommunication.ConsoleOpened(false);
                    L4u.WriteLog("logs\\events.txt", "LOGS.DLL detour called function: __cdecl CNS_Close(int) : VOID", L4u.LogType.EVENT);


                    L4u.WriteLog("logs\\infos.txt", "Message was successfully sent to game!", L4u.LogType.INFO);
                }
                catch
                {
                    L4u.WriteLog("logs\\errors.txt", "Cannot send message to the game (in-game chat).", L4u.LogType.ERROR);
                    Logger.LogError(Language.GetStr(3009), Language.GetStr(2999));
                }
            }
        }

        /// <summary>
        /// Při zavírání hlavního okna.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AskForClose(object sender, FormClosingEventArgs e)
        {
            L4u.WriteLog("logs\\events.txt", "AskForClose(object sender, FormCLosingEventArgs e) : VOID", L4u.LogType.EVENT);
            L4u.WriteLog("logs\\infos.txt", "AskForClose() : VOID, was called, waiting for user response..", L4u.LogType.INFO);

            DialogResult dialogResult = MessageBox.Show(
                Language.GetStr(3060),
                "Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialogResult == DialogResult.Yes)
            {
                if (processId != 0 && Processes.IsProcessRunning(processId))
                {
                    L4u.WriteLog("logs\\infos.txt", "Killing the game with pID: " + processId + ".", L4u.LogType.INFO);
                    Process.GetProcessById(processId).Kill();
                }
                else
                {
                    L4u.WriteLog("logs\\infos.txt", "Game not running, ignoring KillProcess() : VOID statement.", L4u.LogType.INFO);
                }

                L4u.WriteLog("logs\\infos.txt", "VcComHub was successfully exited with code 0! (closed by the user)", L4u.LogType.INFO);

                Environment.Exit(0);
            }
            else
            {
                L4u.WriteLog("logs\\infos.txt", "Closing VcComHub was cancelled by the user.", L4u.LogType.INFO);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Otevře soubor chatu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenChatlogBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(vcGamePath + "\\dev\\chatlog.txt"))
            {
                Process.Start("notepad.exe", vcGamePath + "\\dev\\chatlog.txt");
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\dev\\chatlog.txt not found"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře složku s mapama.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMapsFolderBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(vcGamePath + "\\maps"))
            {
                Process.Start("explorer.exe", vcGamePath + "\\maps");
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\maps"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře DEV složku s logama.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDevFolderBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(vcGamePath + "\\dev"))
            {
                Process.Start("explorer.exe", vcGamePath + "\\dev");
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\dev"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře složku s logama.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLogsFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(vcGamePath + "\\logs"))
            {
                Process.Start("explorer.exe", vcGamePath + "\\logs");
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\logs"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře složku s addonama hry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAddonsFolderBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(vcAddonPath))
            {
                Process.Start("explorer.exe", vcAddonPath);
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcAddonPath), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře složku Vietcongu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGameFolderBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(vcGamePath))
            {
                Process.Start("explorer.exe", vcGamePath);
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře console.txt log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenConsoleBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(vcGamePath + "\\logs\\console.txt"))
            {
                Process.Start("notepad.exe", vcGamePath + "\\logs\\console.txt");
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\logs\\console.txt"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Spustí VCStarter configurator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenVcStarterBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(vcGamePath + "\\VCStarterConfig.exe"))
            {
                Loader.StartProcess("VCStarterConfig.exe", vcGamePath, null, null);
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\VCStarterConfig.exe"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Spustí server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunServerBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(vcGamePath + "\\vcded.exe"))
            {
                Loader.StartProcess("vcded.exe", vcGamePath, null, null);
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\vcded.exe"), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Standartně spustí hru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunGameBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(vcGamePath + "\\" + Constants.vcModuleName))
            {
                Loader.StartProcess(Constants.vcModuleName, vcGamePath, null, null);
            }
            else
            {
                Logger.LogError(Language.GetStr(3011).Replace("%s", vcGamePath + "\\" + Constants.vcModuleName), Language.GetStr(2999));
            }
        }

        /// <summary>
        /// Otevře příkazovou řádku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCommandPromptBtn_Click(object sender, EventArgs e)
        {
            Process.Start("cmd.exe");
        }

        /// <summary>
        /// Otevře process notepad.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenNotepadBtn_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe");
        }

        /// <summary>
        /// Mění pravidlo ignorování neaktivního herního vlákna.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IgnoreUnfocusedWindowChanged(object sender, EventArgs e)
        {
            ignoreUnfocusedGameWindow = IgnoreUnfocusedWindowCheck.Checked;

            if (!ignoreUnfocusedGameWindow)
            {
                Window.FocusWindow(processId);
            }
        }

        /// <summary>
        /// Otevře odkaz na github.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGitLinkLbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/pavelkalas/vccomhub.git");
        }
    }
}
