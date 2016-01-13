using System;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios
{
    public interface iASyncProgress
    {
        //***************************************************************************
        // Event Declarations
        // 
        event EventHandler ProgressUpdate;
        event EventHandler AsyncAbort;
        event EventHandler AsyncStart;
        event EventHandler AsyncComplete;
        //***************************************************************************
        // Property Declarations
        // 
        object ReturnResult { get;}
        int ProgressValue { get;}
        Exception AbortException { get;}
        //***************************************************************************
        // Method Declarations
        // 
        void BeginAsync();
        void Abort();
    }
    public class ASyncProgressMessageEventArgs : EventArgs
    {
        //***************************************************************************
        // Public Fields
        // 
    }
}
