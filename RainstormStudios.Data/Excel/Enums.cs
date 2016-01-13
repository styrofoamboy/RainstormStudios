using System;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data.Excel
{
    public enum StreamType
    {
        Empty = 0x00,
        UserStorage = 0x01,
        UserStream = 0x02,
        LockBytes = 0x03,
        Property = 0x04,
        RootStorage = 0x05
    }
    public enum NodeColor
    {
        Red = 0x00,
        Black = 0x01
    }
}
