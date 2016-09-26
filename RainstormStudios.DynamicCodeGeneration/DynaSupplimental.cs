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
    public class DynaAttributeDeclaration
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeAttributeDeclaration
            cad;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CodeAttributeDeclaration AttributeDeclaration
        {
            get { return cad; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaAttributeDeclaration(string AttributeName, object AttributeArguement)
            : this(AttributeName, new object[] { AttributeArguement })
        { }
        public DynaAttributeDeclaration(string AttributeName, object[] AttributeArguements)
        {
            CodeAttributeArgument[] caa = new CodeAttributeArgument[AttributeArguements.Length];
            for (int i = 0; i < caa.Length; i++)
                caa[i] = new CodeAttributeArgument(new CodePrimitiveExpression(AttributeArguements[i]));
            cad = new CodeAttributeDeclaration(AttributeName, caa);
        }
        #endregion
    }
    /// <summary>
    /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
    /// </summary>
    public class DynaCast
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeCastExpression
            cce;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets the underlying CodeCastExpression object contained within this instance.
        /// </summary>
        public CodeCastExpression CastExpression
        {
            get { return cce; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
        /// </summary>
        /// <param name="CastType">A string value containing the fully-qualified name of a system data type to cast to.</param>
        /// <param name="Expression">An object value containing an expression literal whose value will be cast.</param>
        public DynaCast(string CastType, object Expression)
            : this(Type.GetType(CastType), Expression)
        { }
        /// <summary>
        /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
        /// </summary>
        /// <param name="CastType">A Type object containing the system data type to cast to.</param>
        /// <param name="Expression">An object value containing an expression literal whose value will be cast.</param>
        public DynaCast(Type CastType, object Expression)
        {
            cce = new CodeCastExpression(new CodeTypeReference(CastType), new CodePrimitiveExpression(Expression));
        }
        /// <summary>
        /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
        /// </summary>
        /// <param name="CastType">A string value containg the fully-qualified name of a system data type to cast to.</param>
        /// <param name="VariableName">A string value containing the name of a local variable whose value will be cast.</param>
        public DynaCast(string CastType, string VariableName)
            : this(Type.GetType(CastType), VariableName)
        { }
        /// <summary>
        /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
        /// </summary>
        /// <param name="CastType">A Type object containing the system data type to cast to.</param>
        /// <param name="VariableName">A string value containing the name of a local variable whose value will be cast.</param>
        public DynaCast(Type CastType, string VariableName)
        {
            cce = new CodeCastExpression(new CodeTypeReference(CastType), new CodeVariableReferenceExpression(VariableName));
        }
        /// <summary>
        /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
        /// </summary>
        /// <param name="CastType">A string value containing the fully-qualified name of a system data type to cast to.</param>
        /// <param name="Expression">A DynaStatement object whose result value will be cast.</param>
        public DynaCast(string CastType, DynaStatement Expression)
            : this(Type.GetType(CastType), Expression)
        { }
        /// <summary>
        /// Provides a simple container and methods for building CodeDOM-based value cast expressions.
        /// </summary>
        /// <param name="CastType">A Type object containing the system data type to cast to.</param>
        /// <param name="Expression">A DynaStatement object whose result value will be cast.</param>
        public DynaCast(Type CastType, DynaExpression Expression)
        {
            cce = new CodeCastExpression(new CodeTypeReference(CastType), Expression.Expression);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Sets the expression value to cast from.
        /// </summary>
        /// <param name="ExpressionLiteral">An object value containing an expression literal whose value will be cast.</param>
        public void SetExpression(object ExpressionLiteral)
        {
            cce.Expression = new CodePrimitiveExpression(ExpressionLiteral);
        }
        /// <summary>
        /// Sets the expression value to cast from.
        /// </summary>
        /// <param name="VariableName">A string value containing the name of a local variable whose value will be cast.</param>
        public void SetExpression(string VariableName)
        {
            cce.Expression = new CodeVariableReferenceExpression(VariableName);
        }
        /// <summary>
        /// Sets the expression value to cast from.
        /// </summary>
        /// <param name="ExpressionStatement">A DynaStatement object whose result value will be cast.</param>
        public void SetExpression(DynaExpression ExpressionStatement)
        {
            cce.Expression = ExpressionStatement.Expression;
        }
        /// <summary>
        /// Sets the system data type to attempt to cast to.
        /// </summary>
        /// <param name="CastType">A string value containing the fully-qualified system data type to cast to.</param>
        public void SetCastType(string CastType)
        {
            SetCastType(Type.GetType(CastType));
        }
        /// <summary>
        /// Sets the system data type to attempt to cast to.
        /// </summary>
        /// <param name="CastType">A System.Type object of the system data type to cast to.</param>
        public void SetCastType(Type CastType)
        {
            cce.TargetType = new CodeTypeReference(CastType);
        }
        #endregion
    }
    public class DynaTryCatchFinally
    {
    }
    public class DynaCatchClause
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private CodeCatchClause
            ccc;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public CodeCatchClause CatchClause
        {
            get { return ccc; }
        }
        public string CatchVarName
        {
            get { return ccc.LocalName; }
            set { ccc.LocalName = value; }
        }
        public CodeStatementCollection CatchStatements
        {
            get { return ccc.Statements; }
        }
        public CodeTypeReference CatchType
        {
            get { return ccc.CatchExceptionType; }
            set { ccc.CatchExceptionType = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynaCatchClause()
        {
            ccc = new CodeCatchClause();
        }
        public DynaCatchClause(string ExVarName)
            : this()
        {
            ccc.LocalName = ExVarName;
        }
        public DynaCatchClause(string ExVarName, string CatchType)
            : this(ExVarName)
        {
            ccc.CatchExceptionType = new CodeTypeReference(CatchType);
        }
        public DynaCatchClause(string ExVarName, string CatchType, DynaExpression[] Statements)
            : this(ExVarName, CatchType)
        {
            foreach (DynaExpression ds in Statements)
                ccc.Statements.Add(ds.Expression);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void SetCatchType(string ExceptionType)
        {
            ccc.CatchExceptionType = new CodeTypeReference(ExceptionType);
        }
        public void AddStatement(DynaExpression Statement)
        {
            ccc.Statements.Add(Statement.Expression);
        }
        #endregion
    }
}
