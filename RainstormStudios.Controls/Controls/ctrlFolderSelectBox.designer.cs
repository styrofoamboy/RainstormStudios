namespace RainstormStudios.Controls
{
    partial class FolderSelectBox
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
            this.grpFolderPath = new System.Windows.Forms.GroupBox();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.grpFolderPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFolderPath
            // 
            this.grpFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFolderPath.Controls.Add(this.txtFolderPath);
            this.grpFolderPath.Controls.Add(this.cmdBrowse);
            this.grpFolderPath.Location = new System.Drawing.Point(0, 0);
            this.grpFolderPath.MaximumSize = new System.Drawing.Size(0, 40);
            this.grpFolderPath.MinimumSize = new System.Drawing.Size(280, 40);
            this.grpFolderPath.Name = "grpFolderPath";
            this.grpFolderPath.Size = new System.Drawing.Size(280, 40);
            this.grpFolderPath.TabIndex = 0;
            this.grpFolderPath.TabStop = false;
            this.grpFolderPath.Text = "Folder Path";
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderPath.Location = new System.Drawing.Point(8, 16);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(208, 20);
            this.txtFolderPath.TabIndex = 1;
            this.txtFolderPath.TextChanged += new System.EventHandler(this.txtFolderPath_onTextChanged);
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
            // FolderSelectBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpFolderPath);
            this.MinimumSize = new System.Drawing.Size(280, 40);
            this.Name = "FolderSelectBox";
            this.Size = new System.Drawing.Size(280, 40);
            this.grpFolderPath.ResumeLayout(false);
            this.grpFolderPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFolderPath;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.TextBox txtFolderPath;
    }
}
