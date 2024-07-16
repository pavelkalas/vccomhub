using System;
using System.Threading;
using System.Windows.Forms;

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

namespace VcComHubGUI.GUIs
{
    /// <summary>
    /// Načítací okno VcComHubu.
    /// </summary>
    public partial class LoadingWindow : Form
    {
        /// <summary>
        /// Konstruktor načítacího okna.
        /// </summary>
        public LoadingWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ukončí aplikaci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelLoading_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void OnRender(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.Hide()));
                }
                else
                {
                    this.Hide();
                }
            }).Start();
        }
    }
}
