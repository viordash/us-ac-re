using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.MouseProcess {
	public static class MouseProcessConstants {
		public static int DoubleClickTime = WinAPI.GetDoubleClickTime();
		public static int MaxDoubleClickTime = DoubleClickTime + (DoubleClickTime / 20);
	}
}
