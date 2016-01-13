//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    public partial class IPEntryBox : UserControl
    {
        //***************************************************************************
        // Public Fields
        // 
        public System.Net.IPAddress IPAddress
        {
            get
            {
                // The try/catch blocks prevent errors in the designer if no valid IP has been entered.
                try
                { return System.Net.IPAddress.Parse(this.IPText); }
                catch
                { return System.Net.IPAddress.Parse("0.0.0.0"); }
            }
        }
        public string IPText
        {
            get { return txtIP1.Text + "." + txtIP2.Text + "." + txtIP3.Text + "." + txtIP4.Text; }
        }
        public string IP1
        {
            get { return txtIP1.Text; }
        }
        public string IP2
        {
            get { return txtIP2.Text; }
        }
        public string IP3
        {
            get { return txtIP3.Text; }
        }
        public string IP4
        {
            get { return txtIP4.Text; }
        }
        public bool ValidIP
        {
            get
            {
                try
                {
                    System.Net.IPAddress dummy = System.Net.IPAddress.Parse(this.IPText);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        //***************************************************************************
        // Class Constructors
        // 
        public IPEntryBox()
        {
            InitializeComponent();
        }
        //***************************************************************************
        // Text Box Event Handler
        // 
        private void txt_onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != System.Windows.Forms.Keys.Delete && e.KeyCode != System.Windows.Forms.Keys.Back && e.KeyCode != System.Windows.Forms.Keys.Left && e.KeyCode != System.Windows.Forms.Keys.Right)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Decimal || e.KeyCode == System.Windows.Forms.Keys.OemPeriod)
                {
                    e.SuppressKeyPress = true;
                    switch (((TextBox)sender).Name)
                    {
                        case "txtIP1":
                            txtIP2.Focus();
                            break;
                        case "txtIP2":
                            txtIP3.Focus();
                            break;
                        case "txtIP3":
                            txtIP4.Focus();
                            break;
                        case "txtIP4":
                            break;
                    }
                }
                else if ((e.KeyValue < 48 || e.KeyValue > 57) && (e.KeyValue < 96 || e.KeyValue > 105))
                {
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void txtIP_onGotFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).TextLength > 0)
                ((TextBox)sender).SelectAll();
        }
        private void txtIP_onLostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).TextLength > 0)
            {
                if (Convert.ToInt16(((TextBox)sender).Text) > 255)
                {
                    ((TextBox)sender).Text = "255";
                    ((TextBox)sender).Focus();
                    ((TextBox)sender).SelectAll();
                }
            }
        }
        private void txtBackground_onGotFocus(object sender, EventArgs e)
        {
            txtIP1.Focus();
        }
        private void lblDot_onEnabledChanged(object sender, EventArgs e)
        {
            Label id=(Label)sender;
            if (id.Enabled)
                id.BackColor = SystemColors.Window;
            else
                id.BackColor = SystemColors.InactiveBorder;
        }
        //***************************************************************************
        // Public Methods
        // 
        public new void Focus()
        {
            txtIP1.Focus();
        }
    }
}
