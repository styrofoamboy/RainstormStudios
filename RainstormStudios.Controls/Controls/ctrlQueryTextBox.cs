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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Drawing;
using RainstormStudios.Unmanaged;
using RainstormStudios.Collections;

namespace RainstormStudios.Controls
{
    [Author("Michael Unfried")]
    public sealed class QueryTextParser
    {
        #region Declartions
        //****************************************************************************
        // Global Constants
        // 
        static readonly string[] sqlKeywords = new string[]{
            "add","all","alter","and","any","as","asc","authorization","backup","begin","break",
            "browse","bulk","by","cascade","case","check","checkpoint","close","clustered",
            "coalesce","collate","column","commit","compute","constraint","contains","containsable",
            "continue","create","cross","current","current_date","current_time",
            "current_timestamp","current_user","cursor","database","dbcc","deallocate","declare",
            "default","delete","deny","desc","disk","distinct","distributed","double","drop",
            "dummy","dump","else","end","errlvl","escape","except","exec","execute","exists","exit",
            "fetch","file","fillfactor","for","foreign","freetext","freetexttable","from","full",
            "function","goto","grant","group","having","holdlock","identity_insert",
            "identitycol","if","in","index","inner","insert","intersect","into","is","join","key",
            "kill","left","like","lineno","load","national","nocheck","nonclustered","not","null",
            "nullif","of","off","offsets","on","open","opendatasource","openquery","openrowset",
            "openxml","option","or","order","outer","over","percent","plan","precision","primary",
            "print","proc","procedure","public","raiserror","read","readtext","reconfigure",
            "references","replication","restore","restrict","return","revoke","right","rollback",
            "rowcount","rowguidcol","rule","save","schema","select","session_user","set","setuser",
            "shutdown","some","statistics","system_user","table","textsize","then","to","top","tran",
            "transaction","trigger","truncate","tsequal","union","unique","update","updatetext",
            "use","user","values","varying","view","waitfor","when","where","while","with",
            "writetext","pivot","returns","dateadd",
            "varchar","nvarchar","char","nchar","int","bigint","binary","bit","datetime","decimal",
            "float","image","money","ntext","text","numeric","real","smallint","smalldatetime",
            "smallmoney","text","timestamp","tinyint","uniqueidentifier","varbinary","xml"
        };
        static readonly string[] sqlFunc = new string[]{
            "getdate","datediff","datename","datepart","convert","day","month","year","hour","minute","second",
            "rtrim","ltrim","str","len","isnull","isnumeric","cert_id","charindex","cast","ceiling","floor",
            "round","checksum","sum","substring","charindex","upper","lower","left","right","difference","ascii",
            "nchar","patindex","soundex","space","str","stuff","unicode","quotename","replace","replicate",
            "reverse","avg","HashBytes"
        };
        //***************************************************************************
        // Private Fields
        // 
        private Regex
            _rgxKeyWord = null,
            _rgxComment = null,
            _rgxMLComment = null,
            _rgxStrLiteral = null,
            //_rgxAlias = null,
            _rgxFunc = null,
            _rgxVar = null,
            _rgxTblRef = null;
        static readonly QueryTextParser
            _singleInstance = new QueryTextParser();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public static QueryTextParser Instance
        { get { return _singleInstance; } }
        public Regex KeyWord
        { get { return this._rgxKeyWord; } }
        public Regex Function
        { get { return this._rgxFunc; } }
        public Regex Variable
        { get { return this._rgxVar; } }
        public Regex Comment
        { get { return this._rgxComment; } }
        public Regex CommentML
        { get { return this._rgxMLComment; } }
        public Regex Literal
        { get { return this._rgxStrLiteral; } }
        public Regex TableReference
        { get { return this._rgxTblRef; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// A single constructor, which is private and parameterless.
        /// </summary>
        /// <remarks>
        ///     This prevents other classes from instantiating it
        /// (which would be a violation of the singleton patern). It also prevents
        /// subclassing. If a singleton can be subclassed, and if each of those
        /// subclasses can create an instance, the patern is violated.
        /// 
        /// http://www.yoda.arachsys.com/csharp/singleton.html
        /// </remarks>
        private QueryTextParser()
        {
            this.InitRegex();
        }
        /// <summary>
        /// Explicit static constructor.
        /// </summary>
        /// <remarks>
        /// Explicit static constructor to tell C# compiler not to mark type as
        /// 'beforefieldinit'.
        /// 
        /// http://www.yoda.arachsys.com/csharp/beforefieldinit.html
        /// </remarks>
        static QueryTextParser()
        { }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void InitRegex()
        {
            try
            {
                #region DEPRECIATED CODE
                // These methods for building the '_rgxKeyWord' and '_rgxFunc'
                //   objects had inate performance issues due to the use of
                //   string concatenation.

                //string regStr = "";
                //foreach (string str in QueryTextBox.sqlKeywords)
                //    regStr += "|" + str;
                //this._rgxKeyWord = new Regex("(?<!--.*|\\[)(?:\\b(" + regStr.Substring(1) + ")\\b)(?!\\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                //regStr = "";
                //foreach (string str in QueryTextBox.sqlFunc)
                //    regStr += "|" + str;
                //this._rgxFunc = new Regex("(?<!--.*|\\[)(?:\\b(" + regStr.Substring(1) + ")\\b)(?!\\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                #endregion

                this._rgxKeyWord = new Regex("(?<!--.*|\\[)(?:\\b(" + string.Join("|", QueryTextParser.sqlKeywords) + ")\\b)(?!\\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                this._rgxFunc = new Regex("(?<!--.*|\\[)(?:\\b(" + string.Join("|", QueryTextParser.sqlFunc) + ")\\b)(?!\\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                this._rgxVar = new Regex("(?<!--.*|\\[)@{2}\\w+(?!\\])\\b", RegexOptions.Compiled);
                this._rgxComment = new Regex("--.*", RegexOptions.Compiled);
                this._rgxMLComment = new Regex("/\\*(?:.*?)(?:(\\*/)|$)", RegexOptions.Compiled | RegexOptions.Singleline);
                //this._rgxStrLiteral = new Regex("'{1}(.*?)(('{1})|$)", RegexOptions.Compiled | RegexOptions.Singleline);
                this._rgxStrLiteral = new Regex("(?<!--.*|\\[)'{1}(?:.*?)('{1}|$)", RegexOptions.Compiled);
                //this._rgxAlias = new Regex(@"(?<!(--.*))[a-zA-Z]*\.(?=\w+)", RegexOptions.Compiled);
                //this._rgxAlias = new Regex(@"(?<alias>(?<!--.*)[a-zA-Z]*)\.(?=\w+)/k(\bfrom\b\w+(\s*|as\b)<alias>)", RegexOptions.Compiled);
                this._rgxTblRef = new Regex(@"\b(FROM|JOIN)(?!(\s*/|\(?)SELECT)\s+(\[?(?<db>.*?)\]?\.)?(\[?(?<sch>.*?)\]?\.)?\[?(?<tbl>.*?)\]?\s+(?!(WHERE|ON|JOIN|INNER|OUTER|LEFT|RIGHT|FULL|PIVOT|GROUP|HAVING))(AS)?(\[)?(?<als>\w+)($|[\s\n]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

                // This one was supposed to find table references without an alias,
                //   but doesn't work correctly and "over grabs" the aliases into
                //   the next table definition.
                //this._rgxTblRef = new Regex(@"\b(FROM|JOIN)(?!(\s*/|\(?)SELECT)\s+(\[?(?<db>.*?)\]?\.)?(\[?(?<sch>.*?)\]?\.)?\[?(?<tbl>.*?)\]?\s+((?!(WHERE|ON|JOIN|INNER|OUTER|LEFT|RIGHT|FULL|PIVOT|GROUP|HAVING))(AS)?(\[)?(?<als>\w+))?(\s|\b)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            catch { }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    [DesignerCategory("UserControl"), ToolboxBitmap(typeof(QueryTextBox), "/QueryTextBox.bmp")]
    public class QueryTextBox : RainstormStudios.Controls.AdvRichTextBox
    {
        #region Nested Objects
        //***************************************************************************
        // Enums
        // 
        enum AutoCompleteType
        {
            Field,
            Table
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Global Constants
        // 
        const int
            leftMargin = 45;
        static readonly Color
            leftMarginBgColor = Color.Silver,
            lineNumberColor = Color.DarkBlue;
        //***************************************************************************
        // Private Fields
        // 
        private Color
            _clrKeyword = Color.Blue,
            _clrString = Color.Red,
            _clrComment = Color.Green,
            _clrAlias = Color.Navy,
            _clrFunc = Color.Fuchsia,
            _clrGblVar = Color.DarkViolet;
        private bool
            _parsing = false,
            _init = false,
            _defCtxMenu = false,
            _canEdit,
            _autoComp,
            _insMode;
        private System.Windows.Forms.ContextMenuStrip
            _defMenu;
        private int
            _selStart = 0,
            _selLength = 0,
            _lastParseLine,
            _lastPrintLine,
            _addToSel = 0,
            _autoCompStart,
            _autoCompLine;
        private string
            _connStr;
        private List<Int32>
            _bldChars;
        private StringCollection
            _refSchemas,
            _refTables,
            _refTablesAlias,
            _refTablesCols;
        private List<String>
            _autoCompleteSchemas,
            _autoCompleteTables;
        //private System.Windows.Forms.ContextMenuStrip
        //    _autoCompMenu;
        private System.Windows.Forms.ListBox
            _autoCompMenu;
        private System.Windows.Forms.Keys
            _lastKeyPress;
        //***************************************************************************
        // Delegate Definitions
        // 
        private delegate void DoParseDelegate(int start, int length);
        private delegate string ParseTextDelegate(string value);
        private delegate void UpdateTextDelegate(string rtfValue);
        private delegate void SetCaretPositionDelegate(int StartPos, int SelLength);
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler TextParsed;
        public event EventHandler InsertModeChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Gets or sets the color to be applied to keywords entered into the query textbox.
        /// </summary>
        public Color KeywordColor
        {
            get { return _clrKeyword; }
            set { _clrKeyword = (value != Color.Empty && value != Color.Transparent) ? value : Color.Blue; }
        }
        /// <summary>
        /// Gets or sets the color to be applied to string literals entered into the query textbox.
        /// </summary>
        public Color StringLiteralColor
        {
            get { return _clrString; }
            set { _clrString = (value != Color.Empty && value != Color.Transparent) ? value : Color.Red; }
        }
        /// <summary>
        /// Gets or sets the color to be applied to comments entered into the query textbox.
        /// </summary>
        public Color CommentColor
        {
            get { return _clrComment; }
            set { _clrComment = (value != Color.Empty && value != Color.Transparent) ? value : Color.Green; }
        }
        ///// <summary>
        ///// Gets or sets the color to be applied to alias tags entered into the query textbox.
        ///// </summary>
        public Color AliasColor
        {
            get { return _clrAlias; }
            set { _clrAlias = (value != Color.Empty && value != Color.Transparent) ? value : Color.Navy; }
        }
        /// <summary>
        /// Gets or sets the color to be applied to SQL functions entered into the query textbox.
        /// </summary>
        public Color FunctionColor
        {
            get { return this._clrFunc; }
            set { this._clrFunc = (value != Color.Empty && value != Color.Transparent) ? value : Color.Fuchsia; }
        }
        /// <summary>
        /// Gets or sets the color to be applied to global variables entered into the query textbox.
        /// </summary>
        public Color GlobalVariableColor
        {
            get { return this._clrGblVar; }
            set { this._clrGblVar = (value != Color.Empty && value != Color.Transparent) ? value : Color.DarkViolet; }
        }
        /// <summary>
        /// Gets or sets a bool value indicating whether the control should supply its own context menu.
        /// </summary>
        public bool UseDefaultContextMenu
        {
            get { return _defCtxMenu; }
            set { _defCtxMenu = value; }
        }
        public override System.Windows.Forms.ContextMenu ContextMenu
        {
            get { return base.ContextMenu; }
            set
            {
                base.ContextMenu = value;
                // If the user specifies a context menu, obviously, they're not
                //   going to be using the default one.
                if (value != null)
                    this.UseDefaultContextMenu = false;
            }
        }
        public string AutoCompleteConnectionString
        {
            get { return this._connStr; }
            set
            {
                this._refTables.Clear();
                this._refTablesAlias.Clear();
                this._refTablesCols.Clear();
                this._connStr = value;
                if (!string.IsNullOrEmpty(this._connStr) && this._autoComp)
                    this.InitAutoComplete();
            }
        }
        public bool AutoCompleteColumnNames
        {
            get { return this._autoComp; }
            set
            {
                this._autoComp = value;
                if (this._autoComp && !string.IsNullOrEmpty(this._connStr))
                    this.InitAutoComplete();
            }
        }
        public bool IsInsertMode
        {
            get { return this._insMode; }
            set
            {
                if (this._insMode != value)
                {
                    this._insMode = value;
                    this.OnInsertModeChanged(EventArgs.Empty);
                }
            }
        }
        public List<String> AutoCompleteSchemaNames
        {
            get { return this._autoCompleteSchemas; }
        }
        public List<String> AutoCompleteTableNames
        {
            get { return this._autoCompleteTables; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        //
        public QueryTextBox()
            : base()
        {
            _init = true;
            InitDefaultMenu();
            //InitRtfHeader();
            this.AllowDrop = true;
            this.EnableAutoDragDrop = true;
            this.ShowSelectionMargin = true;
            this._bldChars = new List<Int32>(5);
            this._refTables = new StringCollection();
            this._refTables.ReturnNullForKeyNotFound = true;
            this._refTablesAlias = new StringCollection();
            this._refTablesAlias.ReturnNullForKeyNotFound = true;
            this._refTablesCols = new StringCollection();
            this._refTablesCols.ReturnNullForKeyNotFound = true;
            this._init = false;
            this._autoComp = false;
            this._insMode = true;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void BeginParse(string qryText)
        { this.Text = qryText; this.BeginParse(); }
        public void BeginParse()
        {
            this._parsing = true;
            this._canEdit = !this.ReadOnly;
            this.ReadOnly = true;
            DoParseDelegate del = new DoParseDelegate(this.DoParse);
            del.BeginInvoke(0, this.Text.Length, new AsyncCallback(this.DoParseCallback), del);
        }
        public void Parse(string qryText)
        { this.Text = qryText; this.Parse(); }
        public void Parse()
        {
            // Prevent the control from scroll or redrawing itself.
            this._parsing = true;
            base.SuspendRefresh();
            base.SuspendScroll();
            this.SuspendLayout();

            // Save the current carrat and scroll bar positions.
            int selStart = this.SelectionStart,
                selLength = this.SelectionLength;
            int hScrPos = this.HorizontalScrollPos,
                vScrPos = this.VerticalScrollPos;

            try
            {
                if (this.Visible && !_init && this.TextLength > 0)
                    this.DoParse(0, this.Text.Length);
            }
            finally
            {
                // Set the carat to the end of whatever we just highlighted and set the
                //   color back to the control's ForeColor.  This keeps us from
                //   continuing to type in the color of whatever we just entered.
                if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                    this.Select(selStart + selLength, -selLength);
                else
                    this.Select(selStart, selLength);
                if (selLength == 0)
                    this.SelectionColor = this.ForeColor;
                this._parsing = false;

                // Allow the control to resume drawing and scrolling.
                this.VerticalScrollPos = vScrPos;
                this.HorizontalScrollPos = hScrPos;
                this.ResumeLayout(true);
                base.ResumeScroll();
                base.ResumeRefresh();
                this.OnTextParsed();
            }
        }
        public void InsertText(string txt)
        {
            this._parsing = true;
            try
            {
                //// If text is highlighted, we're replacing it, so wipe out the
                ////   selected text first.
                string oldText = this.SelectedText;
                //if (this.SelectionLength > 0)
                //    this.Text.Remove(this.SelectionStart, this.SelectionLength);
                int selStart = this.SelectionStart;
                int selLength = this.SelectionLength;

                txt = txt.Replace("\n\r", "\n").Replace("\r", "\n");
                this.AddUndo(new UndoState(UndoStateType.Insert, "List Insert", txt, oldText, this.SelectionStart));
                //this.SelectionLength = 0;
                //int iStartln = this.CurrentLine;
                //if (string.IsNullOrEmpty(this.Text))
                //    this.Text = txt;
                //else if (this.SelectionStart == this.Text.Length)
                //    this.Text += txt;
                //else if (this.SelectionStart == 0)
                //    this.Text = txt + this.Text;
                //else
                //    this.Text = this.Text.Substring(0, this.SelectionStart) + txt + this.Text.Substring(this.SelectionStart);
                ////this.DoParse(this.GetFirstCharIndexFromLine(iStartln), txt.Length);
                //try
                //{ this.SelectionStart = selStart + txt.Length - oldText.Length; }
                //catch { }
                //this.Parse();
                this.SelectedText = txt;
                this.DoParse(selStart, txt.Length);
                this.Select(selStart, txt.Length);
            }
            catch
            { throw; }
            finally
            {
                this._parsing = false;
            }
        }
        public void InsertText(string txt, int charPos)
        { this.InsertText(txt, charPos, true); }
        public void InsertText(string txt, int charPos, bool autoSel)
        {
            this._parsing = true;
            try
            {
                int selStart = this.SelectionStart,
                    selLength = this.SelectionLength;
                //this.SelectionLength = 0;
                //this.SelectionStart = charPos;
                this.Select(charPos, 0);

                this.InsertText(txt);

                if (autoSel)
                {
                    this.Select(charPos, txt.Length);
                    //this.SelectionStart = charPos;
                    //this.SelectionLength = txt.Length;
                }
                else
                {
                    if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                        this.Select(selStart + selLength, -selLength);
                    else
                        this.Select(selStart, selLength);
                    //this.SelectionStart = selStart;
                    //this.SelectionLength = selLength;
                }
            }
            catch
            { throw; }
            finally
            {
                this._parsing = false;
            }
        }
        #region DEPRECIATED :: Paste()
        //public new void Paste()
        //{
        //    //this.InsertText(System.Windows.Forms.Clipboard.GetData(System.Windows.Forms.DataFormats.Text).ToString());
        //    this._parsing = true;
        //    try
        //    {
        //        int
        //            iLnCnt = this.Lines.Length,
        //            iStrtLn = this.GetLineFromCharIndex(this.SelectionStart);

        //        base.Paste(System.Windows.Forms.DataFormats.GetFormat(System.Windows.Forms.DataFormats.Text));
        //        if (this.Lines.Length > iLnCnt)
        //        {
        //            int startChr = this.GetFirstCharIndexFromLine(iStrtLn),
        //                endLn = this.GetFirstCharIndexFromLine(((this.Lines.Length - 1) - iLnCnt) + iStrtLn),
        //                endChr = endLn + this.Lines[this.GetLineFromCharIndex(endLn)].Length;
        //            this.DoParse(startChr, endChr);
        //        }
        //        else if (this.Lines.Length == iLnCnt)
        //            this.ParseCurrentLine();
        //        else
        //            Parse();
        //        this.InvokeTextPaste();
        //    }
        //    catch (Exception ex)
        //    { System.Windows.Forms.MessageBox.Show(this, "Unable to paste text data:\n\n" + ex.Message + "\n\nApplication Version: " + System.Windows.Forms.Application.ProductVersion, "Error"); }
        //    finally
        //    { this._parsing = false; }
        //}
        #endregion
        public void Print()
        {
            this.InitPrintDocument();
            using (System.Windows.Forms.PrintDialog dlg = new System.Windows.Forms.PrintDialog())
            {
                dlg.AllowSomePages = false;
                dlg.AllowSelection = false;
                dlg.AllowPrintToFile = false;
                dlg.AllowCurrentPage = false;
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    this._printDoc.PrinterSettings = dlg.PrinterSettings;
                    this._printDoc.Print();
                }
            }
        }
        public new void Print(string docName)
        { this.Print(); }
        public new void PrintPreview()
        {
            using (System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog())
                this.PrintPreview(dlg);
        }
        public void PrintPreview(System.Windows.Forms.PrintPreviewDialog dlg)
        {
            this.InitPrintDocument();
            dlg.Document = this._printDoc;
            dlg.ShowDialog(this.FindForm());
        }
        public void SetAutoCompleteTableNames(string[] tableNms)
        {
            this._autoCompleteTables.Clear();
            for (int i = 0; i < tableNms.Length; i++)
                if (!this._autoCompleteTables.Contains(tableNms[i]))
                    this._autoCompleteTables.Add(tableNms[i]);
        }
        public void SetAutoCompleteSchemaNames(string[] schemaNms)
        {
            this._autoCompleteSchemas.Clear();
            for (int i = 0; i < schemaNms.Length; i++)
                if (!this._autoCompleteSchemas.Contains(schemaNms[i]))
                    this._autoCompleteSchemas.Add(schemaNms[i]);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void InitDefaultMenu()
        {
            this._defMenu = new System.Windows.Forms.ContextMenuStrip();
            this._defMenu.Name = "mnuQryContext";
            this._defMenu.Opening += new CancelEventHandler(contextMenu_onOpening);
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Undo", null, new EventHandler(this.contextMenu_onClick), "mnuUndo"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Cut", null, new EventHandler(this.contextMenu_onClick), "mnuCut"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Copy", null, new EventHandler(this.contextMenu_onClick), "mnuCopy"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Paste", null, new EventHandler(this.contextMenu_onClick), "mnuPaste"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Delete", null, new EventHandler(this.contextMenu_onClick), "mnuDelete"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Insert List", null, new EventHandler(this.contextMenu_onClick), "mnuInsList"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Copy for Code", null, new EventHandler(this.contextMenu_onClick), "mnuCopySl"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            //this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Font", null, new EventHandler(this.contextMenu_onClick), "mnuFont"));
            //this._defMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Print", null, new EventHandler(this.contextMenu_onClick), "mnuPrint"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Print Preview", null, new EventHandler(this.contextMenu_onClick), "mnuPrintPreview"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Load File", null, new EventHandler(this.contextMenu_onClick), "mnuLoad"));
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            this._defMenu.Items.Add(new System.Windows.Forms.ToolStripMenuItem("Save As File", null, new EventHandler(this.contextMenu_onClick), "mnuSaveFile"));
        }
        private string InitRtfHeader()
        {
            // Parse the text colors into their RTF string equivalents.
            string clrComment = "\\red" + _clrComment.R.ToString() + "\\green" + _clrComment.G.ToString() + "\\blue" + _clrComment.B.ToString();
            string clrAlias = "\\red" + _clrAlias.R.ToString() + "\\green" + _clrAlias.G.ToString() + "\\blue" + _clrAlias.B.ToString();
            string clrString = "\\red" + _clrString.R.ToString() + "\\green" + _clrString.G.ToString() + "\\blue" + _clrString.B.ToString();
            string clrKeyword = "\\red" + _clrKeyword.R.ToString() + "\\green" + _clrKeyword.G.ToString() + "\\blue" + _clrKeyword.B.ToString();

            // Build the header for the RTF string data.
            string rtfString = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 "
                + this.Font.Name + ";}}\r\n{\\colortbl ;" + clrComment + ";" + clrAlias + ";" + clrString + ";" + clrKeyword + ";}"
                + "\r\n\\viewkind4\\uc1\\pard\\f0\\fs" + Convert.ToString(System.Math.Round(this.Font.SizeInPoints * 2)) + " ";

            return rtfString;
        }
        private void InitPrintDocument()
        {
            if (this._printDoc != null)
                this._printDoc.Dispose();
            this._printDoc = new System.Drawing.Printing.PrintDocument();
            this._printDoc.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument_onBeginPrint);
            this._printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_onPrintPage);
        }
        private void InitAutoComplete()
        {
            this.ParseTables();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this._connStr))
            {
                conn.Open();
                int openAttempt = 0;
                while (conn.State != System.Data.ConnectionState.Open && openAttempt++ < 5)
                {
                    System.Threading.Thread.Sleep(1200);
                    conn.Open();
                }
                if (conn.State != System.Data.ConnectionState.Open)
                    return;

                if (_autoCompleteSchemas == null)
                    this._autoCompleteSchemas = new List<string>();
                else
                    this._autoCompleteSchemas.Clear();

                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SELECT name FROM sys.schemas ORDER BY name", conn))
                using (System.Data.SqlClient.SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        this._autoCompleteSchemas.Add(rdr.GetValue(0).ToString());

                if (this._autoCompleteTables == null)
                    this._autoCompleteTables = new List<string>();
                else
                    this._autoCompleteTables.Clear();

                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SELECT name FROM sys.sysobjects WHERE xtype = N'U' OR xtype = N'V' ORDER BY name", conn))
                using (System.Data.SqlClient.SqlDataReader rdr = cmd.ExecuteReader())
                    while (rdr.Read())
                        this._autoCompleteTables.Add(rdr.GetValue(0).ToString());
            }
        }
        private void DoParse(int start, int length)
        { this.DoParse(start, length, true); }
        private void DoParse(int start, int length, bool doMLComments)
        {
            // If we got passed an invalid length, just abort rather
            //   than throw an exception.
            if (start < 0 || length < 1 || (start + length) > this.TextLength)
                return;

            int selStart = this.SelectionStart,
                selLength = this.SelectionLength;

            try
            {
                string strCurLine = this.Text.Substring(start, length);

                this.Select(start, length);
                this.SelectionColor = this.ForeColor;
                Match m;

                for (m = QueryTextParser.Instance.KeyWord.Match(strCurLine); m.Success; m = m.NextMatch())
                    this.SetColored(m.Index + start, m.Length, this._clrKeyword);

                for (m = QueryTextParser.Instance.Function.Match(strCurLine); m.Success; m = m.NextMatch())
                    this.SetColored(m.Index + start, m.Length, this._clrFunc);

                for (m = QueryTextParser.Instance.Variable.Match(strCurLine); m.Success; m = m.NextMatch())
                    this.SetColored(m.Index + start, m.Length, this._clrGblVar);

                //int iLitStart = start,
                //    lnNum = this.GetLineFromCharIndex(iLitStart);
                //while (iLitStart > 0 && this.Lines[lnNum].Contains("'"))
                //    iLitStart = this.GetFirstCharIndexFromLine(lnNum--);
                //int iLitEnd = this.Lines[lnNum++].Length + 1;
                //while (iLitEnd < this.Text.Length && this.Lines[lnNum].Contains("'"))
                //    iLitEnd = this.GetFirstCharIndexFromLine(lnNum++) + this.Lines[lnNum].Length;
                //for (m = this._rgxStrLiteral.Match(this.Text.Substring(iLitStart, iLitEnd - iLitStart)); m.Success; m = m.NextMatch())
                for (m = QueryTextParser.Instance.Literal.Match(strCurLine); m.Success; m = m.NextMatch())
                    this.SetColored(m.Index + start, m.Length, this._clrString);

                for (m = QueryTextParser.Instance.Comment.Match(strCurLine); m.Success; m = m.NextMatch())
                    this.SetColored(m.Index + start, m.Length, this._clrComment);

                //for (m = this._rgxAlias.Match(this.Text); m.Success; m = m.NextMatch())
                //    this.SetColored(m.Index + start, m.Length, this.AliasColor);

                if (doMLComments)
                    for (m = QueryTextParser.Instance.CommentML.Match(this.Text); m.Success; m = m.NextMatch())
                        this.SetColored(m.Index, m.Length, this._clrComment);
            }
            catch (Exception ex)
            {
                this.Text = ex.ToString();
            }
            finally
            {
                if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                    this.Select(selStart + selLength, -selLength);
                else
                    this.Select(selStart, selLength);
            }
            #region DEPRECIATED :: Old Method Involved Manually Building RTF data
            //try
            //{
            //    _parseRun++;

            //    string rtfString = InitRtfHeader();
            //    int hdrLen = rtfString.Length;

            //    foreach (string str in this.Lines)
            //        rtfString += str + "\\par\r\n";

            //    Match m;
            //    string strEnd = rtfString;

            //    // Parse for SQL Keywords.
            //    string pstr = ""; int lmPos = 0;
            //    for (m = this._rgxKeyWord.Match(strEnd, hdrLen); m.Success; m = m.NextMatch())
            //    {
            //        if (lmPos < m.Index)
            //            pstr += strEnd.Substring(lmPos, m.Index - lmPos);
            //        pstr += "\\cf4 " + m.Value + "\\cf0 ";
            //        lmPos = m.Index + m.Length;
            //    }
            //    if (lmPos < strEnd.Length)
            //        pstr += strEnd.Substring(lmPos);
            //    strEnd = pstr;

            //    // Parse for string literals.
            //    pstr = ""; lmPos = 0;
            //    for (m = this._rgxStrLiteral.Match(strEnd, hdrLen); m.Success; m = m.NextMatch())
            //    {
            //        if (lmPos < m.Index)
            //            pstr += strEnd.Substring(lmPos, m.Index - lmPos);
            //        pstr += "\\cf3 " + m.Value + "\\cf0 ";
            //        lmPos = m.Index + m.Length;
            //    }
            //    if (lmPos < strEnd.Length)
            //        pstr += strEnd.Substring(lmPos);
            //    strEnd = pstr;

            //    // Parse for table alias'
            //    pstr = ""; lmPos = 0;
            //    for (m = this._rgxAlias.Match(strEnd, hdrLen); m.Success; m = m.NextMatch())
            //    {
            //        if (lmPos < m.Index)
            //            pstr += strEnd.Substring(lmPos, m.Index - lmPos);
            //        pstr += "\\cf2 " + m.Value + "\\cf0 ";
            //        lmPos = m.Index + m.Length;
            //    }
            //    if (lmPos < strEnd.Length)
            //        pstr += strEnd.Substring(lmPos);
            //    strEnd = pstr;

            //    // Parse for comments
            //    pstr = ""; lmPos = 0;
            //    for (m = this._rgxComment.Match(strEnd, hdrLen); m.Success; m = m.NextMatch())
            //    {
            //        if (lmPos < m.Index)
            //            pstr += strEnd.Substring(lmPos, m.Index - lmPos);
            //        pstr += "\\cf1 " + m.Value + "\\cf0 ";
            //        lmPos = m.Index + m.Length;
            //    }
            //    if (lmPos < strEnd.Length)
            //        pstr += strEnd.Substring(lmPos);
            //    strEnd = pstr;

            //    rtfString = strEnd + "}\r\n";

            //    // Decrement the running parsers counter.
            //    _parseRun--;
            //    return rtfString;
            //}
            //catch (Exception ex)
            //{
            //    return ex.ToString();
            //}
            #endregion
        }
        private void SetColored(int start, int length, Color clr)
        {
            // Select the found element and set its for color.
            if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                this.Select(start + length, -length);
            else
                this.Select(start, length);
            this.SelectionColor = clr;
        }
        private void ParseCurrentLine()
        {
            // Prevent the control from scroll or redrawing itself.
            this._parsing = true;
            base.SuspendRefresh();
            base.SuspendScroll();
            this.SuspendLayout();

            // Save the current carrat and scroll bar positions.
            int selStart = this.SelectionStart,
                selLength = this.SelectionLength;
            int hScrPos = this.HorizontalScrollPos,
                vScrPos = this.VerticalScrollPos;

            try
            {
                int iCurLine = this.GetFirstCharIndexOfCurrentLine();
                int iEoLine = this.Lines[this.GetLineFromCharIndex(iCurLine)].Length;
                this.DoParse(iCurLine, iEoLine);
            }
            finally
            {
                // Set the carat to the end of whatever we just highlighted and set the
                //   color back to the control's ForeColor.  This keeps us from
                //   continuing to type in the color of whatever we just entered.
                if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                    this.Select(selStart + selLength, -selLength);
                else
                    this.Select(selStart, selLength);
                this.SelectionColor = this.ForeColor;
                this._parsing = false;

                // Allow the control to resume drawing and scrolling.
                this.VerticalScrollPos = vScrPos;
                this.HorizontalScrollPos = hScrPos;
                this.ResumeLayout(true);
                base.ResumeScroll();
                base.ResumeRefresh();
                this.OnTextParsed();
            }
        }
        private new void LoadFile(string fileName)
        {
            this._parsing = true;
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fs))
                    this.Text = sr.ReadToEnd();
                //this.InsertText(sr.ReadToEnd());
                this.Parse();
            }
            catch (Exception ex)
            { System.Windows.Forms.MessageBox.Show(this.FindForm(), "Unable to load text data:\n\n" + ex.Message + "\n\nApplication Version: " + System.Windows.Forms.Application.ProductVersion, "Error"); }
            finally
            { this._parsing = false; }
        }
        private void DoParseCallback(IAsyncResult state)
        {
            // Get the delegate object out of the thread-state object.
            DoParseDelegate del = (DoParseDelegate)state.AsyncState;
            //string retVal = del.EndInvoke(state);
            del.EndInvoke(state);

            CrossThreadUI.ExecSync = true;
            // Call 'EndInvoke' to get the return value and pass that to the
            //   'UpdateText' method for thread-safe update of the control's text property.
            //CrossThreadUI.SetPropertyValue(this, "Rtf", retVal);
            //System.Windows.Forms.Application.DoEvents();

            // Re-enable user editing and make the "Parsing..." text go away.
            CrossThreadUI.SetPropertyValue(this, "ReadOnly", !this._canEdit);
            this._parsing = false;
        }
        private void CommentSelected()
        { this.CommentText(this.SelectionStart, this.SelectionLength); }
        private void CommentText(int start, int len)
        { this.InsertLeadingString(start, len, "--"); }
        private void IndexSelected()
        { this.IndentText(this.SelectionStart, this.SelectionLength); }
        private void IndentText(int start, int len)
        { this.InsertLeadingString(start, len, "\t"); }
        private void InsertLeadingString(int start, int len, string txt)
        {
            //int selStart = this.SelectionStart,
            //    selLen = this.SelectionLength,
            int addedChar = 0;
            //int vScroll = this.VerticalScrollPos,
            //    hScroll = this.HorizontalScrollPos;

            this.SuspendRefresh();
            this.SuspendScroll();
            try
            {
                int lnNum = -1;
                StringBuilder sbText = new StringBuilder(this.Text);
                for (int i = start; i < (start + len + addedChar); i++)
                {
                    int curLn = this.GetLineFromCharIndex(i);
                    int frChar = this.GetFirstCharIndexFromLine(curLn);
                    if (curLn != lnNum && this.Lines[curLn].Length > 0)
                    {
                        //this.Text = this.Text.Substring(0, i) + txt + this.Text.Substring(i);
                        sbText.Insert(i, txt);
                        addedChar += txt.Length;
                        this.AddUndo(new UndoState(UndoStateType.Insert, "Insert String", txt, "", i));
                    }
                    lnNum = curLn;
                }
                this.Text = sbText.ToString();

                #region DEPRECIATED CODE
                //string newText = string.Empty;
                //int curPos = start;
                //int lnNum = this.GetLineFromCharIndex(curPos);
                //while (curPos < start + len + addedChar+1)
                //{
                //    if (this.Lines[lnNum].Length > 0)
                //    {
                //        //string lnVal = this.Lines[lnNum].Substring(0, this.Lines[lnNum].Length - this.Lines[lnNum].TrimStart().Length) + txt + this.Lines[lnNum].TrimStart();
                //        newText += txt + this.Lines[lnNum];
                //        addedChar += txt.Length;
                //    }
                //    newText += "\n";
                //    if (lnNum++ < this.Lines.Length)
                //        curPos += this.GetFirstCharIndexFromLine(lnNum);
                //    else
                //        break;
                //}

                //this.AddUndo(new UndoState(UndoStateType.Insert, newText, this.Text.Substring(start, len), start));

                //if (start > 0)
                //    newText = this.Text.Substring(0, start) + newText;

                //if (start + len < this.Text.Length)
                //    newText += this.Text.Substring(start + len);

                //this.Text = newText;
                #endregion
            }
            //catch(Exception ex)
            //{ }
            finally
            {
                //this.SelectionStart = selStart;
                //this.SelectionLength = selLen + addedChar;
                if (_selDir == AdvRichTextBox.SelectDirection.Backword)
                    this.Select(start + len + addedChar, -(len + addedChar));
                else
                    this.Select(start, len + addedChar);
                //this.VerticalScrollPos = vScroll;
                //this.HorizontalScrollPos = hScroll;
                this.ResumeRefresh();
                this.ResumeScroll();
            }
        }
        private void UncommentSelected()
        { this.UncommentText(this.SelectionStart, this.SelectionLength); }
        private void UncommentText(int start, int len)
        {
            this.RemoveLeadingString(start, len, "--");
            //int selStart = this.SelectionStart,
            //    selLen = this.SelectionLength,
            //    remChar = 0,
            //    vScroll = this.VerticalScrollPos;

            //this.LockRedraw(true);
            //try
            //{
            //    int lnNum = -1,
            //        charPos = start;
            //    bool eos = false;
            //    string text = "";
            //    if (charPos > 0)
            //        text = this.Text.Substring(0, charPos);
            //    while (!eos)
            //    {
            //        int eol = this.Text.IndexOf('\n', charPos);
            //        string curSel = this.Text.Substring(charPos, eol - charPos);
            //        remChar += curSel.Length - curSel.TrimStart('-').Length;
            //        text += curSel.TrimStart('-') + '\n';

            //        charPos = eol + 1;
            //        if (charPos > this.Text.Length || charPos > (start + len) - remChar)
            //            eos = true;
            //    }
            //    this.Text = text + this.Text.Substring(charPos);
            //}
            //catch { }
            //finally
            //{
            //    this.SelectionStart = selStart;
            //    this.SelectionLength = selLen - remChar;
            //    this.VerticalScrollPos = vScroll;
            //    this.LockRedraw(false);
            //}
        }
        private void UnindentSelected()
        { this.UnindentText(this.SelectionStart, this.SelectionLength); }
        private void UnindentText(int start, int len)
        { this.RemoveLeadingString(start, len, "\t"); }
        private void RemoveLeadingString(int start, int len, string txt)
        {
            int selStart = this.SelectionStart,
                selLen = this.SelectionLength,
                remChar = 0;
            //int vScroll = this.VerticalScrollPos,
            //    hScroll = this.HorizontalScrollPos;

            this.SuspendRefresh();
            this.SuspendScroll();
            try
            {
                int //lnNum = -1,
                    charPos = start;
                bool eos = false;
                string text = "";
                if (charPos > 0)
                    text = this.Text.Substring(0, charPos);
                while (!eos)
                {
                    int eol = this.Text.IndexOf('\n', charPos);
                    string curSel = this.Text.Substring(charPos, eol - charPos);
                    int remFromLn = 0;
                    if (curSel.TrimStart(' ', '\t').StartsWith(txt))
                        remFromLn = txt.Length;
                    remChar += remFromLn; //curSel.Length - curSel.TrimStart('-').Length;
                    int startPos = charPos + remFromLn + (curSel.Length - curSel.TrimStart(' ', '\t').Length);
                    if (startPos - remFromLn > charPos)
                        text += this.Text.Substring(charPos, startPos - charPos - remFromLn);
                    text += this.Text.Substring(startPos, eol - startPos) + '\n';

                    charPos = eol + 1;
                    if (charPos > this.Text.Length || charPos > (start + len) - remChar)
                        eos = true;
                }
                this.Text = text + this.Text.Substring(charPos);
            }
            catch { }
            finally
            {
                //this.SelectionStart = selStart;
                //this.SelectionLength = selLen - remChar;
                if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                    this.Select(selStart + selLen - remChar, -(selLen - remChar));
                else
                    this.Select(selStart, selLen - remChar);
                //this.VerticalScrollPos = vScroll;
                //this.HorizontalScrollPos = hScroll;
                this.ResumeScroll();
                this.ResumeRefresh();
            }
        }
        private void DrawMargin(Graphics g, Rectangle clip)
        {
            using (SolidBrush brush = new SolidBrush(leftMarginBgColor))
                g.FillRectangle(brush, 0, 0, leftMargin - 5, this.ClientSize.Height);
            g.DrawLine(Pens.Black, new Point(leftMargin - 5, 0), new Point(leftMargin - 5, this.ClientSize.Height));

            using (SolidBrush txtBrush = new SolidBrush(lineNumberColor))
            using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox))
            {
                int topLineNum = this.GetLineFromCharIndex(this.GetCharIndexFromPosition(new Point(2, 2)));
                format.LineAlignment = StringAlignment.Far;
                format.Alignment = StringAlignment.Far;
                for (int i = 0; i < this.ClientRectangle.Height; i++)
                    g.DrawString(
                        Convert.ToString(i + topLineNum + 1),
                        this.Font,
                        txtBrush,
                        new RectangleF(
                                new PointF(2.0f, (float)(i * (this.Font.Height + 1) + 2)),
                                new SizeF(
                                        (float)(leftMargin - 5),
                                        (float)this.Font.Height)),
                        format);
            }
        }
        private void ParseTables()
        {
            this._refTables.Clear();
            Match m = null;
            for (m = QueryTextParser.Instance.TableReference.Match(this.Text); m.Success; m = m.NextMatch())
            {
                string
                    dbKey = m.Groups["db"].Value,
                    scmKey = m.Groups["sch"].Value,
                    tblKey = m.Groups["tbl"].Value,
                    alsKey = m.Groups["als"].Value;
                if (!string.IsNullOrEmpty(tblKey))
                {
                    string tblVal = tblKey;

                    if (!string.IsNullOrEmpty(dbKey) && string.IsNullOrEmpty(scmKey))
                    {
                        // This is really the schema value in the db group slot.  Too
                        //   lazy to fix the RegEx ;)
                        tblVal = dbKey + "." + tblVal;
                    }
                    else if (!string.IsNullOrEmpty(scmKey))
                    {
                        // Append the table schema name in from of the table.
                        tblVal = scmKey + "." + tblVal;
                        if (!string.IsNullOrEmpty(dbKey))
                        {
                            // If we got a database qualifier too, append that also.
                            tblVal = dbKey + "." + tblVal;
                        }
                    }

                    this._refTables.AddUnique(tblVal);
                    if (this._refTablesAlias[alsKey] == null || this._refTablesAlias[alsKey] != tblVal)
                        this._refTablesAlias.Update(tblVal, alsKey);
                }
            }
        }
        private void ShowAutoComplete(string[] vals, AutoCompleteType actype)
        {
            Point csrLoc = this.GetPositionFromCharIndex(this.SelectionStart);
            if (this._autoCompMenu != null)
                this._autoCompMenu.Dispose();

            this._autoCompStart = this.SelectionStart;
            this._autoCompLine = this.CurrentLine;
            int mnuLocY = csrLoc.Y + this.Font.Height;
            this._autoCompMenu = new System.Windows.Forms.ListBox();
            this.Controls.Add(this._autoCompMenu);
            this._autoCompMenu.IntegralHeight = true;
            this._autoCompMenu.Height = (int)System.Math.Min(vals.Length, 20) * (this._autoCompMenu.ItemHeight + 2);

            // We now know how tall we *want* the dropdown list to be, we have to
            //   determine where we're going to actually draw the DropDownList and
            //   how tall the list will be.
            if (this.Bottom - mnuLocY >= mnuLocY)
            {
                // There's more space below the line.
                if (this._autoCompMenu.Height > this.Bottom - mnuLocY - 30)
                {
                    // The AutoCompleteMenu is taller than the available space, so we
                    //   have to shrink it to fit.
                    this._autoCompMenu.Height = this.Bottom - mnuLocY - 30;
                }
            }
            else
            {
                // There's more space above the line.
                if (this._autoCompMenu.Height > mnuLocY - this._autoCompMenu.Font.Height)
                {
                    // The AutoCompleteMenu is taller than the available space, so we
                    //   have to calculate how many entries will fit above the
                    //   current line since the dropdown will only list complete
                    //   items that can be completely drawn.
                    int rows = (mnuLocY - this._autoCompMenu.Font.Height) / (this._autoCompMenu.ItemHeight + 2);
                    this._autoCompMenu.Height = rows * (this._autoCompMenu.ItemHeight + 2);
                }
                // The top-most position of the menu will be it's height minus the
                //   height of a light, using the cursor as a base value.
                mnuLocY = csrLoc.Y - this._autoCompMenu.Font.Height - this._autoCompMenu.Height;
            }

            string longestWord = "";
            for (int i = 0; i < vals.Length; i++)
            {
                this._autoCompMenu.Items.Add(vals[i]);
                if (vals[i].Length > longestWord.Length)
                    longestWord = vals[i];
            }
            using (Graphics g = this._autoCompMenu.CreateGraphics())
                this._autoCompMenu.Width = (int)System.Math.Ceiling(g.MeasureString("W" + longestWord, this._autoCompMenu.Font).Width) + 18;

            // Can you believe that in all of this mess I forgot to handle the XPos
            //   for the last 2 years I've been using this auto-complete method?!
            // We'll just take whichever is lower: cursor position or the TextBox's
            //   right edge minus the menu width.
            int mnuLocX = (int)System.Math.Min(csrLoc.X, this.ClientRectangle.Right - this._autoCompMenu.Width);

            this._autoCompMenu.Location = new Point(mnuLocX, mnuLocY);
            this._autoCompMenu.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._autoCompMenu.ScrollAlwaysVisible = true;
            this._autoCompMenu.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._autoCompMenu.Click += new EventHandler(this.autoCompMenu_ItemClicked);
            this._autoCompMenu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.autoCompMenu_KeyPress);
            this._autoCompMenu.Tag = actype;
            this._autoCompMenu.Show();
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void contextMenu_onOpening(object sender, CancelEventArgs e)
        {
            if (this.SelectionLength > 0)
            {
                _defMenu.Items["mnuCut"].Enabled = true;
                _defMenu.Items["mnuCopy"].Enabled = true;
                _defMenu.Items["mnuDelete"].Enabled = (!this.ReadOnly);
                _defMenu.Items["mnuCopySl"].Enabled = true;
            }
            else
            {
                _defMenu.Items["mnuCut"].Enabled = false;
                _defMenu.Items["mnuCopy"].Enabled = false;
                _defMenu.Items["mnuDelete"].Enabled = false;
                _defMenu.Items["mnuCopySl"].Enabled = false;
            }

            if (System.Windows.Forms.Clipboard.ContainsText())
                _defMenu.Items["mnuPaste"].Enabled = (!this.ReadOnly);
            else
                _defMenu.Items["mnuPaste"].Enabled = false;

            if (this.CanUndo)
                _defMenu.Items["mnuUndo"].Enabled = (!this.ReadOnly);
            else
                _defMenu.Items["mnuUndo"].Enabled = false;
        }
        private void contextMenu_onClick(object sender, EventArgs e)
        {
            switch (((System.Windows.Forms.ToolStripMenuItem)sender).Name)
            {
                case "mnuUndo":
                    break;
                case "mnuCut":
                    this.Cut();
                    break;
                case "mnuCopy":
                    this.Copy();
                    break;
                case "mnuCopySl":
                    string cpyTxt = (this.SelectionLength > 0) ? this.SelectedText : this.Text;
                    List<string> sql = new List<string>(cpyTxt.Split('\n'));
                    for (int i = 0; i < sql.Count; i++)
                        sql[i] = sql[i].Trim() + ((sql[i].EndsWith(",")) ? "" : " ");
                    System.Windows.Forms.Clipboard.SetText(RainstormStudios.Data.rsData.DeCommentQuery(sql.ToArray(), true));
                    break;
                case "mnuPaste":
                    this.Paste();
                    break;
                case "mnuDelete":
                    if (this.SelectionLength == 0)
                    {
                        if (this.SelectionStart == this.Text.Length)
                            this.SelectionStart--;
                        this.SelectionLength = 1;
                    }
                    this.AddUndo(new UndoState(UndoStateType.Delete, "", this.SelectedText, this.SelectionStart));
                    this.SelectedText = "";
                    break;
                case "mnuInsList":
                    using (RainstormStudios.Forms.frmCSVList frm = new RainstormStudios.Forms.frmCSVList())
                        if (frm.ShowDialog(this.FindForm()) == System.Windows.Forms.DialogResult.OK)
                            this.InsertText(frm.GetList());
                    break;
                case "mnuFont":
                    using (System.Windows.Forms.FontDialog dlg = new System.Windows.Forms.FontDialog())
                    {
                        dlg.FontMustExist = true;
                        dlg.ShowApply = false;
                        dlg.ShowColor = false;
                        dlg.ShowEffects = false;
                        if (dlg.ShowDialog(this.FindForm()) == System.Windows.Forms.DialogResult.OK)
                            this.Font = dlg.Font;
                    }
                    break;
                case "mnuSaveFile":
                    using (System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog())
                    {
                        dlg.AddExtension = true;
                        dlg.DefaultExt = ".sql";
                        dlg.Filter = "SQL Query Files|*.sql|Text Files|*.txt|All Files|*.*";
                        dlg.FilterIndex = 0;
                        dlg.OverwritePrompt = true;
                        dlg.Title = "Save Query to File";
                        dlg.ValidateNames = true;
                        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                            this.SaveFile(dlg.FileName, System.Windows.Forms.RichTextBoxStreamType.PlainText);
                    }
                    break;
                case "mnuLoad":
                    using (System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog())
                    {
                        dlg.AddExtension = true;
                        dlg.CheckFileExists = true;
                        dlg.CheckPathExists = true;
                        dlg.DefaultExt = ".sql";
                        dlg.Filter = "Text Query Files|*.txt;*.sql|SQL Query Files|*.sql|Text Files|*.txt|All Files|*.*";
                        dlg.FilterIndex = 0;
                        dlg.Multiselect = false;
                        dlg.SupportMultiDottedExtensions = true;
                        dlg.Title = "Select Query File to Load";
                        dlg.ValidateNames = true;
                        if (dlg.ShowDialog(this.FindForm()) == System.Windows.Forms.DialogResult.OK)
                            this.LoadFile(dlg.FileName);
                    }
                    break;
                case "mnuPrint":
                    this.Print();
                    break;
                case "mnuPrintPreview":
                    this.PrintPreview();
                    break;
            }
        }
        private void printDocument_onBeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            this._lastPrintLine = 0;
        }
        private void printDocument_onPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string pgTxt = "";
            this.SuspendRefresh();
            this.SuspendScroll();
            int stChar = this.GetFirstCharIndexFromLine(this._lastPrintLine),
                stLn = this._lastPrintLine,
                selStart = this.SelectionStart,
                selLength = this.SelectionLength;
            try
            {
                using (StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit))
                {
                    int lnCnt = this.Lines.Length;
                    //SizeF lnNumSz = e.Graphics.MeasureString(lnCnt.ToString(), this.Font);
                    SizeF lnNumSz = e.Graphics.MeasureString("0000", this.Font);
                    float numMarginWidth = lnNumSz.Width + 2.0f;

                    while (e.Graphics.MeasureString(pgTxt, this.Font, new Size(e.MarginBounds.Size.Width - (int)numMarginWidth, e.MarginBounds.Size.Height), fmt).Height < e.MarginBounds.Height - (e.MarginBounds.Top / 2) && this._lastPrintLine < this.Lines.Length)
                        pgTxt += ((pgTxt.Length > 0) ? "\n" : "") + this.Lines[this._lastPrintLine++];

                    bool newLine = true;
                    PointF nextCharPos = new PointF((float)e.MarginBounds.Left + numMarginWidth, (float)e.MarginBounds.Top);
                    RectangleF numMarginBounds = new RectangleF((float)e.MarginBounds.Left, (float)e.MarginBounds.Top, numMarginWidth, (float)e.MarginBounds.Height);
                    using (SolidBrush numBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
                        e.Graphics.FillRectangle(numBrush, numMarginBounds);
                    for (int i = 0; i < pgTxt.Length; i++)
                    {
                        this.SelectionStart = stChar + i;
                        if (newLine)
                        {
                            RectangleF numRect = new RectangleF(e.MarginBounds.Left, nextCharPos.Y, numMarginWidth, lnNumSz.Height);
                            using (StringFormat format = new StringFormat(StringFormatFlags.NoWrap))
                            {
                                format.Alignment = StringAlignment.Far;
                                e.Graphics.DrawString(Convert.ToString(this.GetLineFromCharIndex(stChar + i) + 1), this.Font, Brushes.DarkGreen, numRect, format);
                            }
                        }
                        string nextChar = pgTxt.Substring(i, 1);
                        if (nextChar != "\n")
                        {
                            SelectionLength = 1;
                            Color curCol = this.SelectionColor;
                            SizeF charSz = e.Graphics.MeasureString(nextChar, this.Font);
                            if (nextCharPos.X + (charSz.Width / 1.5) > (e.MarginBounds.Width / 0.9))
                                nextCharPos = new PointF((float)e.MarginBounds.Left + numMarginWidth, nextCharPos.Y + (charSz.Height / 1.105f));
                            using (SolidBrush brush = new SolidBrush(curCol))
                                e.Graphics.DrawString(nextChar, this.Font, brush, nextCharPos);
                            nextCharPos = new PointF(nextCharPos.X + charSz.Width / ((nextChar != " ") ? 1.5f : 0.9f), nextCharPos.Y);
                            newLine = false;
                        }
                        else
                        {
                            newLine = true;
                            SizeF charSz = e.Graphics.MeasureString("W", this.Font);
                            nextCharPos = new PointF((float)e.MarginBounds.Left + numMarginWidth, nextCharPos.Y + (charSz.Height / 1.105f));
                        }
                    }
                    //e.Graphics.DrawString(pgTxt, this.Font, Brushes.Black, e.MarginBounds);
                    e.HasMorePages = (this._lastPrintLine < this.Lines.Length);

                    if (!e.HasMorePages)
                    {
                        e.Graphics.FillRectangle(Brushes.White, new RectangleF(0.0f, nextCharPos.Y + lnNumSz.Height, e.PageBounds.Width, e.MarginBounds.Bottom - (nextCharPos.Y + lnNumSz.Height)));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, "Unable to generate page for printing:\n\n" + ex.Message, "Unexpected Error");
            }
            finally
            {
                this.SelectionStart = selStart;
                this.SelectionLength = selLength;
                this.ResumeScroll();
                this.ResumeRefresh();
            }
        }
        private void autoCompMenu_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            this.Focus();
            this.InsertText(e.KeyChar.ToString());
            //for (int i = 0; i < this._autoCompMenu.Items.Count; i++)
            //    if (this._autoCompMenu.Items[i].Selected)
            //        this._autoCompMenu.Items[i].Select();
        }
        private void autoCompMenu_ItemClicked(object sender, EventArgs e)
        {
            try
            {
                this.SuspendRefresh();
                this._parsing = true;
                int selStart = this.SelectionStart,
                    selLength = this.SelectionLength,
                    dotIdx = this.Text.Substring(0, this.SelectionStart + this.SelectionLength).LastIndexOf('.');

                if (this._autoCompMenu.Tag.ToString() == "Table")
                {
                    string[] tblNm = this._autoCompMenu.Items[this._autoCompMenu.SelectedIndex].ToString().Split('.');
                    int qualSt = this.Text.Substring(0, dotIdx).LastIndexOf(' ') + 1;
                    this.Select(qualSt, selStart + selLength - qualSt);
                    if (tblNm[1].EndsWith("()"))
                    {
                        // Function
                        string newText = string.Format("[{0}].{1}", tblNm[0], tblNm[1]);
                        this.SelectedText = newText;
                        this.Select(qualSt + newText.Length - 1, 0);
                    }
                    else if (tblNm[1].EndsWith(" SP"))
                        // Stored Proceedure
                        this.SelectedText = string.Format("EXEC [{0}].{1} ", tblNm[0], tblNm[1]);
                    else
                        // Table or View
                        this.SelectedText = string.Format("[{0}].[{1}]", tblNm[0], tblNm[1]);
                }
                else
                {
                    this.Select(dotIdx, selStart + selLength - dotIdx);
                    this.SelectedText = string.Format(".[{0}]", this._autoCompMenu.Items[this._autoCompMenu.SelectedIndex].ToString());
                }
            }
            finally
            {
                this._parsing = false;
                this.ResumeRefresh();
            }
            this.EndAutoComplete();
        }
        private void EndAutoComplete()
        {
            if (this._autoCompMenu != null && !this._autoCompMenu.IsDisposed)
                this._autoCompMenu.Dispose();
        }
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnTextChanged(EventArgs e)
        {
            if (!_parsing)
            {
                base.OnTextChanged(e);
                if (this.Text.Length > 0)
                    this.ParseCurrentLine();
                //    else if (this._lastParseLine - this.CurrentLine == 1)
                //        this.DoParse(this.GetFirstCharIndexFromLine(this.CurrentLine - 1), this.Lines[this.CurrentLine - 1].Length);
                //    else
                //        this.DoParse(0, this.Text.Length);
                //this._lastParseLine = this.CurrentLine;
                //this.ParseTables();
            }
        }
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            // If we were just sitting next to a bolded parenthesis, we'll
            //   carry the bold with us because the "SelectionChanged" event
            //   does not appear to fire as we type.
            if (this.SelectionLength == 0 && this.SelectionFont != this.Font)
                this.SelectionFont = this.Font;

            if (e.KeyCode == System.Windows.Forms.Keys.C && e.Modifiers == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
            {
                #region CTRL-SHIFT-C
                this._parsing = true;
                try
                {
                    this.CommentSelected();
                    this.Parse();
                }
                finally
                {
                    this._parsing = false;
                    e.Handled = true;
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.U && e.Modifiers == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
            {
                #region CTRL-SHIFT-U
                this._parsing = true;
                try
                {
                    this.UncommentSelected();
                    this.Parse();
                }
                finally
                {
                    this._parsing = false;
                    e.Handled = true;
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Tab)
            {
                #region TAB Key
                this.EndAutoComplete();

                if (e.Modifiers == System.Windows.Forms.Keys.Shift)
                {
                    if (this.SelectionLength > 0)
                    {
                        // Unindent all selected lines.
                        this._parsing = true;
                        try
                        {
                            this.UnindentSelected();
                            this.Parse();
                        }
                        //catch (Exception ex)
                        //{ }
                        finally
                        {
                            this._parsing = false;
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        // Remove a single TAB char.
                        int iLnSt = this.GetFirstCharIndexOfCurrentLine();
                        this._parsing = true;
                        this.SuspendRefresh();
                        this.SuspendScroll();
                        try
                        {
                            if (this.SelectionStart > iLnSt && this.Text.Substring(this.SelectionStart - 1, 1) == "\t")
                            {
                                //int selStart = this.SelectionStart;
                                //string text = string.Empty;
                                //if (this.SelectionStart - 1 > 0)
                                //    text = this.Text.Substring(0, this.SelectionStart - 1);
                                //text += this.Text.Substring(this.SelectionStart);
                                //this.Parse(text);
                                //this.SelectionStart = selStart - 1;

                                // If we simulate a keypress rather then actually
                                //   alter the control's "Text" property, then we
                                //   don't have to parse the entire block of text
                                //   for color.
                                System.Windows.Forms.Message msg = new System.Windows.Forms.Message();
                                msg.HWnd = new IntPtr(5572486);
                                msg.Msg = (int)Win32Messages.WM_CHAR;
                                msg.LParam = new IntPtr(917505);
                                // We're passing a "Backspace" key instead of the tab.
                                msg.WParam = new IntPtr(8);
                                this.WndProc(ref msg);
                            }
                        }
                        finally
                        {
                            this.ResumeRefresh();
                            this.ResumeScroll();
                            this._parsing = false;
                            e.Handled = true;
                        }
                    }
                }
                else if (this.SelectionLength > 0)
                {
                    // Indent all selected lines.
                    this._parsing = true;
                    try
                    {
                        this.IndexSelected();
                        this.DoParse(this.SelectionStart, this.SelectionLength);
                    }
                    //catch (Exception ex)
                    //{ }
                    finally
                    {
                        this._parsing = false;
                        e.Handled = true;
                    }
                }
                // If we fall out completely, it's just a standard TAB key-press, so
                //   we don't need to do anything.
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Enter || e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                #region ENTER / SPACE Key
                int iLn = this.GetLineFromCharIndex(this.SelectionStart);
                bool inAutoComplete = this._autoCompMenu != null && this._autoCompMenu.SelectedIndex > -1;
                try
                {
                    // First thing we want to do is check to see if we were in the
                    //   middle of an "Auto Complete" operation.
                    if (inAutoComplete)
                    {
                        // If the AutoComplete list is open and has an item selected,
                        //   we need to insert the selected text instead of
                        //   inserting the carriage return.
                        this.SuspendRefresh();
                        this.SuspendScroll();
                        this._parsing = true;
                        int selStart = this.SelectionStart,
                            selLength = this.SelectionLength,
                            dotIdx = this.Text.Substring(0, this.SelectionStart + this.SelectionLength).LastIndexOf('.');

                        if (this._autoCompMenu.Tag.ToString() == "Table")
                        {
                            string[] tblNm = this._autoCompMenu.Items[this._autoCompMenu.SelectedIndex].ToString().Split('.');
                            int qualSt = this.Text.Substring(0, dotIdx).LastIndexOf(' ') + 1;
                            this.Select(qualSt, selStart + selLength - qualSt);
                            if (tblNm[1].EndsWith("()"))
                            {
                                // Function
                                string newText = string.Format("[{0}].{1}", tblNm[0], tblNm[1]);
                                this.SelectedText = newText;
                                this.Select(qualSt + newText.Length - 1, 0);
                            }
                            else if (tblNm[1].EndsWith(" SP"))
                                // Stored Proceedure
                                this.SelectedText = string.Format("EXEC [{0}].{1} ", tblNm[0], tblNm[1]);
                            else
                                // Table or View
                                this.SelectedText = string.Format("[{0}].[{1}]", tblNm[0], tblNm[1]);
                        }
                        else
                        {
                            this.Select(dotIdx, selStart + selLength - dotIdx);
                            this.SelectedText = string.Format(".[{0}]", this._autoCompMenu.Items[this._autoCompMenu.SelectedIndex].ToString());
                        }

                        // We only want to suppress the key press if they pushed "ENTER", not "SPACE".
                        if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    }

                    // We *always* want to terminate the autocomplete menu at this 
                    //   point, if it's active.  Either we just did an autocomplete
                    //   or the user made a keypress which bypassed the auto-
                    //   complete.  Either way, we're done with it.
                    this.EndAutoComplete();

                    //// If we weren't just doing an auto-complete, pressing space bar
                    ////   should start one, since we're not waiting for a '.' key
                    ////   press anymore.
                    //if (!inAutoComplete)
                    //    this.ShowAutoComplete(this._autoCompleteTables.ToArray(), AutoCompleteType.Table);

                    #region New Line - Tab-In moved to "KeyUp" event handler
                        //if (!e.SuppressKeyPress)
                        //{
                        //    // When the user hits the return key, we want to keep the
                        //    //   cursor "tabbed" in to match the previous line.
                        //    if (this.Lines.Length > 1)
                        //    {
                        //        string lnTxt = this.Lines[iLn];
                        //        if (lnTxt.StartsWith("\t") || lnTxt.StartsWith(" "))
                        //        {
                        //            // If the current line (we haven't actually processed the
                        //            //   carriage-return yet) has any whitespace, we want to make
                        //            //   the next line have the same leading whitespace.
                        //            string newTxt = ("\n" + lnTxt.Substring(0, lnTxt.Length - lnTxt.TrimStart('\t', ' ').Length));
                        //            this.InsertText(newTxt, this.SelectionStart, false);
                        //            this.Select(this.SelectionStart + newTxt.Length, 0);
                        //            // Tell the base event that we processed this key manually and
                        //            //   not to paste the 'newline' character
                        //            e.Handled = true;
                        //        }
                        //    }
                        //}
                        #endregion
                }
                finally
                {
                    this._parsing = false;
                    this.ResumeScroll();
                    this.ResumeRefresh();
                }

                // When the user hits the enter key, we want to parse the line
                //   we just left.  Keep in mind that this method is called
                //   before the NewLine character actually moves the cursor
                //   to the next line.
                // We only need to do this parsing if the cursor was not at the
                //   beginning of the line we were on.
                // LOGIC: If the cursor was at the beginning of the line, then either
                //   no text was entered on that line, or we just moved any text
                //   on the line down to the next line.
                if (e.KeyCode == System.Windows.Forms.Keys.Enter && !e.SuppressKeyPress)
                {
                    int iSt = this.GetFirstCharIndexFromLine(iLn);
                    if (this.SelectionStart != this.GetFirstCharIndexFromLine(iLn))
                    {
                        //int iCnt = this.SelectionStart - iSt;
                        //this.DoParse(iSt, iCnt);
                        this.ParseCurrentLine();
                    }
                }
                #endregion
            }
            else if (e.KeyValue == 191)
            {
                #region SLASH Key
                // If the user closes a comment block /*  */ we need to make sure we
                //   haven't made anything after the closing block green already.
                if (this.SelectionStart > 0 && this.SelectionStart < this.Text.Length - 1 && this.Text.Substring(this.SelectionStart - 1, 1) == "*")
                {
                    int endBlock = this.Text.IndexOf("/*", this.SelectionStart + 1) - 1;
                    this.DoParse(this.SelectionStart + 1, ((endBlock > 0)
                                                            ? endBlock
                                                            : this.Text.Length)
                                            - (this.SelectionStart + 1), false);
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                #region DELETE Key
                //string cmpStr = this.Text.Substring(this.SelectionStart, 2);
                //if (this.SelectionStart < (this.Text.Length - 2) && (cmpStr == "/*" || cmpStr == "*/"))
                //{
                //    int endBlock = this.Text.IndexOf("/*", this.SelectionStart + 2);
                //    this.DoParse(this.SelectionStart + 1, ((endBlock > 0) ? endBlock : this.Text.Length) - (this.SelectionStart + 1), false);
                //}

                // In case we just deleted a parenthesis, we need to "unbold"
                //   any characters we might have stored a reference to.
                int iSelStart = this.SelectionStart,
                    iSelLength = this.SelectionLength;
                this._parsing = true;
                try
                {
                    for (int i = 0; i < this._bldChars.Count; i++)
                    {
                        this.Select(this._bldChars[i], 1);
                        this.SelectionFont = this.Font;
                    }
                    this._bldChars.Clear();
                }
                catch { }
                finally
                {
                    if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                        this.Select(iSelStart + iSelLength, -iSelLength);
                    else
                        this.Select(iSelStart, iSelLength);
                    this._parsing = false;
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Back)
            {
                #region BACKSPACE Key
                //if (this.SelectionStart > 2)
                //{
                //    string cmpStr = this.Text.Substring(this.SelectionStart - 2, 2);
                //    if (this.SelectionStart < (this.Text.Length - 2) && (cmpStr == "/*" || cmpStr == "*/"))
                //    {
                //        int endBlock = this.Text.IndexOf("/*", this.SelectionStart);
                //        this.DoParse(this.SelectionStart, ((endBlock > 0) ? endBlock : this.Text.Length) - (this.SelectionStart), false);
                //    }
                //}
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                #region UP Key
                if (this._autoCompMenu != null && !this._autoCompMenu.IsDisposed)
                {
                    e.Handled = true;
                    if (this._autoCompMenu.SelectedIndex > 0)
                        this._autoCompMenu.SelectedIndex--;
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                #region DOWN Key
                if (this._autoCompMenu != null && !this._autoCompMenu.IsDisposed)
                {
                    e.Handled = true;
                    if (this._autoCompMenu.SelectedIndex < this._autoCompMenu.Items.Count - 1)
                        this._autoCompMenu.SelectedIndex++;
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Left)
            {
                #region LEFT Key
                if (this.SelectionStart <= this._autoCompStart || this.CurrentLine != this._autoCompLine)
                    this.EndAutoComplete();
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Right)
            {
                #region RIGHT Key
                if (this.CurrentLine != this._autoCompLine)
                    this.EndAutoComplete();
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Home)
            {
                #region HOME Key
                this.EndAutoComplete();

                // Move all this code to the AdvRichTextBox control class.

                //// If the user was holding the CTRL key when they pressed home, then
                ////   we just move to the very beginning of the document.
                //if (e.Control)
                //{
                //    if (e.Shift)
                //        //this.Select(0, this.SelectionStart + this.SelectionLength);
                //        this.Select(this.SelectionStart, 0 - this.SelectionStart);
                //    else
                //        this.Select(0, 0);
                //    e.Handled = true;
                //}
                //else
                //{
                //    // If the user presses the "HOME" key, then we want to move the
                //    //   cursor inteligently to either the beginning of the line,
                //    //   or the beginning of the actual text.
                //    int curLn = this.GetLineFromCharIndex(this.SelectionStart),
                //        curPos = this.SelectionStart,
                //        lnStart = this.GetFirstCharIndexFromLine(curLn);

                //    // If the current line doesn't begin with any whitespace, then we
                //    //   have nothing to do here.
                //    string lnTxt = this.Lines[curLn];
                //    if (lnTxt.StartsWith("\t") || lnTxt.StartsWith(" "))
                //    {
                //        int wsCharCnt = (lnTxt.Length - lnTxt.TrimStart('\t', ' ').Length),
                //            selLen = lnStart - (curPos + SelectionLength);
                //        if (this.SelectionStart == (lnStart + wsCharCnt))
                //        {
                //            // If we're at the beginning of the text, then move to the
                //            //   first of the line.
                //            if (e.Shift)
                //                //this.Select(lnStart, (curPos + this.SelectionLength) - lnStart);
                //                this.Select(curPos + this.SelectionLength, selLen);
                //            else
                //                this.Select(lnStart, 0);
                //        }
                //        else
                //        {
                //            // If the cursor is already at the beginning of the line,
                //            //   or anywhere else in the line, then move to the end
                //            //   of the whitespace.
                //            if (e.Shift)
                //                //this.Select(lnStart + wsCharCnt, (curPos - (lnStart + wsCharCnt)) + this.SelectionLength);
                //                this.Select(curPos + this.SelectionLength, selLen + wsCharCnt);
                //            else
                //                this.Select(lnStart + wsCharCnt, 0);
                //        }
                //        e.Handled = true;
                //    }
                //}
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.End)
            {
                #region END Key
                this.EndAutoComplete();

                // Moved this code to the "AdvRichTextBox" control class.

                //if (e.Control)
                //{
                //    // If the users isn't holding the CTRL key, then it's just a
                //    //   standard End-of-Line keypress & we'll let the baes class
                //    //   deal with it.
                //    if (e.Shift)
                //    {
                //        // User wants to select everything from here to the end of the document.
                //        this.Select(this.SelectionStart, this.Text.Length - this.SelectionStart);
                //    }
                //    else
                //    {
                //        // Just move the cursor to the very end of the document.
                //        this.Select(this.Text.Length, 0);
                //    }
                //    e.Handled = true;
                //}
                #endregion
            }
            else if (e.KeyValue == 56)
            {
                #region ASTRISK Key
                // The ASTRISK (*) will always close the AutoComplete menu,
                //   since this effectively means "All Fields".
                this.EndAutoComplete();
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F9)
            {
                this.Parse();
            }
            else if (e.KeyData != System.Windows.Forms.Keys.Shift && this.IsInputKey(e.KeyData) && !e.Control && !e.Alt)
            {
                #region Any Other Input Character
                if (this._autoCompMenu != null)
                {
                    if ((e.KeyValue >= 65 && e.KeyValue <= 90) || e.KeyValue == 189)
                    { }
                    else if (this._autoCompMenu.SelectedIndex > -1)
                    {
                        try
                        {
                            // If the AutoComplete list is open and has an item selected,
                            //   we need to insert the selected text followed by a 
                            //   single space character.
                            this.SuspendRefresh();
                            this._parsing = true;
                            int selStart = this.SelectionStart,
                                selLength = this.SelectionLength,
                                dotIdx = this.Text.Substring(0, this.SelectionStart + this.SelectionLength).LastIndexOf('.');

                            //System.Windows.Forms.KeysConverter kc = new System.Windows.Forms.KeysConverter();
                            this.Select(dotIdx, selStart + selLength - dotIdx);
                            this.SelectedText = string.Format(".[{0}]", this._autoCompMenu.Items[this._autoCompMenu.SelectedIndex].ToString());
                            this.EndAutoComplete();
                        }
                        finally
                        {
                            this._parsing = false;
                            this.ResumeRefresh();
                        }
                    }
                }
                #endregion
            }

            if (e.Handled)
                e.SuppressKeyPress = true;
            this._lastKeyPress = e.KeyData;
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                #region ENTER Key
                try
                {
                    if (!e.SuppressKeyPress)
                    {
                        // If the user just pressed the "Enter" key, we want to tab in the
                        //   new line to match any whitespace from the previous line.
                        int prevLn = this.GetLineFromCharIndex(this.SelectionStart);
                        string prevTxt = this.Lines[prevLn - 1];
                        string prevWs = string.Empty;
                        if (prevTxt.StartsWith("\t") || prevTxt.StartsWith(" "))
                            prevWs = prevTxt.Substring(0, prevTxt.Length - prevTxt.TrimStart('\t', ' ').Length);

                        // After a return keypress, we're always on a "new" line, so if we
                        //   find any whitespace on the previous line, just send a
                        //   simulated windows msg to make the control think the user pressed
                        //   the keys to recreate that whitespace on this line.
                        // Using this method of simulating the keypress prevents having to
                        //   "color parse" the entire text which is required if we manually
                        //   change the control's "Text" property.
                        if (!string.IsNullOrEmpty(prevWs))
                        {
                            char[] wsChars = prevWs.ToCharArray();
                            for (int i = 0; i < wsChars.Length; i++)
                            {
                                System.Windows.Forms.Message msg = new System.Windows.Forms.Message();
                                msg.Msg = (int)Win32Messages.WM_CHAR;
                                msg.HWnd = new IntPtr(5572486);
                                if (wsChars[i] == '\t')
                                {
                                    msg.WParam = new IntPtr(9);
                                    msg.LParam = new IntPtr(983041);
                                }
                                else if (wsChars[i] == ' ')
                                {
                                    msg.WParam = new IntPtr(32);
                                    msg.LParam = new IntPtr(3735553);
                                    IntPtr tst = new IntPtr();
                                }
                                else
                                    throw new Exception("Unexpected whitespace character encountered.");
                                this.WndProc(ref msg);
                            }
                        }
                    }
                }
                catch
                {
                    // This isn't important enough to let it throw errors,
                    //   especially since I added the WndProc fake keypress.
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                #region SPACE Key - This is all handled in a join ENTER/SPACE clause in the "KeyDown" handler.
                //// NOTE: This occurs *after* the space character has already been
                ////   processed!

                //if (this._autoCompMenu != null && this._autoCompMenu.SelectedIndex > -1)
                //{
                //    try
                //    {
                //        // If the AutoComplete list is open and has an item selected,
                //        //   we need to insert the selected text instead of
                //        //   inserting the space.
                //        this.SuspendRefresh();
                //        this._parsing = true;
                //        int selStart = this.SelectionStart,
                //            selLength = this.SelectionLength,
                //            dotIdx = this.Text.Substring(0, this.SelectionStart + this.SelectionLength).LastIndexOf('.');

                //        this.Select(dotIdx, selStart + selLength - dotIdx);
                //        this.SelectedText = string.Format(".[{0}] ", this._autoCompMenu.Items[this._autoCompMenu.SelectedIndex].ToString());
                //    }
                //    finally
                //    {
                //        this._parsing = false;
                //        this.ResumeRefresh();
                //    }
                //}

                //this.EndAutoComplete();
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.OemPeriod || e.KeyCode == System.Windows.Forms.Keys.Decimal)
            {
                #region PERIOID Key
                if (this._autoComp && !string.IsNullOrEmpty(this._connStr))
                {
                    int iSelStart = this.SelectionStart,
                        // We have to be concious of the fact that, however unlikely,
                        //   it's possible for a query to have a qualifier as the
                        //   very first thing in the text box.
                        iSpcIdx = System.Math.Max(this.Text.Substring(0, iSelStart).LastIndexOfAny(new char[] { ' ', '[', '(', ',', '\n', '\t' }), 0);

                    string sQualNm = this.Text.Substring(iSpcIdx, iSelStart - iSpcIdx).TrimStart(' ', '[', '(', ',', '\n', '\t').TrimEnd('.', ' ', ']');

                    if (this._autoCompleteSchemas.Contains(sQualNm))
                    {
                        try
                        {
                            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this._connStr))
                            {
                                conn.Open();
                                if (conn.State != System.Data.ConnectionState.Open)
                                    throw new Exception();

                                List<string> tbls = new List<string>();
                                int icStart = this._connStr.IndexOf('=', this._connStr.ToLower().IndexOf("initial catalog=")) + 1,
                                    icEnd = this._connStr.IndexOf(';', icStart),
                                    icLen = (icEnd > -1) ? icEnd - icStart : this._connStr.Length - icStart;
                                if (icStart > -1)
                                {
                                    // Get the database name from the connection string.
                                    string databasename = this._connStr.Substring(icStart, icLen);

                                    // Get the tables and views.
                                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(string.Format("SELECT (su.name + '.' + so.name) as [name] FROM sys.sysobjects so INNER JOIN sys.schemas su ON su.schema_id = so.uid WHERE (xtype=N'U' OR xtype=N'V') AND su.name = '{1}' ORDER BY so.name", databasename, sQualNm), conn))
                                    using (System.Data.SqlClient.SqlDataReader rdr = cmd.ExecuteReader())
                                        while (rdr.Read())
                                            tbls.Add(rdr[0].ToString());

                                    // Get the stored proceedures.
                                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(string.Format("SELECT (su.name + '.' + so.name + ' SP') as [name] FROM sys.sysobjects so INNER JOIN sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'P' AND su.name = '{1}' AND (so.name NOT IN ('sp_upgraddiagrams', 'sp_renamediagram', 'sp_helpdiagrams', 'sp_helpdiagramdefinition', 'sp_dropdiagram', 'sp_creatediagram', 'sp_alterdiagram') OR OBJECT_ID(N'dbo.sysdiagrams') IS NULL) ORDER BY so.name", databasename, sQualNm), conn))
                                    using (System.Data.SqlClient.SqlDataReader rdr = cmd.ExecuteReader())
                                        while (rdr.Read())
                                            tbls.Add(rdr[0].ToString());

                                    // Get the functions.
                                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(string.Format("SELECT (su.name + '.' + so.name + '()') as [name] FROM sys.sysobjects so INNER JOIN sys.schemas su ON su.schema_id = so.uid WHERE xtype=N'FN' AND su.name = '{1}' AND (so.name <> 'fn_diagramobjects' OR OBJECT_ID(N'dbo.sysdiagrams') IS NULL) ORDER BY so.name", databasename, sQualNm), conn))
                                    using (System.Data.SqlClient.SqlDataReader rdr = cmd.ExecuteReader())
                                        while (rdr.Read())
                                            tbls.Add(rdr[0].ToString());
                                }

                                if (tbls.Count > 0)
                                    this.ShowAutoComplete(tbls.ToArray(), AutoCompleteType.Table);
                            }
                        }
                        catch
                        {
                            // We don't want this actually throwing exceptions.  If
                            //   and error occurs, we'll just ignore it and give up.
                        }
                    }
                    else
                    {
                        // Check to see if the list of table aliases has been populated.
                        //if (this._refTablesAlias.Count < 1)
                        this.ParseTables();

                        string tblKey = string.Empty;
                        if (this._refTablesAlias.Contains(sQualNm))
                            tblKey = this._refTablesAlias.GetKey(this._refTablesAlias.IndexOf(sQualNm));
                        else if (this._refTables.Contains(sQualNm))
                            tblKey = sQualNm;

                        if (string.IsNullOrEmpty(tblKey))
                            // We don't have a reference to this table name or alias.
                            return;

                        try
                        {
                            if (this._refTablesCols[tblKey] == null)
                            {
                                // If there's no column set stored, then we have to try and
                                //   retreive the table's columns.
                                string
                                    connStr = this._connStr + ((this._connStr.EndsWith(";")) ? "" : ";"),
                                    instanceNm = string.Empty,
                                    dbNm = string.Empty,
                                    schemaNm = string.Empty,
                                    tblNm = string.Empty;

                                // Determine the values for the table/schema/instance names.
                                string[] tblVals = tblKey.Split(new char[] { '.' }, StringSplitOptions.None);
                                int iCatSt = connStr.ToLower().IndexOf("initial catalog=");
                                switch (tblVals.Length)
                                {
                                    case 1:     // There's only a table name.
                                        dbNm = ((iCatSt < 0)
                                                ? "master"
                                                : connStr.Substring(connStr.IndexOf('=', iCatSt) + 1, connStr.IndexOf(';', iCatSt) - (connStr.IndexOf('=', iCatSt) + 1)));
                                        schemaNm = "dbo";
                                        tblNm = tblVals[0];
                                        break;
                                    case 2:     // There's a schema and table name.
                                        dbNm = ((iCatSt < 0)
                                                ? "master"
                                                : connStr.Substring(connStr.IndexOf('=', iCatSt) + 1, connStr.IndexOf(';', iCatSt) - (connStr.IndexOf('=', iCatSt) + 1)));
                                        schemaNm = ((string.IsNullOrEmpty(tblVals[0]))
                                            ? "dbo"
                                            : tblVals[0]);
                                        tblNm = tblVals[1];
                                        break;
                                    case 3:     // There's  database, schema and table name.
                                        dbNm = ((string.IsNullOrEmpty(tblVals[0]))
                                            ? ((iCatSt < 0)
                                                ? "master"
                                                : connStr.Substring(connStr.IndexOf('=', iCatSt) + 1, connStr.IndexOf(';', iCatSt) - (connStr.IndexOf('=', iCatSt) + 1)))
                                            : tblVals[0]);
                                        schemaNm = ((string.IsNullOrEmpty(tblVals[1]))
                                            ? "dbo"
                                            : tblVals[1]);
                                        tblNm = tblVals[2];
                                        break;
                                    case 4:     // There's an instance, database, schema and table name.
                                        int iDsBrk = connStr.IndexOf(';', connStr.ToLower().IndexOf("data source="));
                                        dbNm = ((string.IsNullOrEmpty(tblVals[1]))
                                            ? ((iCatSt < 0)
                                                ? "master"
                                                : connStr.Substring(connStr.IndexOf('=', iCatSt) + 1, connStr.IndexOf(';', iCatSt) - (connStr.IndexOf('=', iCatSt) + 1)))
                                            : tblVals[1]);
                                        schemaNm = ((string.IsNullOrEmpty(tblVals[2]))
                                            ? "dbo"
                                            : tblVals[2]);
                                        tblNm = tblVals[3];
                                        connStr = connStr.Substring(0, iDsBrk) + "/" + tblVals[0] + connStr.Substring(iDsBrk);
                                        break;
                                }

                                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
                                {
                                    conn.Open();

                                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SELECT @@VERSION", conn))
                                        if (!cmd.ExecuteScalar().ToString().ToLower().StartsWith("microsoft sql"))
                                            throw new Exception("The auto-complete feature requires a connection to a Microsoft SQL Server instance.");

                                    string qryCols = string.Format("SELECT sc.Name FROM [{0}].[sys].[syscolumns] sc INNER JOIN [{0}].[sys].[sysobjects] so ON sc.id = so.id AND so.id = (SELECT so2.id FROM [{0}].[sys].[sysobjects] so2 INNER JOIN [{0}].[sys].[schemas] su2 ON su2.schema_id = so2.uid AND su2.name = '{2}' WHERE so2.name = '{1}') ORDER BY sc.Name", dbNm, tblNm, schemaNm);
                                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(qryCols, conn))
                                    using (System.Data.SqlClient.SqlDataReader rdr = cmd.ExecuteReader())
                                    {
                                        if (rdr.HasRows)
                                        {
                                            string colNms = "";
                                            while (rdr.Read())
                                            {
                                                colNms += "|" + rdr.GetString(0);
                                            }
                                            colNms = colNms.TrimStart('|');
                                            this._refTablesCols.Add(colNms, tblKey);
                                        }
                                    }
                                }
                            }

                            if (this._refTablesCols[tblKey] != null)
                            {
                                string[] cols = this._refTablesCols[tblKey].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                this.ShowAutoComplete(cols, AutoCompleteType.Field);

                                #region Moved to Independant Method
                                ////Point csrLoc = this.PointToScreen(this.GetPositionFromCharIndex(this.SelectionStart));
                                //Point csrLoc = this.GetPositionFromCharIndex(this.SelectionStart);
                                //if (this._autoCompMenu != null)
                                //    this._autoCompMenu.Dispose();

                                //this._autoCompStart = this.SelectionStart;
                                //this._autoCompLine = this.CurrentLine;
                                //int mnuLocY = csrLoc.Y;
                                //string[] cols = this._refTablesCols[tblKey].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                //this._autoCompMenu = new System.Windows.Forms.ListBox();
                                //this.Controls.Add(this._autoCompMenu);
                                //this._autoCompMenu.IntegralHeight = true;
                                //this._autoCompMenu.Height = (int)Math.Min(cols.Length, 20) * (this._autoCompMenu.ItemHeight + 1);
                                //if (mnuLocY + this._autoCompMenu.Height > this.Bottom)
                                //    mnuLocY = csrLoc.Y - this._autoCompMenu.Font.Height - this._autoCompMenu.Height;
                                //else
                                //    mnuLocY += this._autoCompMenu.Font.Height;
                                //this._autoCompMenu.Location = new Point(csrLoc.X, mnuLocY);
                                //string longestWord = "";
                                //for (int i = 0; i < cols.Length; i++)
                                //{
                                //    this._autoCompMenu.Items.Add(cols[i]);
                                //    if (cols[i].Length > longestWord.Length)
                                //        longestWord = cols[i];
                                //}
                                //using (Graphics g = this._autoCompMenu.CreateGraphics())
                                //    this._autoCompMenu.Width = (int)Math.Ceiling(g.MeasureString("W" + longestWord, this._autoCompMenu.Font).Width) + 18;
                                //this._autoCompMenu.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                                //this._autoCompMenu.ScrollAlwaysVisible = true;
                                //this._autoCompMenu.Cursor = System.Windows.Forms.Cursors.Arrow;
                                //this._autoCompMenu.Click += new EventHandler(this.autoCompMenu_ItemClicked);
                                //this._autoCompMenu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.autoCompMenu_KeyPress);
                                //this._autoCompMenu.Show();
                                #endregion
                            }
                        }
                        catch
                        {
                            // I don't want this feature throwing errors, but if
                            //   something goes wrong, we'll just reset the system.
                            this.EndAutoComplete();
                            this._refTables.Clear();
                            this._refTablesAlias.Clear();
                            this._refTablesCols.Clear();
                            this.ParseTables();
                        }
                    }
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                #region ESCAPE Key
                this.EndAutoComplete();
                #endregion
            }
            else if ((int)e.KeyCode == 219 && this._connStr.IndexOf("provider=microsoft") > -1 && this._connStr.ToLower().IndexOf("extended properties=excel") > -1)
            {
                #region Open Bracket
                // If we're working on an excel file, then when the user hits the
                //   'Open Bracket' key, we want popup a list of all the sheets
                //   in the Excel file.
                #endregion
            }
            else if (e.KeyValue == 45)
            {
                #region INSERT Key
                this._insMode = !this._insMode;
                this.OnInsertModeChanged(EventArgs.Empty);
                #endregion
            }
            else if (e.KeyData != System.Windows.Forms.Keys.Shift && this.IsInputKey(e.KeyData) && !e.Control && !e.Alt)
            {
                #region Any Other Input Character
                if (this._autoCompMenu != null)
                {
                    if ((e.KeyValue >= 65 && e.KeyValue <= 90) || e.KeyValue == 189)
                    {
                        // If the AutoComplete menu is open, we want to try
                        //   and select a menu item that matches what we're
                        //   typing.
                        int iStPos = this.Text.Substring(0, this.SelectionStart).LastIndexOf('.');
                        if (iStPos < 0)
                            // If we can't figure out which period ('.') to start from,
                            //   then just abort the whole thing.
                            return;
                        string sSrchText = this.Text.Substring(iStPos, this.SelectionStart - iStPos).TrimStart('.', '[').TrimEnd(']').ToLower();
                        for (int i = 0; i < this._autoCompMenu.Items.Count; i++)
                        {
                            string acSrchVal = string.Empty;

                            // The logic differs at this point, depending on whether
                            //   we're searching for a field or table name.
                            if (this._autoCompMenu.Tag.ToString() == "Table")
                            {
                                // Search for table name.
                                string
                                    acItemVal = this._autoCompMenu.Items[i].ToString().ToLower();
                                acSrchVal = acItemVal.Substring(acItemVal.IndexOf('.') + 1);
                            }
                            else
                            {
                                acSrchVal = this._autoCompMenu.Items[i].ToString().ToLower();
                            }
                            if (acSrchVal.StartsWith(sSrchText))
                            {
                                this._autoCompMenu.SelectedIndex = i;
                                this.Focus();
                                break;
                            }
                            else if (this._autoCompMenu.SelectedIndex > -1)
                                this._autoCompMenu.ClearSelected();
                        }
                    }
                }
                //else
                //{
                //    if (_autoComp && this._connStr != null)
                //    {
                //        // If the user is just straight typing, we want to check to see
                //        //   if what they are typing matches anything in the autocomplete
                //        //   lists.
                //        int iSelStart = this.SelectionStart,
                //            // We have to be concious of the fact that, however unlikely,
                //            //   it's possible for a query to have a qualifier as the
                //            //   very first thing in the text box.
                //            iSpcIdx = Math.Max(this.Text.Substring(0, iSelStart).LastIndexOfAny(new char[] { ' ', '[', '(', ',', '\n', '\t' }), 0);
                //        string sQualNm = this.Text.Substring(iSpcIdx, iSelStart - iSpcIdx).TrimStart(' ', '[', '(', ',', '\n', '\t').TrimEnd('.', ' ', ']');

                //        List<String> autoCompleteItems = new List<string>();
                //        if (this._autoCompleteSchemas != null)
                //            for (int i = 0; i < this._autoCompleteSchemas.Count; i++)
                //                if (this._autoCompleteSchemas[i].ToLower().StartsWith(sQualNm.ToLower()))
                //                    autoCompleteItems.Add(this._autoCompleteSchemas[i]);
                //        if (this._autoCompleteTables != null)
                //            for (int i = 0; i < this._autoCompleteTables.Count; i++)
                //                if (this._autoCompleteTables[i].ToLower().StartsWith(sQualNm.ToLower()))
                //                    autoCompleteItems.Add(this._autoCompleteTables[i]);
                //        if (this._refTablesCols != null)
                //            for (int i = 0; i < this._refTablesCols.Count; i++)
                //                if (this._refTablesCols[i].ToLower().StartsWith(sQualNm.ToLower()))
                //                    autoCompleteItems.Add(this._refTablesCols[i]);

                //        if (autoCompleteItems.Count > 0)
                //            this.ShowAutoComplete(autoCompleteItems.ToArray(), AutoCompleteType.Table);
                //    }
                //}
                try
                {
                    if (this.Text.Length > 0 && this.Lines[this.CurrentLine].Length > 0)
                        this.ParseCurrentLine();
                }
                catch { }
                #endregion
            }

            //if (this._addToSel > 0)
            //    this.Select(this.SelectionStart, this.SelectionLength + this._addToSel);
            //this._addToSel = 0;
        }
        protected override void OnPaste(EventArgs e)
        {
            //this.InsertText(System.Windows.Forms.Clipboard.GetData(System.Windows.Forms.DataFormats.Text).ToString());
            this._parsing = true;
            try
            {
                int
                    iLnCnt = this.Lines.Length,
                    iStrtLn = this.GetLineFromCharIndex(this.SelectionStart);

                base.OnPaste(e);
                if (this.Lines.Length > iLnCnt)
                {
                    int startChr = this.GetFirstCharIndexFromLine(iStrtLn),
                        endLn = this.GetFirstCharIndexFromLine(((this.Lines.Length - 1) - iLnCnt) + iStrtLn),
                        endChr = endLn + this.Lines[this.GetLineFromCharIndex(endLn)].Length;
                    this.DoParse(startChr, endChr - startChr);
                }
                else if (this.Lines.Length == iLnCnt)
                    this.ParseCurrentLine();
                else
                    Parse();
                this.RaiseTextPaste();
            }
            catch (Exception ex)
            { System.Windows.Forms.MessageBox.Show(this, "Unable to paste text data:\n\n" + ex.Message + "\n\nApplication Version: " + System.Windows.Forms.Application.ProductVersion, "Error"); }
            finally
            { this._parsing = false; }
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.Parse();
        }
        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this._parsing)
                //|| ((System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left) 
                //|| ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift))
                return;

            // If we're doing an autocomplete and the carat moves off the line or
            //   before the AutoCompStart (the period) index, then cancel the
            //   AutoComplete process.
            if (this._autoCompMenu != null && (this.CurrentLine != this._autoCompLine || this.SelectionStart < this._autoCompStart))
                this.EndAutoComplete();

            if (this.SelectionLength == 0)
            {
                this.SuspendScroll();
                this.SuspendRefresh();

                int iSelStart = this.SelectionStart,
                    iSelLength = this.SelectionLength;
                this._parsing = true;

                try
                {
                    // Unbold any parenthesis we bolded last time.
                    //List<int> chgLns = new List<int>();
                    for (int i = 0; i < this._bldChars.Count; i++)
                    {
                        this.Select(this._bldChars[i], 1);
                        this.SelectionFont = this.Font;
                        //int charLn = this.GetLineFromCharIndex(this._bldChars[i]);
                        //if (!chgLns.Contains(charLn))
                        //    chgLns.Add(charLn);
                    }
                    this._bldChars.Clear();
                    //for (int i = 0; i < chgLns.Count; i++)
                    //{
                    //    this.DoParse(this.GetFirstCharIndexFromLine(chgLns[i]), this.Lines[chgLns[i]].Length);
                    //    this._parsing = true;
                    //}
                    this.Select(iSelStart, 0);

                    // Parse parenthasis here.
                    // Create a bolded version of the textbox's font to highlight
                    //   matching sets of parenthesis.
                    using (Font bFont = new Font(this.Font, FontStyle.Bold))
                    {
                        if (iSelStart > 2)
                        {
                            // If we're move than two char from the beginning of the document
                            //   then look backwards for parenthesis.
                            if (this.Text.Substring(iSelStart - 1, 1) == ")")
                            {
                                // If the character to the left of the cursor is a parenthesis
                                //   close, then look backwards for its mate.
                                int nestCnt = 0;
                                for (int i = iSelStart - 2; i >= 0; i--)
                                {
                                    string curChar = this.Text.Substring(i, 1);
                                    if (curChar == ")")
                                        nestCnt++;
                                    else if (curChar == "(" && nestCnt-- == 0)
                                    {
                                        // We found the matching open parenthesis.
                                        this.Select(i, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(i);
                                        this.Select(iSelStart - 1, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(iSelStart - 1);
                                        // We're done looking.
                                        break;
                                    }
                                }
                            }
                            else if (this.Text.Substring(iSelStart - 1, 1) == "]")
                            {
                                // If the character to the left of the cursor is a bracket
                                //   close, then look backwards for its mate.
                                int nestCnt = 0;
                                for (int i = iSelStart - 2; i >= 0; i--)
                                {
                                    string curChar = this.Text.Substring(i, 1);
                                    if (curChar == "]")
                                        nestCnt++;
                                    else if (curChar == "[" && nestCnt-- == 0)
                                    {
                                        // We found the matching open bracket.
                                        this.Select(i, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(i);
                                        this.Select(iSelStart - 1, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(iSelStart - 1);
                                        // We're done looking.
                                        break;
                                    }
                                }
                            }
                            else if (this.Text.Substring(iSelStart - 1, 1) == "}")
                            {
                                // If the character to the left of the cursor is a closing
                                //   brace, then look backwards for its mate.
                                int nestCnt = 0;
                                for (int i = iSelStart - 2; i >= 0; i--)
                                {
                                    string curChar = this.Text.Substring(i, 1);
                                    if (curChar == "}")
                                        nestCnt++;
                                    else if (curChar == "{" && nestCnt-- == 0)
                                    {
                                        // We found the matching open brace.
                                        this.Select(i, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(i);
                                        this.Select(iSelStart - 1, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(iSelStart - 1);
                                        // We're done looking.
                                        break;
                                    }
                                }
                            }
                        }
                        if (iSelStart < this.Text.Length - 3)
                        {
                            // If we're more than two characters form the end of the
                            //   document, then look forwards for parenthesis.
                            if (this.Text.Substring(iSelStart, 1) == "(")
                            {
                                // If the character to the right of the cursor is an
                                //   "Open Parenthesis", then search for its mate.
                                int iNestCnt = 0;
                                for (int i = iSelStart + 1; i <= this.Text.Length - 1; i++)
                                {
                                    string curChar = this.Text.Substring(i, 1);
                                    if (curChar == "(")
                                        iNestCnt++;
                                    else if (curChar == ")" && iNestCnt-- == 0)
                                    {
                                        // We found the matching close parenthesis
                                        this.Select(i, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(i);
                                        this.Select(iSelStart, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(iSelStart);
                                        // We're done looking.
                                        break;
                                    }
                                }
                            }
                            else if (this.Text.Substring(iSelStart, 1) == "[")
                            {
                                // If the character to the right of the cursor is an
                                //   "Open Bracket", then search for its mate.
                                int iNestCnt = 0;
                                for (int i = iSelStart + 1; i <= this.Text.Length - 1; i++)
                                {
                                    string curChar = this.Text.Substring(i, 1);
                                    if (curChar == "[")
                                        iNestCnt++;
                                    else if (curChar == "]" && iNestCnt-- == 0)
                                    {
                                        // We found the matching close bracket
                                        this.Select(i, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(i);
                                        this.Select(iSelStart, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(iSelStart);
                                        // We're done looking.
                                        break;
                                    }
                                }
                            }
                            else if (this.Text.Substring(iSelStart, 1) == "{")
                            {
                                // If the character to the right of the cursor is an
                                //   "Open Brace", then search for its mate.
                                int iNestCnt = 0;
                                for (int i = iSelStart + 1; i <= this.Text.Length - 1; i++)
                                {
                                    string curChar = this.Text.Substring(i, 1);
                                    if (curChar == "{")
                                        iNestCnt++;
                                    else if (curChar == "}" && iNestCnt-- == 0)
                                    {
                                        // We found the matching close bracket.
                                        this.Select(i, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(i);
                                        this.Select(iSelStart, 1);
                                        this.SelectionFont = bFont;
                                        this._bldChars.Add(iSelStart);
                                        // We're done looking.
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    if (this.SelectionStart != iSelStart || this.SelectionLength != iSelLength)
                        if (this._selDir == AdvRichTextBox.SelectDirection.Backword)
                            this.Select(iSelStart + iSelLength, -iSelLength);
                        else
                            this.Select(iSelStart, iSelLength);
                    this.SelectionFont = this.Font;
                    this.ResumeScroll();
                    this.ResumeRefresh();
                    this._parsing = false;
                }
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this._defCtxMenu)
                this.ContextMenuStrip = this._defMenu;

            //if (this.Text.Length < 1)
            //this.InitRtfHeader();
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //if (this._parsing)
            //    using (Font font = new Font("Tahoma", 9.0f, FontStyle.Italic))
            //        e.Graphics.DrawString("Parsing...", font, SystemBrushes.WindowText, new PointF(5.0f, 5.0f));
            //else
            if (!this._parsing)
                base.OnPaint(e);
            //this.DrawMargin(e.Graphics, e.ClipRectangle);
        }
        protected override void OnDragOver(System.Windows.Forms.DragEventArgs drgevent)
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1} => {2}  Data: {3}  Effect: {4}", drgevent.X, drgevent.Y, this.GetCharIndexFromPosition(this.PointToClient(new Point(drgevent.X, drgevent.Y))), drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text), drgevent.AllowedEffect));
            if (drgevent.Data.GetDataPresent(System.Windows.Forms.DataFormats.Text, true))
            {
                drgevent.Effect = System.Windows.Forms.DragDropEffects.Copy;
                base.OnDragOver(drgevent);
            }
            else
                drgevent.Effect = System.Windows.Forms.DragDropEffects.None;
        }
        protected override void OnDragDrop(System.Windows.Forms.DragEventArgs drgevent)
        {
            this._parsing = true;
            this.SuspendScroll();
            this.SuspendRefresh();

            try
            {
                if (this._internalDrag)
                {
                    base.OnDragDrop(drgevent);
                    //this.AddUndo(new UndoState(UndoStateType.Move, (string)drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text), "", this.GetCharIndexFromPosition(this.PointToClient(new Point(drgevent.X, drgevent.Y)))));
                    this._internalDrag = false;
                    //this.Parse();
                }
                else if (drgevent.Data.GetDataPresent(System.Windows.Forms.DataFormats.Text, true))
                {
                    //string drgTxt = (string)drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text, true);
                    //int selStart = this.SelectionStart;
                    //int selLength = this.SelectionLength;
                    //int charPos = this.GetCharIndexFromPosition(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
                    //this.InsertText(drgTxt, charPos);
                    //System.Windows.Forms.DataObject drgDataObj = new System.Windows.Forms.DataObject(System.Windows.Forms.DataFormats.Text,
                    //    drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text, true));
                    //System.Windows.Forms.DragEventArgs newArgs = new System.Windows.Forms.DragEventArgs(
                    //    drgDataObj, drgevent.KeyState, drgevent.X, drgevent.Y, drgevent.AllowedEffect, drgevent.Effect);
                    object objData = drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text, true);
                    int iSelStart = this.SelectionStart,
                        iSelLength = this.SelectionLength;

                    base.OnDragDrop(drgevent);
                    //this.AddUndo(new UndoState(UndoStateType.Drop, (string)drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text), "", this.GetCharIndexFromPosition(this.PointToClient(new Point(drgevent.X, drgevent.Y)))));
                    //this.Parse();
                }
                //else if (drgevent.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
                //{
                //    // File drops have to handled in a special way.  We only want to
                //    //   allow users to drop text files, so we have to do a little bit
                //    //   of parsing to determine file type.
                //    this._parsing = true;
                //    this.EnableAutoDragDrop = false;
                //    try
                //    {
                //        // The FileDrop object is a string array.  The first value should
                //        //   always be the full path and name of the file.
                //        string[] flNm = (string[])drgevent.Data.GetData(System.Windows.Forms.DataFormats.FileDrop, true);
                //        using (System.IO.FileStream fs = new System.IO.FileStream(flNm[0], System.IO.FileMode.Open, System.IO.FileAccess.Read))
                //        {
                //            byte[] flBytes = new byte[fs.Length];
                //            fs.Read(flBytes, 0, flBytes.Length);
                //            string newStr = RainstormStudios.ArrayConvert.ToString(flBytes);
                //            //bool isText = true;
                //            //for(int i=0;i<flChars.Length;i++)
                //            //    if(System.Text.flChars[i]
                //            int charPos = this.GetCharIndexFromPosition(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
                //            this.InsertText(newStr, charPos);
                //        }
                //    }
                //    catch (Exception ex)
                //    { }
                //    finally
                //    {
                //        this._parsing = false;
                //        this.EnableAutoDragDrop = true;
                //        drgevent.Effect = System.Windows.Forms.DragDropEffects.None;
                //    }
                //}

                int drpLine = this.GetLineFromCharIndex(this.GetCharIndexFromPosition(new Point(drgevent.X, drgevent.Y)));
                this.DoParse(this.GetFirstCharIndexFromLine(drpLine), this.Lines[drpLine].Length + drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text, true).ToString().Length);
            }
            finally
            {
                this._parsing = false;
                this.ResumeScroll();
                this.ResumeRefresh(true);
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //this.SelectionIndent = leftMargin;
            //this.Text = string.Empty;
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // This all helps to prevent scrolling or refreshing the text window
            //   while we're parsing for color, parenthesis, etc.
            if (this._parsing)
                if (m.Msg == (int)Win32Messages.WM_VSCROLL || m.Msg == (int)Win32Messages.WM_HSCROLL)
                    return;
                else if (m.Msg == (int)Win32Messages.EM_LINESCROLL || m.Msg == (int)Win32Messages.EM_SCROLL || m.Msg == (int)Win32Messages.EM_SCROLLCARET)
                    return;
                else if (m.Msg == (int)Win32Messages.WM_PAINT || m.Msg == (int)Win32Messages.WM_NCPAINT)
                    return;
                else if (m.Msg == (int)Win32Messages.WM_ERASEBKGND)
                    return;

            //if (m.Msg == (int)Win32Messages.WM_CHAR)
            //{ }

            //if (((Win32Messages)m.Msg).ToString().Substring(0, 2) != "WM")
            //{ }

            base.WndProc(ref m);
        }
        //**************************************************************************
        // Event Triggers
        // 
        protected void OnTextParsed()
        {
            if (this.TextParsed != null)
                this.TextParsed.Invoke(this, EventArgs.Empty);
        }
        protected void OnInsertModeChanged(EventArgs e)
        {
            if (this.InsertModeChanged != null)
                this.InsertModeChanged(this, e);
        }
        #endregion
    }
}
