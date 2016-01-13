namespace RainstormStudios.Forms
{
    partial class frmAsyncProgress
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
            this.lblCaption = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmdAbort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(16, 16);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(75, 13);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Total Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 32);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(344, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // cmdAbort
            // 
            this.cmdAbort.Location = new System.Drawing.Point(368, 32);
            this.cmdAbort.Name = "cmdAbort";
            this.cmdAbort.Size = new System.Drawing.Size(64, 23);
            this.cmdAbort.TabIndex = 2;
            this.cmdAbort.Text = "Cancel";
            this.cmdAbort.UseVisualStyleBackColor = true;
            // 
            // frmSimpleProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 73);
            this.Controls.Add(this.cmdAbort);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSimpleProgress";
            this.Text = "Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cmdAbort;
    }
}