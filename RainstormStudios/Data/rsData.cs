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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

namespace RainstormStudios.Data
{
    /// <summary>
    /// Provides static methods for working with database connection strings and data type conversions.  This class is sealed.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class rsData
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Attempts to determine the proper value of the RainstormStudios.AdoProviderType enumeration based on the given connection string.
        /// </summary>
        /// <param name="ConnString">The connnectino string to parse.</param>
        /// <returns>A value from the RainstormStudios.AdoProviderType enumeration.</returns>
        public static AdoProviderType ParseProviderType(string ConnString)
        {
            return RainstormStudios.rsString.ParseProviderType(ConnString);
        }
        public static DbConnection GetOpenConnection(AdoProviderType providerType, string connStr)
        { return rsData.GetOpenConnection(providerType, connStr, 5, 1200); }
        public static DbConnection GetOpenConnection(AdoProviderType providerType, string connStr, int retryCount, int retryDelay)
        {
            DbConnection conn = null;
            switch (providerType)
            {
                case AdoProviderType.SqlProvider:
                    conn = new SqlConnection(connStr);
                    break;
                case AdoProviderType.OleProvider:
                    conn = new OleDbConnection(connStr);
                    break;
                case AdoProviderType.OdbcProvider:
                    conn = new OdbcConnection(connStr);
                    break;
                default:
                    throw new Exception("This provider type is not supported.");
            }


            if (conn != null)
            {
                conn.Open();
                int attemptCount = 0;
                while (conn.State!=ConnectionState.Open && ++attemptCount < retryCount)
                {
                    System.Threading.Thread.Sleep(retryDelay);
                    conn.Open();
                }
                if (conn.State != ConnectionState.Open)
                    throw new Exception("Unable to open connection using specified connection string.");
            }
            return conn;
        }
        /// <summary>
        /// Converts a system data type to the OdbcType enum member that most closely matches.
        /// </summary>
        /// <param name="value">A Type object indicating the data type to convert.</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDataType(Type value)
        {
            return rsData.GetSqlDataType(value.Name);
        }
        /// <summary>
        /// Converts a system data type to the OdbcType enum member that most closely matches.
        /// </summary>
        /// <param name="value">A string value containing the name of the system data type to convert.</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDataType(string value)
        {
            SqlDbType retVal = SqlDbType.VarChar;
            string tName = (value.IndexOf('.') > -1) ? value.Substring(value.LastIndexOf('.') + 1) : value;
            switch (tName)
            {
                case "String":
                    retVal = SqlDbType.VarChar;
                    break;
                case "Boolean":
                    retVal = SqlDbType.Bit;
                    break;
                case "Byte":
                    retVal = SqlDbType.TinyInt;
                    break;
                case "Int16":
                    retVal = SqlDbType.SmallInt;
                    break;
                case "Int32":
                    retVal = SqlDbType.Int;
                    break;
                case "Int64":
                    retVal = SqlDbType.BigInt;
                    break;
                case "Single":
                    retVal = SqlDbType.Real;
                    break;
                case "Double":
                    retVal = SqlDbType.Float;
                    break;
                case "DateTime":
                    retVal = SqlDbType.DateTime;
                    break;
                case "Decimal":
                    retVal = SqlDbType.Decimal;
                    break;
                case "Char":
                    retVal = SqlDbType.VarChar;
                    break;
                case "Image":
                    retVal = SqlDbType.Image;
                    break;
                case "Byte[]":
                    retVal = SqlDbType.Binary;
                    break;
                case "Guid":
                    retVal = SqlDbType.UniqueIdentifier;
                    break;
                default:
                    retVal = SqlDbType.Variant;
                    break;
            }
            return retVal;
        }
        /// <summary>
        /// Converts a system data type to the OdbcType enum member that most closely matches.
        /// </summary>
        /// <param name="value">A Type object indicating the data type to convert.</param>
        /// <returns></returns>
        public static OleDbType GetOleDataType(Type value)
        {
            return rsData.GetOleDataType(value.Name);
        }
        /// <summary>
        /// Converts a system data type to the OdbcType enum member that most closely matches.
        /// </summary>
        /// <param name="value">A String value containing the name of the system data type to convert.</param>
        /// <returns></returns>
        public static OleDbType GetOleDataType(string value)
        {
            OleDbType retVal = OleDbType.IUnknown;
            string tName = (value.IndexOf('.') > -1) ? value.Substring(value.LastIndexOf('.') + 1) : value;
            switch (tName)
            {
                case "String":
                    retVal = OleDbType.VarChar;
                    break;
                case "Boolean":
                    retVal = OleDbType.Boolean;
                    break;
                case "Byte":
                    retVal = OleDbType.TinyInt;
                    break;
                case "Int16":
                    retVal = OleDbType.SmallInt;
                    break;
                case "Int32":
                    retVal = OleDbType.Integer;
                    break;
                case "Int64":
                    retVal = OleDbType.BigInt;
                    break;
                case "Single":
                    retVal = OleDbType.Single;
                    break;
                case "Double":
                    retVal = OleDbType.Double;
                    break;
                case "DateTime":
                    retVal = OleDbType.Date;
                    break;
                case "Decimal":
                    retVal = OleDbType.Decimal;
                    break;
                case "Char":
                    retVal = OleDbType.VarChar;
                    break;
                case "Image":
                    retVal = OleDbType.Binary;
                    break;
                case "Byte[]":
                    retVal = OleDbType.Binary;
                    break;
                case "Guid":
                    retVal = OleDbType.Guid;
                    break;
                default:
                    retVal = OleDbType.Variant;
                    break;
            }
            return retVal;
        }
        /// <summary>
        /// Converts a system data type to the OdbcType enum member that most closely matches.
        /// </summary>
        /// <param name="value">A Type object indicating the data type to convert.</param>
        /// <returns></returns>
        public static OdbcType GetOdbcDataType(Type value)
        {
            return rsData.GetOdbcDataType(value.Name);
        }
        /// <summary>
        /// Converts a system data type to the OdbcType enum member that most closely matches.
        /// </summary>
        /// <param name="value">A String value containing the name of the system data type to convert.</param>
        /// <returns></returns>
        public static OdbcType GetOdbcDataType(string value)
        {
            OdbcType retVal = OdbcType.VarChar;
            string tName = (value.IndexOf('.') > -1) ? value.Substring(value.LastIndexOf('.') + 1) : value;
            switch (tName)
            {
                case "String":
                    retVal = OdbcType.VarChar;
                    break;
                case "Boolean":
                    retVal = OdbcType.Bit;
                    break;
                case "Byte":
                    retVal = OdbcType.TinyInt;
                    break;
                case "Int16":
                    retVal = OdbcType.SmallInt;
                    break;
                case "Int32":
                    retVal = OdbcType.Int;
                    break;
                case "Int64":
                    retVal = OdbcType.BigInt;
                    break;
                case "Single":
                    retVal = OdbcType.Real;
                    break;
                case "Double":
                    retVal = OdbcType.Double;
                    break;
                case "DateTime":
                    retVal = OdbcType.DateTime;
                    break;
                case "Decimal":
                    retVal = OdbcType.Decimal;
                    break;
                case "Char":
                    retVal = OdbcType.VarChar;
                    break;
                case "Image":
                    retVal = OdbcType.Image;
                    break;
                case "Byte[]":
                    retVal = OdbcType.Binary;
                    break;
                case "Guid":
                    retVal = OdbcType.UniqueIdentifier;
                    break;
                default:
                    retVal = OdbcType.VarChar;
                    break;
            }
            return retVal;
        }
        /// <summary>
        /// Parses a given QueryString to determine the base table name specified in the query.
        /// </summary>
        /// <param name="QueryString">The query to parse.</param>
        /// <returns>A string value containing the base table name from the query with all leading/trailing spaces removed.</returns>
        public static string[] ParseBaseTableName(string QueryString)
        {
            return rsString.ParseBaseTableName(QueryString, false);
        }
        /// <summary>
        /// Parses a given QueryString to determine the base table name specified in the query.
        /// </summary>
        /// <param name="QueryString">The query to parse.</param>
        /// <param name="RemoveQualifier">If set to true, all table qualifiers are removed from the returned value.</param>
        /// <returns>A string value containing the base table name from the query with all leading/trailing spaces removed.</returns>
        public static string[] ParseBaseTableName(string QueryString, bool RemoveQualifier)
        {
            return rsString.ParseBaseTableName(QueryString, RemoveQualifier);
        }
        /// <summary>
        /// Checks the given connection string for validity by attempting to open a connection with it.
        /// </summary>
        /// <param name="ConnectionString">The connection string to test.</param>
        /// <param name="ProviderArg">An integer (as enumerated in AosCommon.ProviderType) indicating the ADO.NET adapter to use.</param>
        /// <returns>A true or false value indicating whether or not the test was successful.</returns>
        public static bool CheckConnectionString(string ConnectionString, AdoProviderType ProviderArg)
        {
            #region Status Output
#if DEBUG
            Console.WriteLine("Checking connection string validity...");
#endif
            #endregion
            try
            {
                string ServerVersion = "", ConnState = "";
                switch (ProviderArg)
                {
                    case AdoProviderType.Auto:
                        return rsData.CheckConnectionString(ConnectionString, rsData.ParseProviderType(ConnectionString));
                    case AdoProviderType.SqlProvider:
                        using (SqlConnection conTest = new SqlConnection(ConnectionString))
                        {
                            conTest.Open();
                            ServerVersion = conTest.ServerVersion;
                            ConnState = conTest.State.ToString();
                            conTest.Close();
                        }
                        break;
                    case AdoProviderType.OleProvider:
                        #region Status Output
#if DEBUG
                        Console.WriteLine("OleDb provider found.  Creating connection...");
#endif
                        #endregion
                        using (OleDbConnection conOle = new OleDbConnection(ConnectionString))
                        {
                            conOle.Open();
                            ServerVersion = conOle.ServerVersion;
                            ConnState = conOle.State.ToString();
                            conOle.Close();
                        }
                        break;
                    case AdoProviderType.DB2Provider:
                        //using (DB2Connection conDb2 = new DB2Connection(ConnectionString))
                        //{
                        //    conDb2.Open();
                        //    ServerVersion = conDb2.ServerVersion;
                        //    ConnState = conDb2.State.ToString();
                        //    conDb2.Close();
                        //}
                        //break;
                        throw new ArgumentException("DB2 Connection Types are not supported by this function.");
                    case AdoProviderType.OdbcProvider:
                        using (OdbcConnection conOdbc = new OdbcConnection(ConnectionString))
                        {
                            conOdbc.Open();
                            ServerVersion = conOdbc.ServerVersion;
                            ConnState = conOdbc.State.ToString();
                            conOdbc.Close();
                        }
                        break;
                    default:
                        // We got something unhandled, so inform the user & break.
                        // This *should* only ever occur if there is a an error in the calling program.
                        throw new Exception("Invalid provider type.");
                }
                #region Status Output
#if DEBUG
                Console.WriteLine("Connection string valid.  Returning true...");
#endif
                #endregion
                return true;
            }
            catch (System.Data.SqlClient.SqlException)
            { return false; }
            catch (System.Data.OleDb.OleDbException)
            { return false; }
            catch (System.Data.Odbc.OdbcException)
            { return false; }
            //catch (IBM.Data.DB2.DB2Exception)
            //{ return false; }
            catch (InvalidOperationException)
            { throw; }
            catch (Exception)
            { throw; }
        }
        /// <summary>
        /// Parses through the given string array, removing all comments and blank space.
        /// </summary>
        /// <param name="QueryLines">A string array containing lines of SQL query language.</param>
        /// <returns>A string value containing the given query in a single line with all comments removed.</returns>
        public static string DeCommentQuery(string[] QueryLines)
        {
            return rsString.DeCommentQuery(QueryLines);
        }
        public static string DeCommentQuery(string[] QueryLines, bool singleLine)
        {
            return rsString.DeCommentQuery(QueryLines, singleLine);
        }
        /// <summary>
        /// Searches the given connection string an attempts to remove the "Provider=" keyword from it.
        /// </summary>
        /// <param name="ConnectionString">An ADO connection string.</param>
        /// <returns>A string value containing the given connection string without the "Provder=" keyword.</returns>
        public static string RemoveProviderKeyword(string ConnectionString)
        {
            return rsString.RemoveProviderKeyword(ConnectionString);
        }
        /// <summary>
        /// Returns a string containing an SQL Insert query that can be used to add a new row to a table.
        /// </summary>
        /// <param name="currentRow">A pre-populated DataRow containing all the fields and values that will be inserted into the query string.</param>
        /// <returns>A string value containing a SQL INSERT command.</returns>
        public static string GetInsertQuery(DataRow currentRow)
        {
            return GetInsertQuery(currentRow, currentRow.Table.TableName);
        }
        /// <summary>
        /// Returns a string containing an SQL Insert query that can be used to add a new row to a table.
        /// </summary>
        /// <param name="currentRow">A pre-populated DataRow containing all the fields and values that will be inserted into the query string.</param>
        /// <param name="tableName">A string value containing the name of the table on the server that the INSERT command will send data to.</param>
        /// <returns>A string value containing a SQL INSERT command.</returns>
        public static string GetInsertQuery(DataRow currentRow, string tableName)
        {
            // We're going to build the SQL Insert statement in two
            //   pieces, so that the ForEach loop can do it for us.
            StringBuilder sbQry_Insert = new StringBuilder("INSERT INTO " + tableName + " (");
            StringBuilder sbQry_Values = new StringBuilder(") VALUES (");
            //string qry_Insert = "INSERT INTO " + tableName + " (";
            //string qry_Values = ") VALUES (";
            // Then, we parse through all available fields in the
            //   DataRow by creating a DataColumn object referenced
            //   to the "Columns" property of the DataRow's
            //   parent table in the DataSet.
            foreach (DataColumn rowField in currentRow.Table.Columns)
            {
                //qry_Insert += rowField.ColumnName + ", ";
                //qry_Values += "'" + currentRow[rowField.Ordinal].ToString().Replace("'", "''") + "', ";
                sbQry_Insert.Append(rowField.ColumnName + ", ");
                sbQry_Values.Append("'" + currentRow[rowField.Ordinal].ToString().Replace("'", "''") + "', ");
            }

            // Once we've captured all our fields, we return the QueryString
            //return qry_Insert.Substring(0, qry_Insert.Length - 2) + qry_Values.Substring(0, qry_Values.Length - 2) + ")";
            return sbQry_Insert.ToString().Replace(',', ' ') + sbQry_Values.ToString().Replace(',', ' ') + ")";
        }
        public static DbCommand GetInsertCommand(DataRow schemaRow, AdoProviderType providerID)
        {
            return rsData.GetInsertCommand(schemaRow, schemaRow.Table.TableName, providerID);
        }
        public static DbCommand GetInsertCommand(DataRow schemaRow, string tableName, AdoProviderType providerID)
        {
            if (schemaRow.Table == null)
                throw new ArgumentException("DataRow must belong to an existing DataTable with populated columns.", "schemaRow");

            DbCommand cmdObj = null;
            switch (providerID)
            {
                case AdoProviderType.SqlProvider:
                    cmdObj = new SqlCommand();
                    break;
                case AdoProviderType.OleProvider:
                    cmdObj = new OleDbCommand();
                    break;
                case AdoProviderType.OdbcProvider:
                    cmdObj = new OdbcCommand();
                    break;
                case AdoProviderType.DB2Provider:
                    throw new ArgumentException("Dynamic insert command generation is not supported for DB2 providers.", "providerID");
                default:
                    throw new ArgumentException("The supplied provider ID is invalid for this function.", "providerID");
            }
            // We're going to build the SQL Insert statement in two
            //   pieces, so that the ForEach loop can do it for us.
            StringBuilder sbQry_Insert = new StringBuilder("INSERT INTO " + tableName + " (");
            StringBuilder sbQry_Values = new StringBuilder(") VALUES (");
            //string qry_Insert = "INSERT INTO " + tableName + " (";
            //string qry_Values = ") VALUES (";
            // Then, we parse through all available fields in the
            //   DataRow by creating a DataColumn object referenced
            //   to the "Columns" property of the DataRow's
            //   parent table in the DataSet.
            foreach (DataColumn rowField in schemaRow.Table.Columns)
            {
                //qry_Insert += rowField.ColumnName + ", ";
                //qry_Values += "@" + rowField.ColumnName + ", ";
                sbQry_Insert.Append(rowField.ColumnName + ", ");
                sbQry_Values.Append("@" + rowField.ColumnName + ", ");
                switch (providerID)
                {
                    case AdoProviderType.SqlProvider:
                        {
                            SqlParameter p = ((SqlCommand)cmdObj).Parameters.Add("@" + rowField.ColumnName, rsData.GetSqlDataType(rowField.DataType), rowField.MaxLength);
                            p.SourceColumn = rowField.ColumnName; p.SourceVersion = DataRowVersion.Original;
                        }
                        break;
                    case AdoProviderType.OleProvider:
                        {
                            OleDbParameter p = ((OleDbCommand)cmdObj).Parameters.Add("@" + rowField.ColumnName, rsData.GetOleDataType(rowField.DataType), rowField.MaxLength);
                            p.SourceColumn = rowField.ColumnName; p.SourceVersion = DataRowVersion.Original;
                        }
                        break;
                    case AdoProviderType.OdbcProvider:
                        {
                            OdbcParameter p = ((OdbcCommand)cmdObj).Parameters.Add("@" + rowField.ColumnName, rsData.GetOdbcDataType(rowField.DataType), rowField.MaxLength);
                            p.SourceColumn = rowField.ColumnName; p.SourceVersion = DataRowVersion.Original;
                        }
                        break;
                }
            }

            // Once we've captured all our fields, we return the QueryString
            //cmdObj.CommandText = qry_Insert.Substring(0, qry_Insert.Length - 2) + qry_Values.Substring(0, qry_Values.Length - 2) + ")";
            cmdObj.CommandText = sbQry_Insert.ToString().TrimEnd(',', ' ') + sbQry_Values.ToString().TrimEnd(',', ' ') + ")";
            return cmdObj;
        }
        /// <summary>
        /// Returns a string containing an SQL Update query that can be used to update values in this objects connected table.
        /// </summary>
        /// <param name="currentRow">A pre-populated DataRow containing all the fields and values that will be updated with the query string.</param>
        /// <param name="KeyField">A string value indicating which field will define the field to use as the local key.</param>
        /// <param name="KeyValue">A object value linking to the KeyField indicating which row will be updated in the table.</param>
        /// <returns>A string value containing an SQL UPDATE command.</returns>
        public static string GetUpdateQuery(DataRow currentRow, string KeyField, object KeyValue)
        {
            return GetUpdateQuery(currentRow, currentRow.Table.TableName, KeyField, KeyValue);
        }
        /// <summary>
        /// Returns a string containing an SQL Update query that can be used to update values in this objects connected table.
        /// </summary>
        /// <param name="currentRow">A pre-populated DataRow containing all the fields and values that will be updated with the query string.</param>
        /// <param name="tableName">A string value containing the name of the table on the server that the INSERT command will send data to.</param>
        /// <param name="KeyField">A string value indicating which field will define the field to use as the local key.</param>
        /// <param name="KeyValue">A object value linking to the KeyField indicating which row will be updated in the table.</param>
        /// <returns>A string value containing an SQL UPDATE command.</returns>
        public static string GetUpdateQuery(DataRow currentRow, string tableName, string KeyField, object KeyValue)
        {
            // First, we have to tell SQL we're doing an update and give it the table name.
            //string qry_Update = "UPDATE " + tableName + " SET ";
            StringBuilder sbQry_Update = new StringBuilder("UPDATE " + tableName + " SET ");
            // Now we parse through the currentRow object
            foreach (DataColumn rowField in currentRow.Table.Columns)
                sbQry_Update.Append("[" + rowField.ColumnName + "] = '" + currentRow[rowField.ColumnName].ToString().Replace("'", "''") + "', ");
                //qry_Update += "[" + rowField.ColumnName + "] = '" + currentRow[rowField.ColumnName].ToString().Replace("'", "''") + "', ";

            // Then, we have to add the 'WHERE' keyword to define which record to update.
            //qry_Update = qry_Update.Substring(0, qry_Update.Length - 2) + " WHERE " + KeyField + " = '" + Convert.ToString(KeyValue) + "'";
            string qry_Update = sbQry_Update.ToString().TrimEnd(',', ' ') + " WHERE " + KeyField + " = '" + Convert.ToString(KeyValue) + "'";

            // Once we've captured all the fields, we return the QueryString.
            return qry_Update;
        }
        public static DbCommand GetUpdateCommand(DataRow schemaRow, string keyField, AdoProviderType providerID)
        {
            return GetUpdateCommand(schemaRow, schemaRow.Table.TableName, keyField, providerID);
        }
        public static DbCommand GetUpdateCommand(DataRow schemaRow, string tableName, string keyField, AdoProviderType providerID)
        {
            if (schemaRow.Table == null)
                throw new ArgumentException("DataRow must belong to an existing DataTable with populated columns.", "schemaRow");

            DbCommand cmdObj = null;
            switch (providerID)
            {
                case AdoProviderType.SqlProvider:
                    cmdObj = new SqlCommand();
                    break;
                case AdoProviderType.OleProvider:
                    cmdObj = new OleDbCommand();
                    break;
                case AdoProviderType.OdbcProvider:
                    cmdObj = new OdbcCommand();
                    break;
                case AdoProviderType.DB2Provider:
                    throw new ArgumentException("Dynamic insert command generation is not supported for DB2 providers.", "providerID");
                default:
                    throw new ArgumentException("The supplied provider ID is invalid for this function.", "providerID");
            }

            //string updCmd = "UPDATE " + tableName + " SET ";
            StringBuilder sbUpdCmd = new StringBuilder("UPDATE " + tableName + " SET ");
            using (DataTable dtSchema = new DataTable(tableName))
            {
                foreach (DataColumn dc in schemaRow.Table.Columns)
                {
                    switch (providerID)
                    {
                        case AdoProviderType.SqlProvider:
                            {
                                SqlParameter p = ((SqlCommand)cmdObj).Parameters.Add("@" + dc.ColumnName, rsData.GetSqlDataType(dc.DataType), dc.MaxLength);
                                p.SourceColumn = dc.ColumnName; p.SourceVersion = DataRowVersion.Original;
                            }
                            break;
                        case AdoProviderType.OleProvider:
                            {
                                OleDbParameter p = ((OleDbCommand)cmdObj).Parameters.Add("@" + dc.ColumnName, rsData.GetOleDataType(dc.DataType), dc.MaxLength);
                                p.SourceColumn = dc.ColumnName; p.SourceVersion = DataRowVersion.Original;
                            }
                            break;
                        case AdoProviderType.OdbcProvider:
                            {
                                OdbcParameter p = ((OdbcCommand)cmdObj).Parameters.Add("@" + dc.ColumnName, rsData.GetOdbcDataType(dc.DataType), dc.MaxLength);
                                p.SourceColumn = dc.ColumnName; p.SourceVersion = DataRowVersion.Original;
                            }
                            break;
                    }
                    //updCmd += dc.ColumnName + " = @" + dc.ColumnName;
                    sbUpdCmd.Append(dc.ColumnName + " = @" + dc.ColumnName);
                }
            }
            //cmdObj.CommandText = updCmd + " WHERE " + keyField + " = @" + keyField;
            cmdObj.CommandText = sbUpdCmd.ToString() + " WHERE " + keyField + " = @" + keyField;
            return cmdObj;
        }
        /// <summary>
        /// Parses a given query and tries to determine the names of the fields to be retrieved.  Those field names are then returned as a string array.
        /// </summary>
        /// <param name="QueryString">An initialized string value containing the command string to parse for field names.</param>
        /// <returns>Returns a string array containing one element for each field names parse from the command string.</returns>
        public static string[] GetFields(string QueryString)
        {
            return GetFields(QueryString, QueryFieldParseOption.DestinationName);
        }
        /// <summary>
        /// Parses a given query and tries to determine the names of the fields to be retrieved.  Those field names are then returned as a string array.
        /// </summary>
        /// <param name="QueryString">An initialized string value containing the command string to parse for field names.</param>
        /// <param name="option">A value from the QueryFieldParseOption enumeration which defines how to parse the query.</param>
        /// <returns>Returns a string array containing one element for each field names parse from the command string.</returns>
        public static string[] GetFields(string QueryString, QueryFieldParseOption option)
        {
            if (string.IsNullOrEmpty(QueryString))
                return null;

            // Get a string value containing only the field names from the query
            string qryFields = "";
            if (QueryString.ToUpper().IndexOf("DISTINCT") < 0)
                qryFields = QueryString.Substring(QueryString.ToUpper().IndexOf("SELECT") + 7, QueryString.ToUpper().IndexOf("FROM") - (QueryString.ToUpper().IndexOf("SELECT") + 7));
            else
                qryFields = QueryString.Substring(QueryString.ToUpper().IndexOf("DISTINCT") + 9, QueryString.ToUpper().IndexOf("FROM") - (QueryString.ToUpper().IndexOf("DISTINCT") + 9));

            // Now, parse the fields into a string array.
            string[] fields = qryFields.Split(',');

            // Create an string array to hold the 'groomed' field names.
            string[] foundFields = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                string fieldName = "";
                if (fields[i].ToUpper().IndexOf(" AS ") < 0)
                    fieldName = fields[i];
                else
                    switch (option)
                    {
                        case QueryFieldParseOption.None:
                            fieldName = fields[i];
                            break;
                        case QueryFieldParseOption.DestinationName:
                            fieldName = fields[i].Substring(fields[i].ToUpper().IndexOf(" AS ") + 4);
                            break;
                        case QueryFieldParseOption.OriginalName:
                            fieldName = fields[i].Substring(0, fields[i].ToUpper().IndexOf(" AS "));
                            break;
                    }
                foundFields[i] = fieldName.Trim();
            }

            // And, finally, return the completed array of field names.
            return foundFields;
        }
        public static string GetDataSource(string connectionString)
        {
            int iStart = connectionString.IndexOf('=', connectionString.ToLower().IndexOf("data source="));
            if (iStart < 0)
                throw new ArgumentException("Provided connection string is not well formed.  No DataSource parameter found.", "connectionString");

            int iEnd = connectionString.IndexOf(';', iStart);
            return connectionString.Substring(iStart + 1, iEnd - (iStart + 1));
        }
        #endregion
    }
}
