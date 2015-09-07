namespace TrueRandomBitGenerator
{
    partial class Main
    {
        //System.Resources.ResourceManager rm = new System.Resources.ResourceManager("TrueRandomBitGenerator.Main", System.Reflection.Assembly.GetExecutingAssembly());

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
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tbFileLocation = new System.Windows.Forms.TextBox();
            this.btnChangeFile = new System.Windows.Forms.Button();
            this.btnAppend = new System.Windows.Forms.Button();
            this.lblDataCollectedHeading = new System.Windows.Forms.Label();
            this.lblDataCollected = new System.Windows.Forms.Label();
            this.btnCaptureGraphics = new System.Windows.Forms.Button();
            this.btnMedia = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbFileLocation
            // 
            this.tbFileLocation.Location = new System.Drawing.Point(12, 12);
            this.tbFileLocation.Name = "tbFileLocation";
            this.tbFileLocation.Size = new System.Drawing.Size(179, 20);
            this.tbFileLocation.TabIndex = 0;
            // 
            // btnChangeFile
            // 
            this.btnChangeFile.Location = new System.Drawing.Point(197, 9);
            this.btnChangeFile.Name = "btnChangeFile";
            this.btnChangeFile.Size = new System.Drawing.Size(75, 23);
            this.btnChangeFile.TabIndex = 1;
            this.btnChangeFile.Text = "Change";
            this.btnChangeFile.UseVisualStyleBackColor = true;
            this.btnChangeFile.Click += new System.EventHandler(this.btnChangeFile_Click);
            // 
            // btnAppend
            // 
            this.btnAppend.Location = new System.Drawing.Point(12, 38);
            this.btnAppend.Name = "btnAppend";
            this.btnAppend.Size = new System.Drawing.Size(260, 23);
            this.btnAppend.TabIndex = 2;
            this.btnAppend.Text = global::TrueRandomBitGenerator.Properties.Resources.btnAppend_StartText;
            this.btnAppend.UseVisualStyleBackColor = true;
            this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
            // 
            // lblDataCollectedHeading
            // 
            this.lblDataCollectedHeading.AutoSize = true;
            this.lblDataCollectedHeading.Location = new System.Drawing.Point(12, 99);
            this.lblDataCollectedHeading.Name = "lblDataCollectedHeading";
            this.lblDataCollectedHeading.Size = new System.Drawing.Size(83, 13);
            this.lblDataCollectedHeading.TabIndex = 3;
            this.lblDataCollectedHeading.Text = "Data Collected: ";
            // 
            // lblDataCollected
            // 
            this.lblDataCollected.AutoSize = true;
            this.lblDataCollected.Location = new System.Drawing.Point(101, 99);
            this.lblDataCollected.Name = "lblDataCollected";
            this.lblDataCollected.Size = new System.Drawing.Size(32, 13);
            this.lblDataCollected.TabIndex = 4;
            this.lblDataCollected.Text = "0 bits";
            // 
            // btnCaptureGraphics
            // 
            this.btnCaptureGraphics.Location = new System.Drawing.Point(197, 67);
            this.btnCaptureGraphics.Name = "btnCaptureGraphics";
            this.btnCaptureGraphics.Size = new System.Drawing.Size(75, 23);
            this.btnCaptureGraphics.TabIndex = 5;
            this.btnCaptureGraphics.Text = "Graphic";
            this.btnCaptureGraphics.UseVisualStyleBackColor = true;
            this.btnCaptureGraphics.Click += new System.EventHandler(this.btnCaptureGraphics_Click);
            // 
            // btnMedia
            // 
            this.btnMedia.Location = new System.Drawing.Point(12, 67);
            this.btnMedia.Name = "btnMedia";
            this.btnMedia.Size = new System.Drawing.Size(75, 23);
            this.btnMedia.TabIndex = 6;
            this.btnMedia.Text = "Media";
            this.btnMedia.UseVisualStyleBackColor = true;
            this.btnMedia.Click += new System.EventHandler(this.btnMedia_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 121);
            this.Controls.Add(this.btnMedia);
            this.Controls.Add(this.btnCaptureGraphics);
            this.Controls.Add(this.lblDataCollected);
            this.Controls.Add(this.lblDataCollectedHeading);
            this.Controls.Add(this.btnAppend);
            this.Controls.Add(this.btnChangeFile);
            this.Controls.Add(this.tbFileLocation);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "True Random Bit Generator";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox tbFileLocation;
        private System.Windows.Forms.Button btnChangeFile;
        private System.Windows.Forms.Button btnAppend;
        private System.Windows.Forms.Label lblDataCollectedHeading;
        private System.Windows.Forms.Label lblDataCollected;
        private System.Windows.Forms.Button btnCaptureGraphics;
        private System.Windows.Forms.Button btnMedia;
    }
}

