using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Forms
{
    public partial class frmAsyncProgress : Form
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private iASyncSimpleProgress
            _process;
        private Exception
            _abortEx;
        private DateTime
            _asyncStart,
            _asyncStop;
        private Timer
            _timer;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler ProcessComplete;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string Caption
        {
            get { return this.lblCaption.Text; }
            set { this.lblCaption.Text = value; }
        }
        public int ProgressMax
        {
            get { return this.progressBar1.Maximum; }
            set { this.progressBar1.Minimum = value; }
        }
        public int ProgressValue
        {
            get { return this.progressBar1.Value; }
            set { this.progressBar1 = value; }
        }
        public Exception AbortException
        { get { return this._process.AbortException; } }
        public object AsyncResult
        { get { return this._process.ReturnResult; } }
        public DateTime AsyncStart
        { get { return this._asyncStart; } }
        public DateTime AsyncStop
        { get { return this._asyncStop; } }
        public TimeSpan TimeTaken
        { get { return this._asyncStop.Subtract(this._asyncStart); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private frmAsyncProgress()
        {
            InitializeComponent();
            this._timer = new Timer();
            this._timer.Interval = 500;
            this._timer.Tick += new EventHandler(this.timer_onTick);
            this._timer.Start();
        }
        public frmAsyncProgress(iASyncProgress processor)
        {
            this._process = processor;
            this._process.AsyncAbort += new EventHandler(processor_AsyncAbort);
            this._process.AsyncComplete += new EventHandler(processor_AsyncComplete);
            this._process.AsyncStart += new EventHandler(processor_AsyncStart);
            this._process.ProgressUpdate += new EventHandler(processor_ProgressUpdate);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Thread-Safe UI Update Methods
        // 
        private delegate void UpdateProgressDelegate(int value);
        private void UpdateProgress(int value)
        {
            if (this.progressBar1.InvokeRequired)
            {
                UpdateProgressDelegate del = new UpdateProgressDelegate(this.UpdateProgress);
                BeginInvoke(del, value);
            }
            else
                this.progressBar1.Value = value;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        void timer_onTick(object sender, EventArgs e)
        {
            this._timer.Stop();
            this._process.BeginAsync();
        }
        void cmdAbort_onClick(object sender, EventArgs e)
        { this._process.Abort(); }
        void processor_ProgressUpdate(object sender, EventArgs e)
        { this.UpdateProgress(this._process.ProgressValue); }
        void processor_AsyncStart(object sender, EventArgs e)
        { this._asyncStart = DateTime.Now; }
        void processor_AsyncComplete(object sender, EventArgs e)
        {
            this._asyncStop = DateTime.Now;
            this.ProcessCompleteEvent();
        }
        void processor_AsyncAbort(object sender, ASyncProgressAbortEventArgs e)
        {
            this._asyncStop = DateTime.Now;
            this.ProcessCompleteEvent();
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        // 
        private void ProcessCompleteEvent()
        {
            if (this.ProcessComplete != null)
                this.ProcessComplete.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}