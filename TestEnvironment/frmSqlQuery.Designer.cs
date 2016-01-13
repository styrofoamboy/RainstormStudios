namespace TestEnvironment
{
    partial class frmSqlQuery
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
            this.queryTextBox1 = new RainstormStudios.Controls.QueryTextBox();
            this.SuspendLayout();
            // 
            // queryTextBox1
            // 
            this.queryTextBox1.AcceptsTab = true;
            this.queryTextBox1.AliasColor = System.Drawing.Color.Navy;
            this.queryTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.queryTextBox1.CommentColor = System.Drawing.Color.Green;
            this.queryTextBox1.DetectUrls = false;
            this.queryTextBox1.KeywordColor = System.Drawing.Color.Blue;
            this.queryTextBox1.Location = new System.Drawing.Point(12, 12);
            this.queryTextBox1.Name = "queryTextBox1";
            this.queryTextBox1.Size = new System.Drawing.Size(579, 353);
            this.queryTextBox1.StringLiteralColor = System.Drawing.Color.Red;
            this.queryTextBox1.TabIndex = 0;
            this.queryTextBox1.Text = "";
            this.queryTextBox1.UseDefaultContextMenu = true;
            // 
            // frmSqlQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 377);
            this.Controls.Add(this.queryTextBox1);
            this.Name = "frmSqlQuery";
            this.Text = "frmSqlQuery";
            this.ResumeLayout(false);

        }

        #endregion

        private RainstormStudios.Controls.QueryTextBox queryTextBox1;
    }
}