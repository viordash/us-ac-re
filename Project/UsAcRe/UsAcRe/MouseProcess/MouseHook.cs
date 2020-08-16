using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using UsAcRe.WindowsSystem;

namespace UsAcRe.MouseProcess {
	public delegate void MouseEventHandler(object sender, MouseEventArgs args);
	public class MouseEventArgs : EventArgs {
		public MouseEvent Event;
		public List<MouseButtonType> Buttons { get; set; }
		public MouseEventArgs(MouseEvent mouseEvent, List<MouseButtonType> buttons) {
			Event = mouseEvent;
		}
	}
	public delegate void MouseMoveHandler(object sender, MouseMoveArgs args);
	public class MouseMoveArgs : EventArgs {
		public WinAPI.POINT Coord { get; set; }
		public bool Stopped { get; set; }
		public List<MouseButtonType> Buttons { get; set; }
		public MouseMoveArgs(WinAPI.POINT coord, bool stopped, List<MouseButtonType> buttons) {
			Coord = coord;
			Stopped = stopped;
			Buttons = buttons;
		}
	}

	public static class MouseHook {
		static NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");
		public static event MouseEventHandler OnMouseEvent = delegate { };
		public static event MouseMoveHandler OnMouseMove = delegate { };
		private static WinAPI.LowLevelMouseProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;
		private static int doubleClickTime = WinAPI.GetDoubleClickTime();
		private static int maxDoubleClickTime = doubleClickTime + (doubleClickTime / 10);
		private static int onClickMessageTimeStamp;
		private static Timer timerStoringMouseAction = null;
		private static MouseEvent prevMouseEvent = null;
		private static WinAPI.POINT prevMouseCoord = WinAPI.POINT.Empty;
		private static Timer timerStopMouseMoveDetection = null;
		private static List<MouseButtonType> pressedButtons = new List<MouseButtonType>();

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

		static bool IsClickInSamePosition(MouseEvent mouseEvent, int x, int y) {
			return (Math.Abs(x - mouseEvent.DownClickedPoint.X) < 10)
				&& (Math.Abs(y - mouseEvent.DownClickedPoint.Y) < 10);
		}

		static bool IsOverdueClick(int messageTimeStamp) {
			return (messageTimeStamp < onClickMessageTimeStamp) || (messageTimeStamp - onClickMessageTimeStamp > doubleClickTime);
		}

		static bool IsDoubleClick(MouseEvent mouseEvent, int x, int y, int messageTimeStamp) {
			return !IsOverdueClick(messageTimeStamp) && IsClickInSamePosition(mouseEvent, x, y);
		}

		static bool IsSameButton(MouseButtonType button, MouseEvent mouseEvent) {
			return (button == MouseButtonType.Left && (mouseEvent.Type == MouseActionType.LeftClick || mouseEvent.Type == MouseActionType.LeftDoubleClick
						|| mouseEvent.Type == MouseActionType.LeftDrag))
				|| (button == MouseButtonType.Right && (mouseEvent.Type == MouseActionType.RightClick || mouseEvent.Type == MouseActionType.RightDoubleClick
						|| mouseEvent.Type == MouseActionType.RightDrag))
				|| (button == MouseButtonType.Middle && (mouseEvent.Type == MouseActionType.MiddleClick || mouseEvent.Type == MouseActionType.MiddleDoubleClick
						|| mouseEvent.Type == MouseActionType.MiddleDrag));
		}

		static void MouseEventHook(MouseButtonType button, bool isDown, int x, int y, int messageTimeStamp) {
			if(isDown) {
				logger.Info("IsDown 0:              {1}; {0}; {2}", button, DateTime.Now.Ticks, prevMouseEvent == null ? "null" : "");
				if(prevMouseEvent != null) {
					if(IsOverdueClick(messageTimeStamp)) {
						prevMouseEvent = null;
					} else if(!IsClickInSamePosition(prevMouseEvent, x, y)) {
						prevMouseEvent = null;
					} else if(!IsSameButton(button, prevMouseEvent)) {
						prevMouseEvent = null;
					}
				}
				if(prevMouseEvent == null) {
					prevMouseEvent = new MouseEvent() {
						DownClickedPoint = new Point(x, y)
					};
				}
				logger.Info("IsDown 1:              {1}; {0}; {2}", button, DateTime.Now.Ticks, prevMouseEvent == null ? "null" : "");
				pressedButtons.Add(button);
			} else {
				pressedButtons.Remove(button);
				logger.Info("IsUp:                  {1}; {0}; {2}", button, DateTime.Now.Ticks, prevMouseEvent == null ? "null" : "");
				if(prevMouseEvent == null) {
					return;//                    throw new InvalidOperationException("MouseDown is missed");
				}

				if(prevMouseEvent.Type != MouseActionType.None && IsDoubleClick(prevMouseEvent, x, y, messageTimeStamp)) {
					prevMouseEvent.Type = button == MouseButtonType.Left
						? MouseActionType.LeftDoubleClick
						: button == MouseButtonType.Right
						? MouseActionType.RightDoubleClick
						: MouseActionType.MiddleDoubleClick;
					if(timerStoringMouseAction != null) {
						timerStoringMouseAction.Dispose();
						timerStoringMouseAction = null;
					}
				} else if(!IsClickInSamePosition(prevMouseEvent, x, y)) {
					prevMouseEvent.Type = button == MouseButtonType.Left
						? MouseActionType.LeftDrag
						: button == MouseButtonType.Right
						? MouseActionType.RightDrag
						: MouseActionType.MiddleDrag;
					prevMouseEvent.UpClickedPoint = new Point(x, y);
				} else {
					prevMouseEvent.Type = button == MouseButtonType.Left
						? MouseActionType.LeftClick
						: button == MouseButtonType.Right
						? MouseActionType.RightClick
						: MouseActionType.MiddleClick;
				}

				if(timerStoringMouseAction != null) {
					timerStoringMouseAction.Dispose();
					timerStoringMouseAction = null;
				}
				timerStoringMouseAction = new Timer(
					(state) => {
						OnMouseEvent?.Invoke(null, new MouseEventArgs(state as MouseEvent, pressedButtons));
						timerStoringMouseAction = null;
					},
					new MouseEvent(prevMouseEvent), maxDoubleClickTime, Timeout.Infinite);
				logger.Info("IsUp 1:                {1}; {0}", prevMouseEvent.Type, DateTime.Now.Ticks);
			}
			onClickMessageTimeStamp = messageTimeStamp;
		}

		static void MouseMoveHook(int x, int y, int messageTimeStamp) {
			if(prevMouseCoord.WithBoundaries(x, y, 10)) {
				return;
			}
			if(timerStopMouseMoveDetection != null) {
				timerStopMouseMoveDetection.Dispose();
				timerStopMouseMoveDetection = null;
			}

			int dueTime = timerStoringMouseAction != null
				? maxDoubleClickTime + (maxDoubleClickTime / 10)
				: 200;

			timerStopMouseMoveDetection = new Timer((state) => {
				OnMouseMove?.Invoke(null, new MouseMoveArgs(new WinAPI.POINT(x, y), true, pressedButtons));
			}, null, dueTime, Timeout.Infinite);

			prevMouseCoord.x = x;
			prevMouseCoord.y = y;
		}

		static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
			if(nCode >= 0) {
				WinAPI.MSLLHOOKSTRUCT hookStruct = (WinAPI.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.MSLLHOOKSTRUCT));
				switch((uint)wParam) {
					case WindowsMessages.WM_LBUTTONDOWN:
						MouseEventHook(MouseButtonType.Left, true, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_LBUTTONUP:
						MouseEventHook(MouseButtonType.Left, false, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_RBUTTONDOWN:
						MouseEventHook(MouseButtonType.Right, true, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_RBUTTONUP:
						MouseEventHook(MouseButtonType.Right, false, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_MBUTTONDOWN:
						MouseEventHook(MouseButtonType.Middle, true, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
						break;
					case WindowsMessages.WM_MBUTTONUP:
						MouseEventHook(MouseButtonType.Middle, false, hookStruct.pt.x, hookStruct.pt.y, (int)hookStruct.time);
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
