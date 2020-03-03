using System;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Services {
	public interface IWinApiService {
		WinAPI.POINT GetMousePosition();
		IntPtr GetWindow(IntPtr hwnd, uint uCmd);
		IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);
		IntPtr GetWindow(WinAPI.POINT point);
		IntPtr GetRootWindowForElementUnderPoint(WinAPI.POINT point);
	}

	public class WinApiService : IWinApiService {
		public WinApiService() {

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

		public IntPtr GetWindow(IntPtr hwnd, uint uCmd) {
			return WinAPI.GetWindow(hwnd, uCmd);
		}

		public IntPtr GetWindow(WinAPI.POINT point) {
			return WinAPI.WindowFromPoint(point);
		}
	}
}
