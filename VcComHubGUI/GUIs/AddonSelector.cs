using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VcComHub.ConfigParser;
using VcComHub.LanguageParser;

namespace VcComHubGUI.GUIs
{
    public partial class AddonSelector : Form
    {
        public static string addonName = null;

        public AddonSelector()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Načte složku s addonama do listu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddonsDialogLoad(object sender, EventArgs e)
        {
            this.AddonsGrp.Text = Language.GetStr(2017);
            this.PlayWithoutAddonBtn.Text = Language.GetStr(2016);
            this.CancelRunning.Text = Language.GetStr(2014);
            this.ChooseSelectedBtn.Text = Language.GetStr(2015);

            this.BackColor = Constants.darkThemeBackground2;
            this.AddonsGrp.ForeColor = Constants.darkThemeText;
            this.PlayWithoutAddonBtn.ForeColor = Constants.darkThemeText;
            this.CancelRunning.ForeColor = Constants.darkThemeText;
            this.ChooseSelectedBtn.ForeColor = Constants.darkThemeText;

            string path = Config.GetConfig("vc.path.addons");

            if (Directory.Exists(path))
            {
                foreach (var addonDirectories in Directory.GetDirectories(path))
                {
                    AddonsList.Items.Add(new DirectoryInfo(addonDirectories).Name);
                }
            }
        }

        /// <summary>
        /// Potvrdí vybraný addon a spustí hru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseSelectedBtn_Click(object sender, EventArgs e)
        {
            try
            {
                addonName = AddonsList.SelectedItem.ToString();
                this.Hide();
            }
            catch
            {
                Logger.LogError(Language.GetStr(3014), Language.GetStr(2999));
            }
        }

        private void PlayWithoutAddonBtn_Click(object sender, EventArgs e)
        {
            addonName = "noaddon";
            this.Hide();
        }

        private void CancelRunning_Click(object sender, EventArgs e)
        {
            addonName = "norun";
            this.Hide();
        }
    }
}
