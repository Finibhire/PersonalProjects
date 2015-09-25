namespace DragonKnight2_Farmer
{
    partial class MainInterfaceForm
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
            this.btnFarmMonsters = new System.Windows.Forms.Button();
            this.tbOut = new System.Windows.Forms.TextBox();
            this.btnFarmResources = new System.Windows.Forms.Button();
            this.ddlCookie = new System.Windows.Forms.ComboBox();
            this.btnGambling = new System.Windows.Forms.Button();
            this.tbTurnsPerMinute = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chbBreakOnEnchanter = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnFarmMonsters
            // 
            this.btnFarmMonsters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFarmMonsters.Location = new System.Drawing.Point(137, 11);
            this.btnFarmMonsters.Name = "btnFarmMonsters";
            this.btnFarmMonsters.Size = new System.Drawing.Size(112, 23);
            this.btnFarmMonsters.TabIndex = 0;
            this.btnFarmMonsters.Text = "Farm Monsters!";
            this.btnFarmMonsters.UseVisualStyleBackColor = true;
            this.btnFarmMonsters.Click += new System.EventHandler(this.btnFarmMonsters_Click);
            // 
            // tbOut
            // 
            this.tbOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOut.Location = new System.Drawing.Point(12, 41);
            this.tbOut.Multiline = true;
            this.tbOut.Name = "tbOut";
            this.tbOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOut.Size = new System.Drawing.Size(443, 183);
            this.tbOut.TabIndex = 1;
            // 
            // btnFarmResources
            // 
            this.btnFarmResources.Location = new System.Drawing.Point(12, 12);
            this.btnFarmResources.Name = "btnFarmResources";
            this.btnFarmResources.Size = new System.Drawing.Size(119, 23);
            this.btnFarmResources.TabIndex = 3;
            this.btnFarmResources.Text = "Farm Resources!";
            this.btnFarmResources.UseVisualStyleBackColor = true;
            this.btnFarmResources.Click += new System.EventHandler(this.btnFarmResources_Click);
            // 
            // ddlCookie
            // 
            this.ddlCookie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlCookie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCookie.FormattingEnabled = true;
            this.ddlCookie.Items.AddRange(new object[] {
            "635+Finibhire+74bc8b1ee53eb9d0323337582309e1ff+1",
            "784+Fini+74bc8b1ee53eb9d0323337582309e1ff+1"});
            this.ddlCookie.Location = new System.Drawing.Point(12, 230);
            this.ddlCookie.Name = "ddlCookie";
            this.ddlCookie.Size = new System.Drawing.Size(325, 21);
            this.ddlCookie.TabIndex = 5;
            this.ddlCookie.SelectedIndexChanged += new System.EventHandler(this.ddlCookie_SelectedIndexChanged);
            // 
            // btnGambling
            // 
            this.btnGambling.Location = new System.Drawing.Point(255, 11);
            this.btnGambling.Name = "btnGambling";
            this.btnGambling.Size = new System.Drawing.Size(113, 23);
            this.btnGambling.TabIndex = 6;
            this.btnGambling.Text = "Start Gambling!";
            this.btnGambling.UseVisualStyleBackColor = true;
            this.btnGambling.Click += new System.EventHandler(this.btnGambling_Click);
            // 
            // tbTurnsPerMinute
            // 
            this.tbTurnsPerMinute.Location = new System.Drawing.Point(426, 12);
            this.tbTurnsPerMinute.Name = "tbTurnsPerMinute";
            this.tbTurnsPerMinute.Size = new System.Drawing.Size(29, 20);
            this.tbTurnsPerMinute.TabIndex = 7;
            this.tbTurnsPerMinute.Text = "3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(374, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Turns/min";
            // 
            // chbBreakOnEnchanter
            // 
            this.chbBreakOnEnchanter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chbBreakOnEnchanter.AutoSize = true;
            this.chbBreakOnEnchanter.Location = new System.Drawing.Point(343, 234);
            this.chbBreakOnEnchanter.Name = "chbBreakOnEnchanter";
            this.chbBreakOnEnchanter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chbBreakOnEnchanter.Size = new System.Drawing.Size(112, 17);
            this.chbBreakOnEnchanter.TabIndex = 9;
            this.chbBreakOnEnchanter.Text = "Break on Enchant";
            this.chbBreakOnEnchanter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbBreakOnEnchanter.UseVisualStyleBackColor = true;
            // 
            // MainInterfaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 262);
            this.Controls.Add(this.chbBreakOnEnchanter);
            this.Controls.Add(this.tbTurnsPerMinute);
            this.Controls.Add(this.btnGambling);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlCookie);
            this.Controls.Add(this.btnFarmResources);
            this.Controls.Add(this.tbOut);
            this.Controls.Add(this.btnFarmMonsters);
            this.Name = "MainInterfaceForm";
            this.Text = "Dragon Knight 2 Farming";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFarmMonsters;
        private System.Windows.Forms.TextBox tbOut;
        private System.Windows.Forms.Button btnFarmResources;
        private System.Windows.Forms.ComboBox ddlCookie;
        private System.Windows.Forms.Button btnGambling;
        private System.Windows.Forms.TextBox tbTurnsPerMinute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chbBreakOnEnchanter;
    }
}

