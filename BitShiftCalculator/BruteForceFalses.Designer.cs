namespace BitShiftCalculator
{
    partial class BruteForceFalses
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
            this.progressOverall = new System.Windows.Forms.ProgressBar();
            this.numBits = new System.Windows.Forms.NumericUpDown();
            this.progressCurrent = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.tbOut = new System.Windows.Forms.TextBox();
            this.btnBuildCode = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStructure = new System.Windows.Forms.Button();
            this.btnFalseTester = new System.Windows.Forms.Button();
            this.btnTryFour = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBits)).BeginInit();
            this.SuspendLayout();
            // 
            // progressOverall
            // 
            this.progressOverall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressOverall.Location = new System.Drawing.Point(12, 612);
            this.progressOverall.Name = "progressOverall";
            this.progressOverall.Size = new System.Drawing.Size(896, 23);
            this.progressOverall.TabIndex = 0;
            // 
            // numBits
            // 
            this.numBits.Location = new System.Drawing.Point(132, 12);
            this.numBits.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numBits.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numBits.Name = "numBits";
            this.numBits.Size = new System.Drawing.Size(75, 20);
            this.numBits.TabIndex = 1;
            this.numBits.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // progressCurrent
            // 
            this.progressCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressCurrent.Location = new System.Drawing.Point(12, 592);
            this.progressCurrent.Name = "progressCurrent";
            this.progressCurrent.Size = new System.Drawing.Size(896, 14);
            this.progressCurrent.TabIndex = 2;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(27, 576);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(51, 13);
            this.lblProgress.TabIndex = 3;
            this.lblProgress.Text = "Progress:";
            // 
            // tbOut
            // 
            this.tbOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOut.Location = new System.Drawing.Point(12, 37);
            this.tbOut.Multiline = true;
            this.tbOut.Name = "tbOut";
            this.tbOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOut.Size = new System.Drawing.Size(896, 492);
            this.tbOut.TabIndex = 4;
            // 
            // btnBuildCode
            // 
            this.btnBuildCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuildCode.Location = new System.Drawing.Point(833, 563);
            this.btnBuildCode.Name = "btnBuildCode";
            this.btnBuildCode.Size = new System.Drawing.Size(75, 23);
            this.btnBuildCode.TabIndex = 5;
            this.btnBuildCode.Text = "Build Code";
            this.btnBuildCode.UseVisualStyleBackColor = true;
            this.btnBuildCode.Click += new System.EventHandler(this.btnBuildCode_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Total Encrypted Bits:";
            // 
            // btnStructure
            // 
            this.btnStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructure.Location = new System.Drawing.Point(681, 563);
            this.btnStructure.Name = "btnStructure";
            this.btnStructure.Size = new System.Drawing.Size(146, 23);
            this.btnStructure.TabIndex = 7;
            this.btnStructure.Text = "Get Encription Structure";
            this.btnStructure.UseVisualStyleBackColor = true;
            this.btnStructure.Click += new System.EventHandler(this.btnStructure_Click);
            // 
            // btnFalseTester
            // 
            this.btnFalseTester.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFalseTester.Location = new System.Drawing.Point(600, 563);
            this.btnFalseTester.Name = "btnFalseTester";
            this.btnFalseTester.Size = new System.Drawing.Size(75, 23);
            this.btnFalseTester.TabIndex = 8;
            this.btnFalseTester.Text = "False Tester";
            this.btnFalseTester.UseVisualStyleBackColor = true;
            this.btnFalseTester.Click += new System.EventHandler(this.btnFalseTester_Click);
            // 
            // btnTryFour
            // 
            this.btnTryFour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTryFour.Location = new System.Drawing.Point(519, 563);
            this.btnTryFour.Name = "btnTryFour";
            this.btnTryFour.Size = new System.Drawing.Size(75, 23);
            this.btnTryFour.TabIndex = 9;
            this.btnTryFour.Text = "Try Four";
            this.btnTryFour.UseVisualStyleBackColor = true;
            this.btnTryFour.Click += new System.EventHandler(this.btnTryFour_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(438, 563);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Speed Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BruteForceFalses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 647);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTryFour);
            this.Controls.Add(this.btnFalseTester);
            this.Controls.Add(this.btnStructure);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBuildCode);
            this.Controls.Add(this.tbOut);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressCurrent);
            this.Controls.Add(this.numBits);
            this.Controls.Add(this.progressOverall);
            this.Name = "BruteForceFalses";
            this.Text = "BruteForceFalses";
            ((System.ComponentModel.ISupportInitialize)(this.numBits)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressOverall;
        private System.Windows.Forms.NumericUpDown numBits;
        private System.Windows.Forms.ProgressBar progressCurrent;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.TextBox tbOut;
        private System.Windows.Forms.Button btnBuildCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStructure;
        private System.Windows.Forms.Button btnFalseTester;
        private System.Windows.Forms.Button btnTryFour;
        private System.Windows.Forms.Button button1;
    }
}