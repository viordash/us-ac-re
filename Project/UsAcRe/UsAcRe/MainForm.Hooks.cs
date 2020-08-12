using System;
using System.Diagnostics;
using System.Windows.Forms;
using UsAcRe.Highlighter;
using UsAcRe.KeyboardProcess;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		void StartHooks() {
			logger.Warn("Start");
			MouseHook.Start();
			MouseHook.OnMouseEvent -= MouseEventHook;
			MouseHook.OnMouseEvent += MouseEventHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.OnMouseMove += MouseMoveHook;

			KeyboardHook.Start();
			KeyboardHook.KeyAction -= KeyboardEvent;
			KeyboardHook.KeyAction += KeyboardEvent;
		}

		void StopHooks() {
			logger.Warn("Stop");
			CloseHighlighter();
			MouseHook.OnMouseEvent -= MouseEventHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.Stop();

			KeyboardHook.KeyAction -= KeyboardEvent;
			KeyboardHook.Stop();
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

		void MouseEventHook(object sender, MouseProcess.MouseEventArgs e) {
			BeginInvoke((Action<MouseProcess.MouseEventArgs>)((args) => {
				if(args.Event == null) {
					return;
				}
				if(IsRestrictedArea(args.Event.DownClickedPoint) || IsRestrictedArea(args.Event.UpClickedPoint)) {
					return;
				}
				if(elementFromPoint != null) {
					Actions.Add(new Actions.ElementMatchAction(elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement));
					CloseMouseClickBlocker();
					CloseHighlighter();
					elementFromPoint = null;
				}

				Actions.Add(new Actions.MouseAction(args.Event.Type, args.Event.DownClickedPoint, args.Event.UpClickedPoint));
			}), e);
		}

		void MouseMoveHook(object sender, MouseMoveArgs e) {
			BeginInvoke((Action<MouseMoveArgs>)((args) => {
				if(elementFromPoint != null) {
					elementFromPoint.BreakOperations();
					elementFromPoint = null;
				}

				if(!IsRestrictedArea(args.Coord) && args.Stopped && args.Buttons.Count == 0) {
					ShowMouseClickBlocker(args.Coord);
					elementFromPoint = new ElementFromPoint(AutomationElementService, WinApiService, args.Coord, true);
					CloseMouseClickBlocker();
					ShowHighlighter();
					//logger.Info(elementFromPoint);
				} else {
					CloseMouseClickBlocker();
					CloseHighlighter();
				}

			}), e);
		}

		void ShowHighlighter() {
			CloseHighlighter();
			elementHighlighter = new ElementHighlighter(elementFromPoint);
			elementHighlighter.StartHighlighting();
		}

		void CloseHighlighter() {
			if(elementHighlighter != null) {
				var highlighter = elementHighlighter;
				highlighter.StopHighlighting();
				elementHighlighter = null;
			}
		}

		void ShowMouseClickBlocker(WinAPI.POINT coord) {
			CloseMouseClickBlocker();
			mouseClickBlocker = new ElementHighlighter(new System.Windows.Rect(coord.x - 3, coord.y - 3, 6, 6), string.Empty);
			mouseClickBlocker.StartHighlighting();
			Debug.WriteLine($"ShowMouseClickBlocker :   coord:{coord}");
		}

		void CloseMouseClickBlocker() {
			if(mouseClickBlocker != null) {
				mouseClickBlocker.StopHighlighting();
				mouseClickBlocker = null;
			}
		}

		void KeyboardEvent(object sender, RawKeyEventArgs e) {
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
				CloseHighlighter();
			}), e);
		}

	}
}
