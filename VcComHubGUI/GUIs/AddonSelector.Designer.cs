namespace VcComHubGUI.GUIs
{
    partial class AddonSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddonsGrp = new System.Windows.Forms.GroupBox();
            this.ChooseSelectedBtn = new System.Windows.Forms.Button();
            this.AddonsList = new System.Windows.Forms.ListBox();
            this.PlayWithoutAddonBtn = new System.Windows.Forms.Button();
            this.CancelRunning = new System.Windows.Forms.Button();
            this.AddonsGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddonsGrp
            // 
            this.AddonsGrp.Controls.Add(this.AddonsList);
            this.AddonsGrp.Location = new System.Drawing.Point(12, 12);
            this.AddonsGrp.Name = "AddonsGrp";
            this.AddonsGrp.Size = new System.Drawing.Size(210, 125);
            this.AddonsGrp.TabIndex = 0;
            this.AddonsGrp.TabStop = false;
            this.AddonsGrp.Text = "Choose addon you want run:";
            // 
            // ChooseSelectedBtn
            // 
            this.ChooseSelectedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChooseSelectedBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ChooseSelectedBtn.Location = new System.Drawing.Point(11, 143);
            this.ChooseSelectedBtn.Name = "ChooseSelectedBtn";
            this.ChooseSelectedBtn.Size = new System.Drawing.Size(209, 29);
            this.ChooseSelectedBtn.TabIndex = 1;
            this.ChooseSelectedBtn.Text = "Choose addon";
            this.ChooseSelectedBtn.UseVisualStyleBackColor = true;
            this.ChooseSelectedBtn.Click += new System.EventHandler(this.ChooseSelectedBtn_Click);
            // 
            // AddonsList
            // 
            this.AddonsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.AddonsList.FormattingEnabled = true;
            this.AddonsList.ItemHeight = 16;
            this.AddonsList.Location = new System.Drawing.Point(6, 19);
            this.AddonsList.Name = "AddonsList";
            this.AddonsList.Size = new System.Drawing.Size(198, 100);
            this.AddonsList.TabIndex = 0;
            // 
            // PlayWithoutAddonBtn
            // 
            this.PlayWithoutAddonBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayWithoutAddonBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PlayWithoutAddonBtn.Location = new System.Drawing.Point(11, 178);
            this.PlayWithoutAddonBtn.Name = "PlayWithoutAddonBtn";
            this.PlayWithoutAddonBtn.Size = new System.Drawing.Size(209, 29);
            this.PlayWithoutAddonBtn.TabIndex = 2;
            this.PlayWithoutAddonBtn.Text = "Play without addon";
            this.PlayWithoutAddonBtn.UseVisualStyleBackColor = true;
            this.PlayWithoutAddonBtn.Click += new System.EventHandler(this.PlayWithoutAddonBtn_Click);
            // 
            // CancelRunning
            // 
            this.CancelRunning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelRunning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CancelRunning.Location = new System.Drawing.Point(11, 222);
            this.CancelRunning.Name = "CancelRunning";
            this.CancelRunning.Size = new System.Drawing.Size(209, 29);
            this.CancelRunning.TabIndex = 3;
            this.CancelRunning.Text = "Cancel";
            this.CancelRunning.UseVisualStyleBackColor = true;
            this.CancelRunning.Click += new System.EventHandler(this.CancelRunning_Click);
            // 
            // AddonSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 263);
            this.Controls.Add(this.CancelRunning);
            this.Controls.Add(this.PlayWithoutAddonBtn);
            this.Controls.Add(this.ChooseSelectedBtn);
            this.Controls.Add(this.AddonsGrp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AddonSelector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddonSelector";
            this.Load += new System.EventHandler(this.OnAddonsDialogLoad);
            this.AddonsGrp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox AddonsGrp;
        private System.Windows.Forms.Button ChooseSelectedBtn;
        private System.Windows.Forms.ListBox AddonsList;
        private System.Windows.Forms.Button PlayWithoutAddonBtn;
        private System.Windows.Forms.Button CancelRunning;
    }
}