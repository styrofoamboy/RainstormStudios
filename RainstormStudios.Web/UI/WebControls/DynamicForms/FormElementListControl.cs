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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    [Author("Unfried, Michael")]
    public abstract class FormElementListControl : FormElementControl
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected FormElementListControl(FormElementData data)
            : base(data)
        { }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            System.Web.UI.WebControls.ListControl ctrl = null;
            if (this.AnswerControl != null && (ctrl = (this.AnswerControl as System.Web.UI.WebControls.ListControl)) != null)
                ctrl.SelectedIndexChanged += new EventHandler(this.SelectedIndexChanged);

            base.CreateChildControls();
        }
        #endregion
    }
}
