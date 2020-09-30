using System;
using System.Text;
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
	}
}
