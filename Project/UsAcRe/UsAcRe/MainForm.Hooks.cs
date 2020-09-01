using System;
using System.Windows.Forms;
using UsAcRe.Highlighter;
using UsAcRe.KeyboardProcess;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		void StartKeyboardHooks() {
			KeyboardHook.Start();
			KeyboardHook.KeyAction -= KeyboardEvent;
			KeyboardHook.KeyAction += KeyboardEvent;
		}

		void StopKeyboadHooks() {
			KeyboardHook.KeyAction -= KeyboardEvent;
			KeyboardHook.Stop();
		}

		void StartHooks() {
			logger.Warn("Start");
			MouseHook.Start();
			MouseHook.OnMouseClick -= MouseClickHook;
			MouseHook.OnMouseClick += MouseClickHook;
			MouseHook.OnMouseStartDrag -= MouseStartDragHook;
			MouseHook.OnMouseStartDrag += MouseStartDragHook;
			MouseHook.OnMouseDrag -= MouseDragHook;
			MouseHook.OnMouseDrag += MouseDragHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.OnMouseMove += MouseMoveHook;

			StartKeyboardHooks();
		}

		void StopHooks() {
			logger.Warn("Stop");
			TestsLaunchingService.CloseHighlighter();
			MouseHook.OnMouseClick -= MouseClickHook;
			MouseHook.OnMouseStartDrag -= MouseStartDragHook;
			MouseHook.OnMouseDrag -= MouseDragHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.Stop();

			StopKeyboadHooks();
			if(btnStart.Checked) {
				btnStart.Checked = false;
			}
		}

		bool IsRestrictedArea(System.Drawing.Point point) {
			return Bounds.Contains(point);
		}
		bool IsRestrictedArea(WinAPI.POINT coord) {
			return Bounds.Contains(coord.x, coord.y);
		}

		void MouseClickHook(object sender, MouseClickEventArgs e) {
			BeginInvoke((Action<MouseClickEventArgs>)((args) => {
				if(IsRestrictedArea(args.Coord)) {
					return;
				}
				if(elementFromPoint != null) {
					Actions.Add(new Actions.ElementMatchAction(elementFromPoint));
					CloseMouseClickBlocker();
					TestsLaunchingService.CloseHighlighter();
					elementFromPoint = null;
				}

				var mouseEvent = new MouseEvent(args);
				Actions.Add(new Actions.MouseAction(mouseEvent));
			}), e);
		}

		void MouseStartDragHook(object sender, MouseStartDragEventArgs e) {
			BeginInvoke((Action<MouseStartDragEventArgs>)((args) => {
				if(IsRestrictedArea(args.Coord)) {
					return;
				}
				if(elementFromPoint != null) {
					Actions.Add(new Actions.ElementMatchAction(elementFromPoint));
					CloseMouseClickBlocker();
					TestsLaunchingService.CloseHighlighter();
					elementFromPoint = null;
				}
			}), e);
		}

		void MouseDragHook(object sender, MouseDragEventArgs e) {
			BeginInvoke((Action<MouseDragEventArgs>)((args) => {
				if(IsRestrictedArea(args.StartCoord)) {
					return;
				}
				Actions.Add(new Actions.MouseDragAction(args));
			}), e);
		}

		void MouseMoveHook(object sender, MouseMoveArgs e) {
			BeginInvoke((Action<MouseMoveArgs>)((args) => {
				if(elementFromPoint != null) {
					elementFromPoint.BreakOperations();
					elementFromPoint = null;
				}

				if(args.Stopped && !IsRestrictedArea(args.Coord)) {
					ShowMouseClickBlocker(args.Coord);
					elementFromPoint = new ElementFromPoint(AutomationElementService, WinApiService, args.Coord, true);
					TestsLaunchingService.OpenHighlighter(elementFromPoint);
					CloseMouseClickBlocker();
					//logger.Info("elementFromPoint {0}", elementFromPoint == null);
				} else {
					TestsLaunchingService.CloseHighlighter();
					CloseMouseClickBlocker();
				}
			}), e);
		}

		void ShowMouseClickBlocker(WinAPI.POINT coord) {
			CloseMouseClickBlocker();
			mouseClickBlocker = new ElementHighlighter(new System.Windows.Rect(coord.x - 3, coord.y - 3, 6, 6));
			mouseClickBlocker.StartHighlighting();
			//Debug.WriteLine($"ShowMouseClickBlocker :   coord:{coord}");
		}

		void CloseMouseClickBlocker() {
			if(mouseClickBlocker != null) {
				mouseClickBlocker.StopHighlighting();
				mouseClickBlocker = null;
			}
		}

		void KeyboardEvent(object sender, RawKeyEventArgs e) {
			if(!btnStart.Checked) {
				if(e.VKCode == KeyboardHook.KeyStartStop) {
					TestsLaunchingService.Stop();
				}
				return;
			}

			if(e.VKCode == KeyboardHook.KeyStartStop) {
				StopHooks();
				return;
			}

			BeginInvoke((Action<RawKeyEventArgs>)((args) => {
				if(elementFromPoint != null) {
					elementFromPoint.BreakOperations();
					elementFromPoint = null;
				}

				WinAPI.POINT pt;
				WinAPI.GetCursorPos(out pt);

				if(!IsRestrictedArea(pt)) {
					Actions.Add(new Actions.KeybdAction(args.VKCode, e.IsUp));
				}
				CloseMouseClickBlocker();
				TestsLaunchingService.CloseHighlighter();
			}), e);
		}

	}
}
