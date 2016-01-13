namespace RainstormStudios.Forms
{
    partial class frmFindText
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
            System.Drawing.Drawing2D.GraphicsPath graphicsPath1 = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Drawing2D.GraphicsPath graphicsPath2 = new System.Drawing.Drawing2D.GraphicsPath();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSrch = new System.Windows.Forms.TextBox();
            this.grpOpts = new System.Windows.Forms.GroupBox();
            this.chkWholeWord = new System.Windows.Forms.CheckBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkRegEx = new System.Windows.Forms.CheckBox();
            this.grpScope = new System.Windows.Forms.GroupBox();
            this.rdoFromEnd = new System.Windows.Forms.RadioButton();
            this.rdoFromBegin = new System.Windows.Forms.RadioButton();
            this.rdoFromCurPos = new System.Windows.Forms.RadioButton();
            this.grpDir = new System.Windows.Forms.GroupBox();
            this.rdoDirDown = new System.Windows.Forms.RadioButton();
            this.rdoDirUp = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new RainstormStudios.Controls.AdvancedButton();
            this.cmdFindNext = new RainstormStudios.Controls.AdvancedButton();
            this.grpOpts.SuspendLayout();
            this.grpScope.SuspendLayout();
            this.grpDir.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find:";
            // 
            // txtSrch
            // 
            this.txtSrch.Location = new System.Drawing.Point(48, 6);
            this.txtSrch.Name = "txtSrch";
            this.txtSrch.Size = new System.Drawing.Size(298, 20);
            this.txtSrch.TabIndex = 1;
            this.txtSrch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSrch_KeyPress);
            // 
            // grpOpts
            // 
            this.grpOpts.Controls.Add(this.chkWholeWord);
            this.grpOpts.Controls.Add(this.chkMatchCase);
            this.grpOpts.Controls.Add(this.chkRegEx);
            this.grpOpts.Location = new System.Drawing.Point(15, 33);
            this.grpOpts.Name = "grpOpts";
            this.grpOpts.Size = new System.Drawing.Size(104, 92);
            this.grpOpts.TabIndex = 4;
            this.grpOpts.TabStop = false;
            this.grpOpts.Text = "Options";
            // 
            // chkWholeWord
            // 
            this.chkWholeWord.AutoSize = true;
            this.chkWholeWord.Location = new System.Drawing.Point(6, 42);
            this.chkWholeWord.Name = "chkWholeWord";
            this.chkWholeWord.Size = new System.Drawing.Size(86, 17);
            this.chkWholeWord.TabIndex = 7;
            this.chkWholeWord.Text = "Whole Word";
            this.chkWholeWord.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(6, 19);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(83, 17);
            this.chkMatchCase.TabIndex = 6;
            this.chkMatchCase.Text = "Match Case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkRegEx
            // 
            this.chkRegEx.AutoSize = true;
            this.chkRegEx.Location = new System.Drawing.Point(6, 65);
            this.chkRegEx.Name = "chkRegEx";
            this.chkRegEx.Size = new System.Drawing.Size(80, 17);
            this.chkRegEx.TabIndex = 5;
            this.chkRegEx.Text = "Use RegEx";
            this.chkRegEx.UseVisualStyleBackColor = true;
            // 
            // grpScope
            // 
            this.grpScope.Controls.Add(this.rdoFromEnd);
            this.grpScope.Controls.Add(this.rdoFromBegin);
            this.grpScope.Controls.Add(this.rdoFromCurPos);
            this.grpScope.Location = new System.Drawing.Point(125, 33);
            this.grpScope.Name = "grpScope";
            this.grpScope.Size = new System.Drawing.Size(115, 92);
            this.grpScope.TabIndex = 5;
            this.grpScope.TabStop = false;
            this.grpScope.Text = "Scope";
            // 
            // rdoFromEnd
            // 
            this.rdoFromEnd.AutoSize = true;
            this.rdoFromEnd.Location = new System.Drawing.Point(6, 65);
            this.rdoFromEnd.Name = "rdoFromEnd";
            this.rdoFromEnd.Size = new System.Drawing.Size(75, 17);
            this.rdoFromEnd.TabIndex = 2;
            this.rdoFromEnd.Text = "End of File";
            this.rdoFromEnd.UseVisualStyleBackColor = true;
            // 
            // rdoFromBegin
            // 
            this.rdoFromBegin.AutoSize = true;
            this.rdoFromBegin.Location = new System.Drawing.Point(6, 42);
            this.rdoFromBegin.Name = "rdoFromBegin";
            this.rdoFromBegin.Size = new System.Drawing.Size(97, 17);
            this.rdoFromBegin.TabIndex = 1;
            this.rdoFromBegin.Text = "Begining of Flie";
            this.rdoFromBegin.UseVisualStyleBackColor = true;
            // 
            // rdoFromCurPos
            // 
            this.rdoFromCurPos.AutoSize = true;
            this.rdoFromCurPos.Checked = true;
            this.rdoFromCurPos.Location = new System.Drawing.Point(6, 18);
            this.rdoFromCurPos.Name = "rdoFromCurPos";
            this.rdoFromCurPos.Size = new System.Drawing.Size(95, 17);
            this.rdoFromCurPos.TabIndex = 0;
            this.rdoFromCurPos.TabStop = true;
            this.rdoFromCurPos.Text = "Cursor Position";
            this.rdoFromCurPos.UseVisualStyleBackColor = true;
            // 
            // grpDir
            // 
            this.grpDir.Controls.Add(this.rdoDirDown);
            this.grpDir.Controls.Add(this.rdoDirUp);
            this.grpDir.Location = new System.Drawing.Point(246, 33);
            this.grpDir.Name = "grpDir";
            this.grpDir.Size = new System.Drawing.Size(100, 71);
            this.grpDir.TabIndex = 6;
            this.grpDir.TabStop = false;
            this.grpDir.Text = "Direction";
            // 
            // rdoDirDown
            // 
            this.rdoDirDown.AutoSize = true;
            this.rdoDirDown.Checked = true;
            this.rdoDirDown.Location = new System.Drawing.Point(7, 43);
            this.rdoDirDown.Name = "rdoDirDown";
            this.rdoDirDown.Size = new System.Drawing.Size(53, 17);
            this.rdoDirDown.TabIndex = 1;
            this.rdoDirDown.TabStop = true;
            this.rdoDirDown.Text = "Down";
            this.rdoDirDown.UseVisualStyleBackColor = true;
            // 
            // rdoDirUp
            // 
            this.rdoDirUp.AutoSize = true;
            this.rdoDirUp.Location = new System.Drawing.Point(7, 20);
            this.rdoDirUp.Name = "rdoDirUp";
            this.rdoDirUp.Size = new System.Drawing.Size(39, 17);
            this.rdoDirUp.TabIndex = 0;
            this.rdoDirUp.Text = "Up";
            this.rdoDirUp.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.AllowWordWrap = false;
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(246)))));
            this.cmdCancel.BackgroundRotationDegrees = 0F;
            this.cmdCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(116)))));
            this.cmdCancel.BorderWidth = 1;
            graphicsPath1.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
            this.cmdCancel.ButtonShape = graphicsPath1;
            this.cmdCancel.CornerFeather = 3;
            this.cmdCancel.DisabledBackColor = System.Drawing.Color.WhiteSmoke;
            this.cmdCancel.DisabledForeColor = System.Drawing.Color.Gray;
            this.cmdCancel.FlatStyle = RainstormStudios.Controls.AdvancedButton.AdvButtonStyle.Standard;
            this.cmdCancel.HighlightMultiplier = 2F;
            this.cmdCancel.HoverHighlightColor = System.Drawing.Color.Orange;
            this.cmdCancel.HoverHighlightOpacity = 200;
            this.cmdCancel.HoverImage = null;
            this.cmdCancel.ImagePadding = 0;
            this.cmdCancel.Location = new System.Drawing.Point(352, 33);
            this.cmdCancel.MinimumSize = new System.Drawing.Size(8, 8);
            this.cmdCancel.MouseDownImage = null;
            this.cmdCancel.MultiSample = true;
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.TextOutline = false;
            this.cmdCancel.TextOutlineColor = System.Drawing.Color.Empty;
            this.cmdCancel.TextOutlineOpacity = 255;
            this.cmdCancel.TextOutlineWeight = 2F;
            this.cmdCancel.TextRotationDegrees = 0F;
            this.cmdCancel.TextShadow = false;
            this.cmdCancel.TextShadowOffset = 1F;
            this.cmdCancel.TextShadowOpacity = 80;
            this.cmdCancel.TextVeritcal = false;
            this.cmdCancel.ThreeDEffectDepth = 50;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdFindNext
            // 
            this.cmdFindNext.AllowWordWrap = false;
            this.cmdFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdFindNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(246)))));
            this.cmdFindNext.BackgroundRotationDegrees = 0F;
            this.cmdFindNext.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(116)))));
            this.cmdFindNext.BorderWidth = 1;
            graphicsPath2.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
            this.cmdFindNext.ButtonShape = graphicsPath2;
            this.cmdFindNext.CornerFeather = 3;
            this.cmdFindNext.DisabledBackColor = System.Drawing.Color.WhiteSmoke;
            this.cmdFindNext.DisabledForeColor = System.Drawing.Color.Gray;
            this.cmdFindNext.FlatStyle = RainstormStudios.Controls.AdvancedButton.AdvButtonStyle.Standard;
            this.cmdFindNext.HighlightMultiplier = 2F;
            this.cmdFindNext.HoverHighlightColor = System.Drawing.Color.Orange;
            this.cmdFindNext.HoverHighlightOpacity = 200;
            this.cmdFindNext.HoverImage = null;
            this.cmdFindNext.ImagePadding = 0;
            this.cmdFindNext.Location = new System.Drawing.Point(352, 4);
            this.cmdFindNext.MinimumSize = new System.Drawing.Size(8, 8);
            this.cmdFindNext.MouseDownImage = null;
            this.cmdFindNext.MultiSample = true;
            this.cmdFindNext.Name = "cmdFindNext";
            this.cmdFindNext.Size = new System.Drawing.Size(75, 23);
            this.cmdFindNext.TabIndex = 2;
            this.cmdFindNext.Text = "Find Next";
            this.cmdFindNext.TextOutline = false;
            this.cmdFindNext.TextOutlineColor = System.Drawing.Color.Empty;
            this.cmdFindNext.TextOutlineOpacity = 255;
            this.cmdFindNext.TextOutlineWeight = 2F;
            this.cmdFindNext.TextRotationDegrees = 0F;
            this.cmdFindNext.TextShadow = false;
            this.cmdFindNext.TextShadowOffset = 1F;
            this.cmdFindNext.TextShadowOpacity = 80;
            this.cmdFindNext.TextVeritcal = false;
            this.cmdFindNext.ThreeDEffectDepth = 50;
            this.cmdFindNext.UseVisualStyleBackColor = true;
            this.cmdFindNext.Click += new System.EventHandler(this.cmdFindNext_Click);
            // 
            // frmFindText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 135);
            this.Controls.Add(this.grpDir);
            this.Controls.Add(this.grpScope);
            this.Controls.Add(this.grpOpts);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdFindNext);
            this.Controls.Add(this.txtSrch);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmFindText";
            this.Text = "Find Text";
            this.grpOpts.ResumeLayout(false);
            this.grpOpts.PerformLayout();
            this.grpScope.ResumeLayout(false);
            this.grpScope.PerformLayout();
            this.grpDir.ResumeLayout(false);
            this.grpDir.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSrch;
        private RainstormStudios.Controls.AdvancedButton cmdFindNext;
        private RainstormStudios.Controls.AdvancedButton cmdCancel;
        private System.Windows.Forms.GroupBox grpOpts;
        private System.Windows.Forms.CheckBox chkRegEx;
        private System.Windows.Forms.CheckBox chkWholeWord;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.GroupBox grpScope;
        private System.Windows.Forms.RadioButton rdoFromEnd;
        private System.Windows.Forms.RadioButton rdoFromBegin;
        private System.Windows.Forms.RadioButton rdoFromCurPos;
        private System.Windows.Forms.GroupBox grpDir;
        private System.Windows.Forms.RadioButton rdoDirUp;
        private System.Windows.Forms.RadioButton rdoDirDown;
    }
}