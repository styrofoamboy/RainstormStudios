namespace RainstormStudios.Forms
{
    partial class frmProgress
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
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new RainstormStudios.Controls.AdvancedProgressBar();
            this.cmdAbort = new System.Windows.Forms.Button();
            this.grpProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpProgress
            // 
            this.grpProgress.Controls.Add(this.progressBar1);
            this.grpProgress.Location = new System.Drawing.Point(8, 8);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(416, 40);
            this.grpProgress.TabIndex = 1;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.BackColor = System.Drawing.SystemColors.Control;
            this.progressBar1.BlockRotationDegrees = 0F;
            this.progressBar1.BorderColor = System.Drawing.Color.Gray;
            this.progressBar1.CornerFeather = 3;
            this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(131)))), ((int)(((byte)(174)))), ((int)(((byte)(118)))));
            this.progressBar1.Location = new System.Drawing.Point(8, 16);
            this.progressBar1.MinimumSize = new System.Drawing.Size(8, 8);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ProgressBackgroundColor = System.Drawing.Color.White;
            this.progressBar1.ProgressBackgroundImage = null;
            this.progressBar1.ProgressBackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.progressBar1.ProgressBackgroundImageRotateFlip = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this.progressBar1.ProgressBackgroundImageTileMode = System.Drawing.Drawing2D.WrapMode.Tile;
            this.progressBar1.ProgressImage = null;
            this.progressBar1.ProgressImageLayout = System.Windows.Forms.ImageLayout.None;
            this.progressBar1.ProgressImageRotateFlip = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            this.progressBar1.ProgressImageTileMode = System.Drawing.Drawing2D.WrapMode.Tile;
            this.progressBar1.Size = new System.Drawing.Size(400, 16);
            this.progressBar1.TabIndex = 0;
            this.progressBar1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // cmdAbort
            // 
            this.cmdAbort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAbort.Location = new System.Drawing.Point(336, 56);
            this.cmdAbort.Name = "cmdAbort";
            this.cmdAbort.Size = new System.Drawing.Size(75, 19);
            this.cmdAbort.TabIndex = 2;
            this.cmdAbort.Text = "Abort";
            this.cmdAbort.Visible = false;
            // 
            // frmProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 57);
            this.Controls.Add(this.cmdAbort);
            this.Controls.Add(this.grpProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Progress";
            this.Load += new System.EventHandler(this.frmProgress_onLoad);
            this.grpProgress.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.Button cmdAbort;
        private RainstormStudios.Controls.AdvancedProgressBar progressBar1;
    }
}