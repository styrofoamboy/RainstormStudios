namespace RainstormStudios.Controls
{
    partial class ConnectionStringBox
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
            System.Drawing.Drawing2D.GraphicsPath graphicsPath1 = new System.Drawing.Drawing2D.GraphicsPath();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.cmdStringBuilder = new RainstormStudios.Controls.AdvancedButton();
            this.drpProviderType = new System.Windows.Forms.ComboBox();
            this.lblProviderType = new System.Windows.Forms.Label();
            this.txtConnStr = new System.Windows.Forms.TextBox();
            this.grpConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnection.Controls.Add(this.cmdStringBuilder);
            this.grpConnection.Controls.Add(this.drpProviderType);
            this.grpConnection.Controls.Add(this.lblProviderType);
            this.grpConnection.Controls.Add(this.txtConnStr);
            this.grpConnection.Location = new System.Drawing.Point(0, 0);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(312, 72);
            this.grpConnection.TabIndex = 1;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Connection String";
            // 
            // cmdStringBuilder
            // 
            this.cmdStringBuilder.Activated = false;
            this.cmdStringBuilder.AllowWordWrap = false;
            this.cmdStringBuilder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStringBuilder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(246)))));
            this.cmdStringBuilder.BackgroundRotationDegrees = 0F;
            this.cmdStringBuilder.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(116)))));
            this.cmdStringBuilder.BorderWidth = 1;
            graphicsPath1.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
            this.cmdStringBuilder.ButtonShape = graphicsPath1;
            this.cmdStringBuilder.CornerFeather = 3;
            this.cmdStringBuilder.DisabledBackColor = System.Drawing.Color.WhiteSmoke;
            this.cmdStringBuilder.DisabledForeColor = System.Drawing.Color.Gray;
            this.cmdStringBuilder.FlatStyle = RainstormStudios.Controls.AdvancedButton.AdvButtonStyle.Standard;
            this.cmdStringBuilder.HighlightMultiplier = 2F;
            this.cmdStringBuilder.HoverHighlightColor = System.Drawing.Color.Orange;
            this.cmdStringBuilder.HoverHighlightOpacity = 200;
            this.cmdStringBuilder.HoverImage = null;
            this.cmdStringBuilder.ImagePadding = 0;
            this.cmdStringBuilder.Location = new System.Drawing.Point(208, 40);
            this.cmdStringBuilder.MinimumSize = new System.Drawing.Size(8, 8);
            this.cmdStringBuilder.MouseDownImage = null;
            this.cmdStringBuilder.MultiSample = true;
            this.cmdStringBuilder.Name = "cmdStringBuilder";
            this.cmdStringBuilder.Size = new System.Drawing.Size(88, 23);
            this.cmdStringBuilder.TabIndex = 4;
            this.cmdStringBuilder.Text = "String Builder";
            this.cmdStringBuilder.TextOutline = false;
            this.cmdStringBuilder.TextOutlineColor = System.Drawing.Color.Empty;
            this.cmdStringBuilder.TextOutlineOpacity = 255;
            this.cmdStringBuilder.TextOutlineWeight = 2F;
            this.cmdStringBuilder.TextRotationDegrees = 0F;
            this.cmdStringBuilder.TextShadow = false;
            this.cmdStringBuilder.TextShadowOffset = 1F;
            this.cmdStringBuilder.TextShadowOpacity = 80;
            this.cmdStringBuilder.TextVeritcal = false;
            this.cmdStringBuilder.ThreeDEffectDepth = 50;
            this.cmdStringBuilder.ToggleType = false;
            this.cmdStringBuilder.UseVisualStyleBackColor = true;
            this.cmdStringBuilder.Click += new System.EventHandler(this.cmdButton_onClick);
            // 
            // drpProviderType
            // 
            this.drpProviderType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.drpProviderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpProviderType.FormattingEnabled = true;
            this.drpProviderType.Items.AddRange(new object[] {
            "SQL Provider",
            "OLE DB Provider",
            "IBM DB2 Provider",
            "ODBC Provider"});
            this.drpProviderType.Location = new System.Drawing.Point(88, 40);
            this.drpProviderType.Name = "drpProviderType";
            this.drpProviderType.Size = new System.Drawing.Size(112, 21);
            this.drpProviderType.TabIndex = 3;
            this.drpProviderType.SelectedIndexChanged += new System.EventHandler(this.drpProviderType_onSelectedIndexChanged);
            // 
            // lblProviderType
            // 
            this.lblProviderType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProviderType.AutoSize = true;
            this.lblProviderType.Location = new System.Drawing.Point(16, 44);
            this.lblProviderType.Name = "lblProviderType";
            this.lblProviderType.Size = new System.Drawing.Size(76, 13);
            this.lblProviderType.TabIndex = 2;
            this.lblProviderType.Text = "Provider Type:";
            // 
            // txtConnStr
            // 
            this.txtConnStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnStr.Location = new System.Drawing.Point(8, 16);
            this.txtConnStr.Name = "txtConnStr";
            this.txtConnStr.Size = new System.Drawing.Size(296, 20);
            this.txtConnStr.TabIndex = 0;
            this.txtConnStr.TextChanged += new System.EventHandler(this.txtConnectionString_onTextChanged);
            // 
            // ConnectionStringBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpConnection);
            this.MinimumSize = new System.Drawing.Size(312, 72);
            this.Name = "ConnectionStringBox";
            this.Size = new System.Drawing.Size(312, 72);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.ComboBox drpProviderType;
        private System.Windows.Forms.Label lblProviderType;
        private System.Windows.Forms.TextBox txtConnStr;
        private AdvancedButton cmdStringBuilder;
    }
}
