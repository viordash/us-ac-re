using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using UsAcRe.Extensions;
using UsAcRe.WindowsSystem;

namespace UsAcRe.MouseProcess {
	public delegate void MouseStartDragEventHandler(object sender, MouseStartDragEventArgs args);
	public class MouseStartDragEventArgs : EventArgs {
		public Point Coord { get; set; }
		public MouseButtonType Button { get; set; }
		public MouseStartDragEventArgs(Point coord, MouseButtonType button) {
			Coord = coord;
			Button = button;
		}
	}
	public delegate void MouseDragEventHandler(object sender, MouseDragEventArgs args);
	public class MouseDragEventArgs : EventArgs {
		public Point StartCoord { get; set; }
		public Point EndCoord { get; set; }
		public MouseButtonType Button { get; set; }
		public MouseDragEventArgs(Point startCoord, Point endCoord, MouseButtonType button) {
			StartCoord = startCoord;
			EndCoord = endCoord;
			Button = button;
		}
	}

	public delegate void MouseClickEventHandler(object sender, MouseClickEventArgs args);
	public class MouseClickEventArgs : EventArgs {
		public Point Coord { get; set; }
		public MouseButtonType Button { get; set; }
		public bool DoubleClick { get; set; }
		public MouseClickEventArgs(Point coord, MouseButtonType button, bool doubleClick) {
			Coord = coord;
			Button = button;
			DoubleClick = doubleClick;
		}
	}

	public delegate void MouseMoveHandler(object sender, MouseMoveArgs args);
	public class MouseMoveArgs : EventArgs {
		public WinAPI.POINT Coord { get; set; }
		public bool Stopped { get; set; }
		public MouseMoveArgs(WinAPI.POINT coord, bool stopped) {
			Coord = coord;
			Stopped = stopped;
		}
	}

	public static class MouseHook {
		static NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");
		public static event MouseClickEventHandler OnMouseClick = delegate { };
		public static event MouseStartDragEventHandler OnMouseStartDrag = delegate { };
		public static event MouseDragEventHandler OnMouseDrag = delegate { };
		public static event MouseMoveHandler OnMouseMove = delegate { };
		private static WinAPI.LowLevelMouseProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;
		public static int DoubleClickTime = WinAPI.GetDoubleClickTime();
		public static int MaxDoubleClickTime = DoubleClickTime + (DoubleClickTime / 20);
		private static int onClickMessageTimeStamp;
		private static Timer timerStoringMouseAction = null;
		private static WinAPI.POINT prevMouseClickCoord = WinAPI.POINT.Empty;
		private static WinAPI.POINT prevMouseCoord = WinAPI.POINT.Empty;
		private static Timer timerStopMouseMoveDetection = null;

		private static MouseButtonType prevMouseButton = MouseButtonType.None;


		private static bool startMouseDrag = false;
		private static MouseButtonType mouseButtonDown = MouseButtonType.None;
		private static Point startDragCoord;

		public static void Start() {
			_hookID = SetHook(_proc);
		}

		public static void Stop() {
			WinAPI.UnhookWindowsHookEx(_hookID);
		}

		private static IntPtr SetHook(WinAPI.LowLevelMouseProc proc) {
			using(Process curProcess = Process.GetCurrentProcess())
			using(ProcessModule curModule = curProcess.MainModule) {
				return WinAPI.SetWindowsHookEx(WinAPI.WH_MOUSE_LL, proc,
				  WinAPI.GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		static void MouseButtonUp(MouseButtonType button, int x, int y, int messageTimeStamp) {
			if(timerStoringMouseAction != null) {
				timerStoringMouseAction.Dispose();
				timerStoringMouseAction = null;
			}

			var isDoubleClick = messageTimeStamp - onClickMessageTimeStamp <= DoubleClickTime;

			if(mouseButtonDown == button && !startDragCoord.WithBoundaries(x, y, 10)) {
				OnMouseDrag?.Invoke(null, new MouseDragEventArgs(startDragCoord, new Point(x, y), mouseButtonDown));
				logger.Trace("-----------------        OnMouseDragEvent: end  {0} -> {1}", startDragCoord, new WinAPI.POINT(x, y));
			} else if(prevMouseButton == button && prevMouseClickCoord.WithBoundaries(x, y, 10) && isDoubleClick) {
				OnMouseClick?.Invoke(null, new MouseClickEventArgs(new Point(x, y), button, true));
				logger.Trace("-----------------        MouseButtonUp: DbCl  {0}; {1}", messageTimeStamp, button);
			} else {
				timerStoringMouseAction = new Timer(
					(state) => {
						OnMouseClick?.Invoke(null, state as MouseClickEventArgs);
						timerStoringMouseAction = null;
						logger.Trace("-----------------        MouseButtonUp:       {0}; {1}", messageTimeStamp, button);
					},
					new MouseClickEventArgs(new Point(x, y), button, false),
					MaxDoubleClickTime,
					Timeout.Infinite);
			}

			startMouseDrag = false;
			mouseButtonDown = MouseButtonType.None;
			prevMouseButton = button;
			prevMouseClickCoord.x = x;
			prevMouseClickCoord.y = y;
			onClickMessageTimeStamp = messageTimeStamp;
		}

		static void AssumeMouseDrag(MouseButtonType button, int x, int y) {
			if(timerStoringMouseAction != null) {
				timerStoringMouseAction.Dispose();
				timerStoringMouseAction = null;
			}
			startMouseDrag = true;
			mouseButtonDown = button;
			startDragCoord = new Point(x, y);
		}

		static void MouseMoveHook(int x, int y, int messageTimeStamp) {
			if(startMouseDrag && mouseButtonDown != MouseButtonType.None) {
				OnMouseStartDrag?.Invoke(null, new MouseStartDragEventArgs(startDragCoord, mouseButtonDown));
				startMouseDrag = false;
				logger.Trace("-----------------        OnMouseDragEvent: str  {0} -> {1}", startDragCoord, new WinAPI.POINT(x, y));
			}

			if(prevMouseCoord.WithBoundaries(x, y, 10)) {
				return;
			}
			if(timerStopMouseMoveDetection != null) {
				timerStopMouseMoveDetection.Dispose();
				timerStopMouseMoveDetection = null;
			}

			int dueTime = timerStoringMouseAction != null
				? MaxDoubleClickTime + (MaxDoubleClickTime / 10)
				: 200;

			timerStopMouseMoveDetection = new Timer((state) => {
				if(WinAPI.GetCursorPos(out WinAPI.POINT pt)) {
					OnMouseMove?.Invoke(null, new MouseMoveArgs(pt, true));
				}
			}, null, dueTime, Timeout.Infinite);

			prevMouseCoord.x = x;
			prevMouseCoord.y = y;
		}

		static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
			if(nCode >= 0) {
				WinAPI.MSLLHOOKSTRUCT hookStruct = (WinAPI.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.MSLLHOOKSTRUCT));
				switch((uint)wParam) {
					case WindowsMessages.WM_LBUTTONDOWN:
						AssumeMouseDrag(MouseButtonType.Left, hookStruct.pt.x, hookStruct.pt.y);
						break;
					case WindowsMessages.WM_LBUTTONUP:
						MouseButtonUp(MouseButtonType.Left, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_RBUTTONDOWN:
						AssumeMouseDrag(MouseButtonType.Right, hookStruct.pt.x, hookStruct.pt.y);
						break;
					case WindowsMessages.WM_RBUTTONUP:
						MouseButtonUp(MouseButtonType.Right, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_MBUTTONDOWN:
						AssumeMouseDrag(MouseButtonType.Middle, hookStruct.pt.x, hookStruct.pt.y);
						break;
					case WindowsMessages.WM_MBUTTONUP:
						MouseButtonUp(MouseButtonType.Middle, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_MOUSEMOVE:
						MouseMoveHook(hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
				}
			}
			return WinAPI.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}
	}
}
