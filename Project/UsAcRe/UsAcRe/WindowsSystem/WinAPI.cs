using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UsAcRe.WindowsSystem {
	public static class WinAPI {
		public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
		public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		public const int SW_HIDE = 0;
		public const int SW_SHOWNORMAL = 1;
		public const int SW_SHOWMINIMIZED = 2;
		public const int SW_SHOWMAXIMIZED = 3;
		public const int SW_SHOW = 5;

		public const int WH_MOUSE_LL = 14;
		public const int WH_KEYBOARD_LL = 13;

		public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
		public const uint MOUSEEVENTF_MOVE = 0x0001;
		public const uint MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
		public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		public const uint MOUSEEVENTF_LEFTUP = 0x0004;
		public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;


		public const uint MK_CONTROL = 0x0008; //The CTRL key is down.

		public const uint MK_LBUTTON = 0x0001; //The left mouse button is down.

		public const uint MK_MBUTTON = 0x0010; //The middle mouse button is down.

		public const uint MK_RBUTTON = 0x0002; //The right mouse button is down.

		public const uint MK_SHIFT = 0x0004; //The SHIFT key is down.

		public const uint MK_XBUTTON1 = 0x0020; //The first X button is down.

		public const uint MK_XBUTTON2 = 0x0040; //The second X button is down.


		public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		public const uint KEYEVENTF_KEYUP = 0x0002;


		public const int CWP_ALL = 0;
		public const int CWP_SKIPDISABLED = 0x0002;
		public const int CWP_SKIPINVISIBLE = 0x0001;
		public const int CWP_SKIPTRANSPARENT = 0x0004;

		public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

		[StructLayout(LayoutKind.Sequential)]
		public struct KBDLLHOOKSTRUCT {
			public uint vkCode;
			public uint scanCode;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MSLLHOOKSTRUCT {
			public POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int x;
			public int y;

			public POINT(int x, int y) {
				this.x = x;
				this.y = y;
			}

			public bool WithBoundaries(int otherX, int otherY, int toleranceX, int toleranceY) {
				return Math.Abs(x - otherX) <= toleranceX && Math.Abs(y - otherY) <= toleranceX;
			}

			public bool WithBoundaries(POINT other, int tolerance) {
				return Math.Abs(x - other.x) <= tolerance && Math.Abs(y - other.y) <= tolerance;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT {
			public uint Type;
			public MOUSEKEYBDHARDWAREINPUT Data;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct MOUSEKEYBDHARDWAREINPUT {
			[FieldOffset(0)]
			public HARDWAREINPUT Hardware;
			[FieldOffset(0)]
			public KEYBDINPUT Keyboard;
			[FieldOffset(0)]
			public MOUSEINPUT Mouse;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT {
			public uint Msg;
			public ushort ParamL;
			public ushort ParamH;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT {
			public ushort Vk;
			public ushort Scan;
			public uint Flags;
			public uint Time;
			public IntPtr ExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT {
			public int X;
			public int Y;
			public uint MouseData;
			public uint Flags;
			public uint Time;
			public IntPtr ExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public uint left;
			public uint top;
			public uint right;
			public uint bottom;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr WindowFromPoint(POINT point);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, POINT pt, uint uFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(int idHook,
		  Delegate lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
		  IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		//[DllImport("user32.dll"), CharSet = CharSet.Auto, SetLastError = true]
		//public static extern int MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern short GetAsyncKeyState(int vKey);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetKeyboardState(byte[] keystate);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern short GetKeyState(int keyCode);

		//[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		//public static extern int SendMessage(IntPtr hWnd, int msg, int Param, StringBuilder text);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern UInt32 GetTickCount();

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetDoubleClickTime();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetCursorPos(int x, int y);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetCursorPos(out POINT pt);


		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("User32", SetLastError = true)]
		public static extern int SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetClientRect(IntPtr hWnd, ref RECT rect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, int flags);

		internal const int SRCCOPY = 13369376;
		internal const int SM_CXSCREEN = 0;
		internal const int SM_CYSCREEN = 1;

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		internal static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr GetDC(IntPtr ptr);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern int GetSystemMetrics(int abc);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr GetWindowDC(IntPtr ptr);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SendMessage(IntPtr hwnd, uint wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetFocus(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

	}
}
