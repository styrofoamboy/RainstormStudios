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
using System.CodeDom;

namespace RainstormStudios.DynamicCodeGeneration
{
    [Author("Unfried, Michael")]
    public class DynaArgument
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string 
            _type = "System.Void",
            _name = "";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CodeParameterDeclarationExpression CodeArgument
        {
            get { return new CodeParameterDeclarationExpression(new CodeTypeReference(this._type), this._name); }
        }
        public string DataType
        {
            get { return this._type; }
            set { this._type = value; }
        }
        public string ArgumentName
        {
            get { return this._name; }
            set { this._name = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaArgument(string Name, string ValueType)
        {
            this._name = Name;
            this._type = ValueType;
        }
        public DynaArgument(string Name, Type ValueType)
            : this(Name, ValueType.FullName)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SetDataType(Type DataType)
        {
            SetDataType(DataType.FullName);
        }
        public void SetDataType(string DataType)
        {
            this._type = DataType;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaArgumentCollection
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeParameterDeclarationExpressionCollection
            codeArgs;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CodeParameterDeclarationExpressionCollection CodeArguementCollection
        {
            get { return codeArgs; }
        }
        public int Count
        {
            get { return codeArgs.Count; }
        }
        public int Capacity
        {
            get { return codeArgs.Capacity; }
            set { codeArgs.Capacity = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaArgumentCollection()
        {
            codeArgs = new CodeParameterDeclarationExpressionCollection();
        }
        public DynaArgumentCollection(DynaArgument[] args)
            : this()
        {
            foreach (DynaArgument arg in args)
                codeArgs.Add(arg.CodeArgument);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(DynaArgument newArg)
        {
            codeArgs.Add(newArg.CodeArgument);
        }
        public void Remove(DynaArgument oldArg)
        {
            codeArgs.Remove(oldArg.CodeArgument);
        }
        public void Insert(int index, DynaArgument newArg)
        {
            codeArgs.Insert(index, newArg.CodeArgument);
        }
        public void RemoveAt(int index)
        {
            codeArgs.RemoveAt(index);
        }
        public bool Contains(DynaArgument thisArg)
        {
            return codeArgs.Contains(thisArg.CodeArgument);
        }
        public int IndexOf(DynaArgument thisArg)
        {
            return codeArgs.IndexOf(thisArg.CodeArgument);
        }
        #endregion
    }
}
