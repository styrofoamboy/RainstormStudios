using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace RainstormStudios.Controls
{
    partial class IPEntryBox
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
            this.txtIP1 = new System.Windows.Forms.TextBox();
            this.txtIP2 = new System.Windows.Forms.TextBox();
            this.txtIP3 = new System.Windows.Forms.TextBox();
            this.txtIP4 = new System.Windows.Forms.TextBox();
            this.lblDot1 = new System.Windows.Forms.Label();
            this.lblDot2 = new System.Windows.Forms.Label();
            this.lblDot3 = new System.Windows.Forms.Label();
            this.txtBackground = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtIP1
            // 
            this.txtIP1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIP1.Location = new System.Drawing.Point(2, 4);
            this.txtIP1.MaxLength = 3;
            this.txtIP1.Name = "txtIP1";
            this.txtIP1.Size = new System.Drawing.Size(24, 13);
            this.txtIP1.TabIndex = 0;
            this.txtIP1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtIP1.Enter += new System.EventHandler(this.txtIP_onGotFocus);
            this.txtIP1.Leave += new System.EventHandler(this.txtIP_onLostFocus);
            this.txtIP1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_onKeyDown);
            // 
            // txtIP2
            // 
            this.txtIP2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIP2.Location = new System.Drawing.Point(32, 4);
            this.txtIP2.MaxLength = 3;
            this.txtIP2.Name = "txtIP2";
            this.txtIP2.Size = new System.Drawing.Size(24, 13);
            this.txtIP2.TabIndex = 1;
            this.txtIP2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtIP2.Enter += new System.EventHandler(this.txtIP_onGotFocus);
            this.txtIP2.Leave += new System.EventHandler(this.txtIP_onLostFocus);
            this.txtIP2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_onKeyDown);
            // 
            // txtIP3
            // 
            this.txtIP3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIP3.Location = new System.Drawing.Point(62, 4);
            this.txtIP3.MaxLength = 3;
            this.txtIP3.Name = "txtIP3";
            this.txtIP3.Size = new System.Drawing.Size(24, 13);
            this.txtIP3.TabIndex = 2;
            this.txtIP3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtIP3.Enter += new System.EventHandler(this.txtIP_onGotFocus);
            this.txtIP3.Leave += new System.EventHandler(this.txtIP_onLostFocus);
            this.txtIP3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_onKeyDown);
            // 
            // txtIP4
            // 
            this.txtIP4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIP4.Location = new System.Drawing.Point(92, 4);
            this.txtIP4.MaxLength = 3;
            this.txtIP4.Name = "txtIP4";
            this.txtIP4.Size = new System.Drawing.Size(24, 13);
            this.txtIP4.TabIndex = 3;
            this.txtIP4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtIP4.Enter += new System.EventHandler(this.txtIP_onGotFocus);
            this.txtIP4.Leave += new System.EventHandler(this.txtIP_onLostFocus);
            this.txtIP4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_onKeyDown);
            // 
            // lblDot1
            // 
            this.lblDot1.AutoSize = true;
            this.lblDot1.BackColor = System.Drawing.SystemColors.Window;
            this.lblDot1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDot1.Location = new System.Drawing.Point(26, 5);
            this.lblDot1.MaximumSize = new System.Drawing.Size(0, 13);
            this.lblDot1.MinimumSize = new System.Drawing.Size(0, 13);
            this.lblDot1.Name = "lblDot1";
            this.lblDot1.Size = new System.Drawing.Size(7, 13);
            this.lblDot1.TabIndex = 5;
            this.lblDot1.Text = ".";
            this.lblDot1.EnabledChanged += new System.EventHandler(this.lblDot_onEnabledChanged);
            // 
            // lblDot2
            // 
            this.lblDot2.AutoSize = true;
            this.lblDot2.BackColor = System.Drawing.SystemColors.Window;
            this.lblDot2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDot2.Location = new System.Drawing.Point(56, 5);
            this.lblDot2.Name = "lblDot2";
            this.lblDot2.Size = new System.Drawing.Size(7, 13);
            this.lblDot2.TabIndex = 6;
            this.lblDot2.Text = ".";
            this.lblDot2.EnabledChanged += new System.EventHandler(this.lblDot_onEnabledChanged);
            // 
            // lblDot3
            // 
            this.lblDot3.AutoSize = true;
            this.lblDot3.BackColor = System.Drawing.SystemColors.Window;
            this.lblDot3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDot3.Location = new System.Drawing.Point(86, 5);
            this.lblDot3.Name = "lblDot3";
            this.lblDot3.Size = new System.Drawing.Size(7, 13);
            this.lblDot3.TabIndex = 7;
            this.lblDot3.Text = ".";
            this.lblDot3.EnabledChanged += new System.EventHandler(this.lblDot_onEnabledChanged);
            // 
            // txtBackground
            // 
            this.txtBackground.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtBackground.Location = new System.Drawing.Point(0, 0);
            this.txtBackground.MaxLength = 0;
            this.txtBackground.Name = "txtBackground";
            this.txtBackground.Size = new System.Drawing.Size(118, 20);
            this.txtBackground.TabIndex = 8;
            this.txtBackground.TabStop = false;
            this.txtBackground.Enter += new System.EventHandler(this.txtBackground_onGotFocus);
            // 
            // IPEntryBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDot3);
            this.Controls.Add(this.lblDot2);
            this.Controls.Add(this.lblDot1);
            this.Controls.Add(this.txtIP4);
            this.Controls.Add(this.txtIP3);
            this.Controls.Add(this.txtIP2);
            this.Controls.Add(this.txtIP1);
            this.Controls.Add(this.txtBackground);
            this.MaximumSize = new System.Drawing.Size(118, 20);
            this.MinimumSize = new System.Drawing.Size(118, 20);
            this.Name = "IPEntryBox";
            this.Size = new System.Drawing.Size(118, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIP1;
        private System.Windows.Forms.TextBox txtIP2;
        private System.Windows.Forms.TextBox txtIP3;
        private System.Windows.Forms.TextBox txtIP4;
        private System.Windows.Forms.Label lblDot1;
        private System.Windows.Forms.Label lblDot2;
        private System.Windows.Forms.Label lblDot3;
        private TextBox txtBackground;
    }
}
