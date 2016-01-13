using System;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data
{
    public delegate void RowProcessedEventHandler(object sender, RowProcessedEventArgs e);
    public delegate void WriteToDbMethodCompleteEventHandler(object sender, WriteToDbMethodCompleteEventArgs e);
    public delegate void WriteToDbOperationCompleteEventHandler(object sender, WriteToDbOperationCompleteEventArgs e);
}
