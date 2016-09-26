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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.Common;
using RainstormStudios.Data;

namespace RainstormStudios.Forms
{
    public partial class frmStringBuilder : Form
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private DataTable dtProvider;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string ConnectionString
        {
            get { return BuildString(); }
        }
        public AdoProviderType AdoProvider
        {
            get { return rsString.ParseProviderType(this.ConnectionString); }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public frmStringBuilder()
        {
            InitializeComponent();
            InitProviderList();
        }
        public frmStringBuilder(AdoProviderType LockProviderId)
            : this()
        {
            lstProvider.Enabled = false;
            lstProvider.SelectedIndex = (int)LockProviderId;
            Application.DoEvents();
            txtIP.Focus();
        }
        #endregion

        #region Private Functions
        //***************************************************************************
        // Private Functions
        // 
        private void InitProviderList()
        {
            Cursor.Current = Cursors.WaitCursor;

            dtProvider = new DataTable("Providers");
            DataRow thisRow;
            dtProvider.Columns.Add("DisplayName");
            dtProvider.Columns.Add("InternalName");
            dtProvider.Columns.Add("ExtraParams");
            dtProvider.Columns.Add("ExtendProp");

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "SQL Driver";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "IBM DB2 Driver";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft Jet 4.0 OLE DB";
            thisRow["InternalName"] = "Microsoft.Jet.OLEDB.4.0";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for ODBC";
            thisRow["InternalName"] = "MSDASQL";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for OLAP 8.0";
            thisRow["InternalName"] = "MSOLAP";
            thisRow["ExtraParams"] = "Client Cache Size=25;Auto Synch Period=10000";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for Oracle";
            thisRow["InternalName"] = "MSDAORA";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for DB2";
            thisRow["InternalName"] = "DB2OLEDB";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for SQL";
            thisRow["InternalName"] = "SQLOLEDB";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for DataShape";
            thisRow["InternalName"] = "MSDataShape";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for MySQL";
            thisRow["InternalName"] = "MsSQLProv";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for Visual FoxPro";
            thisRow["InternalName"] = "vfpoledb.1";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for dBase IV";
            thisRow["InternalName"] = "Microsoft.Jet.OLEDB.4.0";
            thisRow["ExtendProp"] = "dBase IV";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Provider for Paradox";
            thisRow["InternalName"] = "Microsoft.Jet.OLEDB.4.0";
            thisRow["ExtendProp"] = "Paradox 5.x";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft OLE DB Simple Provider";
            thisRow["InternalName"] = "MSDAOSP";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "Microsoft Office 2007 Excel Provider";
            thisRow["InternalName"] = "Microsoft.ACE.OLEDB.12.0";
            thisRow["ExtendProp"] = "Excel 12.0";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "IBM OLE DB Provider for DB2";
            thisRow["InternalName"] = "IBMDADB2";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "SQL Native Client";
            thisRow["InternalName"] = "SQLNCLI";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Microsoft Access";
            thisRow["InternalName"] = "Microsoft Access Driver (*.mdb)";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for DB2";
            thisRow["InternalName"] = "IBM DB2 ODBC DRIVER";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Oracle";
            thisRow["InternalName"] = "Microsoft ODBC for Oracle";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Informix-CLI 2.5";
            thisRow["InternalName"] = "Informix-CLI 2.5 (32 Bit)";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Informix 3.30";
            thisRow["InternalName"] = "INFORMIX 3.30 32 BIT";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC 2.50 Driver for MySQL";
            thisRow["InternalName"] = "mySQL";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC 3.51 Driver for MySQL";
            thisRow["InternalName"] = "MySQL ODBC 3.51 Driver";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Visual FoxPro";
            thisRow["InternalName"] = "Microsoft Visual FoxPro Driver";
            thisRow["ExtraParams"] = "Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Paradox v5.x";
            thisRow["InternalName"] = "Microsoft Paradox Driver (*.db)";
            thisRow["ExtraParams"] = "DriverID=538;Fil=Paradox 5.X;CollatingSquence=ASCII";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for Paradox v7.x";
            thisRow["InternalName"] = "@MSDASQL.1";
            thisRow["ExtraParams"] = "Persist Security Info=False;Mode=Read;";
            thisRow["ExtendProp"] = "DSN=Paradox;DriverID=538;Fil=Paradox 7.X;MaxBufferSize=2048;PageTimeout=600;";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC Driver for AS/400";
            thisRow["InternalName"] = "Client Access ODBC Driver (32-Bit)";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC for System DSN";
            dtProvider.Rows.Add(thisRow);

            thisRow = dtProvider.NewRow();
            thisRow["DisplayName"] = "ODBC for File DSN";
            dtProvider.Rows.Add(thisRow);

            // Add the provider to the listbox.
            foreach (DataRow dr in dtProvider.Rows)
                lstProvider.Items.Add(dr["DisplayName"].ToString());
            lstProvider.SelectedIndex = 0;

            Cursor.Current = Cursors.Default;
        }
        private void FillDatabaseList()
        {
            Cursor.Current = Cursors.WaitCursor;
            drpInitCatalog.Items.Clear();
            string connStr = this.BuildString();
            try
            {
                if (dtProvider.Rows[lstProvider.SelectedIndex]["DisplayName"].ToString().ToLower().IndexOf("ole") > -1)
                {
                    OleDbConnection conOle = new OleDbConnection(connStr);
                    conOle.Open();
                    OleDbCommand cmdOle = new OleDbCommand("SELECT * FROM dbo.sysdatabases", conOle);
                    OleDbDataReader rdrOle = cmdOle.ExecuteReader();
                    while (rdrOle.Read())
                        drpInitCatalog.Items.Add(rdrOle["name"].ToString());
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
                        drpInitCatalog.Items.Add(rdrSql[0].ToString());
                    conSql.Close();
                    rdrSql.Dispose();
                    cmdSql.Dispose();
                    conSql.Dispose();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.ToString(), "Fill SQL Database List");
#else
                //AosCommon.WriteToLogFile(ex.ToString());
                MessageBox.Show("Unable to retrieve list of databases from SQL server.\n\n" + ex.Message + "\n\nApplication Version: " + Application.ProductVersion, "Error");
#endif
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private string BuildString()
        {
            string
                tmpConString = string.Empty,
                extProp = string.Empty;

            string
                provDspNm = dtProvider.Rows[lstProvider.SelectedIndex]["DisplayName"].ToString().ToLower();
            string provIntNm = string.Empty;
            try
            { provIntNm = dtProvider.Rows[lstProvider.SelectedIndex]["InternalName"].ToString(); }
            catch
            { provIntNm = string.Empty; }

            if (provDspNm.IndexOf(" ole ") > -1 || provDspNm.IndexOf(" office 2007 ") > -1)
            {
                // === OLEDB Builder ===
                if (!string.IsNullOrEmpty(provIntNm))
                    tmpConString = "Provider=" + provIntNm + ";";
                if (provIntNm.ToLower().IndexOf(".jet.") > -1 || provIntNm.ToLower().IndexOf(".ace.") > -1)
                {   // File-based Connections (Access, dBase, Excel, Text, etc)
                    if (txtInitCatalog.TextLength > 0)
                        tmpConString += "Data Source=" + txtInitCatalog.Text + ";";
                    if (txtLoginName.TextLength > 0)
                        tmpConString += "User ID=" + txtLoginName.Text + ";";
                    if (txtPassword.TextLength > 0)
                        tmpConString += "Password=" + txtPassword.Text + ";";

                    // For Excel Files, these two check boxes determine how the XLS file is parsed.
                    extProp += ";HDR=" + ((this.chkHDR.Checked) ? "Yes" : "No");
                    if (this.chkIMEX.Checked)
                        extProp += ";IMEX=1";
                }
                else
                {   // Server-based Connections (SQL, Oracle, etc)
                    if (provDspNm.IndexOf("datashape") > -1)
                        tmpConString += "Data Provider=SQLOLEDB;";
                    if (chkIPAddr.Checked == true && txtIP.ValidIP == true)
                    {   // Set IP address datasource
                        if (provDspNm.IndexOf("db2") > -1 && provDspNm.IndexOf("microsoft") > -1)
                        {   // Microsoft DB2 uses different keywords
                            tmpConString += "Network Transport Library=TCPIP;Network Address=" + txtIP.IPText + ";";
                        }
                        else if (provDspNm.IndexOf("db2") > -1)
                        {
                            tmpConString += "HOSTNAME=" + txtIP.IPText + ";PROTOCOL=TCPIP;";
                            if (txtPort.Text.Length > 0)
                                tmpConString += "PORT=" + txtPort.Text + ";";
                        }
                        else
                        {   // This should be everything else
                            tmpConString += "Data Source=" + txtIP.IPText;
                            if (txtPort.Text.Length > 0)
                                tmpConString += "," + txtPort.Text;
                            tmpConString += ";";
                            if (provDspNm.IndexOf("sql") > -1)
                                tmpConString += "Network Library=DBMSSOCN;";
                        }
                    }
                    else if (chkServerName.Checked == true && ((txtServerName.Visible && !string.IsNullOrEmpty(txtServerName.Text)) || !string.IsNullOrEmpty(drpSqlServers.Text)))
                    {   // Set DNS Server Name Data Source
                        if (provDspNm.IndexOf("db2") > -1 && provDspNm.IndexOf("microsoft") > -1)
                        {   // Microsoft OLE DB2
                            tmpConString += "Network Transport Library=TCPIP;Network Address=" + ((txtServerName.Visible) ? txtServerName.Text : drpSqlServers.Text) + ";";
                        }
                        else if (provDspNm.IndexOf("db2") > -1)
                        {   // IBM OLE DB2
                            tmpConString += "HOSTNAME=" + txtIP.IPText + ";PROTOCOL=TCPIP;";
                            if (txtPort.Text.Length > 0)
                                tmpConString += "PORT=" + txtPort.Text + ";";
                        }
                        else
                        {   // Everything else
                            tmpConString += "Data Source=" + ((txtServerName.Visible) ? txtServerName.Text : drpSqlServers.Text) + ";";
                        }
                    }
                    if (drpInitCatalog.SelectedIndex < 0)
                        if (provDspNm.IndexOf("ibm") > -1)
                            tmpConString += "Database=" + txtInitCatalog.Text + ";";
                        else
                            tmpConString += "Initial Catalog=" + txtInitCatalog.Text + ";";
                    else
                        tmpConString += "Initial Catalog=" + drpInitCatalog.Text + ";";
                }
                if (chkWinAuth.Checked)
                {
                    tmpConString += "Integrated Security=SSPI;";
                }
                else
                {
                    if (txtLoginName.Text.Length > 0)
                    {
                        if (provDspNm.IndexOf("ibm") > -1)
                            tmpConString += "uid=" + txtLoginName.Text + ";";
                        else
                            tmpConString += "User ID=" + txtLoginName.Text + ";";
                    }
                    if (txtPassword.Text.Length > 0)
                    {
                        if (provDspNm.IndexOf("ibm") > -1)
                            tmpConString += "pwd=" + txtPassword.Text + ";";
                        else
                            tmpConString += "Password=" + txtPassword.Text + ";";
                    }
                }
            }
            else if (provDspNm.IndexOf("odbc") > -1)
            {
                // === ODBC Builder ===
                if (provIntNm.Substring(0, 1) == "@")
                {
                    tmpConString = "Provider=" + provIntNm + ");";
                }
                else if (!string.IsNullOrEmpty(provIntNm))
                {
                    tmpConString = "driver={" + provIntNm + "};";
                }
                else
                {
                    if (provDspNm.IndexOf("system dsn") > -1)
                    {
                        tmpConString = "DSN=";
                    }
                    else if (provDspNm.IndexOf("file dsn") > -1)
                    {
                        tmpConString = "FILEDSN=";
                    }
                }
                if (provDspNm.IndexOf("foxpro") > -1)
                {
                    tmpConString += "SourceType=";
                    if (chkFolder.Checked)
                        tmpConString += "DBF;";
                    else
                        tmpConString += "DBC;";
                }
                if (txtServerName.TextLength > 0)
                {
                    if (provDspNm.IndexOf("db2") > -1)
                        tmpConString += "hostname=";
                    else
                        tmpConString += "Server=";
                    tmpConString += txtServerName.Text + ";";
                }

                // Set the initial data source
                if (txtInitCatalog.TextLength > 0)
                {
                    if (provDspNm.IndexOf("access") > -1)
                        tmpConString += "Dbq=";
                    else if (provDspNm.IndexOf("foxpro") > -1)
                        tmpConString += "SourceDB=";
                    else if (provDspNm.IndexOf("paradox v7") > -1)
                        tmpConString += "Initial Catalog=";
                    else
                        tmpConString += "Database=";
                    tmpConString += txtInitCatalog.Text + ";";
                }

                // Set the login credentials
                if (txtLoginName.TextLength > 0)
                    tmpConString += "Uid=" + txtLoginName.Text + ";";
                if (txtPassword.TextLength > 0)
                    tmpConString += "Pwd=" + txtPassword.Text + ";";

                // Extended Properties
                //if (dtProvider.Rows[lstProvider.SelectedIndex]["DisplayName"].ToString().ToLower().IndexOf("paradox v7") > -1)
                //    tmpConString += "Extended Properties='" + dtProvider.Rows[lstProvider.SelectedIndex]["ExtendProp"].ToString() + "DBQ=" + txtInitCatalog.Text + ";DefaultDir=" + txtInitCatalog.Text + ";'";
                //else if (txtExtendedParams.TextLength > 0)
                //    tmpConString += "Extended Properties=" + txtExtendedParams.Text;
                if (provDspNm.Contains("paradox v7"))
                    extProp += "DBQ=" + txtInitCatalog.Text + ";DefaultDir=" + txtInitCatalog.Text + ";";
            }
            else if (provDspNm.IndexOf("sql") > -1)
            {
                // === Pure SQL Builder ===
                if (chkIPAddr.Checked == true && txtIP.ValidIP == true)
                {
                    tmpConString = "Data Source=" + txtIP.IPText;
                    if (txtPort.TextLength > 0)
                        tmpConString += "," + txtPort.Text;
                    tmpConString += ";Network Library=DBMSSOCN;";
                }
                else if (chkServerName.Checked == true)
                {
                    tmpConString = "Data Source=" + ((txtServerName.Visible && !string.IsNullOrEmpty(txtServerName.Text)) ? txtServerName.Text : drpSqlServers.Text) + ";";
                }
                if (drpInitCatalog.Text.Length > 0)
                    tmpConString += "Initial Catalog=" + drpInitCatalog.Text + ";";
                if (chkWinAuth.Checked)
                    tmpConString += "trusted_connection=true;";
                else
                {
                    if (txtLoginName.TextLength > 0)
                        tmpConString += "User ID=" + txtLoginName.Text + ";";
                    if (txtPassword.TextLength > 0)
                        tmpConString += "Password=" + txtPassword.Text + ";";
                }
            }
            else if (provDspNm.IndexOf("db2") > -1)
            {
                // === IBM DB2 Builder ===
            }

            // Any additional parameters specified under the "Advanced" tab.
            extProp = txtExtendedParams.Text + extProp;
            if ((provIntNm.ToLower().IndexOf(".jet.") < 0 && provIntNm.ToLower().IndexOf(".ace.") < 0) || System.IO.Path.GetExtension(this.txtInitCatalog.Text).Trim('.', ' ').ToLower() != "mdb")
                if (extProp.Length > 0)
                    tmpConString += "Extended Properties=\"" + extProp.TrimStart(';', ' ') + "\";";
            if (txtExtraParams.TextLength > 0)
                tmpConString += txtExtraParams.Text + ";";

            return tmpConString;
        }
        private void SetJetProperties()
        {
            if (txtInitCatalog.Text.IndexOf(".txt") > -1)
            {
                txtExtendedParams.Text = "'text;HDR=";
                if (chkHDR.Checked == true)
                    txtExtendedParams.Text += "Yes";
                else
                    txtExtendedParams.Text += "No";
                txtExtendedParams.Text += ";FMT=Delimited'";
            }
            else if (txtInitCatalog.Text.EndsWith(".xls") || txtInitCatalog.Text.EndsWith(".xlsx"))
            {
                txtExtendedParams.Text = "'Excel 8.0;HDR=";
                if (chkHDR.Checked == true)
                    txtExtendedParams.Text += "Yes";
                else
                    txtExtendedParams.Text += "No";
                txtExtendedParams.Text += ";IMEX=";
                if (chkIMEX.Checked == true)
                    txtExtendedParams.Text += "1";
                else
                    txtExtendedParams.Text += "0";
            }
        }
        private void AcceptClose()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // User Control Event Handlers
        // 
        private void tabControl_onChange(object sender, EventArgs e)
        {
            this.SuspendLayout();
            string
                provDspNm = dtProvider.Rows[lstProvider.SelectedIndex]["DisplayName"].ToString().ToLower(),
                provIntNm = dtProvider.Rows[lstProvider.SelectedIndex]["InternalName"].ToString().ToLower();
            switch (tabControl.SelectedIndex)
            {
                case 0:         // Provider
                    break;
                case 1:         // Connection
                    drpInitCatalog.Visible = false;
                    txtInitCatalog.Visible = false;
                    grpFileOpt.Enabled = false;
                    chkFolder.Visible = false;
                    panIPAddr.Enabled = false;
                    panServerName.Enabled = false;
                    chkIPAddr.Enabled = false;
                    chkServerName.Enabled = false;
                    cmdBrowseDb.Visible = false;
                    this.drpSqlServers.Visible = false;
                    this.txtServerName.Visible = true;
                    if (provDspNm.IndexOf("sql") > -1)
                    {
                        lblDbType.Text = "Initial Catalog";
                        drpInitCatalog.Visible = true;
                        chkServerName.Enabled = true;
                        chkIPAddr.Enabled = true;
                        chkWinAuth.Visible = true;
                        txtLoginName.Enabled = (!chkWinAuth.Checked);
                        txtPassword.Enabled = (!chkWinAuth.Checked);
                        this.drpSqlServers.Visible = true;
                        this.txtServerName.Visible = false;
                    }
                    else if (provDspNm.IndexOf(" ole ") > -1 || provDspNm.IndexOf(" office ") > -1)
                    {
                        chkWinAuth.Visible = false;
                        if ((provIntNm.IndexOf(".jet.") > -1 || provIntNm.IndexOf(".ace.") > -1) || (provIntNm.IndexOf("foxpro") > -1))
                        {
                            if (chkFolder.Checked)
                                lblDbType.Text = "Database Folder";
                            else
                                lblDbType.Text = "Database File";
                            if (provIntNm.IndexOf("jet") > -1 || provIntNm.IndexOf(".ace.") > -1)
                            {
                                chkFolder.Text = "Folder-Based";
                                grpFileOpt.Enabled = true;
                            }
                            else
                            {
                                chkFolder.Text = "Free Table Folder";
                            }
                            txtInitCatalog.Visible = true;
                            cmdBrowseDb.Visible = true;
                            if (provIntNm.IndexOf(".ace.") < 0)
                                chkFolder.Visible = true;
                            txtInitCatalog.Focus();
                        }
                        else
                        {
                            chkServerName.Enabled = true;
                            chkIPAddr.Enabled = true;
                            txtInitCatalog.Visible = true;
                            txtLoginName.Enabled = true;
                            txtPassword.Enabled = true;
                        }
                    }
                    else if (provDspNm.IndexOf("odbc") > -1)
                    {
                    }
                    if (chkIPAddr.Checked && chkIPAddr.Enabled)
                    {
                        panIPAddr.Enabled = true;
                        txtIP.Focus();
                    }
                    else if(chkServerName.Enabled)
                    {
                        panServerName.Enabled = true;
                        txtServerName.Focus();
                    }
                    if (txtIP.Enabled) txtIP.Focus();
                    break;
                case 2:         // Advanced
                    if (dtProvider.Rows[lstProvider.SelectedIndex]["ExtraParams"] != null && dtProvider.Rows[lstProvider.SelectedIndex]["ExtraParams"].ToString().Length > 0)
                        txtExtraParams.Text = dtProvider.Rows[lstProvider.SelectedIndex]["ExtraParams"].ToString();

                    txtExtendedParams.Text = string.Empty;
                    if (dtProvider.Rows[lstProvider.SelectedIndex]["ExtendProp"] != null && dtProvider.Rows[lstProvider.SelectedIndex]["ExtendProp"].ToString().Length > 0)
                        txtExtendedParams.Text = dtProvider.Rows[lstProvider.SelectedIndex]["ExtendProp"].ToString();

                    // Determine IASM based on file extension.
                    string isamText = "";
                    if (!this.chkFolder.Checked)
                    {
                        string flType = System.IO.Path.GetExtension(this.txtInitCatalog.Text).Trim(' ', '.').ToLower();
                        if (flType == "xlsx")
                            isamText = " Xml";
                        else if (flType == "xlsm")
                            isamText = " Macro";
                        else if (flType == "xls")
                            isamText = "Excel 8.0";
                        else if (flType == "mdb")
                            this.txtExtendedParams.Text = string.Empty;
                        else
                            isamText = "text;FMT=DELIMITED";
                    }
                    else
                        isamText = "text;FMT=DELIMITED";
                    txtExtendedParams.Text += isamText;

                    if (provIntNm.IndexOf("jet") > -1)
                    {
                        grpFileOpt.Enabled = true;
                        if (txtInitCatalog.Text.ToLower().IndexOf(".xls") > -1)
                            chkIMEX.Enabled = true;
                        else
                            chkIMEX.Enabled = false;
                    }
                    else
                        grpFileOpt.Enabled = false;
                    cmdBuildString.Enabled = true;
                    break;
            }
            this.ResumeLayout();
        }
        private void chkFolder_onChange(object sender, EventArgs e)
        {
            if (chkFolder.Checked == true)
            {
                lblDbType.Text = "Database Folder:";
            }
            else
            {
                lblDbType.Text = "Database File:";
            }
        }
        private void txtPort_onKeyPress(object sender, KeyEventArgs e)
        {
            if ((e.KeyValue < 48 || e.KeyValue > 57) && (e.KeyValue < 96 || e.KeyValue > 105))
                e.SuppressKeyPress = true;
        }
        private void userControl_onKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter || e.KeyCode == System.Windows.Forms.Keys.Return)
            {
                e.SuppressKeyPress = true;
                if (tabControl.SelectedIndex == 1 && (dtProvider.Rows[lstProvider.SelectedIndex]["ExtendProp"].ToString().Length > 0 && dtProvider.Rows[lstProvider.SelectedIndex]["ExtraParams"].ToString().Length > 0))
                    AcceptClose();
                else if (tabControl.SelectedIndex < 2)
                    tabControl.SelectTab(tabControl.SelectedIndex + 1);
                else
                    AcceptClose();
            }
        }
        private void lstProvider_onSelectedIndexChanged(object sender, EventArgs e)
        {
            cmdBuildString.Enabled = false;
            txtInitCatalog.Text = "";
            txtExtraParams.Text = "";
            txtExtendedParams.Text = "";
            drpInitCatalog.Items.Clear();
        }
        private void txtDatabaseFile_onChange(object sender, EventArgs e)
        {
            if (txtInitCatalog.Text.IndexOf(".txt") > -1)
            {
                SetJetProperties();
                panExcelTip.Visible = false;
            }
            else if (txtInitCatalog.Text.IndexOf(".xls") > -1)
            {
                SetJetProperties();
                panExcelTip.Visible = true;
            }
            else if (txtInitCatalog.Text.IndexOf(".mdb") > -1)
            {
                txtExtendedParams.Text = "";
                panExcelTip.Visible = false;
            }
        }
        private void chkFileOptions_onChecked(object sender, EventArgs e)
        {
            SetJetProperties();
        }
        private void drpInitCatalogList_onDrop(object sender, EventArgs e)
        {
            FillDatabaseList();
        }
        private void cmdBrowseDb_onClick(object sender, EventArgs e)
        {
            if (chkFolder.Checked == true)
            {
                FolderBrowserDialog dlgOpen = new FolderBrowserDialog();
                dlgOpen.Description = "Select the Free Table Directory";
                dlgOpen.ShowNewFolderButton = false;
                if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                    txtInitCatalog.Text = dlgOpen.SelectedPath;
                dlgOpen.Dispose();
            }
            else
            {
                OpenFileDialog dlgOpen = new OpenFileDialog();
                dlgOpen.RestoreDirectory = true;
                dlgOpen.CheckFileExists = true;
                dlgOpen.CheckPathExists = true;
                dlgOpen.Multiselect = false;
                dlgOpen.SupportMultiDottedExtensions = true;
                dlgOpen.ValidateNames = true;
                dlgOpen.InitialDirectory = Environment.CurrentDirectory;
                if (dtProvider.Rows[lstProvider.SelectedIndex]["InternalName"].ToString().ToLower().IndexOf(".ace.") > -1)
                    dlgOpen.Filter = "Excel 2007 Files|*.xlsx;*.xlsb;*.xlsm|Excel 2007 Standard|*.xlsx|Excel 2007 Binary|*.xlsb|Excel 2007 Macro|*.xlsm";
                else
                    dlgOpen.Filter = "All Supported Types|*.mdb;*.txt;*.csv;*.xls|Comma Seperated Values Files (*.csv)|*.csv|Access DB Files (*.mdb)|*.mdb|Text Files (*.txt)|*.txt|Excel Files (*.xls)|*.xls|All Files (*.*)|*.*";
                dlgOpen.FilterIndex = 0;
                if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                    txtInitCatalog.Text = dlgOpen.FileName;
                dlgOpen.Dispose();
            }
        }
        private void cmdNext_onClick(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "cmdNext0":
                    tabControl.SelectedIndex = 1;
                    break;
                case "cmdNext1":
                    tabControl.SelectedIndex = 2;
                    break;
                case "cmdNext2":
                    break;
            }
        }
        private void chkSource_onClick(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "chkIPAddr":
                    panIPAddr.Enabled = true;
                    panServerName.Enabled = false;
                    txtIP.Focus();
                    break;
                case "chkServerName":
                    panIPAddr.Enabled = false;
                    panServerName.Enabled = true;
                    txtServerName.Focus();
                    break;
            }
        }
        private void chkWinAuth_onCheckedChanged(object sender, EventArgs e)
        {
            this.txtLoginName.Enabled = (!this.chkWinAuth.Checked);
            this.txtPassword.Enabled = (!this.chkWinAuth.Checked);
        }
        //***************************************************************************
        // System Event Handlers
        // 
        private void frmStringBuilder_onLoad(object sender, EventArgs e)
        {
            if (!lstProvider.Enabled)
            {
                tabControl.SelectedIndex = 0;   // Try to force a tabControl_onChange event.
                tabControl.SelectedIndex = 1;   // Connection tab
                txtIP.Focus();
            }
        }
        #endregion
    }
}