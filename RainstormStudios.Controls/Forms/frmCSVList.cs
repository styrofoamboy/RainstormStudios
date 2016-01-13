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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RainstormStudios.Collections;

namespace RainstormStudios.Forms
{
    [Author("Unfried, Michael")]
    public partial class frmCSVList : Form
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string RawInput
        { get { return this.txtItems.Text; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public frmCSVList()
        {
            InitializeComponent();
        }
        public frmCSVList(string title)
            : this()
        {
            this.Text = title;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string GetList()
        {
            int longLen = 0;
            StringCollection strCol = new StringCollection();
            string[] lns = this.txtItems.Text.Split(new char[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lns.Length; i++)
            {
                if (chkDistinct.Checked)
                    strCol.AddUnique(lns[i]);
                else
                    strCol.Add(lns[i]);
                longLen = (int)System.Math.Max(longLen, lns[i].Length);
            }

            string retVal = "";
            lns = strCol.ToArray();
            for (int i = 0; i < lns.Length; i++)
            {
                string newVal = string.Format("{2}{3}{1}{0}{1}{4}",
                                        lns[i],
                                        (this.chkQuotes.Checked)
                                                ? "'"
                                                : "",
                                        (i > 0) ? "," : "",
                                        (this.chkCheckSpace.Checked)
                                                ? ((i > 0) ? " " : "")
                                                : "",
                                        (this.numLnBreak.Value > 0 && ((i + 1) % (int)numLnBreak.Value == 0))
                                                ? "\n"
                                                : "");
                if (this.chkAlign.Checked)
                    newVal = newVal.PadLeft(longLen + ((this.chkCheckSpace.Checked) ? 1 : 0) + ((this.chkQuotes.Checked) ? 2 : 0) + ((this.numLnBreak.Value > 0 && ((i + 1) % (int)numLnBreak.Value == 0)) ? 1 : 0) + 1, ' ');
                retVal += newVal;
            }

            return retVal.TrimEnd(',', '\n');
        }
        #endregion
    }
}