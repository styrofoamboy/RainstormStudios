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
    [Serializable, Author("Unfried, Michael")]
    public struct FormElementDataType
    {
        #region Declarations
        //***************************************************************************
        // Pre-Defined Constants
        // 
        public static readonly FormElementDataType
            Empty;
        public static readonly FormElementDataType
            Text = new FormElementDataType(1, "Text", string.Empty, string.Empty);
        public static readonly FormElementDataType
            Date = new FormElementDataType(2, "Date", @"^((0?[13578]|10|12)(-|\/)((0[0-9])|([12])([0-9]?)|(3[01]?))(-|\/)((\d{4})|(\d{2}))|(0?[2469]|11)(-|\/)((0[0-9])|([12])([0-9]?)|(3[0]?))(-|\/)((\d{4}|\d{2})))$", "00/00/0000");
        public static readonly FormElementDataType
            Decimal = new FormElementDataType(3, "Decimal", @"^[-+]?((((\d{1,3},)*\d{3})|(\d+)))(\.\d+)?$", string.Empty);
        public static readonly FormElementDataType
            EmailAddress = new FormElementDataType(4, "Email Address", @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", string.Empty);
        public static readonly FormElementDataType
            Integer = new FormElementDataType(5, "Integer", @"^[-+]?(((\d{1,3},)*\d{3})|(\d+))$", string.Empty);
        public static readonly FormElementDataType
            Money = new FormElementDataType(6, "Money", @"^\$?(((\d{1,3},)*\d{3})|(\d+))(\.\d{2})?$", string.Empty);
        public static readonly FormElementDataType
            PhoneNumber = new FormElementDataType(7, "Phone Number", @"^\(?\d{3}(?:\)?\s?|[\-\.\s]?)\d{3}[\-\.\s]?\d{4}$", "(000) 000-0000");
        public static readonly FormElementDataType
            ZipCode = new FormElementDataType(8, "Zip Code", @"^\d{5}(\-\d{4})?$", "00000-0000");
        //***************************************************************************
        // Private Fields
        // 
        private object
            _providerKey;
        private string
            _name,
            _rgxStr,
            _maskStr;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object DataTypeProviderKey
        { get { return this._providerKey; } }
        public string DataTypeName
        { get { return this._name; } }
        public string ValidationString
        { get { return this._rgxStr; } }
        public string MaskString
        { get { return this._maskStr; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementDataType(object providerKey, string name, string validationStr, string maskStr)
        {
            this._rgxStr = validationStr;
            this._maskStr = maskStr;
            this._providerKey = providerKey;
            this._name = name;
        }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public override bool Equals(object obj)
        {
            if (!(obj is FormElementDataType))
                return false;

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return (this.DataTypeName + "_" + this.DataTypeProviderKey.ToString() + "_" + this.ValidationString).GetHashCode();
        }
        public override string ToString()
        {
            return this.DataTypeName;
        }
        public static bool operator ==(FormElementDataType v1, FormElementDataType v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(FormElementDataType v1, FormElementDataType v2)
        {
            return !v1.Equals(v2);
        }
        #endregion
    }
    [Serializable, Author("Unfried, Michael")]
    public sealed class FormElementDataTypeCollection : Collections.ObjectCollectionBase<FormElementDataType>
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public FormElementDataType Text
        { get { return this["Text"]; } }
        public FormElementDataType Date
        { get { return this["Date"]; } }
        public FormElementDataType Decimal
        { get { return this["Decimal"]; } }
        public FormElementDataType EmailAddress
        { get { return this["Email Address"]; } }
        public FormElementDataType Integer
        { get { return this["Integer"]; } }
        public FormElementDataType Money
        { get { return this["Money"]; } }
        public FormElementDataType PhoneNumber
        { get { return this["Phone Number"]; } }
        public FormElementDataType ZopCode
        { get { return this["Zip Code"]; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FormElementDataTypeCollection()
            : base()
        {
            this.ReturnNullForKeyNotFound = true;
            this.ReturnNullForIndexNotFound = false;

            this.LoadDefaultDataTypes();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(FormElementDataType dType)
        { base.Add(dType, dType.DataTypeName); }
        public void Replace(FormElementDataType dType, string dataTypeNameToReplace)
        {
            if (this.ContainsKey(dataTypeNameToReplace))
                this[dataTypeNameToReplace] = dType;
            else
                this.Add(dType);
        }
        public FormElementDataType GetByKey(object providerKey)
        {
            var types = this.Where(t => t.DataTypeProviderKey == providerKey);
            if (types.Count() > 1)
                throw new Exception("More than one data type found for the specified key.");
            else if (types.Count() < 1)
                return FormElementDataType.Empty;
            else
                return types.Single();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void LoadDefaultDataTypes()
        {
            Type dtType = typeof(FormElementDataType);
            System.Reflection.FieldInfo[] flds = dtType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.FlattenHierarchy);
            for (int i = 0; i < flds.Length; i++)
            {
                if (flds[i].ReflectedType != dtType)
                    continue;

                FormElementDataType fldDefType = (FormElementDataType)flds[i].GetValue(null);

                if (fldDefType == FormElementDataType.Empty)
                    continue;

                if (!this.ContainsKey(fldDefType.DataTypeName))
                    this.Add(fldDefType);
            }
        }
        #endregion
    }
}
