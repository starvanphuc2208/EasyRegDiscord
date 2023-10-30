using System;
using System.Runtime.InteropServices;

namespace MCommon
{
	[StructLayout(LayoutKind.Explicit)]
	public struct MOUSEKEYBDHARDWAREINPUT
	{
		[FieldOffset(0)]
		public HARDWAREINPUT Hardware;

		[FieldOffset(0)]
		public KEYBDINPUT Keyboard;

		[FieldOffset(0)]
		public MOUSEINPUT Mouse;
	}
}