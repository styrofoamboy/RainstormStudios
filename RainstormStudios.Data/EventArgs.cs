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

namespace RainstormStudios.Data
{
    /// <summary>
    /// Custom event argument class for the RowProcessed events.
    /// </summary>
    public class RowProcessedEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private long
            _rowCount,
            _rowTotal;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Returns a long value indicating the total number of rows processed.
        /// </summary>
        public long RowCount
        { get { return _rowCount; } }
        /// <summary>
        /// Returns a long value indicating the total number of rows to be processed.
        /// </summary>
        public long RowTotal
        { get { return _rowTotal; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        /// <summary>
        /// Custom event argument class for the RowProcessed events.
        /// </summary>
        /// <param name="rowCount">A long value indicating the total number of rows processed.</param>
        /// <param name="rowTotal">A long value indicating the total number of rows to be processed.</param>
        public RowProcessedEventArgs(long rowCount, long rowTotal)
            : base()
        {
            this._rowCount = rowCount;
            this._rowTotal = rowTotal;
        }
        #endregion
    }
    public class WriteToDbMethodCompleteEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private WriteToDbAttempt
            _atmpt;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public WriteToDbAttempt AttemptDetails
        { get { return this._atmpt; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public WriteToDbMethodCompleteEventArgs(WriteToDbAttempt details)
        {
            this._atmpt = details;
        }
        #endregion
    }
    public class WriteToDbOperationCompleteEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private WriteToDbAttemptCollection
            _atmpts;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public WriteToDbAttemptCollection AttemptDetails
        { get { return this._atmpts; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public WriteToDbOperationCompleteEventArgs(WriteToDbAttemptCollection details)
        {
            this._atmpts = details;
        }
        public WriteToDbOperationCompleteEventArgs(WriteToDbAttempt[] details)
            : this(new WriteToDbAttemptCollection(details))
        { }
        #endregion
    }
}
