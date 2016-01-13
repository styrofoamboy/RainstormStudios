using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace RainstormStudios.Unmanaged
{
    public class HotKeys
    {
		[Flags()]
			public enum HotkeyModifiers
		{
			MOD_ALT         = 0x0001,
			MOD_CONTROL     = 0x0002,
			MOD_SHIFT       = 0x0004,
			MOD_WIN         = 0x0008
		}

		[DllImport("User32")]
		public static extern int RegisterHotKey(IntPtr hWnd, int id, uint modifiers, uint virtualkeyCode);

		[DllImport("User32")]
		public static extern int UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("Kernel32")]
		public static extern short GlobalAddAtom(string atomName);

		[DllImport("Kernel32")]
		public static extern short GlobalDeleteAtom(short atom);		
    }
}
