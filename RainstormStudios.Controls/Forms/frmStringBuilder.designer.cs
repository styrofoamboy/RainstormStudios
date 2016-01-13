namespace RainstormStudios.Forms
{
    partial class frmStringBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdBuildString = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpgProvider = new System.Windows.Forms.TabPage();
            this.cmdNext0 = new System.Windows.Forms.Button();
            this.lstProvider = new System.Windows.Forms.ListBox();
            this.tpgConnection = new System.Windows.Forms.TabPage();
            this.cmdNext1 = new System.Windows.Forms.Button();
            this.panExcelTip = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panSQL = new System.Windows.Forms.Panel();
            this.panServerName = new System.Windows.Forms.Panel();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.panIPAddr = new System.Windows.Forms.Panel();
            this.txtIP = new RainstormStudios.Controls.IPEntryBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panCredentials = new System.Windows.Forms.Panel();
            this.txtLoginName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkWinAuth = new System.Windows.Forms.CheckBox();
            this.chkIPAddr = new System.Windows.Forms.RadioButton();
            this.chkServerName = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.drpInitCatalog = new System.Windows.Forms.ComboBox();
            this.cmdBrowseDb = new System.Windows.Forms.Button();
            this.lblDbType = new System.Windows.Forms.Label();
            this.chkFolder = new System.Windows.Forms.CheckBox();
            this.txtInitCatalog = new System.Windows.Forms.TextBox();
            this.tpgAdvanced = new System.Windows.Forms.TabPage();
            this.grpFileOpt = new System.Windows.Forms.GroupBox();
            this.chkIMEX = new System.Windows.Forms.CheckBox();
            this.chkHDR = new System.Windows.Forms.CheckBox();
            this.txtExtraParams = new System.Windows.Forms.TextBox();
            this.txtExtendedParams = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.drpSqlServers = new RainstormStudios.Controls.SqlServerComboBox();
            this.tabControl.SuspendLayout();
            this.tpgProvider.SuspendLayout();
            this.tpgConnection.SuspendLayout();
            this.panExcelTip.SuspendLayout();
            this.panSQL.SuspendLayout();
            this.panServerName.SuspendLayout();
            this.panIPAddr.SuspendLayout();
            this.panCredentials.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tpgAdvanced.SuspendLayout();
            this.grpFileOpt.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdBuildString
            // 
            this.cmdBuildString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBuildString.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdBuildString.Enabled = false;
            this.cmdBuildString.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdBuildString.Location = new System.Drawing.Point(200, 392);
            this.cmdBuildString.Name = "cmdBuildString";
            this.cmdBuildString.Size = new System.Drawing.Size(75, 23);
            this.cmdBuildString.TabIndex = 27;
            this.cmdBuildString.Text = "Build String";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Location = new System.Drawing.Point(112, 392);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 26;
            this.cmdCancel.Text = "Cancel";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tpgProvider);
            this.tabControl.Controls.Add(this.tpgConnection);
            this.tabControl.Controls.Add(this.tpgAdvanced);
            this.tabControl.Location = new System.Drawing.Point(8, 8);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(272, 376);
            this.tabControl.TabIndex = 2;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_onChange);
            // 
            // tpgProvider
            // 
            this.tpgProvider.Controls.Add(this.cmdNext0);
            this.tpgProvider.Controls.Add(this.lstProvider);
            this.tpgProvider.Location = new System.Drawing.Point(4, 25);
            this.tpgProvider.Name = "tpgProvider";
            this.tpgProvider.Padding = new System.Windows.Forms.Padding(3);
            this.tpgProvider.Size = new System.Drawing.Size(264, 347);
            this.tpgProvider.TabIndex = 0;
            this.tpgProvider.Text = "Provider";
            // 
            // cmdNext0
            // 
            this.cmdNext0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdNext0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdNext0.Location = new System.Drawing.Point(176, 312);
            this.cmdNext0.Name = "cmdNext0";
            this.cmdNext0.Size = new System.Drawing.Size(75, 23);
            this.cmdNext0.TabIndex = 1;
            this.cmdNext0.Text = "Next >>";
            this.cmdNext0.Click += new System.EventHandler(this.cmdNext_onClick);
            // 
            // lstProvider
            // 
            this.lstProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstProvider.FormattingEnabled = true;
            this.lstProvider.IntegralHeight = false;
            this.lstProvider.Location = new System.Drawing.Point(8, 8);
            this.lstProvider.Name = "lstProvider";
            this.lstProvider.Size = new System.Drawing.Size(248, 296);
            this.lstProvider.TabIndex = 0;
            this.lstProvider.SelectedIndexChanged += new System.EventHandler(this.lstProvider_onSelectedIndexChanged);
            this.lstProvider.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // tpgConnection
            // 
            this.tpgConnection.Controls.Add(this.cmdNext1);
            this.tpgConnection.Controls.Add(this.panExcelTip);
            this.tpgConnection.Controls.Add(this.panSQL);
            this.tpgConnection.Location = new System.Drawing.Point(4, 25);
            this.tpgConnection.Name = "tpgConnection";
            this.tpgConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tpgConnection.Size = new System.Drawing.Size(264, 347);
            this.tpgConnection.TabIndex = 1;
            this.tpgConnection.Text = "Connection";
            // 
            // cmdNext1
            // 
            this.cmdNext1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdNext1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdNext1.Location = new System.Drawing.Point(176, 312);
            this.cmdNext1.Name = "cmdNext1";
            this.cmdNext1.Size = new System.Drawing.Size(75, 23);
            this.cmdNext1.TabIndex = 25;
            this.cmdNext1.Text = "Next >>";
            this.cmdNext1.Click += new System.EventHandler(this.cmdNext_onClick);
            // 
            // panExcelTip
            // 
            this.panExcelTip.Controls.Add(this.label13);
            this.panExcelTip.Controls.Add(this.label12);
            this.panExcelTip.Controls.Add(this.label11);
            this.panExcelTip.Location = new System.Drawing.Point(8, 312);
            this.panExcelTip.Name = "panExcelTip";
            this.panExcelTip.Size = new System.Drawing.Size(208, 64);
            this.panExcelTip.TabIndex = 16;
            this.panExcelTip.Visible = false;
            // 
            // label13
            // 
            this.label13.ForeColor = System.Drawing.Color.DimGray;
            this.label13.Location = new System.Drawing.Point(24, 32);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(168, 32);
            this.label13.TabIndex = 16;
            this.label13.Text = "i.e. worksheet name followed by a \"$\" and wapped in \"[ ]\" brackets.";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.DarkRed;
            this.label12.Location = new System.Drawing.Point(0, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Excel Tip!";
            // 
            // label11
            // 
            this.label11.AutoEllipsis = true;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(208, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "SQL Syntax: \"SELECT * FROM [sheet1$]\"";
            // 
            // panSQL
            // 
            this.panSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panSQL.Controls.Add(this.panServerName);
            this.panSQL.Controls.Add(this.panIPAddr);
            this.panSQL.Controls.Add(this.panCredentials);
            this.panSQL.Controls.Add(this.chkIPAddr);
            this.panSQL.Controls.Add(this.chkServerName);
            this.panSQL.Controls.Add(this.panel1);
            this.panSQL.Location = new System.Drawing.Point(8, 8);
            this.panSQL.Name = "panSQL";
            this.panSQL.Size = new System.Drawing.Size(240, 296);
            this.panSQL.TabIndex = 3;
            // 
            // panServerName
            // 
            this.panServerName.Controls.Add(this.drpSqlServers);
            this.panServerName.Controls.Add(this.txtServerName);
            this.panServerName.Enabled = false;
            this.panServerName.Location = new System.Drawing.Point(24, 72);
            this.panServerName.Name = "panServerName";
            this.panServerName.Size = new System.Drawing.Size(200, 24);
            this.panServerName.TabIndex = 3;
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(3, 0);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(184, 20);
            this.txtServerName.TabIndex = 6;
            this.txtServerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // panIPAddr
            // 
            this.panIPAddr.Controls.Add(this.txtIP);
            this.panIPAddr.Controls.Add(this.txtPort);
            this.panIPAddr.Controls.Add(this.label5);
            this.panIPAddr.Location = new System.Drawing.Point(24, 24);
            this.panIPAddr.Name = "panIPAddr";
            this.panIPAddr.Size = new System.Drawing.Size(200, 24);
            this.panIPAddr.TabIndex = 2;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(8, 0);
            this.txtIP.MaximumSize = new System.Drawing.Size(120, 20);
            this.txtIP.MinimumSize = new System.Drawing.Size(120, 20);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(120, 20);
            this.txtIP.TabIndex = 4;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(152, 0);
            this.txtPort.MaxLength = 5;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(40, 20);
            this.txtPort.TabIndex = 5;
            this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPort_onKeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(128, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Port:";
            // 
            // panCredentials
            // 
            this.panCredentials.Controls.Add(this.txtLoginName);
            this.panCredentials.Controls.Add(this.txtPassword);
            this.panCredentials.Controls.Add(this.label1);
            this.panCredentials.Controls.Add(this.label2);
            this.panCredentials.Controls.Add(this.chkWinAuth);
            this.panCredentials.Location = new System.Drawing.Point(24, 112);
            this.panCredentials.Name = "panCredentials";
            this.panCredentials.Size = new System.Drawing.Size(200, 112);
            this.panCredentials.TabIndex = 11;
            // 
            // txtLoginName
            // 
            this.txtLoginName.Location = new System.Drawing.Point(8, 16);
            this.txtLoginName.Name = "txtLoginName";
            this.txtLoginName.Size = new System.Drawing.Size(184, 20);
            this.txtLoginName.TabIndex = 8;
            this.txtLoginName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(8, 64);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(184, 20);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Login Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Password";
            // 
            // chkWinAuth
            // 
            this.chkWinAuth.AutoSize = true;
            this.chkWinAuth.Location = new System.Drawing.Point(32, 88);
            this.chkWinAuth.Name = "chkWinAuth";
            this.chkWinAuth.Size = new System.Drawing.Size(119, 17);
            this.chkWinAuth.TabIndex = 7;
            this.chkWinAuth.Text = "Trusted Connection";
            this.chkWinAuth.CheckedChanged += new System.EventHandler(this.chkWinAuth_onCheckedChanged);
            // 
            // chkIPAddr
            // 
            this.chkIPAddr.AutoSize = true;
            this.chkIPAddr.Checked = true;
            this.chkIPAddr.Location = new System.Drawing.Point(8, 8);
            this.chkIPAddr.Name = "chkIPAddr";
            this.chkIPAddr.Size = new System.Drawing.Size(76, 17);
            this.chkIPAddr.TabIndex = 0;
            this.chkIPAddr.TabStop = true;
            this.chkIPAddr.Text = "IP Address";
            this.chkIPAddr.CheckedChanged += new System.EventHandler(this.chkSource_onClick);
            // 
            // chkServerName
            // 
            this.chkServerName.AutoSize = true;
            this.chkServerName.Location = new System.Drawing.Point(8, 56);
            this.chkServerName.Name = "chkServerName";
            this.chkServerName.Size = new System.Drawing.Size(87, 17);
            this.chkServerName.TabIndex = 1;
            this.chkServerName.Text = "Server Name";
            this.chkServerName.CheckedChanged += new System.EventHandler(this.chkSource_onClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.drpInitCatalog);
            this.panel1.Controls.Add(this.cmdBrowseDb);
            this.panel1.Controls.Add(this.lblDbType);
            this.panel1.Controls.Add(this.chkFolder);
            this.panel1.Controls.Add(this.txtInitCatalog);
            this.panel1.Location = new System.Drawing.Point(24, 229);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 64);
            this.panel1.TabIndex = 12;
            // 
            // drpInitCatalog
            // 
            this.drpInitCatalog.FormattingEnabled = true;
            this.drpInitCatalog.Location = new System.Drawing.Point(8, 16);
            this.drpInitCatalog.Name = "drpInitCatalog";
            this.drpInitCatalog.Size = new System.Drawing.Size(184, 21);
            this.drpInitCatalog.TabIndex = 20;
            this.drpInitCatalog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            this.drpInitCatalog.DropDown += new System.EventHandler(this.drpInitCatalogList_onDrop);
            // 
            // cmdBrowseDb
            // 
            this.cmdBrowseDb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdBrowseDb.Location = new System.Drawing.Point(120, 40);
            this.cmdBrowseDb.Name = "cmdBrowseDb";
            this.cmdBrowseDb.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowseDb.TabIndex = 2;
            this.cmdBrowseDb.Text = "Browse";
            this.cmdBrowseDb.Visible = false;
            this.cmdBrowseDb.Click += new System.EventHandler(this.cmdBrowseDb_onClick);
            // 
            // lblDbType
            // 
            this.lblDbType.AutoSize = true;
            this.lblDbType.Location = new System.Drawing.Point(0, 0);
            this.lblDbType.Name = "lblDbType";
            this.lblDbType.Size = new System.Drawing.Size(70, 13);
            this.lblDbType.TabIndex = 11;
            this.lblDbType.Text = "Initial Catalog";
            // 
            // chkFolder
            // 
            this.chkFolder.AutoSize = true;
            this.chkFolder.Location = new System.Drawing.Point(24, 40);
            this.chkFolder.Name = "chkFolder";
            this.chkFolder.Size = new System.Drawing.Size(88, 17);
            this.chkFolder.TabIndex = 3;
            this.chkFolder.Text = "Folder-Based";
            this.chkFolder.Visible = false;
            this.chkFolder.CheckedChanged += new System.EventHandler(this.chkFolder_onChange);
            // 
            // txtInitCatalog
            // 
            this.txtInitCatalog.Location = new System.Drawing.Point(8, 16);
            this.txtInitCatalog.Name = "txtInitCatalog";
            this.txtInitCatalog.Size = new System.Drawing.Size(184, 20);
            this.txtInitCatalog.TabIndex = 9;
            this.txtInitCatalog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // tpgAdvanced
            // 
            this.tpgAdvanced.Controls.Add(this.grpFileOpt);
            this.tpgAdvanced.Controls.Add(this.txtExtraParams);
            this.tpgAdvanced.Controls.Add(this.txtExtendedParams);
            this.tpgAdvanced.Controls.Add(this.label9);
            this.tpgAdvanced.Controls.Add(this.label10);
            this.tpgAdvanced.Location = new System.Drawing.Point(4, 25);
            this.tpgAdvanced.Name = "tpgAdvanced";
            this.tpgAdvanced.Size = new System.Drawing.Size(264, 347);
            this.tpgAdvanced.TabIndex = 2;
            this.tpgAdvanced.Text = "Advanced";
            // 
            // grpFileOpt
            // 
            this.grpFileOpt.Controls.Add(this.chkIMEX);
            this.grpFileOpt.Controls.Add(this.chkHDR);
            this.grpFileOpt.Location = new System.Drawing.Point(24, 128);
            this.grpFileOpt.Name = "grpFileOpt";
            this.grpFileOpt.Size = new System.Drawing.Size(216, 72);
            this.grpFileOpt.TabIndex = 5;
            this.grpFileOpt.TabStop = false;
            this.grpFileOpt.Text = "File-Based Options";
            // 
            // chkIMEX
            // 
            this.chkIMEX.AutoSize = true;
            this.chkIMEX.Checked = true;
            this.chkIMEX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIMEX.Location = new System.Drawing.Point(8, 48);
            this.chkIMEX.Name = "chkIMEX";
            this.chkIMEX.Size = new System.Drawing.Size(206, 17);
            this.chkIMEX.TabIndex = 5;
            this.chkIMEX.Text = "Read Intermixed Data Columns as text";
            this.chkIMEX.CheckedChanged += new System.EventHandler(this.chkFileOptions_onChecked);
            // 
            // chkHDR
            // 
            this.chkHDR.AutoSize = true;
            this.chkHDR.Checked = true;
            this.chkHDR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHDR.Location = new System.Drawing.Point(8, 24);
            this.chkHDR.Name = "chkHDR";
            this.chkHDR.Size = new System.Drawing.Size(188, 17);
            this.chkHDR.TabIndex = 4;
            this.chkHDR.Text = "First Row Contains Column Names";
            this.chkHDR.CheckedChanged += new System.EventHandler(this.chkFileOptions_onChecked);
            // 
            // txtExtraParams
            // 
            this.txtExtraParams.Location = new System.Drawing.Point(24, 88);
            this.txtExtraParams.Name = "txtExtraParams";
            this.txtExtraParams.Size = new System.Drawing.Size(216, 20);
            this.txtExtraParams.TabIndex = 3;
            this.txtExtraParams.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // txtExtendedParams
            // 
            this.txtExtendedParams.Location = new System.Drawing.Point(24, 32);
            this.txtExtendedParams.Name = "txtExtendedParams";
            this.txtExtendedParams.Size = new System.Drawing.Size(216, 20);
            this.txtExtendedParams.TabIndex = 1;
            this.txtExtendedParams.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userControl_onKeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Extended Properties";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Extra Parameters";
            // 
            // drpSqlServers
            // 
            this.drpSqlServers.FormattingEnabled = true;
            this.drpSqlServers.Location = new System.Drawing.Point(3, 0);
            this.drpSqlServers.Name = "drpSqlServers";
            this.drpSqlServers.Size = new System.Drawing.Size(189, 21);
            this.drpSqlServers.TabIndex = 7;
            this.drpSqlServers.Visible = false;
            // 
            // frmStringBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 425);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdBuildString);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStringBuilder";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "AOS Connection String Builder";
            this.Load += new System.EventHandler(this.frmStringBuilder_onLoad);
            this.tabControl.ResumeLayout(false);
            this.tpgProvider.ResumeLayout(false);
            this.tpgConnection.ResumeLayout(false);
            this.panExcelTip.ResumeLayout(false);
            this.panExcelTip.PerformLayout();
            this.panSQL.ResumeLayout(false);
            this.panSQL.PerformLayout();
            this.panServerName.ResumeLayout(false);
            this.panServerName.PerformLayout();
            this.panIPAddr.ResumeLayout(false);
            this.panIPAddr.PerformLayout();
            this.panCredentials.ResumeLayout(false);
            this.panCredentials.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tpgAdvanced.ResumeLayout(false);
            this.tpgAdvanced.PerformLayout();
            this.grpFileOpt.ResumeLayout(false);
            this.grpFileOpt.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdBuildString;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpgProvider;
        private System.Windows.Forms.TabPage tpgConnection;
        private System.Windows.Forms.Button cmdNext0;
        private System.Windows.Forms.ListBox lstProvider;
        private System.Windows.Forms.TabPage tpgAdvanced;
        private System.Windows.Forms.Button cmdNext1;
        private System.Windows.Forms.Panel panSQL;
        private System.Windows.Forms.RadioButton chkIPAddr;
        private System.Windows.Forms.RadioButton chkServerName;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Panel panServerName;
        private System.Windows.Forms.Panel panIPAddr;
        private System.Windows.Forms.TextBox txtLoginName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox drpInitCatalog;
        private System.Windows.Forms.TextBox txtInitCatalog;
        private System.Windows.Forms.Label lblDbType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Panel panCredentials;
        private System.Windows.Forms.Button cmdBrowseDb;
        private System.Windows.Forms.TextBox txtExtendedParams;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtExtraParams;
        private System.Windows.Forms.GroupBox grpFileOpt;
        private System.Windows.Forms.CheckBox chkHDR;
        private System.Windows.Forms.CheckBox chkIMEX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panExcelTip;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox chkFolder;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkWinAuth;
        private RainstormStudios.Controls.IPEntryBox txtIP;
        private RainstormStudios.Controls.SqlServerComboBox drpSqlServers;
    }
}