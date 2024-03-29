﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UsAcRe.Core.WindowsSystem {
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

		public const int INPUT_MOUSE = 0;
		public const int INPUT_KEYBOARD = 1;
		public const int INPUT_HARDWARE = 2;
		public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		public const uint KEYEVENTF_KEYUP = 0x0002;
		public const uint KEYEVENTF_UNICODE = 0x0004;
		public const uint KEYEVENTF_SCANCODE = 0x0008;

		public const int VKeyShiftMask = 0x0100;
		public const int VKeyCtrlMask = 0x0200;
		public const int VKeyAltMask = 0x0400;
		public const int VKeyCharMask = 0x00FF;

		public const uint XBUTTON1 = 0x0001;
		public const uint XBUTTON2 = 0x0002;

		public const int VK_LBUTTON = 0x0001;
		public const int VK_RBUTTON = 0x0002;
		public const int VK_MBUTTON = 0x0004;
		public const int VK_XBUTTON1 = 0x0005;
		public const int VK_XBUTTON2 = 0x0006;

		public const int SMXvirtualscreen = 76;
		public const int SMYvirtualscreen = 77;
		public const int SMCxvirtualscreen = 78;
		public const int SMCyvirtualscreen = 79;

		public const int MouseeventfVirtualdesk = 0x4000;
		public const int WheelDelta = 120;

		public const int CWP_ALL = 0;
		public const int CWP_SKIPDISABLED = 0x0002;
		public const int CWP_SKIPINVISIBLE = 0x0001;
		public const int CWP_SKIPTRANSPARENT = 0x0004;


		public const int GW_CHILD = 5;
		public const int GW_ENABLEDPOPUP = 6;
		public const int GW_HWNDFIRST = 0;
		public const int GW_HWNDLAST = 1;
		public const int GW_HWNDNEXT = 2;
		public const int GW_HWNDPREV = 3;
		public const int GW_OWNER = 4;

		public const int GA_PARENT = 1;
		public const int GA_ROOT = 2;
		public const int GA_ROOTOWNER = 3;

		public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_MINIMIZE = 0x0F020;
		public const int SC_MAXIMIZE = 0xF030;
		public const int SC_RESTORE = 0xF120;

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

			public bool WithBoundaries(int otherX, int otherY, int tolerance) {
				return Math.Abs(x - otherX) <= tolerance && Math.Abs(y - otherY) <= tolerance;
			}

			public static POINT Empty {
				get { return new POINT(0, 0); }
			}

			public override string ToString() {
				return $"x={x},y={y}";
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

			public bool Contains(int x, int y) {
				var width = right - left;
				var height = bottom - top;
				return ((x >= left) && (x - width <= left) &&
						(y >= top) && (y - height <= top));
			}

			public override string ToString() {
				var width = right - left;
				var height = bottom - top;
				return string.Format("x:{0}, y:{1}, width:{2}, height:{3}", left, top, width, height);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWINFO {
			public uint cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public uint dwStyle;
			public uint dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public ushort atomWindowType;
			public ushort wCreatorVersion;

			public WINDOWINFO(Boolean? filler) : this() {  // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
				cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SIZE {
			public int cx;
			public int cy;
		}

		[Flags]
		public enum SendMouseInputFlags {
			Move = 0x0001,
			LeftDown = 0x0002,
			LeftUp = 0x0004,
			RightDown = 0x0008,
			RightUp = 0x0010,
			MiddleDown = 0x0020,
			MiddleUp = 0x0040,
			XDown = 0x0080,
			XUp = 0x0100,
			Wheel = 0x0800,
			Absolute = 0x8000,
		};

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr WindowFromPoint(POINT point);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, POINT pt, uint uFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr RealChildWindowFromPoint(IntPtr hWndParent, POINT pt);

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

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

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

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern short VkKeyScan(char ch);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("User32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hwnd);

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
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, int flags);

		public const int SRCCOPY = 13369376;
		public const int SM_CXSCREEN = 0;
		public const int SM_CYSCREEN = 1;

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDC(IntPtr ptr);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetSystemMetrics(int abc);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetWindowDC(IntPtr ptr);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

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

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetTopWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hwnd, uint uCmd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

		public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);
		[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IntersectRect([Out] out RECT lprcDst, [In] ref RECT lprcSrc1, [In] ref RECT lprcSrc2);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsIconic(IntPtr hWnd);

		[Flags]
		public enum KeyModifiers {
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			// Either WINDOWS key was held down. These keys are labeled with the Windows logo.
			// Keyboard shortcuts that involve the WINDOWS key are reserved for use by the
			// operating system.
			Windows = 8
		}

		[DllImport("user32", SetLastError = true)]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport("user32", SetLastError = true)]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("kernel32", SetLastError = true)]
		public static extern short GlobalAddAtom(string lpString);
		[DllImport("kernel32", SetLastError = true)]
		public static extern short GlobalDeleteAtom(short nAtom);
	}
}
