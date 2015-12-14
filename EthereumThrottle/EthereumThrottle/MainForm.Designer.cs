namespace EthereumThrottle
{
    partial class MainForm
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
            this.btnStart = new System.Windows.Forms.Button();
            this.txtFileLocation = new System.Windows.Forms.TextBox();
            this.txtOut = new System.Windows.Forms.TextBox();
            this.btnTestRead = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(298, 39);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtFileLocation
            // 
            this.txtFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileLocation.Location = new System.Drawing.Point(13, 13);
            this.txtFileLocation.Name = "txtFileLocation";
            this.txtFileLocation.Size = new System.Drawing.Size(360, 20);
            this.txtFileLocation.TabIndex = 1;
            this.txtFileLocation.Text = "c:\\Program Files\\Ethereum 1.0.1\\Release\\ethminer.exe";
            // 
            // txtOut
            // 
            this.txtOut.AcceptsReturn = true;
            this.txtOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOut.Location = new System.Drawing.Point(12, 72);
            this.txtOut.Multiline = true;
            this.txtOut.Name = "txtOut";
            this.txtOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOut.Size = new System.Drawing.Size(361, 141);
            this.txtOut.TabIndex = 2;
            // 
            // btnTestRead
            // 
            this.btnTestRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestRead.Enabled = false;
            this.btnTestRead.Location = new System.Drawing.Point(154, 43);
            this.btnTestRead.Name = "btnTestRead";
            this.btnTestRead.Size = new System.Drawing.Size(75, 23);
            this.btnTestRead.TabIndex = 3;
            this.btnTestRead.Text = "test read";
            this.btnTestRead.UseVisualStyleBackColor = true;
            this.btnTestRead.Click += new System.EventHandler(this.btnTestRead_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 225);
            this.Controls.Add(this.btnTestRead);
            this.Controls.Add(this.txtOut);
            this.Controls.Add(this.txtFileLocation);
            this.Controls.Add(this.btnStart);
            this.Name = "MainForm";
            this.Text = "Ethereum Throttle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtFileLocation;
        private System.Windows.Forms.TextBox txtOut;
        private System.Windows.Forms.Button btnTestRead;
    }
}

