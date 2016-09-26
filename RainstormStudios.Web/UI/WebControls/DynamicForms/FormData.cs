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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    [Serializable]
    public class FormData : INotifyPropertyChanged
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        object
            _provId;
        string
            _name;
        FormElementDataCollection
            _elements;
        bool
            _isDirty;
        //***************************************************************************
        // Public Events
        public event PropertyChangedEventHandler
            PropertyChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object FormProviderKey
        {
            get { return this._provId; }
            set { this._provId = value; }
        }
        public string FormName
        {
            get { return this._name; }
            set { this._name = value; }
        }
        public FormElementDataCollection Elements
        { get { return this._elements; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormData()
        {
            this._elements = new FormElementDataCollection(null);
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this._isDirty = true;
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, e);
        }
        #endregion
    }
}
