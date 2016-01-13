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
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    public class RsUserControlBase : UserControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected FlatStyle _flatStyle = FlatStyle.Standard;
        protected Color _bgColor = SystemColors.Window;
        protected Color _fgColor = SystemColors.WindowText;
        private static string _nClr = "GroupBox,Panel,UserControl,Label";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Appearance"), Description("Set the flat-style of all buttons within the control."), Browsable(true)]
        public virtual FlatStyle GlobalFlatStyle
        {
            get { return this._flatStyle; }
            set
            {
                _flatStyle = value;
                RsUserControlBase.SetAllFlatStyles(this.Controls, value);
            }
        }
        public virtual Color GlobalBackgroundColor
        {
            get { return this._bgColor; }
            set
            {
                this._bgColor = value;
                RsUserControlBase.SetAllBackgroundColor(this.Controls, value);
            }
        }
        public virtual Color GlobalForegroundColor
        {
            get { return this._fgColor; }
            set
            {
                this._fgColor = value;
                RsUserControlBase.SetAllForegroundColor(this.Controls, value);
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void DisposeAllControls(ControlCollection controls)
        { this.DisposeAllControls(controls, false); }
        protected void DisposeAllControls(ControlCollection controls, bool throwErr)
        {
            foreach (Control cn in controls)
            {
                this.DisposeAllControls(cn.Controls);
                try
                { cn.Dispose(); }
                catch
                { if (throwErr) throw; }
            }
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static void SetAllFlatStyles(Control.ControlCollection controls, FlatStyle value)
        {
            foreach (Control cn in controls)
            {
                Type cnType = cn.GetType();
                //System.Reflection.PropertyInfo pi = cnType.GetProperty("FlatStyle");
                System.Reflection.MemberInfo[] pi = cnType.FindMembers(System.Reflection.MemberTypes.Property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, System.Type.FilterNameIgnoreCase, "FlatStyle");
                if (pi != null && pi.Length > 0)
                {
                    if (cnType.FullName != "RainstormStudios.Controls.AdvancedButton")
                        ((System.Reflection.PropertyInfo)pi[0]).SetValue(cn, value, null);
                    else if (cnType.FullName == "RainstormStudios.Controls.AdvancedButton")
                    {
                        try
                        {
                            AdvancedButton.AdvButtonStyle btnStyle = (AdvancedButton.AdvButtonStyle)Enum.Parse(typeof(AdvancedButton.AdvButtonStyle), value.ToString());
                            ((System.Reflection.PropertyInfo)pi[0]).SetValue(cn, btnStyle, null);
                        }
                        finally { }
                    }
                }
                if (cn.Controls.Count > 0)
                    RsUserControlBase.SetAllFlatStyles(cn.Controls, value);
            }
        }
        public static void SetAllBackgroundColor(Control.ControlCollection controls, Color value)
        {
            foreach (Control cn in controls)
            {
                string cnTypeName = cn.GetType().Name;
                if (!_nClr.Contains(cnTypeName))
                    cn.BackColor = value;
                if (cn.Controls.Count > 0)
                    RsUserControlBase.SetAllBackgroundColor(cn.Controls, value);
            }
        }
        public static void SetAllForegroundColor(Control.ControlCollection controls, Color value)
        {
            foreach (Control cn in controls)
            {
                string cnTypeName = cn.GetType().Name;
                if (!_nClr.Contains(cnTypeName))
                    cn.ForeColor = value;
                if (cn.Controls.Count > 0)
                    RsUserControlBase.SetAllForegroundColor(cn.Controls, value);
            }
        }
        public static bool IsPrintableChar(Keys e)
        {
            return (char.IsLetterOrDigit(e.ToString(), 0) || char.IsPunctuation(e.ToString(), 0) || char.IsWhiteSpace(e.ToString(), 0));
        }
        #endregion
    }
}
