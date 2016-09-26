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

namespace RainstormStudios.Drawing.PDF
{
    public class PDFObject
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private object
            _objVal;
        private int
            _objNum,
            _genNum;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public object Value
        {
            get { return this._objVal; }
            set { this._objVal = value; }
        }
        public int ObjectNumber
        {
            get { return this._objNum; }
            set { this._objNum = value; }
        }
        public int GenerationNumber
        {
            get { return this._genNum; }
            set { this._genNum = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PDFObject()
        { }
        public PDFObject(int objNum, int genNum, object value)
            : this()
        {
            this._objNum = objNum;
            this._genNum = genNum;
            this._objVal = value;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static PDFObject ParseObjectData(byte[] data)
        {
            // Try and decode the provided data into a basic ASCII string.
            string strData = System.Text.Encoding.ASCII.GetString(data, 0, data.Length).TrimStart(' ');

            // Find the position of the first two 'whitespace' characters
            //   within the decoded string.  These should delimit the object's
            //   ID number and Generation number.
            int objNum, genNum,
                objNumLen = strData.IndexOf(' '),
                genNumLen = strData.IndexOf(' ', objNumLen + 1);

            // Read the ID number and Generation number into seperate string values.
            string
                sONum = strData.Substring(0, objNumLen),
                sGNum = strData.Substring(objNumLen + 1, genNumLen - objNumLen);

            // Attempt to parse the ID number and Generation number, throwing an
            //   Exception is either one should fail to parse.
            if (!int.TryParse(sONum, out objNum))
                throw new ArgumentException("Supplied data is not in the expected format.");
            if (!int.TryParse(sGNum, out genNum))
                throw new ArgumentException("Supplied data is not in the expected format.");

            object objVal = null;
            if (genNumLen + 3 > data.Length && strData.Substring(genNumLen, 2) == "<<")
            {
                PDFDictionary dicObj = new PDFDictionary();
                string dictData = strData.Substring(genNum + 2, strData.IndexOf(">>", genNum + 2));
                string[] dictvals = dictData.Split('/');
                for (int i = 0; i < dictvals.Length; i += 2)
                    dicObj.Add(dictvals[i + 1].Trim(), dictvals[i].Trim());
                objVal = dicObj;
            }
            else
            {
            }

            return new PDFObject(objNum, genNum, objVal);
        }
        #endregion
    }
    public class PDFDictionary : RainstormStudios.Collections.ObjectCollection
    {
        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public new void Add(object value, string key)
        {
            base.Add(value, key);
        }
        public void Remove(object value)
        {
            base.RemoveAt(base.IndexOf(value));
        }
        #endregion
    }
    public class PDFObjectCollection : RainstormStudios.Collections.ObjectCollectionBase<PDFObject>
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new PDFObject this[int index]
        {
            get { return (PDFObject)base[index]; }
            set { base[index] = value; }
        }
        public new PDFObject this[string key]
        {
            get { return (PDFObject)base[key]; }
            set { base[key] = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PDFObjectCollection()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(PDFObject value, int objNumber, int genNumber)
        {
            base.Add(value, string.Format("{0} {1}", objNumber, genNumber));
        }
        public void Remove(PDFObject value)
        {
            base.RemoveAt(this.IndexOf(value));
        }
        public PDFObject GetObject(int objNumber, int genNumber)
        {
            string objKey = string.Format("{0} {1}", objNumber, genNumber);
            if (this._keys.Contains(objKey))
                return this[objKey];
            else
                return null;
        }
        #endregion
    }
}
