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
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using RainstormStudios.Unmanaged;
using RainstormStudios.Collections;

namespace RainstormStudios.Controls
{
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.RichTextBox)), DesignerCategoryAttribute("UserControl")]
    public class AdvRichTextBox : System.Windows.Forms.RichTextBox
    {
        #region Nested Types
        //***************************************************************************
        // Private Types
        // 
        protected class POINT
        {
            public int
                x, y;

            public POINT(int x, int y)
            { this.x = x; this.y = y; }

            public static implicit operator System.Drawing.Point(POINT p)
            { return new System.Drawing.Point(p.x, p.y); }
            public static implicit operator POINT(System.Drawing.Point p)
            { return new POINT(p.X, p.Y); }
        }
        [StructLayout(LayoutKind.Sequential)]
        protected struct CharFormat2
        {
            public Int32
                cbSize,
                dwMask,
                dwEffects,
                yHeight,
                yOffset,
                crTextColor;
            public Byte
                bCharSet,
                pPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string
                szFaceName;
            public Int16
                wweight,
                sSpacing;
            public Int32
                crBackColor,
                lcid,
                dwReserved;
            public Int16
                sStyle,
                wKerning;
            public Byte
                bUnderlineType,
                bAnimation,
                bRevAuthor,
                bReserved1;
        }
        public struct AdvRichTextBoxPadding
        {
            public int
                Top, Left, Right, Bottom;

            public AdvRichTextBoxPadding(int t, int r, int b, int l)
            {
                this.Top = t;
                this.Right = r;
                this.Bottom = b;
                this.Left = l;
            }
        }
        //***************************************************************************
        // Private Enums
        // 
        protected enum SelectDirection
        {
            None,
            Forward,
            Backword
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Constants
        // 
        const Int32
            LF_FACESIZE = 32,
            CFM_BACKCOLOR = 0x40000000,
            CFE_AUTOBACKCOLOR = CFM_BACKCOLOR,
            WM_USER = 0x400,
            EM_SETCHARFORMAT = (WM_USER + 68),
            EM_SETBKGNDCOLOR = (WM_USER + 67),
            EM_GETCHARFORMAT = (WM_USER + 58),
            WM_SETTEXT = 0xC,
            SCF_SELECTION = 0x1;
        //***************************************************************************
        // Private Fields
        // 
        UndoStateCollection
            _undoCol,
            _redoCol;
        string
            _lastVer,
            _clearedText;
        Keys
            _lastKeyPress,
            _lastModifierPress;
        int
            _lastCharPos,
            _lastVScPos,
            _lastHScPos,
            _printCurPg;
        IntPtr
            _evntMask;
        //protected AdvRichTextBoxPadding
        //    _intrPadding;
        NCCALCSIZE_PARAMS 
            nccsp;
        RainstormStudios.Forms.frmFindText
            _frmFindText;
#if DEBUG
        private int
            paintCount = 0,
            redrawCount = 0;
#endif
        protected bool
            _internalDrag = false,
            _hideCrt = false,
            _noScroll = false,
            _lockRedraw = false;
        protected System.Drawing.Printing.PrintDocument
            _printDoc;
        internal string
            DocumentName;
        protected AdvRichTextBox.SelectDirection
            _selDir = AdvRichTextBox.SelectDirection.None;
        //***************************************************************************
        // Public Events
        // 
        [Category("Action"), Description("Occurs whenever a 'cut' command is issued to the control.")]
        public event EventHandler TextCut;
        [Category("Action"), Description("Occurs whenever a 'paste' command is issued to the control.")]
        public event EventHandler TextPaste;
        [Category("Action"), Description("Occurs whenever an 'undo' command is issued to the control.")]
        public event EventHandler TextUndo;
        [Category("Property Changed"), Description("Occurs whenever current number of undo levels has changed.")]
        public event EventHandler TextUndoLevelChanged;
        [Category("Action"), Description("Occurs whenever a 'redo' command is issued to the control.")]
        public event EventHandler TextRedo;
        [Category("Behavior"), Description("Occurs whenever the contents of the control are printed.")]
        public event System.Drawing.Printing.PrintEventHandler TextPrint;
        [Category("Property Changed"), Description("Occurs whenever the vertical scroll position is changed.")]
        public new event ScrollEventHandler VScroll;
        [Category("Property Changed"), Description("Occurs whenever the horizontal scroll position is changed.")]
        public new event ScrollEventHandler HScroll;
        //***************************************************************************
        // External Aliases
        // 
        [DllImport("user32.dll")]
        protected static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, ref CharFormat2 lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern int GetScrollPos(IntPtr hWnd, int nBar);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern bool SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll")]
        protected static extern bool LockWindowUpdate(IntPtr hWndLock);
        [DllImport("user32.dll")]
        protected static extern bool GetScrollRange(IntPtr hWnd, int nBar, ref int lpMinPos, ref int lpMaxPos);
        [DllImport("user32.dll")]
        protected static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        protected static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        protected static extern bool HideCaret(IntPtr hWnd);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new Color SelectionBackColor
        {
            get
            {
                // We need to ask the RTB for the backcolor of the current selection.
                //   This is done using SendMessage with a format structure which
                //   the RTB will fill in for us.

                // First, we force the creation of the window handle.
                IntPtr HWND = this.Handle;
                CharFormat2 Format = new CharFormat2();
                Format.dwMask = CFM_BACKCOLOR;
                Format.cbSize = Marshal.SizeOf(Format);
                SendMessage(HWND, EM_GETCHARFORMAT, SCF_SELECTION, ref Format);
                return ColorTranslator.FromOle(Format.crBackColor);
            }
            set
            {
                // Here, we do relatively the same thing as in the Get accessor, but
                //   we are telling the RTB to set the color this time instead of
                //   returning it to us.
                IntPtr HWND = this.Handle;
                CharFormat2 Format = new CharFormat2();
                Format.crBackColor = ColorTranslator.ToOle(value);
                Format.dwMask = CFM_BACKCOLOR;
                Format.cbSize = Marshal.SizeOf(Format);
                SendMessage(HWND, EM_SETCHARFORMAT, SCF_SELECTION, ref Format);
            }
        }
        public int HorizontalScrollPos
        {
            get { return GetScrollPos(this.Handle, (int)ScrollBarTypes.SB_HORZ); }
            set { SetScrollPos(this.Handle, (int)ScrollBarTypes.SB_HORZ, value, false); }
        }
        public int VerticalScrollPos
        {
            get { return GetScrollPos(this.Handle, (int)ScrollBarTypes.SB_VERT); }
            set { SetScrollPos(this.Handle, (int)ScrollBarTypes.SB_VERT, value, false); }
        }
        public int CurrentLine
        { get { return this.GetLineFromCharIndex(this.SelectionStart + this.SelectionLength); } }
        public int UndoLevelCount
        { get { return this._undoCol.Count; } }
        public int MaxUndoLevel
        {
            get { return this._undoCol.MaxUndoLevel; }
            set { this._undoCol.MaxUndoLevel = (value >= 0) ? value : 0; }
        }
        //public AdvRichTextBoxPadding Padding
        //{
        //    get { return this._intrPadding; }
        //}
        public bool HideCaretWhenReadOnly
        {
            get { return this._hideCrt; }
            set { this._hideCrt = value; }
        }
        private bool PreventScrolling
        {
            get { return this._noScroll; }
            set
            {
                if (value)
                    this.ResumeScroll();
                else
                    this.SuspendScroll();
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public AdvRichTextBox()
            : base()
        {
            this._redoCol = new UndoStateCollection();
            this._undoCol = new UndoStateCollection();
            this._undoCol.Inserted += new CollectionEventHandler(this.undoCol_onInserted);
            this._undoCol.Removed+=new CollectionEventHandler(this.undoCol_onRemoved);
            this._undoCol.Cleared += new EventHandler(this.undoCol_onCleared);
            //this._intrPadding = new AdvRichTextBoxPadding(0, 0, 0, 0);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this.Dispose(true);
        }
        public void ScrollToBottom()
        {
            int min = -1, max = -1;
            GetScrollRange(this.Handle, (int)ScrollBarFlags.SBS_VERT, ref min, ref max);
            this.VerticalScrollPos = max - this.Height;
        }
        public void SetForeColor(string findWhat, Color highlight, bool matchCase, bool matchWholeWord)
        {
            //LockWindowUpdate(this.Handle); // Prevent the control from redrawing itself.
            this.SuspendRefresh();
            this.SuspendScroll();
            this.SuspendLayout();
            int ScrollPosVert = this.VerticalScrollPos;
            int ScrollPosHoriz = this.HorizontalScrollPos;
            int selStart = this.SelectionStart;
            int selLength = this.SelectionLength;

            int startFrom = 0;
            int length = findWhat.Length;
            RichTextBoxFinds finds = RichTextBoxFinds.None;

            // Setup the flags for searching.
            if (matchCase) finds |= RichTextBoxFinds.MatchCase;
            if (matchWholeWord) finds |= RichTextBoxFinds.WholeWord;

            try
            {
                // Do the search.
                while (this.Find(findWhat, startFrom, finds) > -1)
                {
                    this.SelectionBackColor = highlight;
                    startFrom = this.SelectionStart + this.SelectionLength;
                }
            }
            finally
            {
                // Return the previous values.
                this.SelectionStart = selStart;
                this.SelectionLength = selLength;
                this.HorizontalScrollPos = ScrollPosHoriz;
                this.VerticalScrollPos = ScrollPosVert;
                this.ResumeLayout();
                //LockWindowUpdate(IntPtr.Zero); // Unlock drawing
                this.ResumeScroll();
                this.ResumeRefresh();
            }
        }
        public void SetBackColor(string findWhat, Color highlight, bool matchCase, bool matchWholeWord)
        {
            //LockWindowUpdate(this.Handle); // Prevent the control from redrawing itself.
            this.SuspendRefresh();
            this.SuspendScroll();
            this.SuspendLayout();
            int ScrollPosVert = this.VerticalScrollPos;
            int ScrollPosHoriz = this.HorizontalScrollPos;
            int selStart = this.SelectionStart;
            int selLength = this.SelectionLength;

            int startFrom = 0;
            int length = findWhat.Length;
            RichTextBoxFinds finds = RichTextBoxFinds.None;

            // Setup the flags for searching.
            if (matchCase) finds |= RichTextBoxFinds.MatchCase;
            if (matchWholeWord) finds |= RichTextBoxFinds.WholeWord;

            try
            {
                // Do the search.
                while (this.Find(findWhat, startFrom, finds) > -1)
                {
                    this.SelectionColor = Color.White;
                    this.SelectionBackColor = highlight;
                    startFrom = this.SelectionStart + this.SelectionLength;
                }
            }
            finally
            {
                // Return the previous values.
                this.SelectionStart = selStart;
                this.SelectionLength = selLength;
                this.HorizontalScrollPos = ScrollPosHoriz;
                this.VerticalScrollPos = ScrollPosVert;
                this.ResumeLayout();
                //LockWindowUpdate(IntPtr.Zero); // Unlock drawing
                this.ResumeRefresh();
                this.ResumeScroll();
            }
        }
        public void ClearBackColor()
        {this.ClearBackColor(true);}
        public void ClearBackColor(bool clearAll)
        {
            //LockWindowUpdate(HWND);
            this.SuspendRefresh();
            this.SuspendScroll();
            this.SuspendLayout();
            int ScrollPosVert = this.HorizontalScrollPos;
            int ScrollPosHoriz = this.VerticalScrollPos;
            int selStart = this.SelectionStart;
            int selLength = this.SelectionLength;

            try
            {
                IntPtr HWND = this.Handle;
                if (clearAll) this.SelectAll();
                CharFormat2 format = new CharFormat2();
                format.crBackColor = -1;
                format.dwMask = CFM_BACKCOLOR;
                format.dwEffects = CFE_AUTOBACKCOLOR;
                format.cbSize = Marshal.SizeOf(format);
                SendMessage(HWND, EM_SETCHARFORMAT, SCF_SELECTION, ref format);
            }
            finally
            {
                // Return the previous values...
                this.SelectionStart = selStart;
                this.SelectionLength = selLength;
                this.HorizontalScrollPos = ScrollPosHoriz;
                this.VerticalScrollPos = ScrollPosVert;
                this.ResumeLayout();
                //LockWindowUpdate(IntPtr.Zero);
                this.ResumeRefresh();
                this.ResumeScroll();
            }
        }
        public void SuspendRefresh()
        {
            this._lockRedraw = true;
            //LockWindowUpdate(this.Handle);
            HandleRef myHandle = new HandleRef(this, this.Handle);
            SendMessage(myHandle.Handle, (int)Win32Messages.WM_SETREDRAW, 0, IntPtr.Zero);
            this._evntMask = SendMessage(myHandle.Handle, (int)Win32Messages.EM_GETEVENTMASK, 0, IntPtr.Zero);
        }
        public void ResumeRefresh()
        {
            this.ResumeRefresh(true);
        }
        public void ResumeRefresh(bool refreshNow)
        {
            this._lockRedraw = false;
            //LockWindowUpdate(IntPtr.Zero);
            HandleRef myHandle = new HandleRef(this, this.Handle);
            SendMessage(myHandle.Handle, (int)Win32Messages.EM_SETEVENTMASK, 0, this._evntMask);
            SendMessage(myHandle.Handle, (int)Win32Messages.WM_SETREDRAW, 1, IntPtr.Zero);
            if (refreshNow)
                this.Refresh();
        }
        public void SuspendScroll()
        {
            this._noScroll = true;
        }
        public void ResumeScroll()
        {
            this._noScroll = false;
        }
        public new void Cut()
        {
            this.OnCut(EventArgs.Empty);
            this.RaiseTextCut();
        }
        public new void Paste()
        {
            this.OnPaste(EventArgs.Empty);
            this.RaiseTextPaste();
        }
        public new void Undo()
        {
            this.OnUndo(EventArgs.Empty);
            this.RaiseTextUndo();
        }
        public new void Redo()
        {
            this.OnRedo(EventArgs.Empty);
            this.RaiseTextRedo();
        }
        public void Print(string docName)
        {
            if (this.PrintSettings())
                this.QuickPrint(docName);
        }
        public void QuickPrint(string docName)
        {
            this.CreatePrintDocument();
            if (this._printDoc.PrinterSettings == null)
                if (!this.PrintSettings())
                    return;
            this._printDoc.DocumentName = docName;
            this._printDoc.Print();
        }
        public bool PrintSettings()
        {
            this.CreatePrintDocument();
            using (PrintDialog dlg = new PrintDialog())
            {
                dlg.Document = this._printDoc;
                dlg.AllowCurrentPage = false;
                dlg.UseEXDialog = false;
                DialogResult dlgResult = dlg.ShowDialog(this.FindForm());
                if (dlgResult == DialogResult.OK)
                    this._printDoc.PrinterSettings = dlg.PrinterSettings;
                return (dlgResult == DialogResult.OK);
            }
        }
        public void PrintPreview()
        {
            this.CreatePrintDocument();
            using (PrintPreviewDialog dlg = new PrintPreviewDialog())
            {
                dlg.Document = this._printDoc;
                dlg.ShowDialog(this.FindForm());
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Dispose(bool disposing)
        {
            if (this._lockRedraw)
                this.ResumeRefresh();
            base.Dispose(disposing);
        }
        protected virtual void OnCut(EventArgs e)
        {
            this.AddUndo(new UndoState(UndoStateType.Cut, string.Empty, this.SelectedText, this.SelectionStart));
            base.Cut();
        }
        protected virtual void OnPaste(EventArgs e)
        {
            this.AddUndo(new UndoState(UndoStateType.Paste, Clipboard.GetText(), this.SelectedText, this.SelectionStart));
            base.Paste(DataFormats.GetFormat(DataFormats.Text));
        }
        protected virtual void OnUndo(EventArgs e)
        {
            try
            {
                if (this._undoCol.Count > 0)
                {
                    this.SuspendRefresh();
                    this.SuspendScroll();
                    int iSelStart = this.SelectionStart;
                    UndoState uState = this._undoCol.GetLast();
                    this.SelectionStart = uState.CharPosition;
                    this.SelectionLength = uState.CharLength;
                    this.SelectedText = uState.PreviousText;
                    this._redoCol.Add(uState);
                    this._undoCol.RemoveLast();

                    // For delete keypresses, we want to keep the cursor where it
                    //   was when the delete key was pressed.
                    if (uState.StateType == UndoStateType.Delete || uState.StateType == UndoStateType.Clear)
                        this.SelectionStart = (int)System.Math.Min(iSelStart, this.Text.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occured while trying to undo:\n\n" + ex.Message, "Sorry");
            }
            finally
            {
                this.ResumeScroll();
                this.ResumeRefresh();
            }
        }
        protected virtual void OnRedo(EventArgs e)
        {
            try
            {
                if (this._redoCol.Count > 0)
                {
                    this.SuspendRefresh();
                    this.SuspendScroll();
                    int iSelStart = this.SelectionStart;
                    UndoState uState = this._redoCol.GetLast();
                    this.SelectionStart = uState.CharPosition;
                    this.SelectionLength = uState.PreviousText.Length;
                    this.SelectedText = uState.Text;
                    this._undoCol.Add(uState);
                    this._redoCol.RemoveLast();

                    // For delete keypresses, we want to keep the cursor where it
                    //   was when the delete key was pressed.
                    if (uState.StateType == UndoStateType.Delete || uState.StateType == UndoStateType.Clear)
                        this.SelectionStart = (int)System.Math.Min(iSelStart, this.Text.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occured while trying to undo:\n\n" + ex.Message, "Sorry");
            }
            finally
            {
                this.ResumeRefresh();
                this.ResumeScroll();
            }
        }
        protected void AddUndo(UndoState val)
        {
            this._redoCol.Clear();
            this._undoCol.Add(val);
        }
        private void CreatePrintDocument()
        {
            if (this._printDoc == null)
            {
                this._printDoc = new System.Drawing.Printing.PrintDocument();
                this._printDoc.OriginAtMargins = false;
                this._printDoc.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDoc_onBeginPrint);
                this._printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_onPrintPage);
            }
        }
        protected void StartFind()
        {
            this._frmFindText = new RainstormStudios.Forms.frmFindText();
            this._frmFindText.FindNext += new EventHandler(this.frmFindText_onFindNext);
            this._frmFindText.FormClosed += new FormClosedEventHandler(this.frmFindText_onClosed);
            this._frmFindText.Show(this.FindForm());
        }
        protected void FindNext()
        {
            bool advCurPos = false;
            if (this.SelectionLength > 0)
            {
                if (this._frmFindText.MatchCase)
                    advCurPos = (this.SelectedText == this._frmFindText.SearchText);
                else
                    advCurPos = (this.SelectedText.ToLower() == this._frmFindText.SearchText.ToLower());
            }

            int selStart = this.SelectionStart;
            int selLength = this.SelectionLength;
            int srchStart = this.SelectionStart + ((advCurPos) ? this.SelectionLength : 0);
            int srchLength = this.Text.Length - this.SelectionStart;
            RichTextBoxFinds findType = RichTextBoxFinds.None;

            if (this._frmFindText.MatchCase)
                findType |= RichTextBoxFinds.MatchCase;
            if (this._frmFindText.WholeWordOnly)
                findType |= RichTextBoxFinds.WholeWord;
            if (this._frmFindText.SearchUp)
            {
                findType |= RichTextBoxFinds.Reverse;
                srchLength = srchStart;
                srchStart = 0;
            }

            if (this._frmFindText.SearchScope != RainstormStudios.Forms.frmFindText.Scope.FromCurrentPosition && !advCurPos)
            {
                srchStart = 0;
                srchLength = this.Text.Length;
            }

            int iChar = this.Find(this._frmFindText.SearchText, srchStart, srchLength + srchStart, findType);

            if (iChar < 0)
            {
                MessageBox.Show(this.FindForm(), "Requested text was not found.", "Not Found");
                this.SelectionStart = selStart;
                this.SelectionLength = selLength;
                this._frmFindText.Focus();
            }
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void undoCol_onInserted(object sender, CollectionEventArgs e)
        {
            this.RaiseTextUndoLevelChanged();
        }
        private void undoCol_onRemoved(object sender, CollectionEventArgs e)
        {
            this.RaiseTextUndoLevelChanged();
        }
        private void undoCol_onCleared(object sender, EventArgs e)
        {
            this.RaiseTextUndoLevelChanged();
        }
        private void frmFindText_onFindNext(object sender, EventArgs e)
        {
            this.FindNext();
        }
        private void frmFindText_onClosed(object sender, FormClosedEventArgs e)
        {
            if (!this._frmFindText.IsDisposed)
                this._frmFindText.Dispose();
            this._frmFindText = null;
        }
        private void printDoc_onBeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            this._printCurPg = 0;
        }
        private void printDoc_onPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Increment the current page counter.
            this._printCurPg++;

            // Get the printable area of the page.
            System.Drawing.RectangleF bnds = new RectangleF(
                (float)e.MarginBounds.X,
                (float)e.MarginBounds.Y,
                (float)e.MarginBounds.Width,
                (float)e.MarginBounds.Height);

            // Determine which text should appear on this page.
            string printTxt = string.Empty;
            for (int i = 0; i < this.Lines.Length; i++)
            {

            }
        }
        private void printDoc_onQueryPageSettings(object sender, System.Drawing.Printing.QueryPageSettingsEventArgs e)
        {
        }
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnKeyDown(KeyEventArgs e)
        {
            //this._lastKeyPress = Keys.None;
            //this._lastModifierPress = Keys.None;
            //this._lastCharPos = -1;
            //this._clearedText = string.Empty;

            AdvRichTextBox.SelectDirection newSelDir = AdvRichTextBox.SelectDirection.None;

            if (e.KeyCode == System.Windows.Forms.Keys.X && e.Modifiers == System.Windows.Forms.Keys.Control)
            {
                #region CTRL-X - Cut
                this.Cut();
                e.Handled = true;
                e.SuppressKeyPress = true;
                #endregion
            }
            if (e.KeyCode == System.Windows.Forms.Keys.V && e.Modifiers == System.Windows.Forms.Keys.Control)
            {
                #region CTRL-V - Paste
                this.Paste();
                e.Handled = true;
                e.SuppressKeyPress = true;
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Z && e.Modifiers == (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift))
            {
                #region CTRL-SHIFT-Z - Redo
                this.Redo();
                e.Handled = true;
                e.SuppressKeyPress = true;
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Z && e.Modifiers == System.Windows.Forms.Keys.Control)
            {
                #region CTRL-Z - Undo
                this.Undo();
                e.Handled = true;
                e.SuppressKeyPress = true;
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Z && e.Modifiers == System.Windows.Forms.Keys.Control)
            {
                #region CTRL-P - Print
                this.Print(this.DocumentName);
                e.Handled = true;
                e.SuppressKeyPress = true;
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                // This key does not trigger this event. Check the 'OnPreviewKeyDown' method.
                #region DELETE
                //if (this.SelectionLength > 0)
                //{
                //    // Grab the text that's about to be whacked.
                //    this.AddUndo(new UndoState(UndoStateType.Clear, string.Empty, this.SelectedText, this.SelectionStart));
                //}
                //else
                //{
                //    // Treat it like a delete.
                //    if (this.SelectionStart < this.Text.Length)
                //        try
                //        { this.AddUndo(new UndoState(UndoStateType.Delete, string.Empty, this.Text.Substring(this.SelectionStart, 1), this.SelectionStart)); }
                //        catch { }
                //}
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Back)
            {
                // This key does not trigger this event. Check the 'OnPreviewKeyDown' method.
                #region BACKSPACE
                //if (this.SelectionLength > 0)
                //{
                //    // Grab the text that's about to be whacked.
                //    this.AddUndo(new UndoState(UndoStateType.Clear, string.Empty, this.SelectedText, this.SelectionStart));
                //}
                //else
                //{
                //    // Treat it like a backspace.
                //    if (this.SelectionStart > 0)
                //        try
                //        { this.AddUndo(new UndoState(UndoStateType.Backspace, string.Empty, this.Text.Substring(this.SelectionStart - 1, 1), this.SelectionStart - 1)); }
                //        catch { }
                //}
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F && e.Modifiers == System.Windows.Forms.Keys.Control)
            {
                #region CTRL-F - Find Text
                if (this._frmFindText == null || this._frmFindText.IsDisposed)
                {
                    this.StartFind();
                }
                else
                {
                    this._frmFindText.Dispose();
                    this._frmFindText = null;
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                #region F3 - Find Next
                if (this._frmFindText != null && !this._frmFindText.IsDisposed)
                    this.FindNext();
                else
                    this.StartFind();
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Shift || e.KeyCode == System.Windows.Forms.Keys.ShiftKey)
            {
                // Prevent these keys from being processed.
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Home)
            {
                #region HOME Key
                // If the user was holding the CTRL key when they pressed home, then
                //   we just move to the very beginning of the document.
                if (e.Control)
                {
                    if (e.Shift)
                        //this.Select(0, this.SelectionStart + this.SelectionLength);
                        this.Select(this.SelectionStart, 0 - this.SelectionStart);
                    else
                        this.Select(0, 0);
                    e.Handled = true;
                }
                else
                {
                    // If the user presses the "HOME" key, then we want to move the
                    //   cursor inteligently to either the beginning of the line,
                    //   or the beginning of the actual text.
                    int curLn = this.GetLineFromCharIndex(this.SelectionStart),
                        curPos = this.SelectionStart,
                        lnStart = this.GetFirstCharIndexFromLine(curLn);

                    // If the current line doesn't begin with any whitespace, then we
                    //   have nothing to do here.
                    string lnTxt = this.Lines[curLn];
                    if (lnTxt.StartsWith("\t") || lnTxt.StartsWith(" "))
                    {
                        int wsCharCnt = (lnTxt.Length - lnTxt.TrimStart('\t', ' ').Length),
                            selLen = lnStart - (curPos + SelectionLength);
                        if (this.SelectionStart == (lnStart + wsCharCnt))
                        {
                            // If we're at the beginning of the text, then move to the
                            //   first of the line.
                            if (e.Shift)
                                //this.Select(lnStart, (curPos + this.SelectionLength) - lnStart);
                                this.Select(curPos + this.SelectionLength, selLen);
                            else
                                this.Select(lnStart, 0);
                        }
                        else
                        {
                            // If the cursor is already at the beginning of the line,
                            //   or anywhere else in the line, then move to the end
                            //   of the whitespace.
                            if (e.Shift)
                                //this.Select(lnStart + wsCharCnt, (curPos - (lnStart + wsCharCnt)) + this.SelectionLength);
                                this.Select(curPos + this.SelectionLength, selLen + wsCharCnt);
                            else
                                this.Select(lnStart + wsCharCnt, 0);
                        }
                        newSelDir = (selLen > 0) ? AdvRichTextBox.SelectDirection.Forward : AdvRichTextBox.SelectDirection.Backword;
                        e.Handled = true;
                    }
                }
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.PageUp)
            {
                #region PAGE-UP Key
                //TODO:: Write logic to determine selection direction based on
                //   current selection direction and selection length.
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.PageDown)
            {
                #region PAGE-DOWN Key
                //TODO:: Write logic to determine selection direction based on
                //   current selection direction and selection length.
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                #region UP Key
                //TODO:: Write logic to determine selection direction based on
                //   current selection direction and selection length.
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                #region DOWN Key
                //TODO:: Write logic to determine selection direction based on
                //   current selection direction and selection length.
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Left)
            {
                #region LEFT Key
                //TODO:: Write logic to determine selection direction based on
                //   current selection direction and selection length.
                #endregion
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Right)
            {
                #region RIGHT Key
                //TODO:: Write logic to determine selection direction based on
                //   current selection direction and selection length.
                #endregion
            }

            this._lastCharPos = this.SelectionStart;
            this._clearedText = this.SelectedText;
            this._lastKeyPress = e.KeyCode;
            this._lastModifierPress = e.Modifiers;
            this._selDir = newSelDir;
            if (this.IsInputKey(e.KeyCode) && !e.Control && !e.Alt)
            {
                //string keyValue = this.Text.Substring(this._lastCharPos, 1);
                KeysConverter kConv = new KeysConverter();
                string keyValue = kConv.ConvertToString(e.KeyCode);
                if (!e.Shift)
                    keyValue = keyValue.ToLower();
                this.AddUndo(new UndoState(UndoStateType.Insert, keyValue, this._clearedText, this._lastCharPos));
            }
            base.OnKeyDown(e);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            //try
            //{
            //    if (this._lastKeyPress != Keys.None && this._lastCharPos > -1 && this.IsInputKey(this._lastKeyPress))
            //    {
            //        string keyValue = this.Text.Substring(this._lastCharPos, 1);
            //        this._undoCol.Add(new UndoState(UndoStateType.Insert, keyValue, this._clearedText, this._lastCharPos));
            //    }
            //}
            //catch { }
            base.OnTextChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this._lockRedraw)
                return;

            base.OnPaint(e);
        }
        protected override void OnReadOnlyChanged(EventArgs e)
        {
            base.OnReadOnlyChanged(e);
            if (this.ReadOnly)
                HideCaret(this.Handle);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (this.ReadOnly)
                HideCaret(this.Handle);
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.ReadOnly)
                HideCaret(this.Handle);
        }
        protected override void OnVScroll(EventArgs e)
        {
            if (this._noScroll)
                return;

            base.OnVScroll(e);
            this.RaiseVScroll();
            this._lastVScPos = this.VerticalScrollPos;
        }
        protected override void OnHScroll(EventArgs e)
        {
            if (this._noScroll)
                return;

            base.OnHScroll(e);
            this.RaiseHScroll();
            this._lastHScPos = this.HorizontalScrollPos;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.ShowSelectionMargin && e.X < 10)
            {
                Cursor.Current = CustomCursor.LineSelectArrow;
            }
            else if (this.SelectionLength > 0 && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this._internalDrag = true;
                //this.Cursor = Cursors.Arrow;
            }
            else if (this.SelectionLength > 0 && e.X > ((this.ShowSelectionMargin) ? 10 : 0))
            {
                if (this.GetLineFromCharIndex(this.SelectionStart) != this.GetLineFromCharIndex(this.SelectionStart + this.SelectionLength))
                {
                    int charUnderCursor = this.GetCharIndexFromPosition(e.Location);
                    if (charUnderCursor >= this.SelectionStart && charUnderCursor <= this.SelectionStart + this.SelectionLength)
                        this.Cursor = System.Windows.Forms.Cursors.Arrow;
                    else
                        this.Cursor = System.Windows.Forms.Cursors.IBeam;
                }
                else
                {
                    Point pSelStart = this.GetPositionFromCharIndex(this.SelectionStart);
                    Point pSelEnd = this.GetPositionFromCharIndex(this.SelectionStart + this.SelectionLength);
                    using (Graphics gTmp = this.CreateGraphics())
                    {
                        SizeF charSz = gTmp.MeasureString("W", this.Font);
                        if (e.X >= pSelStart.X && e.X <= pSelEnd.X && e.Y >= pSelStart.Y && e.Y <= pSelEnd.Y + charSz.Height)
                            this.Cursor = System.Windows.Forms.Cursors.Arrow;
                        else
                            this.Cursor = System.Windows.Forms.Cursors.IBeam;
                    }
                }
            }
            //else if (this.Cursor != System.Windows.Forms.Cursors.IBeam)
            //    this.Cursor = System.Windows.Forms.Cursors.IBeam;
        }
        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            if (this.SelectionLength == 0 && this.Cursor != System.Windows.Forms.Cursors.IBeam && System.Windows.Forms.Control.MousePosition.X > ((this.ShowSelectionMargin) ? 10 : 0))
                this.Cursor = System.Windows.Forms.Cursors.IBeam;
        }
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            drgevent.Effect = (this._internalDrag)
                        ? System.Windows.Forms.DragDropEffects.Move
                        : (this.AllowDrop)
                                ? System.Windows.Forms.DragDropEffects.Copy
                                : System.Windows.Forms.DragDropEffects.None;

            base.OnDragOver(drgevent);
        }
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            try
            {
                drgevent.Effect = (this._internalDrag)
                        ? System.Windows.Forms.DragDropEffects.Move
                        : (this.AllowDrop)
                                ? System.Windows.Forms.DragDropEffects.Copy
                                : System.Windows.Forms.DragDropEffects.None;
                this.AddUndo(new UndoState(UndoStateType.Move, (string)drgevent.Data.GetData(System.Windows.Forms.DataFormats.Text), "", this.GetCharIndexFromPosition(this.PointToClient(new Point(drgevent.X, drgevent.Y)))));
                base.OnDragDrop(drgevent);
            }
            finally
            {
                this._internalDrag = false;
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 8)
            {
                #region BACKSPACE
                if (this.SelectionLength > 0)
                {
                    // Grab the text that's about to be whacked.
                    this.AddUndo(new UndoState(UndoStateType.Clear, string.Empty, this.SelectedText, this.SelectionStart));
                }
                else
                {
                    // Treat it like a backspace.
                    if (this.SelectionStart > 0)
                        try
                        { this.AddUndo(new UndoState(UndoStateType.Backspace, string.Empty, this.Text.Substring(this.SelectionStart - 1, 1), this.SelectionStart - 1)); }
                        catch { }
                }
                #endregion
            }
            else if (e.KeyValue == 46)
            {
                // Delete Key
                #region DELETE
                if (this.SelectionLength > 0)
                {
                    // Grab the text that's about to be whacked.
                    this.AddUndo(new UndoState(UndoStateType.Clear, string.Empty, this.SelectedText, this.SelectionStart));
                }
                else
                {
                    // Treat it like a delete.
                    if (this.SelectionStart < this.Text.Length)
                        try
                        { this.AddUndo(new UndoState(UndoStateType.Delete, string.Empty, this.Text.Substring(this.SelectionStart, 1), this.SelectionStart)); }
                        catch { }
                }
                #endregion
            }
            base.OnPreviewKeyDown(e);
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)Win32Messages.WM_PAINT || m.Msg == (int)Win32Messages.WM_SYNCPAINT || m.Msg == (int)Win32Messages.WM_ERASEBKGND)
            {
                if (this._lockRedraw)
                    return;

                #region DEPRECIATED - This Control *always* redraws its entire client area
                //Rectangle clipRegion = Rectangle.Empty;
                //if (m.LParam != IntPtr.Zero)
                //{
                //    RECT clip = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                //    clipRegion = new Rectangle(clip.left, clip.top, clip.Width, clip.Height);
                //}
                //else
                //{
                //    clipRegion = this.Bounds;
                //}
                //this.OnPaint(new PaintEventArgs(Graphics.FromHwndInternal(m.HWnd), clipRegion));
                #endregion
                base.WndProc(ref m);
            }
            else if (m.Msg == (int)Win32Messages.WM_VSCROLL || m.Msg == (int)Win32Messages.WM_HSCROLL)
            {
                if (this._noScroll)
                    return;

                base.WndProc(ref m);
            }
            base.WndProc(ref m);
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void RaiseTextCut()
        {
            if (this.TextCut != null)
                this.TextCut.Invoke(this, EventArgs.Empty);
        }
        protected virtual void RaiseTextPaste()
        {
            if (this.TextPaste != null)
                this.TextPaste.Invoke(this, EventArgs.Empty);
        }
        protected virtual void RaiseTextUndo()
        {
            if (this.TextUndo != null)
                this.TextUndo.Invoke(this, EventArgs.Empty);
        }
        protected virtual void RaiseTextRedo()
        {
            if (this.TextRedo != null)
                this.TextRedo.Invoke(this, EventArgs.Empty);
        }
        protected virtual void RaiseTextUndoLevelChanged()
        {
            if (this.TextUndoLevelChanged != null)
                this.TextUndoLevelChanged.Invoke(this, EventArgs.Empty);
        }
        protected virtual void RaiseVScroll()
        {
            if (this.VScroll != null)
                this.VScroll.Invoke(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, this._lastVScPos, this.VerticalScrollPos, ScrollOrientation.VerticalScroll));
        }
        protected virtual void RaiseHScroll()
        {
            if (this.HScroll != null)
                this.HScroll.Invoke(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, this._lastHScPos, this.HorizontalScrollPos, ScrollOrientation.HorizontalScroll));
        }
        #endregion
    }
    public class AdvRichTextPrintController : System.Drawing.Printing.StandardPrintController
    {
        #region Public Methods
        //***************************************************************************
        // Base-Overrides
        // 
        public override void OnStartPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e)
        {
            base.OnStartPrint(document, e);
        }
        public override Graphics OnStartPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e)
        {
            return base.OnStartPage(document, e);
        }
        public override void OnEndPage(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintPageEventArgs e)
        {
            base.OnEndPage(document, e);
        }
        public override void OnEndPrint(System.Drawing.Printing.PrintDocument document, System.Drawing.Printing.PrintEventArgs e)
        {
            base.OnEndPrint(document, e);
        }
        #endregion
    }
}