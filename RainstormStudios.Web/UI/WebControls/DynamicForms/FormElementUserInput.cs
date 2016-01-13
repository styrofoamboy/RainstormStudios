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
    [Author("Unfried, Michael")]
    public class FormElementUserInput
    {
        #region Nested Types
        //***************************************************************************
        // Nested Types
        // 
        public sealed class FormElementUserInputAnswerCollection : Collections.ObjectCollectionBase<FormElementDataAnswer>
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            private FormElementUserInput
                _parent;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public new FormElementDataAnswer this[int idx]
            {
                get { return base[idx]; }
                internal set { base[idx] = value; }
            }
            public new FormElementDataAnswer this[string key]
            {
                get { return base[key]; }
                internal set { base[key] = value; }
            }
            public FormElementUserInput Owner
            {
                get { return this._parent; }
                private set { this._parent = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            internal FormElementUserInputAnswerCollection(FormElementDataAnswer[] answers)
            {
                if (answers != null)
                    for (int i = 0; i < answers.Length; i++)
                        this.Add(answers[i]);
            }
            #endregion

            #region Private Methods
            //***********************************************************************
            // Private Methods
            // 
            internal void Add(FormElementDataAnswer val)
            {
                base.Add(val, val.AnswerProviderKey.ToString());
            }
            #endregion
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private FormElementData
            _qData;
        private FormElementControl
            _parent;
        private FormElementUserInputAnswerCollection
            _answers;
        private string
            _ansText;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FormElementData ElementData
        { get { return this._qData; } }
        public FormElementControl Owner
        { get { return this._parent; } }
        public string AnswerText
        { get { return this._ansText; } }
        public FormElementUserInputAnswerCollection SelectedAnswers
        { get { return this._answers; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementUserInput(FormElementControl parent, FormElementData data, string answerText)
            : this(parent, data, answerText, null)
        { }
        public FormElementUserInput(FormElementControl parent, FormElementData data, string answerText, params FormElementDataAnswer[] ansItems)
        {
            this._parent = parent;
            this._qData = data;
            this._ansText = answerText;
            this._answers = new FormElementUserInputAnswerCollection(ansItems);
        }
        #endregion
    }
}
