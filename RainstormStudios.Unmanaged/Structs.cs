using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RainstormStudios.Unmanaged
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowInfo
    {
        // Declarations
        public int cbSize;
        public Rectangle window;
        public Rectangle client;
        public int style;
        public int exStyle;
        public int windowStatus;
        public uint xWindowBorders;
        public uint yWindowBorders;
        public short atomWindowtype;
        public short creatorVersion;
    }

    public struct WindowPlacement
    {
        // Declarations
        public int length;
        public int flags;
        public int showCmd;
        public Point minPosition;
        public Point maxPosition;
        public Rectangle normalPosition;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ScrollBarInfo
    {
        // Declarations
        public int cbSize;
        public RECT rcScrollBar;
        public int dxyLineBottom;
        public int xyThumbTop;
        public int xyThumbBottom;
        public int reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] rgState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        // Declarations
        public int dx, dy;
        public uint mouseData, dwFlags, time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        // Declarations
        public ushort wVk, wScan;
        public uint dwFlags, time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL, wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public int type;
        [FieldOffset(4)]
        public MOUSEINPUT mi;
        [FieldOffset(4)]
        public KEYBDINPUT ki;
        [FieldOffset(4)]
        public HARDWAREINPUT hi;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        #region Declarations
        //***************************************************************************
        // Public Functions
        // 
        public int
            left,
            top,
            right,
            bottom;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int Width
        { get { return right - left; } }
        public int Height
        { get { return bottom - top; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static RECT FromRectangle(Rectangle rect)
        {
            return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
        #endregion
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static POINT FromPoint(System.Drawing.Point pt)
        {
            return new POINT(pt.X, pt.Y);
        }
    }
}
