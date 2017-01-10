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
using System.Linq;
using System.Text;
using System.Threading;
using RainstormStudios.Collections;

namespace RainstormStudios.IO
{
    /// <summary>
    /// Manages auto-save files across different applications.
    /// NOTE:  This does not currently work properly across application boundries.
    /// </summary>
    /// <remarks>
    /// In order to make this work correctly (can handle multiple running instances of an application) I am going to have
    /// to use Remoting and setup some kind of Remote Host to handle the data and keep track all the processes.
    /// </remarks>
    public static class AutoSaveManager
    {
        #region Nested Types
        //***************************************************************************
        // Public Types
        // 
        public delegate bool
            SaveFileDelegate(string autosaveFileName);
        //***************************************************************************
        // Private Types
        // 
        class AutoSaveProc
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            Guid
                _id;
            string
                _name,
                _appDataPath,
                _fn;
            SaveFileDelegate
                _del;
            bool
                _dirty;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public Guid ProcessID
            { get { return this._id; } }
            public string ProcessName
            { get { return this._name; } }
            public SaveFileDelegate SaveFileMethod
            { get { return this._del; } }
            public bool IsDirty
            { get { return this._dirty; } }
            public string AppDataPath
            { get { return this._appDataPath; } }
            public string AutoSaveFileName
            {
                get
                {
                    if (string.IsNullOrEmpty(this._fn))
                        this._fn = System.IO.Path.Combine(this._appDataPath, AutoSaveManager.GetAutoSaveFileName(this));
                    return this._fn;
                }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public AutoSaveProc(Guid id, string appDataPath, string name, SaveFileDelegate del)
            {
                this._id = id;
                this._appDataPath = appDataPath;
                this._name = name;
                this._del = del;
                this._dirty = false;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void MarkDirty()
            { this._dirty = true; }
            public void MarkSaved()
            { this._dirty = false; }
            #endregion
        }
        class AutoSaveProcCollection : ObjectCollectionBase<AutoSaveProc>
        {
            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public AutoSaveProc this[Guid key]
            { get { return base[key.ToString()]; } }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public AutoSaveProcCollection()
                : base()
            {
                this.ReturnNullForIndexNotFound = false;
                this.ReturnNullForKeyNotFound = true;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void Add(AutoSaveProc proc)
            { base.Add(proc, proc.ProcessID.ToString()); }
            public bool ContainsKey(Guid procID)
            { return base.ContainsKey(procID.ToString()); }
            public void RemoveByKey(Guid procID)
            { base.RemoveByKey(procID.ToString()); }
            #endregion
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        static AutoSaveProcCollection
            _activeProc;
        static int
            _autoSaveInterval;
        static bool
            _terminate,
            _threadRunning;
        static ManualResetEvent
            _mre;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public static bool AutoSaveThreadRunning
        { get { return _threadRunning; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static AutoSaveManager()
        {
            _activeProc = new AutoSaveProcCollection();
            _mre = new ManualResetEvent(false);
            _autoSaveInterval = 300;
            _threadRunning = false;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public static void RegisterProcess(Guid procID, string appDataPath, string name, SaveFileDelegate del)
        {
            _activeProc.Add(new AutoSaveProc(procID, appDataPath, name, del));
            if (!_threadRunning && _activeProc.Count > 0)
                StartThread();
        }
        public static void DeregisterProcess(Guid procID)
        {
            var proc = _activeProc[procID];
            if (proc == null)
                throw new Exception("Specified process is not registered with the AutoSaveManager.");

            if (System.IO.File.Exists(proc.AutoSaveFileName))
                System.IO.File.Delete(proc.AutoSaveFileName);

            _activeProc.RemoveByKey(procID);

            if (_threadRunning && _activeProc.Count < 1)
                StopThread();
        }
        public static void SetInterval(int intervalInSeconds)
        {
            _autoSaveInterval = intervalInSeconds;
            if (_threadRunning)
                ResetThread();
        }
        public static void MarkNeedSave(Guid procID)
        { _activeProc[procID].MarkDirty(); }
        public static System.IO.FileInfo[] GetExistingFiles(Guid procID)
        {
            var proc = _activeProc[procID];
            System.IO.DirectoryInfo tempDir = new System.IO.DirectoryInfo(proc.AppDataPath);
            System.IO.FileInfo[] tempFiles = tempDir.GetFiles(AutoSaveManager.GetAutoSaveSearch(proc));
            tempFiles = tempFiles.Where(f => !_activeProc.Any(p => p.AutoSaveFileName == f.Name)).ToArray();
            return tempFiles;
        }
        public static void ClearExistingTempFiles(Guid procID)
        {
            System.IO.FileInfo[] tempFiles = GetExistingFiles(procID);
            for (int i = 0; i < tempFiles.Length; i++)
                tempFiles[i].Delete();
        }
        public static void ForceAutoSave(Guid procID)
        {
            var proc = _activeProc[procID];
            if (proc == null)
                throw new Exception("Specified processID was not found.");
            DoAutoSave(proc);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private static void StartThread()
        {
            _terminate = false;
            RainstormStudios.GenericCrossThreadDelegate del = new RainstormStudios.GenericCrossThreadDelegate(AutoSaveThread);
            del.BeginInvoke(new AsyncCallback(AutoSaveThreadCallback), del);
        }
        private static void StopThread()
        {
            _terminate = true;
            _mre.Set();
        }
        private static void ResetThread()
        {
            StopThread();

            DateTime dtWaitStart = DateTime.Now;
            while (_threadRunning && DateTime.Now.Subtract(dtWaitStart).TotalSeconds < 5)
                System.Threading.Thread.SpinWait(1000);

            if (_threadRunning)
                // If the thread is still running at this point, something is very wrong.
                throw new Exception("Unable to reinitialize auto save thread.");

            StartThread();
        }
        private static void DoAutoSave(AutoSaveProc proc)
        {
            if (proc.SaveFileMethod.Invoke(proc.AutoSaveFileName))
                proc.MarkSaved();
        }
        private static string GetAutoSaveSearch(AutoSaveProc proc)
        {
            return string.Format("autosave_{0}_*.tmp", proc.ProcessName);
        }
        private static string GetAutoSaveFileName(AutoSaveProc proc)
        {
            return string.Format("autosave_{0}_{1}_{2}.tmp", proc.ProcessName, DateTime.Now.ToString("MMddyy"), Guid.NewGuid());
        }
        //***************************************************************************
        // Thread Workers
        // 
        private static void AutoSaveThread()
        {
            TimeSpan tsWait = new TimeSpan(0, 0, _autoSaveInterval);
            _threadRunning = true;
            while (!_terminate)
            {
                _mre.WaitOne(tsWait);
                if (_terminate)
                    break;

                // Reset now, because the "Reset" call is async and can sometimes take long enough to complete
                //   that another loop iteration will have occured before the MRE is flipped back.
                _mre.Reset();

                // Save the files.
                foreach (var proc in _activeProc.Where(p => p.IsDirty))
                    DoAutoSave(proc);
            }
        }
        private static void AutoSaveThreadCallback(IAsyncResult state)
        {
            // Nothing to do here except make sure we properly terminate the secondary thread.
            RainstormStudios.GenericCrossThreadDelegate del = (RainstormStudios.GenericCrossThreadDelegate)state.AsyncState;
            del.EndInvoke(state);
            _threadRunning = false;
        }
        #endregion
    }
}
