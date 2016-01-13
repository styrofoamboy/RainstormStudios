namespace RainstormStudios.Controls
{
    partial class FileSelectBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpFileName = new System.Windows.Forms.GroupBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.grpFileName.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFileName
            // 
            this.grpFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFileName.Controls.Add(this.txtFileName);
            this.grpFileName.Controls.Add(this.cmdBrowse);
            this.grpFileName.Location = new System.Drawing.Point(0, 0);
            this.grpFileName.MaximumSize = new System.Drawing.Size(0, 40);
            this.grpFileName.MinimumSize = new System.Drawing.Size(280, 40);
            this.grpFileName.Name = "grpFileName";
            this.grpFileName.Size = new System.Drawing.Size(280, 40);
            this.grpFileName.TabIndex = 0;
            this.grpFileName.TabStop = false;
            this.grpFileName.Text = "File Path";
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Location = new System.Drawing.Point(8, 16);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(208, 20);
            this.txtFileName.TabIndex = 1;
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowse.Location = new System.Drawing.Point(216, 16);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(56, 20);
            this.cmdBrowse.TabIndex = 0;
            this.cmdBrowse.Text = "Browse";
            this.cmdBrowse.Click += new System.EventHandler(this.cmdButton_onClick);
            // 
            // FileSelectBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpFileName);
            this.MinimumSize = new System.Drawing.Size(280, 40);
            this.Name = "FileSelectBox";
            this.Size = new System.Drawing.Size(280, 40);
            this.grpFileName.ResumeLayout(false);
            this.grpFileName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFileName;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.TextBox txtFileName;
    }
}
