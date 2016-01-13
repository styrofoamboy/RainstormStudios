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
    public abstract class DynaMember
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected string
            _name = "",
            _type = "System.Void";
        protected CodeCommentStatementCollection
            ccsc = new CodeCommentStatementCollection();
        protected MemberAttributes
            _scope = MemberAttributes.Private;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public virtual CodeTypeMember TypeMember
        {
            get { return new CodeTypeMember(); }
        }
        public virtual CodeCommentStatementCollection Comments
        {
            get { return this.ccsc; }
        }
        public virtual MemberAttributes ScopeOptions
        {
            get { return this._scope; }
        }
        public virtual string MemberName
        {
            get { return this._name; }
            set { this._name = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Set the scope of this DynaMember object.
        /// </summary>
        /// <param name="ScopeOptions">A string value indicating the desired scope (aka: private, public, static, etc...).</param>
        public void SetScope(string ScopeOptions)
        {
            SetScope(DynaCode.GetScope(ScopeOptions));
        }
        /// <summary>
        /// Set the scope of this DynaMember object.
        /// </summary>
        /// <param name="ScopeOptions"></param>
        public virtual void SetScope(MemberAttributes ScopeOptions)
        {
            this._scope = ScopeOptions;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaMethod : DynaMember
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected CodeStatementCollection
            csc = new CodeStatementCollection();
        protected CodeParameterDeclarationExpressionCollection
            cpdec = new CodeParameterDeclarationExpressionCollection();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeMemberMethod TypeMember
        {
            get
            {
                CodeMemberMethod cmm = new CodeMemberMethod();
                cmm.Attributes = this._scope;
                foreach (CodeCommentStatement cs in ccsc)
                    cmm.Comments.Add(cs);
                foreach (CodeParameterDeclarationExpression de in cpdec)
                    cmm.Parameters.Add(de);
                foreach (CodeStatement cs in csc)
                    cmm.Statements.Add(cs);
                cmm.ReturnType = new CodeTypeReference(this._type);
                return cmm;
            }
        }
        /// <summary>
        /// Gets or sets the member's return value type.
        /// </summary>
        public string ReturnType
        {
            get { return this._type; }
            set { this._type = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaMethod()
        { }
        public DynaMethod(string MethodName)
            : this(MethodName, MemberAttributes.Private)
        { }
        public DynaMethod(string MethodName, string ScopeOptions)
            : this(MethodName, DynaCode.GetScope(ScopeOptions))
        { }
        public DynaMethod(string MethodName, MemberAttributes ScopeOptions)
            : this(MethodName, ScopeOptions, "System.Void")
        { }
        public DynaMethod(string MethodName, string ScopeOptions, Type ReturnType)
            : this(MethodName, DynaCode.GetScope(ScopeOptions), ReturnType.FullName)
        { }
        public DynaMethod(string MethodName, MemberAttributes ScopeOptions, Type ReturnType)
            : this(MethodName, ScopeOptions, ReturnType.FullName)
        { }
        public DynaMethod(string MethodName, string ScopeOptions, string ReturnType)
            : this(MethodName, DynaCode.GetScope(ScopeOptions), ReturnType)
        { }
        public DynaMethod(string MethodName, MemberAttributes ScopeOptions, string ReturnType)
        {
            this._name = MethodName;
            this._scope = ScopeOptions;
            this._type = ReturnType;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Adds a statement to this DynaMember object.
        /// </summary>
        /// <param name="newStatement">An DynaStatement object containing the expression you want to add.</param>
        public void AddStatement(DynaStatement value)
        {
            #region DEBUG Output
#if DEBUG
            Console.WriteLine(value.GetType().Name);
#endif
            #endregion
            switch (value.GetType().Name.ToLower())
            {
                case "dynaiterationstatement":
                    csc.Add((value as DynaIterationStatement).Statement);
                    break;
                case "dynaassignmentstatement":
                    csc.Add((value as DynaAssignmentStatement).Statement);
                    break;
                case "dynaconditionstatement":
                    csc.Add((value as DynaConditionStatement).Statement);
                    break;
                case "dynareturnstatement":
                    csc.Add((value as DynaReturnStatement).Statement);
                    break;
                case "dynadeclarationstatement":
                    csc.Add((value as DynaDeclarationStatement).Statement);
                    break;
                case "dynacommentstatement":
                    csc.Add((value as DynaCommentStatement).Statement);
                    break;
                case "dynamethodexpression":
                    csc.Add((value as DynaMethodExpression).Statement);
                    break;
                case "dynaarraycreateexpression":
                    csc.Add((value as DynaArrayCreateExpression).Statement);
                    break;
                case "dynaarrayindexerexpression":
                    csc.Add((value as DynaArrayIndexerExpression).Statement);
                    break;
                case "dynaoperatorexpression":
                    csc.Add((value as DynaOperatorExpression).Statement);
                    break;
            }
            #region DEBUG Output
#if DEBUG
            Console.WriteLine(csc[csc.Count - 1].ToString());
#endif
            #endregion
        }
        /// <summary>
        /// Adds an expression statement to this DynaMember object.
        /// </summary>
        /// <param name="newStatement">A string value containing a literal expression statement to be added.</param>
        public void AddStatement(string newStatement)
        {
            if (newStatement.Substring(newStatement.Length - 1, 1) != ";")
                newStatement += ";";
            csc.Add(new CodeSnippetStatement(newStatement));
            #region DEBUG Output
#if DEBUG
            Console.WriteLine(csc[csc.Count - 1].ToString());
#endif
            #endregion
        }
        /// <summary>
        /// Adds a member expression parameter to this DynaMember object.
        /// </summary>
        /// <param name="Name">A string value containing the name of this parameter.</param>
        /// <param name="ValueType">A string value indicating the data-type of this parameter.</param>
        public void AddParameter(string Name, string ValueType)
        {
            this.cpdec.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(ValueType), Name));
        }
        /// <summary>
        /// Adds a member expression parameter to this DynaMember object.
        /// </summary>
        /// <param name="Name">A string value containing the name of this parameter.</param>
        /// <param name="ValueType">A Type class object indicating the data-type of this parameter.</param>
        public void AddParameter(string Name, Type ValueType)
        {
            AddParameter(Name, ValueType.FullName);
        }
        /// <summary>
        /// Adds a member expression parameter to this DynaMember object.
        /// </summary>
        /// <param name="arg">An AosDynaArguement object containing the values for this parameter.</param>
        public void AddParameter(DynaArgument arg)
        {
            AddParameter(arg.ArgumentName, arg.DataType);
        }
        /// <summary>
        /// Adds inline documentation to this dynamic member.
        /// </summary>
        /// <param name="value">The comment text.</param>
        public void AddComment(string value)
        {
            AddComment(value, false);
        }
        /// <summary>
        /// Adds inline documentation to this dynamic member.
        /// </summary>
        /// <param name="value">The comment text.</param>
        /// <param name="docTag">A System.Boolean value indicating 'True' if this comment documents a specific type member. Otherwise, false.</param>
        public void AddComment(string value, bool docTag)
        {
            ccsc.Add(new CodeCommentStatement(value, docTag));
        }
        /// <summary>
        /// Sets the data return type of this DynaMember object.
        /// </summary>
        /// <param name="ReturnType">A string value containing the the fully-qualified name of a system data type.</param>
        public void SetReturnType(string value)
        {
            this._type = value;
        }
        /// <summary>
        /// Sets the data return type of this DynaMember object.
        /// </summary>
        /// <param name="ReturnType">A string value containing the the fully-qualified name of a system data type.</param>
        public void SetReturnType(Type value)
        {
            SetReturnType(value.FullName);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaConstructor : DynaMethod
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeConstructor TypeMember
        {
            get
            {
                CodeConstructor cc = new CodeConstructor();
                cc.Name = this._name;
                cc.Attributes = this._scope;
                foreach (CodeCommentStatement cs in ccsc)
                    cc.Comments.Add(cs);
                foreach (CodeParameterDeclarationExpression de in cpdec)
                    cc.Parameters.Add(de);
                foreach (CodeStatement cs in csc)
                    cc.Statements.Add(cs);
                return cc;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaConstructor()
        { }
        public DynaConstructor(string ScopeOptions)
            : this("", ScopeOptions)
        { }
        public DynaConstructor(MemberAttributes ScopeOptions)
            : this("", ScopeOptions)
        { }
        public DynaConstructor(string MethodName, string ScopeOptions)
            : this(MethodName, DynaCode.GetScope(ScopeOptions))
        { }
        public DynaConstructor(string MethodName, MemberAttributes ScopeOptions)
        {
            this._name = MethodName;
            this._scope = ScopeOptions;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaEntryPoint : DynaMethod
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeEntryPointMethod TypeMember
        {
            get
            {
                CodeEntryPointMethod ep = new CodeEntryPointMethod();
                ep.Name = this._name;
                ep.Attributes = MemberAttributes.Private;
                ep.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("System.String[]"), "args"));
                foreach (CodeCommentStatement cs in ccsc)
                    ep.Comments.Add(cs);
                foreach (CodeStatement cs in csc)
                    ep.Statements.Add(cs);
                return ep;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaEntryPoint()
        {
            this._name = "static void Main(string[] args)";
            this.cpdec.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("System.String[]"), "args"));
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaField : DynaMember
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeMemberField TypeMember
        {
            get
            {
                CodeMemberField mf = new CodeMemberField(new CodeTypeReference(this._type), this._name);
                mf.Attributes = this._scope;
                foreach (CodeCommentStatement cs in ccsc)
                    mf.Comments.Add(cs);
                return mf;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaField(string VarName, string ValueType)
            : this(VarName, ValueType, MemberAttributes.Private)
        { }
        public DynaField(string VarName, Type ValueType)
            : this(VarName, ValueType.FullName, MemberAttributes.Private)
        { }
        public DynaField(string VarName, string ValueType, string ScopeOptions)
            : this(VarName, ValueType, DynaCode.GetScope(ScopeOptions))
        { }
        public DynaField(string VarName, Type ValueType, string ScopeOptions)
            : this(VarName, ValueType.FullName, DynaCode.GetScope(ScopeOptions))
        { }
        public DynaField(string VarName, string ValueType, MemberAttributes ScopeOptions)
        {
            this._name = VarName;
            this._type = ValueType;
            this._scope = ScopeOptions;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaProperty : DynaMethod
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeStatementCollection
            csc2 = new CodeStatementCollection();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeMemberProperty TypeMember
        {
            get
            {
                CodeMemberProperty mp = new CodeMemberProperty();
                mp.Attributes = this._scope;
                foreach (CodeCommentStatement cs in ccsc)
                    mp.Comments.Add(cs);
                foreach (CodeStatement cs in csc)
                    mp.GetStatements.Add(cs);
                foreach (CodeStatement cs in csc2)
                    mp.SetStatements.Add(cs);
                mp.Type = new CodeTypeReference(this._type);
                mp.Name = this._name;
                return mp;
            }
        }
        public string PropertyValueType
        {
            get { return this._type; }
            set { this._type = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaProperty()
        { }
        public DynaProperty(string PropertyName, string ValueType, string ScopeOptions)
            : this(PropertyName, ValueType, DynaCode.GetScope(ScopeOptions))
        { }
        public DynaProperty(string PropertyName, Type ValueType, string ScopeOptions)
            : this(PropertyName, ValueType.FullName, DynaCode.GetScope(ScopeOptions))
        { }
        public DynaProperty(string PropertyName, Type ValueType, MemberAttributes ScopeOptions)
            : this(PropertyName, ValueType.FullName, ScopeOptions)
        { }
        public DynaProperty(string PropertyName, string ValueType, MemberAttributes ScopeOptions)
        {
            this._name = PropertyName;
            this._type = ValueType;
            this._scope = ScopeOptions;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void AddGetStatement(DynaExpression value)
        {
            csc.Add(value.Statement);
        }
        public void AddGetStatement(DynaStatement value)
        {
            csc.Add(value.Statement);
        }
        public void AddGetReturn(string VariableName)
        {
            csc.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(VariableName)));
        }
        public void AddSetStatement(DynaExpression value)
        {
            csc2.Add(value.Statement);
        }
        public void AddSetStatement(DynaStatement value)
        {
            csc2.Add(value.Statement);
        }
        public void AddSetAssignment(string VariableName)
        {
            csc2.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(VariableName)));
        }
        public new void AddStatement(string value)
        {
        }
        #endregion
    }
}
