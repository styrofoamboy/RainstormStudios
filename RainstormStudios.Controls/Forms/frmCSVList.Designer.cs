namespace RainstormStudios.Forms
{
    partial class frmCSVList
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
            this.txtItems = new System.Windows.Forms.TextBox();
            this.lblInstr = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.chkDistinct = new System.Windows.Forms.CheckBox();
            this.lblBreakRows = new System.Windows.Forms.Label();
            this.numLnBreak = new System.Windows.Forms.NumericUpDown();
            this.lblLnBreak2 = new System.Windows.Forms.Label();
            this.chkQuotes = new System.Windows.Forms.CheckBox();
            this.chkCheckSpace = new System.Windows.Forms.CheckBox();
            this.chkAlign = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numLnBreak)).BeginInit();
            this.SuspendLayout();
            // 
            // txtItems
            // 
            this.txtItems.AcceptsReturn = true;
            this.txtItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItems.Location = new System.Drawing.Point(12, 25);
            this.txtItems.Multiline = true;
            this.txtItems.Name = "txtItems";
            this.txtItems.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtItems.Size = new System.Drawing.Size(464, 240);
            this.txtItems.TabIndex = 0;
            this.txtItems.WordWrap = false;
            // 
            // lblInstr
            // 
            this.lblInstr.AutoSize = true;
            this.lblInstr.Location = new System.Drawing.Point(12, 9);
            this.lblInstr.Name = "lblInstr";
            this.lblInstr.Size = new System.Drawing.Size(330, 13);
            this.lblInstr.TabIndex = 1;
            this.lblInstr.Text = "Insert list items seperated by a comma, semi-colon, or carriage return:";
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(401, 303);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(320, 303);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // chkDistinct
            // 
            this.chkDistinct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDistinct.AutoSize = true;
            this.chkDistinct.Checked = true;
            this.chkDistinct.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDistinct.Location = new System.Drawing.Point(15, 274);
            this.chkDistinct.Name = "chkDistinct";
            this.chkDistinct.Size = new System.Drawing.Size(120, 17);
            this.chkDistinct.TabIndex = 4;
            this.chkDistinct.Text = "Distinct Entries Only";
            this.chkDistinct.UseVisualStyleBackColor = true;
            // 
            // lblBreakRows
            // 
            this.lblBreakRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBreakRows.AutoSize = true;
            this.lblBreakRows.Location = new System.Drawing.Point(321, 273);
            this.lblBreakRows.Name = "lblBreakRows";
            this.lblBreakRows.Size = new System.Drawing.Size(65, 13);
            this.lblBreakRows.TabIndex = 5;
            this.lblBreakRows.Text = "Break Every";
            // 
            // numLnBreak
            // 
            this.numLnBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numLnBreak.Location = new System.Drawing.Point(392, 271);
            this.numLnBreak.Name = "numLnBreak";
            this.numLnBreak.Size = new System.Drawing.Size(36, 20);
            this.numLnBreak.TabIndex = 6;
            // 
            // lblLnBreak2
            // 
            this.lblLnBreak2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLnBreak2.AutoSize = true;
            this.lblLnBreak2.Location = new System.Drawing.Point(435, 273);
            this.lblLnBreak2.Name = "lblLnBreak2";
            this.lblLnBreak2.Size = new System.Drawing.Size(41, 13);
            this.lblLnBreak2.TabIndex = 7;
            this.lblLnBreak2.Text = "entries.";
            // 
            // chkQuotes
            // 
            this.chkQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkQuotes.AutoSize = true;
            this.chkQuotes.Location = new System.Drawing.Point(179, 274);
            this.chkQuotes.Name = "chkQuotes";
            this.chkQuotes.Size = new System.Drawing.Size(82, 17);
            this.chkQuotes.TabIndex = 8;
            this.chkQuotes.Text = "Add Quotes";
            this.chkQuotes.UseVisualStyleBackColor = true;
            // 
            // chkCheckSpace
            // 
            this.chkCheckSpace.AutoSize = true;
            this.chkCheckSpace.Checked = true;
            this.chkCheckSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCheckSpace.Location = new System.Drawing.Point(15, 297);
            this.chkCheckSpace.Name = "chkCheckSpace";
            this.chkCheckSpace.Size = new System.Drawing.Size(116, 17);
            this.chkCheckSpace.TabIndex = 9;
            this.chkCheckSpace.Text = "Space After Check";
            this.chkCheckSpace.UseVisualStyleBackColor = true;
            // 
            // chkAlign
            // 
            this.chkAlign.AutoSize = true;
            this.chkAlign.Location = new System.Drawing.Point(179, 297);
            this.chkAlign.Name = "chkAlign";
            this.chkAlign.Size = new System.Drawing.Size(84, 17);
            this.chkAlign.TabIndex = 10;
            this.chkAlign.Text = "Align Entries";
            this.chkAlign.UseVisualStyleBackColor = true;
            // 
            // frmCSVList
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(488, 338);
            this.Controls.Add(this.chkAlign);
            this.Controls.Add(this.chkCheckSpace);
            this.Controls.Add(this.chkQuotes);
            this.Controls.Add(this.lblLnBreak2);
            this.Controls.Add(this.numLnBreak);
            this.Controls.Add(this.lblBreakRows);
            this.Controls.Add(this.chkDistinct);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.lblInstr);
            this.Controls.Add(this.txtItems);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(496, 364);
            this.Name = "frmCSVList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Comma Seperated List Generator";
            ((System.ComponentModel.ISupportInitialize)(this.numLnBreak)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtItems;
        private System.Windows.Forms.Label lblInstr;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.CheckBox chkDistinct;
        private System.Windows.Forms.Label lblBreakRows;
        private System.Windows.Forms.NumericUpDown numLnBreak;
        private System.Windows.Forms.Label lblLnBreak2;
        private System.Windows.Forms.CheckBox chkQuotes;
        private System.Windows.Forms.CheckBox chkCheckSpace;
        private System.Windows.Forms.CheckBox chkAlign;
    }
}