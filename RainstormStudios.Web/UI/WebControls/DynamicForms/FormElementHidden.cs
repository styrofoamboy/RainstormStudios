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
    public class FormElementHidden : FormElementControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private HiddenField
            _hdnFld;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal FormElementHidden(FormElementData data)
            : base(data)
        { }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void CreateChildControls()
        {
            string idVal = this._qData.ElementProviderKey.ToString();

            Panel pnl = FormElementControl.CreateControlContainer(this._qData, this.EditMode);
            this.Controls.Add(pnl);
            this.ControlContainer = pnl;

            this._hdnFld = new HiddenField();
            pnl.Controls.Add(this._hdnFld);
            this._hdnFld.ID = "hdnElementValue_" + idVal;
            this._hdnFld.Value = (this._qData.Answers.Count > 0 ? this._qData.Answers[0].AnswerProviderKey.ToString() : string.Empty);
            this.AnswerControl = this._hdnFld;

            base.CreateChildControls();
        }
        #endregion
    }
}
