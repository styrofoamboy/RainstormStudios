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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls.DynamicForms
{
    [Serializable]
    public class FormElementData : System.ComponentModel.INotifyPropertyChanged
    {
        #region Declaratons
        //***************************************************************************
        // Private Fields
        // 
        object
            _provID,
            _parentProvID,
            _formProvID;
        string
            _providerName,
            _qText,
            _hintText,
            _suffix,
            _cssClass;
        bool
            _isDirty,
            _required,
            _immutable;
        Unit
            _width,
            _height,
            _bdrSz,
            _marginLeft,
            _marginRight;
        int
            _rowCount,
            _colCount,
            _rowIdx,
            _colIdx,
            _ordinalPos;
        FormElementDataType
            _dataType;
        FormElementDisplayType
            _dispType;
        FormElementDisplayOptions
            _dispOptions;
        string
            _navUrl,
            _imgUrl;
        ImageAlign
            _imgAlign;
        System.Drawing.Color
            _fgClr,
            _bgClr,
            _bdrClr;
        BorderStyle
            _bdrStyle;
        FontInfo
            _font;
        Style
            _style;
        FormElementDataCollection
            _children;
        FormElementDataAnswerCollection
            _answers;
        RainstormStudios.Collections.ObjectCollection
            _dependantKeys;
        bool
            _hasLoadedAnswers,
            _hasLoadedChildren,
            _hasLoadedDependants;
        //***************************************************************************
        // Public Events
        // 
        public event PropertyChangedEventHandler
            PropertyChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object ElementProviderKey
        {
            get { return this._provID; }
            set
            {
                if (value != this._provID)
                {
                    this._provID = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ElementProviderKey"));
                }
            }
        }
        public object ParentElementProviderKey
        {
            get { return this._parentProvID; }
            set
            {
                if (value != this._parentProvID)
                {
                    this._parentProvID = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ParentElementProviderKey"));
                }
            }
        }
        public object FormProviderKey
        {
            get { return this._formProvID; }
            set
            {
                if (value != this._formProvID)
                {
                    this._formProvID = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("FormProviderKey"));
                }
            }
        }
        public string ProviderName
        {
            get { return this._providerName; }
            set { this._providerName = value; }
        }
        public string Text
        {
            get { return this._qText; }
            set
            {
                if (value != this._qText)
                {
                    this._qText = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Text"));
                }
            }
        }
        public string HintText
        {
            get { return this._hintText; }
            set
            {
                if (value != this._hintText)
                {
                    this._hintText = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("HintText"));
                }
            }
        }
        public string CssClass
        {
            get { return this._cssClass; }
            set
            {
                if (value != this._cssClass)
                {
                    this._cssClass = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("CssClass"));
                }
            }
        }
        public string Suffix
        {
            get { return this._suffix; }
            set
            {
                if (value != this._suffix)
                {
                    this._suffix = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Suffix"));
                }
            }
        }
        public bool Required
        {
            get { return this._required; }
            set
            {
                if (value != this._required)
                {
                    this._required = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Required"));
                }
            }
        }
        public Unit ElementWidth
        {
            get { return this._width; }
            set
            {
                if (value != this._width)
                {
                    this._width = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ElementWidth"));
                }
            }
        }
        public Unit ElementHeight
        {
            get { return this._height; }
            set
            {
                if (value != this._height)
                {
                    this._height = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ElementHeight"));
                }
            }
        }
        public Unit MarginLeft
        {
            get { return this._marginLeft; }
            set
            {
                if (value != this._marginLeft)
                {
                    this._marginLeft = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("MarginLeft"));
                }
            }
        }
        public Unit MarginRight
        {
            get { return this._marginRight; }
            set
            {
                if (value != this._marginRight)
                {
                    this._marginRight = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("MarginRight"));
                }
            }
        }
        public int RowCount
        {
            get { return this._rowCount; }
            set
            {
                if (value != this._rowCount)
                {
                    this._rowCount = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("RowCount"));
                }
            }
        }
        public int ColumnCount
        {
            get { return this._colCount; }
            set
            {
                if (value != this._colCount)
                {
                    this._colCount = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ColumnCount"));
                }
            }
        }
        public int RowIndex
        {
            get { return this._rowIdx; }
            set { this._rowIdx = value; }
        }
        public int ColumnIndex
        {
            get { return this._colIdx; }
            set { this._colIdx = value; }
        }
        public int OrdinalPosition
        {
            get { return this._ordinalPos; }
            set { this._ordinalPos = value; }
        }
        public FormElementDataType DataType
        {
            get { return this._dataType; }
            set
            {
                if (value != this._dataType)
                {
                    this._dataType = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DataType"));
                }
            }
        }
        public FormElementDisplayType DisplayType
        {
            get { return this._dispType; }
            set
            {
                if (value != this._dispType)
                {
                    this._dispType = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DisplayType"));
                }
            }
        }
        public FormElementDisplayOptions DisplayOptions
        {
            get { return this._dispOptions; }
            set
            {
                if (value != this._dispOptions)
                {
                    this._dispOptions = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DisplayOptions"));
                }
            }
        }
        public string NavigationUrl
        {
            get { return this._navUrl; }
            set
            {
                if (value != this._navUrl)
                {
                    this._navUrl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("NavigationUrl"));
                }
            }
        }
        public string ImageUrl
        {
            get { return this._imgUrl; }
            set
            {
                if (value != this._imgUrl)
                {
                    this._imgUrl = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ImageUrl"));
                }
            }
        }
        public ImageAlign ImageAlignment
        {
            get { return this._imgAlign; }
            set
            {
                if (value != this._imgAlign)
                {
                    this._imgAlign = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ImageAlignment"));
                }
            }
        }
        public System.Drawing.Color ForeColor
        {
            get { return this._fgClr; }
            set
            {
                if (value != this._fgClr)
                {
                    this._fgClr = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ForeColor"));
                }
            }
        }
        public System.Drawing.Color BackColor
        {
            get { return this._bgClr; }
            set
            {
                if (value != this._bgClr)
                {
                    this._bgClr = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("BackColor"));
                }
            }
        }
        public BorderStyle BorderStyle
        {
            get { return this._bdrStyle; }
            set
            {
                if (value != this._bdrStyle)
                {
                    this._bdrStyle = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("BorderStyle"));
                }
            }
        }
        public Unit BorderWidth
        {
            get { return this._bdrSz; }
            set
            {
                if (value != this._bdrSz)
                {
                    this._bdrSz = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("BorderWidth"));
                }
            }
        }
        public System.Drawing.Color BorderColor
        {
            get { return this._bdrClr; }
            set
            {
                if (value != this._bdrClr)
                {
                    this._bdrClr = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("BorderColor"));
                }
            }
        }
        public FontInfo Font
        {
            get { return this._font; }
        }
        public Style Style
        {
            get { return this._style; }
        }
        public FormElementDataCollection Children
        {
            get
            {
                if (!this._hasLoadedChildren)
                    this._children.AddRange(this.Provider.GetChildElements(this));
                return this._children;
            }
        }
        public Collections.ObjectCollection DependantKeys
        {
            get
            {
                if (!this._hasLoadedDependants)
                    this.DependantKeys.AddRange(this.Provider.GetDependantKeys(this));
                return this._dependantKeys;
            }
        }
        public FormElementDataAnswerCollection Answers
        {
            get
            {
                if (!this._hasLoadedAnswers)
                {
                    FormElementDataAnswer[] ans = this.Provider.GetAnswers(this);
                    if (ans != null)
                        this._answers.AddRange(ans);
                }
                return this._answers;
            }
        }
        public bool HasVerticalOption
        { get { return this._dispOptions.HasFlag(FormElementDisplayOptions.DisplayVertical); } }
        public bool HasHorizontalOption
        { get { return this._dispOptions.HasFlag(FormElementDisplayOptions.DisplayHorizontal); } }
        public bool IsDirty
        { get { return this._isDirty; } }
        public bool Immutable
        {
            get { return this._immutable; }
            set { this._immutable = value; }
        }
        public Providers.DynamicFormProvider Provider
        {
            get
            {
                return (!string.IsNullOrEmpty(this._providerName)
                    ? Providers.DynamicFormProviderManager.Providers[this._providerName]
                    : Providers.DynamicFormProviderManager.Provider);
            }
        }
        //***************************************************************************
        // Private Properties
        // 
        internal bool HasLoadedChildren
        {
            get { return this._hasLoadedChildren; }
            set { this._hasLoadedChildren = value; }
        }
        internal bool HasLoadedDependants
        {
            get { return this._hasLoadedDependants; }
            set { this._hasLoadedDependants = value; }
        }
        internal bool HasLoadedAnswers
        {
            get { return this._hasLoadedAnswers; }
            set { this._hasLoadedAnswers = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementData()
        {
            this._children = new FormElementDataCollection(this);
            this._dependantKeys = new RainstormStudios.Collections.ObjectCollection();
            this._answers = new FormElementDataAnswerCollection(this);
            this._style = new Style();
            this._font = this._style.Font;
            this._isDirty = false;
            this._hasLoadedAnswers =
                this._hasLoadedChildren =
                this._hasLoadedDependants = false;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SaveChanges()
        {
            this.Provider.SaveFormElementChanges(this);
            this._isDirty = false;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this._isDirty = true;
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, e);
        }
        #endregion
    }
    [Serializable]
    public class FormElementDataCollection : Collections.ObjectCollectionBase<FormElementData>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        FormElementData
            _parent;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FormElementData Owner
        { get { return this._parent; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementDataCollection(FormElementData owner)
            : base()
        { this._parent = owner; }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new void Add(FormElementData val)
        {
            val.ParentElementProviderKey = this._parent.ElementProviderKey;
            base.Add(val, val.ElementProviderKey.ToString());
        }
        #endregion
    }
    [Serializable]
    public class FormElementDataAnswer
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        object
            _provID;
        string
            _ansText;
        FormElementData
            _parent;
        int
            _ordPos;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object AnswerProviderKey
        {
            get { return this._provID; }
            set { this._provID = value; }
        }
        public string AnswerText
        {
            get { return this._ansText; }
            set { this._ansText = value; }
        }
        public FormElementData Parent
        {
            get { return this._parent; }
            set { this._parent = value; }
        }
        public int OrdinalPosition
        {
            get { return this._ordPos; }
            internal set { this._ordPos = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementDataAnswer(object providerKey, string text)
        {
            this._provID = providerKey;
            this._ansText = text;
        }
        #endregion
    }
    [Serializable]
    public class FormElementDataAnswerCollection : Collections.ObjectCollectionBase<FormElementDataAnswer>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        FormElementData
            _owner;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementDataAnswerCollection(FormElementData parent)
            : base()
        {
            this._owner = parent;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(FormElementDataAnswer val)
        {
            val.Parent = this._owner;
            base.Add(val, val.AnswerProviderKey.ToString());
        }
        #endregion
    }
}
