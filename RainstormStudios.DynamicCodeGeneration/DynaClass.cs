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
    /// <summary>
    /// Provides a simple container and methods for building a CodeDOM-based class.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaClass
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeTypeDeclaration
            _dynaCls;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets the CodeTypeDelclaration (CodeDOM Class) object built within this instance.
        /// </summary>
        public CodeTypeDeclaration DynamicClass
        {
            get { return this._dynaCls; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// Provides a simple container and methods for building a CodeDOM-based class.
        /// </summary>
        /// <param name="ClassName"></param>
        public DynaClass(string ClassName)
            : this(ClassName, MemberAttributes.Private)
        { }
        public DynaClass(string ClassName, string ScopeOptions)
            : this(ClassName, DynaCode.GetScope(ScopeOptions))
        { }
        /// <summary>
        /// Provides a simple container and methods for building a CodeDOM-based class.
        /// </summary>
        /// <param name="ClassName">A string value containing the name of this class.</param>
        /// <param name="Scope">A value of type MemberAttributes which defines the scope of this class.</param>
        public DynaClass(string ClassName, System.CodeDom.MemberAttributes Scope)
        {
            this._dynaCls = new CodeTypeDeclaration(ClassName);
            this._dynaCls.Attributes = Scope;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Adds a new method to this DynaClass object.
        /// </summary>
        /// <param name="newMethod">An AosDynaMethod object containing the details of the method you want to add.</param>
        public void AddMethod(DynaMethod newMethod)
        {
            this._dynaCls.Members.Add(newMethod.TypeMember);
        }
        /// <summary>
        /// Adds a new class constructor to this DynaClass object.
        /// </summary>
        /// <param name="newConstructor">A DynaConstructor object containing the details of the constructor you want to add.</param>
        public void AddConstructor(DynaConstructor newConstructor)
        {
            this._dynaCls.Members.Add(newConstructor.TypeMember);
        }
        /// <summary>
        /// Adds a new program EntryPoint to this DynaClass object.
        /// When adding this class to an DynaCode object containing other classes, there should be only *one* (1) EntryPoint for the entire DynaCode object.
        /// </summary>
        /// <param name="newEntryPoint">A DynaEntryPoint object containing the details of the entry point you want to add.</param>
        public void AddEntryPoint(DynaEntryPoint newEntryPoint)
        {
            this._dynaCls.Members.Add(newEntryPoint.TypeMember);
        }
        /// <summary>
        /// Adds a new field to this DynaClass object.
        /// </summary>
        /// <param name="newField">A DynaField object containing the details of the field you want to add.</param>
        public void AddField(DynaField newField)
        {
            this._dynaCls.Members.Add(newField.TypeMember);
        }
        /// <summary>
        /// Adds a new property to this DynaClass object.
        /// </summary>
        /// <param name="newProperty">A DynaProperty object containing the details of the property you want to add.</param>
        public void AddProperty(DynaProperty newProperty)
        {
            this._dynaCls.Members.Add(newProperty.TypeMember);
        }
        /// <summary>
        /// Creates a new method member within this DynaClass object.
        /// </summary>
        /// <param name="Name">A string value containing the name of the new method.</param>
        /// <param name="Arguements">An array of DynaArguement objects specifying the incoming arguements for the new methdod.</param>
        /// <param name="Statements">An array of DynaStatement objects specifying the expression statements contained within this method.</param>
        /// <param name="Scope">A value of type MemberAttributes indicating the scope of the new method.</param>
        public void CreateMethod(string Name, DynaArgument[] Arguements, DynaExpression[] Statements, System.CodeDom.MemberAttributes Scope)
        {
            DynaMethod dm = new DynaMethod(Name);
            dm.TypeMember.Attributes = Scope;
            foreach (DynaExpression stmt in Statements)
                dm.TypeMember.Statements.Add(stmt.Expression);
            AddMethod(dm);
        }
        /// <summary>
        /// Creates a new class constructor member within this AosDynaClass object.
        /// </summary>
        /// <param name="Arguements">An array of DynaArguement objects containing the incoming arguements for this class constructor.</param>
        /// <param name="Statements">An array of DynaStatement objects containing the expression statements contained within this class constructor.</param>
        /// <param name="Scope">A value of type MemberAttributes indicating the scope of this class constructor.</param>
        public void CreateConstructor(DynaArgument[] Arguments, DynaExpression[] Statements, System.CodeDom.MemberAttributes Scope)
        {
            DynaConstructor cc = new DynaConstructor();
            cc.TypeMember.Attributes = Scope;
            foreach (DynaExpression stmt in Statements)
                cc.TypeMember.Statements.Add(stmt.Expression);
            AddConstructor(cc);
        }
        /// <summary>
        /// Creates a new entry point member within this DynaClass object.
        /// When adding this class to an DynaCode object containing other classes, there should be only *one* (1) EntryPoint for the entire DynaCode object.
        /// </summary>
        /// <param name="Arguements">An array of DynaArguement objects containing the incoming arguements for the new entry point.</param></param>
        /// <param name="Statements">An array of DynaStatement objects containing the expression statements contained within the new entry point.</param></param>
        /// <param name="Scope">A value of type MemberAttributes indicating the scope of this class constructor.</param>
        public void CreateEntryPoint(DynaArgument[] Arguements, DynaExpression[] Statements, System.CodeDom.MemberAttributes Scope)
        {
            DynaEntryPoint ep = new DynaEntryPoint();
            ep.TypeMember.Attributes = Scope;
            foreach (DynaExpression stmt in Statements)
                ep.TypeMember.Statements.Add(stmt.Expression);
            AddEntryPoint(ep);
        }
        /// <summary>
        /// Creates a new field member within this DynaClass object.
        /// </summary>
        /// <param name="Name">A string value containing the name of the new field.</param>
        /// <param name="ValueType">A Type class object indicating the data-type of the new field.</param>
        /// <param name="GetStatements">An array of DynaStatement objects containing the expression statements contained within the 'Set' portion of the field.</param>
        /// <param name="SetStatements">An array of DynaStatement objects containing the expression statements contained within the 'Get' portion of the field.</param>
        /// <param name="Scope">A value of type MemberAttributes indicating the scope of this field.</param>
        public void CreateProperty(string Name, Type ValueType, DynaExpression[] SetStatements, DynaExpression[] GetStatements, System.CodeDom.MemberAttributes Scope)
        {
            DynaProperty prop = new DynaProperty(Name, ValueType, Scope);
            foreach (DynaExpression stmt in SetStatements)
                prop.AddSetStatement(stmt);
            foreach (DynaExpression stmt in GetStatements)
                prop.AddGetStatement(stmt);
            this.AddProperty(prop);
        }
        public void SetScope(string ScopeOptions)
        {
            SetScope(DynaCode.GetScope(ScopeOptions));
        }
        public void SetScope(MemberAttributes ScopeOptions)
        {
            this._dynaCls.Attributes = ScopeOptions;
        }
        #endregion
    }
}
