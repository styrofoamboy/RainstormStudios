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
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.ComboBox))]
    public class SqlDataTypeList : ComboBox
    {
        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        [System.ComponentModel.Browsable(false)]
        public System.Data.SqlDbType SelectedType
        {
            get
            {
                if (!this.DesignMode)
                {
                    if (this.Items.Count > 0 && this.SelectedIndex > -1)
                        return (System.Data.SqlDbType)Enum.Parse(typeof(System.Data.SqlDbType), this.SelectedItem.ToString());
                    else
                        throw new ApplicationException("Control is not ready to return value.");
                }
                else
                {
                    return System.Data.SqlDbType.Variant;
                }
            }
            set
            {
                if (!this.DesignMode)
                    this.SelectedIndex = (int)value;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SqlDataTypeList()
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.CreateControl();
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (this.Items.Count == 0)
                this.Items.AddRange(Enum.GetNames(typeof(System.Data.SqlDbType)));
        }
        #endregion
    }
}
