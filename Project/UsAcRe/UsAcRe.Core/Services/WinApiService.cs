using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Services {
	public interface IWinApiService {
		WinAPI.POINT GetMousePosition();
		IntPtr GetWindow(IntPtr hwnd, uint uCmd);
		IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);
		IntPtr GetWindow(WinAPI.POINT point);
		IntPtr GetRootWindowForElementUnderPoint(WinAPI.POINT point);
		IntPtr GetRootOwnerWindowForElementUnderPoint(WinAPI.POINT point);
		IntPtr ChildWindowFromPointEx(IntPtr hWndParent, WinAPI.POINT pt, uint uFlags);
		IntPtr RealChildWindowFromPoint(IntPtr hWndParent, WinAPI.POINT pt);
		bool ScreenToClient(IntPtr hWnd, ref WinAPI.POINT point);
		bool SetForegroundWindow(IntPtr hWnd);
		IntPtr GetWindowHandle(int dwProcessId);
		void SendKeyboardKey(VirtualKeyCodes vKCode, bool isKeyDown, bool isExtended, bool isUnicode);
		void SendMouseInput(int x, int y, uint data, WinAPI.SendMouseInputFlags flags);
		short GetKeyScan(char c);
	}

	public class WinApiService : IWinApiService {
		public WinApiService() {
		}

		public IntPtr ChildWindowFromPointEx(IntPtr hWndParent, WinAPI.POINT pt, uint uFlags) {
			return WinAPI.ChildWindowFromPointEx(hWndParent, pt, uFlags);
		}

		public IntPtr RealChildWindowFromPoint(IntPtr hWndParent, WinAPI.POINT pt) {
			return WinAPI.RealChildWindowFromPoint(hWndParent, pt);
		}

		public IntPtr GetAncestor(IntPtr hwnd, uint gaFlags) {
			return WinAPI.GetAncestor(hwnd, gaFlags);
		}

		public WinAPI.POINT GetMousePosition() {
			WinAPI.POINT pt;
			if(!WinAPI.GetCursorPos(out pt)) {
				pt = new WinAPI.POINT();
			}
			return pt;
		}

		public IntPtr GetRootWindowForElementUnderPoint(WinAPI.POINT point) {
			var hwnd = GetWindow(point);
			if(hwnd != IntPtr.Zero) {
				hwnd = GetAncestor(hwnd, WinAPI.GA_ROOT);
			}
			return hwnd;
		}

		public IntPtr GetRootOwnerWindowForElementUnderPoint(WinAPI.POINT point) {
			var hwnd = GetWindow(point);
			if(hwnd != IntPtr.Zero) {
				hwnd = GetAncestor(hwnd, WinAPI.GA_ROOTOWNER);
			}
			return hwnd;
		}

		public IntPtr GetWindow(IntPtr hwnd, uint uCmd) {
			return WinAPI.GetWindow(hwnd, uCmd);
		}

		public IntPtr GetWindow(WinAPI.POINT point) {
			return WinAPI.WindowFromPoint(point);
		}

		public bool ScreenToClient(IntPtr hWnd, ref WinAPI.POINT point) {
			return WinAPI.ScreenToClient(hWnd, ref point);
		}

		public bool SetForegroundWindow(IntPtr hWnd) {
			return WinAPI.SetForegroundWindow(hWnd);
		}

		public IntPtr GetWindowHandle(int dwProcessId) {
			try {
				IntPtr prevWindow = IntPtr.Zero;
				var desktopWindow = WinAPI.GetDesktopWindow();
				if(desktopWindow == IntPtr.Zero) {
					return IntPtr.Zero;
				}

				while(true) {
					var nextWindow = WinAPI.FindWindowEx(desktopWindow, prevWindow, null, null);
					if(nextWindow == IntPtr.Zero) {
						break;
					}

					uint procId = 0;
					WinAPI.GetWindowThreadProcessId(nextWindow, out procId);

					if(procId == dwProcessId) {
						var windowText = new StringBuilder(4096 + 1);
						if(WinAPI.IsWindowVisible(nextWindow)
							&& !WinAPI.IsIconic(nextWindow)
							&& WinAPI.GetWindowText(nextWindow, windowText, 4096) != 0
							&& WinAPI.GetParent(nextWindow) == IntPtr.Zero)
							return nextWindow;
					}
					prevWindow = nextWindow;
				}
			} catch { }
			return IntPtr.Zero;
		}

		public void SendKeyboardKey(VirtualKeyCodes vKCode, bool isKeyDown, bool isExtended, bool isUnicode) {
			var input = new WinAPI.INPUT {
				Type = WinAPI.INPUT_KEYBOARD
			};
			if(!isKeyDown) {
				input.Data.Keyboard.Flags |= WinAPI.KEYEVENTF_KEYUP;
			}

			if(isUnicode) {
				input.Data.Keyboard.Flags |= WinAPI.KEYEVENTF_UNICODE;
				input.Data.Keyboard.Scan = (ushort)vKCode;
				input.Data.Keyboard.Vk = 0;
			} else {
				input.Data.Keyboard.Scan = 0;
				input.Data.Keyboard.Vk = (ushort)vKCode;
			}

			if(isExtended) {
				input.Data.Keyboard.Flags |= WinAPI.KEYEVENTF_EXTENDEDKEY;
			}

			input.Data.Keyboard.Time = 0;
			input.Data.Keyboard.ExtraInfo = IntPtr.Zero;

			WinAPI.SendInput(1, new WinAPI.INPUT[] { input }, Marshal.SizeOf(input));
			Thread.Sleep(100);
		}

		public void SendMouseInput(int x, int y, uint data, WinAPI.SendMouseInputFlags flags) {
			uint intflags = (uint)flags;

			bool absolutPosition = (intflags & (int)WinAPI.SendMouseInputFlags.Absolute) != 0;
			if(absolutPosition) {
				NormalizeCoordinates(ref x, ref y);
				intflags |= WinAPI.MouseeventfVirtualdesk;
			}

			var mi = new WinAPI.INPUT();
			mi.Type = WinAPI.INPUT_MOUSE;
			mi.Data.Mouse.X = x;
			mi.Data.Mouse.Y = y;
			mi.Data.Mouse.MouseData = data;
			mi.Data.Mouse.Flags = intflags;
			mi.Data.Mouse.Time = 0;
			mi.Data.Mouse.ExtraInfo = new IntPtr(0);

			if(WinAPI.SendInput(1, new WinAPI.INPUT[] { mi }, Marshal.SizeOf(mi)) == 0) {
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		static void NormalizeCoordinates(ref int x, ref int y) {
			int vScreenWidth = WinAPI.GetSystemMetrics(WinAPI.SMCxvirtualscreen);
			int vScreenHeight = WinAPI.GetSystemMetrics(WinAPI.SMCyvirtualscreen);
			int vScreenLeft = WinAPI.GetSystemMetrics(WinAPI.SMXvirtualscreen);
			int vScreenTop = WinAPI.GetSystemMetrics(WinAPI.SMYvirtualscreen);

			x = ((x - vScreenLeft) * 65536) / vScreenWidth + 65536 / (vScreenWidth * 2);
			y = ((y - vScreenTop) * 65536) / vScreenHeight + 65536 / (vScreenHeight * 2);
		}

		public short GetKeyScan(char c) {
			return WinAPI.VkKeyScan(c);
		}
	}
}
