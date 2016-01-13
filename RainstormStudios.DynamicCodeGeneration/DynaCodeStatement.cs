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
    /// Provides a base class to the other AllOneSystem.DynaCode statement classes.
    /// This is an abstract class and cannot be instanced directly.
    /// </summary>
    [Author("Unfried, Michael")]
    public abstract class DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected CodeStatementCollection
            csc = new CodeStatementCollection();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public virtual CodeStatement Statement
        { get { return new CodeStatement(); } }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void AddToMember(ref DynaMethod value)
        {
            value.AddStatement(this);
        }
        public void AddToMember(ref DynaConstructor value)
        {
            value.AddStatement(this);
        }
        public void AddToMember(ref DynaEntryPoint value)
        {
            value.AddStatement(this);
        }
        //***************************************************************************
        // Static Methods
        // 
        public static void AddToMember(ref DynaMethod memberObj, string LiteralExpression)
        {
            memberObj.AddStatement(LiteralExpression);
        }
        public static void AddToMember(ref DynaConstructor memberObj, string LiteralExpression)
        {
            memberObj.AddStatement(LiteralExpression);
        }
        public static void AddToMember(ref DynaEntryPoint memberObj, string LiteralExpression)
        {
            memberObj.AddStatement(LiteralExpression);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public abstract class DynaExpression : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected CodeExpressionCollection
            cec = new CodeExpressionCollection();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public virtual CodeExpression Expression
        { get { return new CodeExpression(); } }
        public new CodeExpressionStatement Statement
        {
            get { return new CodeExpressionStatement(this.Expression); }
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and routines for building CodeDOM-based method invoke expression statements.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaMethodExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _methodName = "",
            _nameSpace = "";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeMethodInvokeExpression Expression
        {
            get
            {
                CodeExpression[] ceArray = null;
                if (cec.Count > 0)
                {
                    ceArray = new CodeExpression[cec.Count];
                    for (int i = 0; i < ceArray.Length; i++)
                        ceArray[i] = cec[i];
                }
                if (!string.IsNullOrEmpty(_nameSpace))
                    if (ceArray != null)
                        return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(_nameSpace)), _methodName), ceArray);
                    else
                        return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(_nameSpace)), _methodName), new CodeExpression[] { });
                else
                    if (ceArray != null)
                        return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), _methodName), ceArray);
                    else
                        return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), _methodName), new CodeExpression[] { });
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaMethodExpression(string MethodName)
            : this("", MethodName, new object[] { })
        { }
        public DynaMethodExpression(string MethodName, object ExpressionArg)
            : this("", MethodName, new object[] { ExpressionArg })
        { }
        public DynaMethodExpression(string MethodName, object[] ExpressionArgs)
            : this("", MethodName, ExpressionArgs)
        { }
        /// <summary>
        /// Provides a simple container and routines for building InvokeExpressions for CodeDOM objects.
        /// </summary>
        /// <param name="ExpressionNamespace">A string value qualifying the class namespace for the expression.</param>
        /// <param name="MethodName">A string value containing the expression to be executed.</param>
        public DynaMethodExpression(string ExpressionNamespace, string MethodName)
            : this(ExpressionNamespace, MethodName, new object[] { })
        { }
        /// <summary>
        /// Provides a simple container and routines for building InvokeExpressions for CodeDOM objects.
        /// </summary>
        /// <param name="ExpressionNamespace">A string value qualifying the class namespace for the expression.</param>
        /// <param name="MethodName">A string value containing the expression to be executed.</param>
        /// <param name="ExpressionArgument">A value of type object that will be passed to the expression statement.</param>
        public DynaMethodExpression(string ExpressionNamespace, string MethodName, object ExpressionArg)
            : this(ExpressionNamespace, MethodName, new object[] { ExpressionArg })
        { }
        /// <summary>
        /// Provides a simple container and routines for building InvokeExpressions for CodeDOM objects.
        /// </summary>
        /// <param name="ExpressionNamespace">A string value qualifying the class namespace for the expression.</param>
        /// <param name="MethodName">A string value containing the expression to be executed.</param>
        /// <param name="ExpressionArguments">An array of type object containing values to be passed to the expression statement.</param>
        public DynaMethodExpression(string ExpressionNamespace, string MethodName, object[] ExpressionArgs)
        {
            _methodName = MethodName;
            _nameSpace = ExpressionNamespace;
            if (ExpressionArgs != null && ExpressionArgs.Length > 0)
            {
                foreach (object obj in ExpressionArgs)
                    cec.Add(new CodePrimitiveExpression(obj));
            }
        }
        public DynaMethodExpression(string ExpressionNamespace, string MethodName, DynaExpression ExpressionArg)
            : this(ExpressionNamespace, MethodName, new DynaExpression[] { ExpressionArg })
        { }
        public DynaMethodExpression(string ExpressionNamespace, string MethodName, DynaExpression[] ExpressionArgs)
        {
            _methodName = MethodName;
            _nameSpace = ExpressionNamespace;
            if (ExpressionArgs != null && ExpressionArgs.Length > 0)
            {
                foreach (DynaExpression de in ExpressionArgs)
                    switch (de.GetType().Name.ToLower())
                    {
                        case "dynamethodexpression":
                            cec.Add((de as DynaMethodExpression).Expression);
                            break;
                        case "dynaarraycreateexpression":
                            cec.Add((de as DynaArrayCreateExpression).Expression);
                            break;
                        case "dynaarrayindexerexpression":
                            cec.Add((de as DynaArrayIndexerExpression).Expression);
                            break;
                        case "dynaoperatorexpression":
                            cec.Add((de as DynaOperatorExpression).Expression);
                            break;
                    }
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Adds an argument value to this method call.  Be cautious, when adding values here, that they are added in the proper order.
        /// </summary>
        /// <param name="value">An object value to pass durring the method call.</param>
        public void AddArgument(object value)
        {
            cec.Add(new CodePrimitiveExpression(value));
        }
        /// <summary>
        /// Adds an argument value to this method call.  Be cautious, when adding values here, that they are added in the proper order.
        /// </summary>
        /// <param name="value">A string value containing the name of a local variable whose value will be passed as a value.</param>
        public void AddArgument(string value)
        {
            cec.Add(new CodeVariableReferenceExpression(value));
        }
        /// <summary>
        /// Adds an argument value to this method call.  Be cautious, when adding values here, that they are added in the proper order.
        /// </summary>
        /// <param name="value">A DynaExpression object containing an expression whose result value will be passed to the method.</param>
        public void AddArgument(DynaExpression value)
        {
            switch (value.GetType().Name.ToLower())
            {
                case "dynamethodexpression":
                    cec.Add((value as DynaMethodExpression).Expression);
                    break;
                case "dynaarraycreateexpression":
                    cec.Add((value as DynaArrayCreateExpression).Expression);
                    break;
                case "dynaarrayindexerexpression":
                    cec.Add((value as DynaArrayIndexerExpression).Expression);
                    break;
                case "dynaoperatorexpression":
                    cec.Add((value as DynaOperatorExpression).Expression);
                    break;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-based iteration loops.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaIterationStatement : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeStatement
            _initValue = null,
            _valueInc = null;
        private CodeExpression
            _testCondition = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Fields
        // 
        public new CodeIterationStatement Statement
        {
            get
            {
                CodeStatement[] csArray = null;
                if (csc.Count > 0)
                {
                    csArray = new CodeStatement[csc.Count];
                    for (int i = 0; i < csArray.Length; i++)
                        csArray[i] = csc[i];
                    return new CodeIterationStatement(_initValue, _testCondition, _valueInc, csArray);
                }
                else
                    return new CodeIterationStatement(_initValue, _testCondition, _valueInc, csArray);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaIterationStatement()
        { }
        public DynaIterationStatement(DynaStatement InitValue, DynaOperatorExpression LoopCondition, DynaAssignmentStatement ValueIncrement)
            : this(InitValue, LoopCondition, ValueIncrement, null)
        { }
        public DynaIterationStatement(DynaStatement InitValue, DynaExpression LoopCondition, DynaStatement ValueIncrement, DynaStatement[] LoopStatements)
        {
            switch (InitValue.GetType().Name.ToLower())
            {
                case "dynaiterationstatement":
                    this._initValue = (InitValue as DynaIterationStatement).Statement;
                    break;
                case "dynaassignmentstatement":
                    this._initValue = (InitValue as DynaAssignmentStatement).Statement;
                    break;
                case "dynaconditionstatement":
                    this._initValue = (InitValue as DynaConditionStatement).Statement;
                    break;
                case "dynareturnstatement":
                    this._initValue = (InitValue as DynaReturnStatement).Statement;
                    break;
                case "dynadeclarationstatement":
                    this._initValue = (InitValue as DynaDeclarationStatement).Statement;
                    break;
                case "dynacommentstatement":
                    this._initValue = (InitValue as DynaCommentStatement).Statement;
                    break;
            }
            switch (LoopCondition.GetType().Name.ToLower())
            {
                case "dynamethodexpression":
                    this._testCondition = (LoopCondition as DynaMethodExpression).Expression;
                    break;
                case "dynaarraycreateexpression":
                    this._testCondition = (LoopCondition as DynaArrayCreateExpression).Expression;
                    break;
                case "dynaarrayindexerexpression":
                    this._testCondition = (LoopCondition as DynaArrayIndexerExpression).Expression;
                    break;
                case "dynaoperatorexpression":
                    this._testCondition = (LoopCondition as DynaOperatorExpression).Expression;
                    break;
            }
            switch (ValueIncrement.GetType().Name.ToLower())
            {
                case "dynaiterationstatement":
                    this._valueInc = (ValueIncrement as DynaIterationStatement).Statement;
                    break;
                case "dynaassignmentstatement":
                    this._valueInc = (ValueIncrement as DynaAssignmentStatement).Statement;
                    break;
                case "dynaconditionstatement":
                    this._valueInc = (ValueIncrement as DynaConditionStatement).Statement;
                    break;
                case "dynareturnstatement":
                    this._valueInc = (ValueIncrement as DynaReturnStatement).Statement;
                    break;
                case "dynadeclarationstatement":
                    this._valueInc = (ValueIncrement as DynaDeclarationStatement).Statement;
                    break;
                case "dynacommentstatement":
                    this._valueInc = (ValueIncrement as DynaCommentStatement).Statement;
                    break;
            }
            if (LoopStatements != null)
            {
                foreach (DynaStatement ds in LoopStatements)
                    switch (ds.GetType().Name.ToLower())
                    {
                        case "dynaiterationstatement":
                            csc.Add((ds as DynaIterationStatement).Statement);
                            break;
                        case "dynaassignmentstatement":
                            csc.Add((ds as DynaAssignmentStatement).Statement);
                            break;
                        case "dynaconditionstatement":
                            csc.Add((ds as DynaConditionStatement).Statement);
                            break;
                        case "dynareturnstatement":
                            csc.Add((ds as DynaReturnStatement).Statement);
                            break;
                        case "dynadeclarationstatement":
                            csc.Add((ds as DynaDeclarationStatement).Statement);
                            break;
                        case "dynacommentstatement":
                            csc.Add((ds as DynaCommentStatement).Statement);
                            break;
                    }
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
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
        public void SetLoopCondition(DynaExpression value)
        {
            switch (value.GetType().Name.ToLower())
            {
                case "dynamethodexpression":
                    this._testCondition = (value as DynaMethodExpression).Expression;
                    break;
                case "dynaarraycreateexpression":
                    this._testCondition = (value as DynaArrayCreateExpression).Expression;
                    break;
                case "dynaarrayindexerexpression":
                    this._testCondition = (value as DynaArrayIndexerExpression).Expression;
                    break;
                case "dynaoperatorexpression":
                    this._testCondition = (value as DynaOperatorExpression).Expression;
                    break;
            }
        }
        public void SetValueInit(DynaStatement value)
        {
            switch (value.GetType().Name.ToLower())
            {
                case "dynaiterationstatement":
                    this._initValue = (value as DynaIterationStatement).Statement;
                    break;
                case "dynaassignmentstatement":
                    this._initValue = (value as DynaAssignmentStatement).Statement;
                    break;
                case "dynaconditionstatement":
                    this._initValue = (value as DynaConditionStatement).Statement;
                    break;
                case "dynareturnstatement":
                    this._initValue = (value as DynaReturnStatement).Statement;
                    break;
                case "dynadeclarationstatement":
                    this._initValue = (value as DynaDeclarationStatement).Statement;
                    break;
                case "dynacommentstatement":
                    this._initValue = (value as DynaCommentStatement).Statement;
                    break;
            }
        }
        public void SetValueIncrement(DynaStatement value)
        {
            switch (value.GetType().Name.ToLower())
            {
                case "dynaiterationstatement":
                    this._valueInc = (value as DynaIterationStatement).Statement;
                    break;
                case "dynaassignmentstatement":
                    this._valueInc = (value as DynaAssignmentStatement).Statement;
                    break;
                case "dynaconditionstatement":
                    this._valueInc = (value as DynaConditionStatement).Statement;
                    break;
                case "dynareturnstatement":
                    this._valueInc = (value as DynaReturnStatement).Statement;
                    break;
                case "dynadeclarationstatement":
                    this._valueInc = (value as DynaDeclarationStatement).Statement;
                    break;
                case "dynacommentstatement":
                    this._valueInc = (value as DynaCommentStatement).Statement;
                    break;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-based assignment statements.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaAssignmentStatement : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _varName = "";
        private CodeExpression
            _value = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new virtual CodeAssignStatement Statement
        {
            get
            {
                return new CodeAssignStatement(new CodeVariableReferenceExpression(_varName), _value);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaAssignmentStatement(string VarName, object AssignedValue)
            : this(VarName, new CodePrimitiveExpression(AssignedValue))
        { }
        public DynaAssignmentStatement(string VarName, DynaExpression AssignValue)
            : this(VarName, AssignValue.Expression)
        { }
        private DynaAssignmentStatement(string VarName, CodeExpression AssignValue)
        {
            this._varName = VarName;
            //switch (AssignValue.GetType().Name.ToLower())
            //{
            //    case "dynamethodexpression":
            //        this._value = (AssignValue as DynaMethodExpression).Statement;
            //        break;
            //    case "dynaarraycreateexpression":
            //        this._value = (AssignValue as DynaArrayCreateExpression).Statement;
            //        break;
            //    case "dynaarrayindexerexpression":
            //        this._value = (AssignValue as DynaArrayIndexerExpression).Statement;
            //        break;
            //    case "dynaoperatorexpression":
            //        this._value = (AssignValue as DynaOperatorExpression).Statement;
            //        break;
            //    case "dynaliteralexpression":
            //        this._value = (AssignValue as DynaLiteralExpression).Statement;
            //        break;
            //}
            this._value = AssignValue;
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-based condition statements.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaConditionStatement : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        CodeExpression
            ce;
        CodeStatementCollection
            csc2 = new CodeStatementCollection();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new virtual CodeConditionStatement Statement
        {
            get
            {
                CodeStatement[] csTrue = null;
                if (csc.Count > 0)
                {
                    csTrue = new CodeStatement[csc.Count];
                    for (int i = 0; i < csTrue.Length; i++)
                        csTrue[i] = csc[i];
                }
                CodeStatement[] csFalse = null;
                if (csc2.Count > 0)
                {
                    csFalse = new CodeStatement[csc2.Count];
                    for (int i = 0; i < csFalse.Length; i++)
                        csFalse[i] = csc2[i];
                }
                if (csFalse == null)
                    return new CodeConditionStatement(ce, csTrue);
                else
                    return new CodeConditionStatement(ce, csTrue, csFalse);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DynaConditionStatement(CodeExpression condition)
        {
            //switch (condition.GetType().Name.ToLower())
            //{
            //    case "dynamethodexpression":
            //        this.ce = (condition as DynaMethodExpression).Expression;
            //        break;
            //    case "dynaarraycreateexpression":
            //        this.ce = (condition as DynaArrayCreateExpression).Expression;
            //        break;
            //    case "dynaarrayindexerexpression":
            //        this.ce = (condition as DynaArrayIndexerExpression).Expression;
            //        break;
            //    case "dynaoperatorexpression":
            //        this.ce = (condition as DynaOperatorExpression).Expression;
            //        break;
            //}
            this.ce = condition;
        }
        public DynaConditionStatement(string VarName)
            : this(new CodeVariableReferenceExpression(VarName))
        { }
        public DynaConditionStatement(DynaExpression TestCondition)
            : this(TestCondition.Expression)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Adds a statement expression to be executed if the condition statement evaluates to true.
        /// </summary>
        /// <param name="expr">An object inherited from the AllOneSystem.DynaCode.DynaStatement class.</param>
        public void AddTrueStatement(DynaStatement value)
        {
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
        }
        /// <summary>
        /// Adds a series of statement expressions to be executed if the condition statement evaluates to true.
        /// </summary>
        /// <param name="expr">An array of objects inherited from the AllOneSystem.DynaCode.DynaStatement class.</param>
        public void AddTrueStatement(DynaStatement[] value)
        {
            foreach (DynaStatement ex in value)
                switch (ex.GetType().Name.ToLower())
                {
                    case "dynaiterationstatement":
                        csc.Add((ex as DynaIterationStatement).Statement);
                        break;
                    case "dynaassignmentstatement":
                        csc.Add((ex as DynaAssignmentStatement).Statement);
                        break;
                    case "dynaconditionstatement":
                        csc.Add((ex as DynaConditionStatement).Statement);
                        break;
                    case "dynareturnstatement":
                        csc.Add((ex as DynaReturnStatement).Statement);
                        break;
                    case "dynadeclarationstatement":
                        csc.Add((ex as DynaDeclarationStatement).Statement);
                        break;
                    case "dynacommentstatement":
                        csc.Add((ex as DynaCommentStatement).Statement);
                        break;
                    case "dynamethodexpression":
                        csc.Add((ex as DynaMethodExpression).Statement);
                        break;
                    case "dynaarraycreateexpression":
                        csc.Add((ex as DynaArrayCreateExpression).Statement);
                        break;
                    case "dynaarrayindexerexpression":
                        csc.Add((ex as DynaArrayIndexerExpression).Statement);
                        break;
                    case "dynaoperatorexpression":
                        csc.Add((ex as DynaOperatorExpression).Statement);
                        break;
                }
        }
        /// <summary>
        /// Adds a statement expression to be executed if the condition statement evaluates to false.
        /// </summary>
        /// <param name="expr">An object inherited from the AllOneSystem.DynaCode.DynaStatement class.</param>
        public void AddElseStatement(DynaStatement value)
        {
            switch (value.GetType().Name.ToLower())
            {
                case "dynaiterationstatement":
                    csc2.Add((value as DynaIterationStatement).Statement);
                    break;
                case "dynaassignmentstatement":
                    csc2.Add((value as DynaAssignmentStatement).Statement);
                    break;
                case "dynaconditionstatement":
                    csc2.Add((value as DynaConditionStatement).Statement);
                    break;
                case "dynareturnstatement":
                    csc2.Add((value as DynaReturnStatement).Statement);
                    break;
                case "dynadeclarationstatement":
                    csc2.Add((value as DynaDeclarationStatement).Statement);
                    break;
                case "dynacommentstatement":
                    csc2.Add((value as DynaCommentStatement).Statement);
                    break;
                case "dynamethodexpression":
                    csc2.Add((value as DynaMethodExpression).Statement);
                    break;
                case "dynaarraycreateexpression":
                    csc2.Add((value as DynaArrayCreateExpression).Statement);
                    break;
                case "dynaarrayindexerexpression":
                    csc2.Add((value as DynaArrayIndexerExpression).Statement);
                    break;
                case "dynaoperatorexpression":
                    csc2.Add((value as DynaOperatorExpression).Statement);
                    break;
            }
        }
        /// <summary>
        /// Adds a series of statement expressions to be executed if the condition statement evaluates to false.
        /// </summary>
        /// <param name="expr"></param>
        public void AddElseStatement(DynaStatement[] value)
        {
            foreach (DynaStatement ex in value)
                switch (ex.GetType().Name.ToLower())
                {
                    case "dynaiterationstatement":
                        csc2.Add((ex as DynaIterationStatement).Statement);
                        break;
                    case "dynaassignmentstatement":
                        csc2.Add((ex as DynaAssignmentStatement).Statement);
                        break;
                    case "dynaconditionstatement":
                        csc2.Add((ex as DynaConditionStatement).Statement);
                        break;
                    case "dynareturnstatement":
                        csc2.Add((ex as DynaReturnStatement).Statement);
                        break;
                    case "dynadeclarationstatement":
                        csc2.Add((ex as DynaDeclarationStatement).Statement);
                        break;
                    case "dynacommentstatement":
                        csc2.Add((ex as DynaCommentStatement).Statement);
                        break;
                    case "dynamethodexpression":
                        csc2.Add((ex as DynaMethodExpression).Statement);
                        break;
                    case "dynaarraycreateexpression":
                        csc2.Add((ex as DynaArrayCreateExpression).Statement);
                        break;
                    case "dynaarrayindexerexpression":
                        csc2.Add((ex as DynaArrayIndexerExpression).Statement);
                        break;
                    case "dynaoperatorexpression":
                        csc2.Add((ex as DynaOperatorExpression).Statement);
                        break;
                }
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-based operator (comparison) statements.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaOperatorExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeExpression
            _ceLeft = null,
            _ceRight = null;
        private CodeBinaryOperatorType
            _oper;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeBinaryOperatorExpression Expression
        {
            get { return new CodeBinaryOperatorExpression(_ceLeft, _oper, _ceRight); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DynaOperatorExpression(CodeExpression leftExpression, CodeExpression rightExpression, CodeBinaryOperatorType Operation)
        {
            this._ceLeft = leftExpression;
            this._ceRight = rightExpression;
            this._oper = Operation;
        }
        /// <summary>
        /// Creates an operator expression between two values.
        /// </summary>
        /// <param name="leftVariable">A string value containing the name of the local variable to be used on the left-hand side of the operator expression.</param>
        /// <param name="rightVariable">A string value containing the name of the local variable to be used on the right-hand side of the operator expression.</param>
        /// <param name="Operation">A value from the CodeBinaryOperatorType enum which defines the operation to be performed on the values.</param>
        public DynaOperatorExpression(string leftVariable, string rightVariable, CodeBinaryOperatorType Operation)
            : this(new CodeVariableReferenceExpression(leftVariable), new CodeVariableReferenceExpression(rightVariable), Operation)
        { }
        /// <summary>
        /// Creates an operator expression between two values.
        /// </summary>
        /// <param name="leftVariable">A string value containing the name of the local variable to be used on the left-hand side of the operator expression.</param>
        /// <param name="rightExpression">An object value or expression to be used on the right-hand side of the operator expression.</param>
        /// <param name="Operation">A value from the CodeBinaryOperatorType enum which defines the operation to be performed on the values.</param>
        public DynaOperatorExpression(string leftVariable, object rightExpression, CodeBinaryOperatorType Operation)
            : this(new CodeVariableReferenceExpression(leftVariable), new CodePrimitiveExpression(rightExpression), Operation)
        { }
        /// <summary>
        /// Creates an operator expression between two values.
        /// </summary>
        /// <param name="leftExpression">An object value or expression to be used on the left-hand side of the operator expression.</param>
        /// <param name="rightExpression">An object value or expression to be used on the right-hand side of the operator expression.</param>
        /// <param name="Operation">A value from the CodeBinaryOperatorType enum which defines the operation to be performed on the values.</param>
        public DynaOperatorExpression(object leftExpression, object rightExpression, CodeBinaryOperatorType Operation)
            : this(new CodePrimitiveExpression(leftExpression), new CodePrimitiveExpression(rightExpression), Operation)
        { }
        public DynaOperatorExpression(DynaExpression leftExpression, DynaExpression rightExpression, CodeBinaryOperatorType Operation)
            : this(leftExpression.Expression, rightExpression.Expression, Operation)
        { }
        public DynaOperatorExpression(DynaExpression leftExpression, object rightExpression, CodeBinaryOperatorType Operation)
            : this(leftExpression.Expression, new CodePrimitiveExpression(rightExpression), Operation)
        { }
        public DynaOperatorExpression(DynaExpression leftExpression, string rightVariable, CodeBinaryOperatorType Operation)
            : this(leftExpression.Expression, new CodeVariableReferenceExpression(rightVariable), Operation)
        { }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-based method return statements.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaReturnStatement : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeExpression
            _ce = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new virtual CodeMethodReturnStatement Statement
        {
            get { return new CodeMethodReturnStatement(this._ce); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DynaReturnStatement(CodeExpression value)
        {
            this._ce = value;
        }
        public DynaReturnStatement(DynaExpression ReturnExpression)
            : this(ReturnExpression.Expression)
        { }
        public DynaReturnStatement(object ReturnExpression)
            : this(new CodePrimitiveExpression(ReturnExpression))
        { }
        public DynaReturnStatement(string ReturnExpression)
            : this(new CodeVariableReferenceExpression(ReturnExpression))
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Sets the return value of this statement.
        /// </summary>
        /// <param name="VariableName">A string value containing the name of a local variable.</param>
        public void SetReturnExpression(string VariableName)
        {
            this._ce = new CodeVariableReferenceExpression(VariableName);
        }
        /// <summary>
        /// Sets the return value of this statement.
        /// </summary>
        /// <param name="ReturnExpression">A DynaStatement object whose result will be the return value.</param>
        public void SetReturnExpression(DynaExpression ReturnExpression)
        {
            this._ce = ReturnExpression.Expression;
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-base array definitions.
    /// </summary>
    [Author("Unfried, Michael")]
    public class DynaArrayCreateExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _type = "System.Object[]";
        private CodeExpression
            _size = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets the underlying CodeArrayCreateExpression object contained within this instance.
        /// </summary>
        public new CodeArrayCreateExpression Expression
        {
            get
            {
                return new CodeArrayCreateExpression(new CodeTypeReference(this._type), this._size);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DynaArrayCreateExpression(string dType)
        {
            this._type = dType;
        }
        private DynaArrayCreateExpression(string dType, CodeExpression aSize)
            : this(dType)
        {
            this._size = aSize;
        }
        public DynaArrayCreateExpression(Type DataType, int ArraySize)
            : this(DataType.FullName, ArraySize)
        { }
        public DynaArrayCreateExpression(string DataType, int ArraySize)
            : this(DataType, new CodePrimitiveExpression(ArraySize))
        { }
        public DynaArrayCreateExpression(Type DataType, DynaExpression SizeExpression)
            : this(DataType.FullName, SizeExpression)
        { }
        public DynaArrayCreateExpression(string DataType, DynaExpression SizeExpression)
            : this(DataType)
        {
            this._size = SizeExpression.Expression;
        }
        public DynaArrayCreateExpression(Type DataType, DynaExpression[] ArrayInitializers)
            : this(DataType.FullName, ArrayInitializers)
        { }
        public DynaArrayCreateExpression(string DataType, DynaExpression[] ArrayInitializers)
            : this(DataType)
        {
            foreach (DynaExpression de in ArrayInitializers)
                cec.Add(de.Expression);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SetDataType(string DataType)
        {
            this._type = DataType;
        }
        public void SetDataType(Type DataType)
        {
            SetDataType(DataType.FullName);
        }
        public void SetArraySize(int value)
        {
            this._size = new CodePrimitiveExpression(value);
        }
        public void SetArraySize(DynaExpression value)
        {
            this._size = value.Expression;
        }
        public CodeVariableDeclarationStatement GetDeclarationStatement(string variableName)
        {
            return GetDeclarationStatement(variableName, this._type);
        }
        public CodeVariableDeclarationStatement GetDeclarationStatement(string variableName, Type DataType)
        {
            return GetDeclarationStatement(variableName, DataType.FullName);
        }
        public CodeVariableDeclarationStatement GetDeclarationStatement(string variableName, string DataType)
        {
            if (DataType.Substring(DataType.Length - 2) != "[]")
                DataType += "[]";
            return new CodeVariableDeclarationStatement(new CodeTypeReference(DataType), variableName, this.Expression);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaArrayIndexerExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _varName = "";
        private CodeExpression
            _exp = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeArrayIndexerExpression Expression
        {
            get { return new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(this._varName), new CodeExpression[] { this._exp }); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DynaArrayIndexerExpression(string vName, CodeExpression cEx)
        {
            this._varName = vName;
            this._exp = cEx;
        }
        public DynaArrayIndexerExpression(string VariableName, object IndexExpression)
            : this(VariableName, new CodePrimitiveExpression(IndexExpression))
        { }
        public DynaArrayIndexerExpression(string VariableName, DynaExpression IndexExpression)
            : this(VariableName, IndexExpression.Expression)
        { }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaDeclarationStatement : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _type = "System.Object",
            _name = "";
        private CodeExpression
            _init = null;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeVariableDeclarationStatement Statement
        {
            get
            {
                if (this._init == null)
                    return new CodeVariableDeclarationStatement(new CodeTypeReference(this._type), this._name);
                else
                    return new CodeVariableDeclarationStatement(new CodeTypeReference(this._type), this._name, this._init);
            }
        }
        public string VariableName
        {
            get { return this._name; }
            set { this._name = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DynaDeclarationStatement(string vName, string dType, CodeExpression vInit)
            : this(vName, dType)
        {
            this._init = vInit;
        }
        public DynaDeclarationStatement(string VariableName, string DataType)
        {
            this._name = VariableName;
            this._type = DataType;
        }
        public DynaDeclarationStatement(string VariableName, Type DataType)
            : this(VariableName, DataType.FullName)
        { }
        public DynaDeclarationStatement(string VariableName, string DataType, object InitExpression)
            : this(VariableName, DataType, new CodePrimitiveExpression(InitExpression))
        { }
        public DynaDeclarationStatement(string VariableName, Type DataType, object InitExpression)
            : this(VariableName, DataType.FullName, InitExpression)
        { }
        public DynaDeclarationStatement(string VariableName, string DataType, DynaExpression InitExpression)
            : this(VariableName, DataType, InitExpression.Expression)
        { }
        public DynaDeclarationStatement(string VariableName, Type DataType, DynaExpression InitExpression)
            : this(VariableName, DataType.FullName, InitExpression)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SetVariableDataType(string DataType)
        {
            this._type = DataType;
        }
        public void SetVariableDataType(Type DataType)
        {
            SetVariableDataType(DataType.FullName);
        }
        public void SetInitExpression(object InitExpression)
        {
            this._init = new CodePrimitiveExpression(InitExpression);
        }
        public void SetInitExpression(DynaExpression InitExpression)
        {
            this._init = InitExpression.Expression;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaLiteralExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        //
        private string
            _value = "";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeSnippetExpression Expression
        {
            get { return new CodeSnippetExpression(this._value); }
        }
        public string ExpressionString
        {
            get { return this._value; }
            set { this._value = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaLiteralExpression(string Expression)
        {
            this._value = Expression;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaLiteralStatement : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _value = "";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeSnippetStatement Statement
        {
            get { return new CodeSnippetStatement(this._value); }
        }
        public string StatementString
        {
            get { return this._value; }
            set { this._value = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaLiteralStatement(string Statement)
        {
            this._value = Statement;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaObjectCreateExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _type = "System.Object";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeObjectCreateExpression Expression
        {
            get
            {
                CodeExpression[] args = null;
                if (cec.Count > 0)
                {
                    args = new CodeExpression[cec.Count];
                    for (int i = 0; i < args.Length; i++)
                        args[i] = cec[i];
                    return new CodeObjectCreateExpression(new CodeTypeReference(this._type), args);
                }
                else
                    return new CodeObjectCreateExpression(new CodeTypeReference(this._type), new CodeExpression[] { });
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaObjectCreateExpression(string ValueType)
        {
            this._type = ValueType;
        }
        public DynaObjectCreateExpression(Type ValueType)
            : this(ValueType.FullName)
        { }
        public DynaObjectCreateExpression(string ValueType, DynaExpression[] Parameters)
            : this(ValueType)
        {
            foreach (DynaExpression de in Parameters)
                cec.Add(de.Expression);
        }
        public DynaObjectCreateExpression(Type ValueType, DynaExpression[] Parameters)
            : this(ValueType.FullName, Parameters)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void AddParameter(DynaExpression value)
        {
            cec.Add(value.Expression);
        }
        public void AddParameter(object value)
        {
            cec.Add(new CodePrimitiveExpression(value));
        }
        public void AddParameter(string value)
        {
            cec.Add(new CodeVariableReferenceExpression(value));
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaPrimitive : DynaExpression
    {
        #region Global Objects
        //***************************************************************************
        // Global Variables
        // 
        private object
            _value;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodePrimitiveExpression Expression
        {
            get { return new CodePrimitiveExpression(this._value); }
        }
        public object PrimitiveValue
        {
            get { return this._value; }
            set { this._value = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaPrimitive(object value)
        {
            this._value = value;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaVariableReferenceExpression : DynaExpression
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _name = "";
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new CodeVariableReferenceExpression Expression
        {
            get { return new CodeVariableReferenceExpression(this._name); }
        }
        public string VariableName
        {
            get { return this._name; }
            set { this._name = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaVariableReferenceExpression(string VarName)
        {
            this._name = VarName;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class DynaCommentStatement : DynaStatement
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _text = "";
        private bool
            _doc = false;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new virtual CodeCommentStatement Statement
        {
            get { return new CodeCommentStatement(this._text, this._doc); }
        }
        public string CommentText
        {
            get { return this._text; }
            set { this._text = value; }
        }
        public bool DocumentationComment
        {
            get { return this._doc; }
            set { this._doc = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaCommentStatement(string CommentText)
            : this(CommentText, false)
        { }
        public DynaCommentStatement(string CommentText, bool DocComment)
        {
            this._doc = DocComment;
            this._text = CommentText;
        }
        #endregion
    }
}
