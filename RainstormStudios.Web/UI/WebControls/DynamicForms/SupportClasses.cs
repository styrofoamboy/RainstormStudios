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
using System.Linq;
using System.Text;

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    public delegate void DynamicFormRenderingEventHandler(object sender, DynamicFormRenderingEventArgs e);
    public delegate void DynamicFormEventHandler(object sender, DynamicFormEventArgs e);
    public delegate void DynamicFormElementRenderingEventHandler(object sender, DynamicFormElementRenderingEventArgs e);
    public delegate void DynamicFormElementEventHandler(object sender, DynamicFormElementEventArgs e);
    public class DynamicFormEventArgs : System.EventArgs
    {
    }
    public class DynamicFormRenderingEventArgs : DynamicFormEventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public bool
            Cancel = false;
        #endregion
    }
    public class DynamicFormElementEventArgs : System.EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public readonly FormElementControl
            FormElement;
        public readonly FormElementData
            FormElementData;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicFormElementEventArgs(FormElementControl element, FormElementData data)
        {
            this.FormElement = element;
            this.FormElementData = data;
        }
        #endregion
    }
    public class DynamicFormElementRenderingEventArgs : DynamicFormElementEventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public bool
            Cancel = false;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicFormElementRenderingEventArgs(FormElementControl element, FormElementData data)
            : base(element, data)
        { }
        #endregion
    }
}
