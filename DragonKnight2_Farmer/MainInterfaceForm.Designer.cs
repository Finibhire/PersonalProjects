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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblGold = new System.Windows.Forms.Label();
            this.lblTurns = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblStone = new System.Windows.Forms.Label();
            this.lblIron = new System.Windows.Forms.Label();
            this.lblWood = new System.Windows.Forms.Label();
            this.lblFish = new System.Windows.Forms.Label();
            this.lblDragonPoints = new System.Windows.Forms.Label();
            this.lblBankGold = new System.Windows.Forms.Label();
            this.lblBankDragonPoints = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblCycles = new System.Windows.Forms.Label();
            this.btnCollectResources = new System.Windows.Forms.Button();
            this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFarmMonsters
            // 
            this.btnFarmMonsters.Location = new System.Drawing.Point(284, 12);
            this.btnFarmMonsters.Name = "btnFarmMonsters";
            this.btnFarmMonsters.Size = new System.Drawing.Size(119, 23);
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
            this.tbOut.Location = new System.Drawing.Point(12, 223);
            this.tbOut.Multiline = true;
            this.tbOut.Name = "tbOut";
            this.tbOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOut.Size = new System.Drawing.Size(391, 60);
            this.tbOut.TabIndex = 1;
            // 
            // btnFarmResources
            // 
            this.btnFarmResources.Location = new System.Drawing.Point(284, 45);
            this.btnFarmResources.Name = "btnFarmResources";
            this.btnFarmResources.Size = new System.Drawing.Size(119, 23);
            this.btnFarmResources.TabIndex = 3;
            this.btnFarmResources.Text = "Farm Resources!";
            this.btnFarmResources.UseVisualStyleBackColor = true;
            this.btnFarmResources.Click += new System.EventHandler(this.btnFarmResources_Click);
            // 
            // ddlCookie
            // 
            this.ddlCookie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCookie.FormattingEnabled = true;
            this.ddlCookie.Items.AddRange(new object[] {
            "635+Finibhire+74bc8b1ee53eb9d0323337582309e1ff+1",
            "784+Fini+74bc8b1ee53eb9d0323337582309e1ff+1"});
            this.ddlCookie.Location = new System.Drawing.Point(12, 12);
            this.ddlCookie.Name = "ddlCookie";
            this.ddlCookie.Size = new System.Drawing.Size(255, 21);
            this.ddlCookie.TabIndex = 5;
            this.ddlCookie.SelectedIndexChanged += new System.EventHandler(this.ddlCookie_SelectedIndexChanged);
            // 
            // btnGambling
            // 
            this.btnGambling.Location = new System.Drawing.Point(284, 74);
            this.btnGambling.Name = "btnGambling";
            this.btnGambling.Size = new System.Drawing.Size(119, 23);
            this.btnGambling.TabIndex = 6;
            this.btnGambling.Text = "Start Gambling!";
            this.btnGambling.UseVisualStyleBackColor = true;
            this.btnGambling.Click += new System.EventHandler(this.btnGambling_Click);
            // 
            // tbTurnsPerMinute
            // 
            this.tbTurnsPerMinute.Location = new System.Drawing.Point(374, 151);
            this.tbTurnsPerMinute.Name = "tbTurnsPerMinute";
            this.tbTurnsPerMinute.Size = new System.Drawing.Size(29, 20);
            this.tbTurnsPerMinute.TabIndex = 7;
            this.tbTurnsPerMinute.Text = "3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(313, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Turns/min";
            // 
            // chbBreakOnEnchanter
            // 
            this.chbBreakOnEnchanter.AutoSize = true;
            this.chbBreakOnEnchanter.Location = new System.Drawing.Point(291, 200);
            this.chbBreakOnEnchanter.Name = "chbBreakOnEnchanter";
            this.chbBreakOnEnchanter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chbBreakOnEnchanter.Size = new System.Drawing.Size(112, 17);
            this.chbBreakOnEnchanter.TabIndex = 9;
            this.chbBreakOnEnchanter.Text = "Break on Enchant";
            this.chbBreakOnEnchanter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbBreakOnEnchanter.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Turns";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Gold";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Dragon Points";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Fish";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Iron";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Stone";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Wood";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Cycles";
            // 
            // lblGold
            // 
            this.lblGold.Location = new System.Drawing.Point(87, 29);
            this.lblGold.Name = "lblGold";
            this.lblGold.Size = new System.Drawing.Size(75, 13);
            this.lblGold.TabIndex = 18;
            this.lblGold.Text = "lblGold";
            this.lblGold.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblTurns
            // 
            this.lblTurns.Location = new System.Drawing.Point(87, 16);
            this.lblTurns.Name = "lblTurns";
            this.lblTurns.Size = new System.Drawing.Size(75, 13);
            this.lblTurns.TabIndex = 19;
            this.lblTurns.Text = "lblTurns";
            this.lblTurns.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLocation);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lblAction);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.lblStone);
            this.groupBox1.Controls.Add(this.lblIron);
            this.groupBox1.Controls.Add(this.lblWood);
            this.groupBox1.Controls.Add(this.lblFish);
            this.groupBox1.Controls.Add(this.lblDragonPoints);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.lblBankGold);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblBankDragonPoints);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblGold);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.lblCycles);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblTurns);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 178);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            // 
            // lblLocation
            // 
            this.lblLocation.Location = new System.Drawing.Point(90, 96);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(153, 13);
            this.lblLocation.TabIndex = 33;
            this.lblLocation.Text = "lblLocation";
            this.lblLocation.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(6, 96);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 13);
            this.label12.TabIndex = 32;
            this.label12.Text = "Location";
            // 
            // lblAction
            // 
            this.lblAction.Location = new System.Drawing.Point(90, 76);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(153, 13);
            this.lblAction.TabIndex = 31;
            this.lblAction.Text = "lblAction";
            this.lblAction.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "Next Action";
            // 
            // lblStone
            // 
            this.lblStone.Location = new System.Drawing.Point(87, 144);
            this.lblStone.Name = "lblStone";
            this.lblStone.Size = new System.Drawing.Size(75, 13);
            this.lblStone.TabIndex = 28;
            this.lblStone.Text = "lblStone";
            this.lblStone.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblIron
            // 
            this.lblIron.Location = new System.Drawing.Point(87, 131);
            this.lblIron.Name = "lblIron";
            this.lblIron.Size = new System.Drawing.Size(75, 13);
            this.lblIron.TabIndex = 26;
            this.lblIron.Text = "lblIron";
            this.lblIron.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblWood
            // 
            this.lblWood.Location = new System.Drawing.Point(87, 157);
            this.lblWood.Name = "lblWood";
            this.lblWood.Size = new System.Drawing.Size(75, 13);
            this.lblWood.TabIndex = 29;
            this.lblWood.Text = "lblWood";
            this.lblWood.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblFish
            // 
            this.lblFish.Location = new System.Drawing.Point(87, 118);
            this.lblFish.Name = "lblFish";
            this.lblFish.Size = new System.Drawing.Size(75, 13);
            this.lblFish.TabIndex = 27;
            this.lblFish.Text = "lblFish";
            this.lblFish.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDragonPoints
            // 
            this.lblDragonPoints.Location = new System.Drawing.Point(87, 42);
            this.lblDragonPoints.Name = "lblDragonPoints";
            this.lblDragonPoints.Size = new System.Drawing.Size(75, 13);
            this.lblDragonPoints.TabIndex = 20;
            this.lblDragonPoints.Text = "lblDragonPoints";
            this.lblDragonPoints.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblBankGold
            // 
            this.lblBankGold.Location = new System.Drawing.Point(168, 29);
            this.lblBankGold.Name = "lblBankGold";
            this.lblBankGold.Size = new System.Drawing.Size(75, 13);
            this.lblBankGold.TabIndex = 23;
            this.lblBankGold.Text = "lblBankGold";
            this.lblBankGold.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblBankDragonPoints
            // 
            this.lblBankDragonPoints.Location = new System.Drawing.Point(168, 42);
            this.lblBankDragonPoints.Name = "lblBankDragonPoints";
            this.lblBankDragonPoints.Size = new System.Drawing.Size(75, 13);
            this.lblBankDragonPoints.TabIndex = 22;
            this.lblBankDragonPoints.Text = "lblBankDragonPoints";
            this.lblBankDragonPoints.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(156, 42);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(15, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "::";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(156, 29);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(15, 13);
            this.label15.TabIndex = 24;
            this.label15.Text = "::";
            this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCycles
            // 
            this.lblCycles.Location = new System.Drawing.Point(87, 55);
            this.lblCycles.Name = "lblCycles";
            this.lblCycles.Size = new System.Drawing.Size(75, 13);
            this.lblCycles.TabIndex = 21;
            this.lblCycles.Text = "lblCycles";
            this.lblCycles.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnCollectResources
            // 
            this.btnCollectResources.Location = new System.Drawing.Point(284, 103);
            this.btnCollectResources.Name = "btnCollectResources";
            this.btnCollectResources.Size = new System.Drawing.Size(119, 23);
            this.btnCollectResources.TabIndex = 21;
            this.btnCollectResources.Text = "Collect Resources";
            this.btnCollectResources.UseVisualStyleBackColor = true;
            this.btnCollectResources.Click += new System.EventHandler(this.btnCollectResources_Click);
            // 
            // chkAlwaysOnTop
            // 
            this.chkAlwaysOnTop.AutoSize = true;
            this.chkAlwaysOnTop.Checked = true;
            this.chkAlwaysOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlwaysOnTop.Location = new System.Drawing.Point(305, 177);
            this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            this.chkAlwaysOnTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkAlwaysOnTop.Size = new System.Drawing.Size(98, 17);
            this.chkAlwaysOnTop.TabIndex = 22;
            this.chkAlwaysOnTop.Text = "Always On Top";
            this.chkAlwaysOnTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
            this.chkAlwaysOnTop.CheckedChanged += new System.EventHandler(this.chkAlwaysOnTop_CheckedChanged);
            // 
            // MainInterfaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 295);
            this.Controls.Add(this.chkAlwaysOnTop);
            this.Controls.Add(this.btnCollectResources);
            this.Controls.Add(this.groupBox1);
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
            this.Load += new System.EventHandler(this.MainInterfaceForm_Load);
            this.groupBox1.ResumeLayout(false);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblGold;
        private System.Windows.Forms.Label lblTurns;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblBankGold;
        private System.Windows.Forms.Label lblBankDragonPoints;
        private System.Windows.Forms.Label lblDragonPoints;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblCycles;
        private System.Windows.Forms.Label lblStone;
        private System.Windows.Forms.Label lblIron;
        private System.Windows.Forms.Label lblWood;
        private System.Windows.Forms.Label lblFish;
        private System.Windows.Forms.Button btnCollectResources;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chkAlwaysOnTop;
    }
}

