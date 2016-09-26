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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios
{
    public class LogMessage
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private SeverityLevel
            severity;
        private string
            message,
            module;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public LogMessage()
            : this(SeverityLevel.Debug, "", "")
        { }
        public LogMessage(SeverityLevel Level, string Message, string ModuleName)
        {
            severity = Level;
            message = Message;
            module = ModuleName;
        }
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string ModuleName
        {
            get { return module; }
            set { module = value; }
        }
        public SeverityLevel Severity
        {
            get { return severity; }
            set { severity = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        //***************************************************************************
        // Static Methods
        // 
        static public string GetSeverityDescription(SeverityLevel Level)
        {
            switch (Level)
            {
                case SeverityLevel.Fatal:
                    return "This is a non-recoverable problem.";
                case SeverityLevel.Error:
                    return "This is a serious problem that may be recoverable.";
                case SeverityLevel.Warning:
                    return "This is something to look into, but it probably isn't serious.";
                case SeverityLevel.Information:
                    return "This is a general bit of application information.";
                case SeverityLevel.Debug:
                    return "This is a note that helps track down a bug.";
                default:
                    return "Cannot recognize the Severity Level provided!";
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides an encapsulated logging class which writes data in a strictly-defined
    /// format, enabling cohesive viewing of log files.
    /// </summary>
    /// <remarks>
    /// This class is implemented as a Singleton.
    /// A Singleton is a class which only allows a single instance of itself to be
    /// created, and ususally gives simple access to that instance.
    /// 
    /// This class is sealed. This is unnecessary, strictly speaking, due to the
    /// type of constructor used, but may help the JIT to optimize things more.
    /// 
    /// http://www.yoda.arachsys.com/csharp/singleton.html
    /// </remarks>
    public sealed class Logger
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _logFn,
            _logPath,
            _arcPath,
            _logExt,
            _lastWrtFn;
        private bool
            _dtRoll;
        private int
            _maxSz;
        //***************************************************************************
        // Static Fields
        // 
        /// <summary>
        /// A static variable which holds a reference to the single created instance.
        /// </summary>
        static readonly Logger
            internalInstance = new Logger();
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// A puplic static means of getting the reference to the single created
        /// instance, creating one if necessary.
        /// </summary>
        /// <remarks>
        /// Singletons are created "lazily" - the instance isn't created until
        /// it is first needed.
        /// </remarks>
        public static Logger Instance
        {
            get { return internalInstance; }
        }
        /// <summary>
        /// Gets or sets the maximum file size in bytes that should be used for log
        /// files. Set this value to '0' to disable log size handling.
        /// </summary>
        public int MaxFileSize
        {
            get { return this._maxSz; }
            set { this._maxSz = value; }
        }
        /// <summary>
        /// Gets or sets the directory where log files will be created.
        /// </summary>
        public string LogFilesDirectory
        {
            get { return this._logPath; }
            set { this._logPath = rsString.RemoveChars(value, Path.GetInvalidPathChars()); }
        }
        /// <summary>
        /// Gets or sets the name to append to the beginning of generated
        /// log file names.
        /// </summary>
        public string LogFileName
        {
            get { return this._logFn; }
            set { this._logFn = rsString.RemoveChars(value, Path.GetInvalidFileNameChars()); }
        }
        /// <summary>
        /// Gets or sets the extension used in log file names.
        /// </summary>
        public string LogFilesExtension
        {
            get { return this._logExt; }
            set { this._logExt = rsString.RemoveChars(value, Path.GetInvalidFileNameChars()).TrimStart('.').Trim(); }
        }
        /// <summary>
        /// Gets or sets the directory location where log files get archived to if
        /// they exceed the maxium file size. Set this value to an empty string to
        /// prevent log files from being archived.
        /// </summary>
        public string LogArchivePath
        {
            get { return this._arcPath; }
            set { this._arcPath = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value indicating 'true' if a new log file should
        /// be created for each new date, or 'false' if the same file should be used
        /// until it reaches the maximum log file size.
        /// </summary>
        public bool RollOverOnNewDate
        {
            get { return this._dtRoll; }
            set { this._dtRoll = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// A single constructor, which is private and parameterless.
        /// </summary>
        /// <remarks>
        /// This prevents other classes from instantiating it
        /// (which would be a violation of the singleton patern). It also prevents
        /// subclassing. If a singleton can be subclassed, and if each of those
        /// subclasses can create an instance, the patern is violated.
        /// 
        /// http://www.yoda.arachsys.com/csharp/singleton.html
        /// </remarks>
        private Logger()
        {
            this._maxSz = 500000;
            this._logFn = "ex";
            this._logPath = Path.Combine(System.Environment.CurrentDirectory, "logs");
            this._arcPath = string.Empty;
            this._logExt = ".log";
            this._dtRoll = true;
        }
        /// <summary>
        /// Explicit static constructor.
        /// </summary>
        /// <remarks>
        /// Explicit static constructor to tell C# compiler not to mark type as 'beforefieldinit'.
        /// 
        /// http://www.yoda.arachsys.com/csharp/beforefieldinit.html
        /// </remarks>
        static Logger()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Writes an entry to the defined event log.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteToLog(LogMessage msg)
        {
            try
            {
                if (!Directory.Exists(this._logPath))
                    Directory.CreateDirectory(this._logPath);

                // Determine the filename and path of the the logfile we need to write to.
                string logFileNm = string.Empty;
                if (this._dtRoll)
                {
                    logFileNm = Path.Combine(this._logPath, this._logFn + DateTime.Now.ToString("yyyyMMdd") + "." + this._logExt.Trim('.', ' '));
                }
                else
                {
                    // If we're not rolling to a new file each day, check to see if we've
                    //   already stored an explicit filename to write to.
                    if (!string.IsNullOrEmpty(this._lastWrtFn) && Path.GetDirectoryName(this._lastWrtFn) == this._logPath && File.Exists(this._lastWrtFn))
                        logFileNm = this._lastWrtFn;
                    if (!File.Exists(logFileNm))
                    {
                        // If not, grab the last file created in the specified log folder.
                        string[] exFiles = Directory.GetFiles(this._logPath);
                        if (exFiles.Length > 0)
                            logFileNm = exFiles[exFiles.Length - 1];
                        else
                        {
                            // If no files exist in the folder, then we create a new one.
                            logFileNm = Path.Combine(this._logPath, this._logFn + DateTime.Now.ToString("yyyyMMdd") + "." + this._logExt.Trim('.', ' '));
                        }
                    }
                }

                // Make sure the log file we intend to write to isn't over the size limit.
                if (this._maxSz > 0 && File.Exists(logFileNm))
                {
                    string tmpLogFn = logFileNm;
                    FileInfo fi = new FileInfo(tmpLogFn);
                    int flNum = 1;
                    while (fi.Length / 1024 > this._maxSz)
                    {
                        // If the current file exists and is over the limit, append
                        //   an incremental number to the end of the filename until
                        //   we find a file which either doesn't exist, or isn't
                        //   over the limit.
                        tmpLogFn = Path.Combine(Path.GetDirectoryName(logFileNm), Path.GetFileNameWithoutExtension(logFileNm) + "-" + Convert.ToString(flNum++) + Path.GetExtension(logFileNm));
                        if (!File.Exists(tmpLogFn))
                            break;
                        else
                            fi = new FileInfo(tmpLogFn);
                    }
                    logFileNm = tmpLogFn;
                }

                // Now that we've determined our filename, it's time to write
                //   the log entry.
                using (FileStream fs = new FileStream(logFileNm, FileMode.Append, FileAccess.Write))
                using (StreamWriter sr = new StreamWriter(fs))
                {
                    this._lastWrtFn = logFileNm;
                    string msgText = string.Format("{0}|{1}|{2}|{3}", DateTime.Now.ToString("yyyyMMddTHH:mm:ss"), ((int)msg.Severity).ToString(), msg.ModuleName, msg.Message);
                    sr.WriteLine(msgText);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(internalInstance, "Error writing to log: " + ex.Message);
                throw;
            }
        }
        #endregion
    }
}
