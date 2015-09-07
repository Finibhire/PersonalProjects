namespace LogicAlgebra
{
    partial class FormFront
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
            this.tbInput = new System.Windows.Forms.TextBox();
            this.tbOut = new System.Windows.Forms.TextBox();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.tbExtract = new System.Windows.Forms.TextBox();
            this.btnTestReduce = new System.Windows.Forms.Button();
            this.btnFullExpand = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbInput
            // 
            this.tbInput.Location = new System.Drawing.Point(12, 12);
            this.tbInput.Multiline = true;
            this.tbInput.Name = "tbInput";
            this.tbInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbInput.Size = new System.Drawing.Size(334, 186);
            this.tbInput.TabIndex = 0;
            // 
            // tbOut
            // 
            this.tbOut.Location = new System.Drawing.Point(484, 12);
            this.tbOut.Multiline = true;
            this.tbOut.Name = "tbOut";
            this.tbOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOut.Size = new System.Drawing.Size(334, 186);
            this.tbOut.TabIndex = 1;
            // 
            // btnExpand
            // 
            this.btnExpand.Location = new System.Drawing.Point(352, 96);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(126, 23);
            this.btnExpand.TabIndex = 2;
            this.btnExpand.Text = "Expand";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(484, 204);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(126, 23);
            this.btnExtract.TabIndex = 3;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // tbExtract
            // 
            this.tbExtract.Location = new System.Drawing.Point(484, 234);
            this.tbExtract.Name = "tbExtract";
            this.tbExtract.Size = new System.Drawing.Size(334, 20);
            this.tbExtract.TabIndex = 4;
            // 
            // btnTestReduce
            // 
            this.btnTestReduce.Location = new System.Drawing.Point(692, 436);
            this.btnTestReduce.Name = "btnTestReduce";
            this.btnTestReduce.Size = new System.Drawing.Size(126, 23);
            this.btnTestReduce.TabIndex = 5;
            this.btnTestReduce.Text = "Test Reduce";
            this.btnTestReduce.UseVisualStyleBackColor = true;
            this.btnTestReduce.Click += new System.EventHandler(this.btnTestReduce_Click);
            // 
            // btnFullExpand
            // 
            this.btnFullExpand.Location = new System.Drawing.Point(692, 407);
            this.btnFullExpand.Name = "btnFullExpand";
            this.btnFullExpand.Size = new System.Drawing.Size(126, 23);
            this.btnFullExpand.TabIndex = 6;
            this.btnFullExpand.Text = "Full Expand";
            this.btnFullExpand.UseVisualStyleBackColor = true;
            this.btnFullExpand.Click += new System.EventHandler(this.btnFullExpand_Click);
            // 
            // FormFront
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 471);
            this.Controls.Add(this.btnFullExpand);
            this.Controls.Add(this.btnTestReduce);
            this.Controls.Add(this.tbExtract);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.tbOut);
            this.Controls.Add(this.tbInput);
            this.Name = "FormFront";
            this.Text = "Expander";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.TextBox tbOut;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.TextBox tbExtract;
        private System.Windows.Forms.Button btnTestReduce;
        private System.Windows.Forms.Button btnFullExpand;
    }
}

