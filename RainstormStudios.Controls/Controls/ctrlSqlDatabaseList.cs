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
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    public class SqlDatabaseList : System.Windows.Forms.ComboBox
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _connStr;
        private bool
            _noMsg = false;
        //***************************************************************************
        // Public Events
        // 
        public event ExceptionEventHandler PopulateError;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Behavior"),Description("The SQL connection string used to connect to the instance for which the database list will be displayed.")]
        public string ConnectionString
        {
            get { return this._connStr; }
            set
            {
                this._connStr = value;
                // If we're updating the connection string, we need to invalidate any
                //   previously loaded table names.
                if (!this.DesignMode)
                    this.ResetList();
            }
        }
        [Category("Behavior"),Description("Gets or sets a bool value indicating true to prevent the drop-down box from showing a message box when an error occurs. Errors can still be captured with the PopulateError event."),DefaultValue(false)]
        public bool SupressErrorMessages
        {
            get { return this._noMsg; }
            set { this._noMsg = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SqlDatabaseList()
            : base()
        {
            if (!this.DesignMode)
                this.ResetList();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ResetList()
        {
            this.BeginUpdate();
            this.Items.Clear();
            this.Items.Add("<default>");
            this.SelectedIndex = 0;
            this.EndUpdate();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void OnPopulateError(ExceptionEventArgs e)
        {
            if (this.PopulateError != null)
                this.PopulateError.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Override Methods
        // 
        protected override void OnDropDown(EventArgs e)
        {
            if (!this.DesignMode && !string.IsNullOrEmpty(this._connStr) && this.Items.Count < 2)
            {
                Cursor.Current = Cursors.WaitCursor;
                //this.ResetList();
                string connStr = this._connStr;
                try
                {
                    if (connStr.ToLower().Contains("provider="))
                    {
                        OleDbConnection conOle = new OleDbConnection(connStr);
                        conOle.Open();
                        OleDbCommand cmdOle = new OleDbCommand("SELECT * FROM dbo.sysdatabases", conOle);
                        OleDbDataReader rdrOle = cmdOle.ExecuteReader();
                        while (rdrOle.Read())
                            this.Items.Add(rdrOle["name"].ToString());
                        conOle.Close();
                        rdrOle.Dispose();
                        cmdOle.Dispose();
                        conOle.Dispose();
                    }
                    else
                    {
                        connStr += "Initial Catalog=master;";
                        SqlConnection conSql = new SqlConnection(connStr);
                        conSql.Open();
                        SqlCommand cmdSql = new SqlCommand("SELECT name FROM [dbo].[sysdatabases] ORDER BY 1 ASC", conSql);
                        SqlDataReader rdrSql = cmdSql.ExecuteReader();
                        while (rdrSql.Read())
                            this.Items.Add(rdrSql[0].ToString());
                        conSql.Close();
                        rdrSql.Dispose();
                        cmdSql.Dispose();
                        conSql.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    if (!this._noMsg)
                    {
                        #region DEBUG Output
#if DEBUG
                        MessageBox.Show(ex.ToString(), "Fill SQL Database List");
#else
                        MessageBox.Show("Unable to retrieve list of databases from SQL server.\n\n" + ex.Message + "\n\nApplication Version: " + Application.ProductVersion, "Error");
#endif
                        #endregion
                    }
                    this.OnPopulateError(
                        new ExceptionEventArgs(ex));
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            base.OnDropDown(e);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.SelectAll();
        }
        #endregion
    }
}
