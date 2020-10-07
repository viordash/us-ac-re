using System;
using System.Windows;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UI.Highlighter;
using UsAcRe.Core.WindowsSystem;
using UsAcRe.Recorder.KeyboardProcess;
using UsAcRe.Recorder.MouseProcess;
using UsAcRe.Recorder.UIAutomationElement;

namespace UsAcRe.Recorder.UI {
	public partial class MainWindow : Window {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		public System.Drawing.Rectangle Bounds { get { return new System.Drawing.Rectangle((int)Left, (int)Top, (int)Width, (int)Height); } }

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
			if(mainMenu.miStartStop.IsChecked) {
				mainMenu.miStartStop.IsChecked = false;
			}
		}

		bool IsRestrictedArea(System.Drawing.Point point) {
			return Bounds.Contains(point);
		}
		bool IsRestrictedArea(WinAPI.POINT coord) {
			return Bounds.Contains(coord.x, coord.y);
		}

		void MouseClickHook(object sender, MouseClickEventArgs e) {
			Dispatcher.BeginInvoke((Action<MouseClickEventArgs>)((args) => {
				if(IsRestrictedArea(args.Coord)) {
					return;
				}
				if(elementFromPoint != null) {
					Actions.Add(ElementMatchAction.Record(elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement));
					CloseMouseClickBlocker();
					TestsLaunchingService.CloseHighlighter();
					elementFromPoint = null;
				}
				Actions.Add(MouseClickAction.Record(args.Button, args.Coord, args.DoubleClick));
			}), e);
		}

		void MouseStartDragHook(object sender, MouseStartDragEventArgs e) {
			Dispatcher.BeginInvoke((Action<MouseStartDragEventArgs>)((args) => {
				if(IsRestrictedArea(args.Coord)) {
					return;
				}
				if(elementFromPoint != null) {
					Actions.Add(ElementMatchAction.Record(elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement));
					CloseMouseClickBlocker();
					TestsLaunchingService.CloseHighlighter();
					elementFromPoint = null;
				}
			}), e);
		}

		void MouseDragHook(object sender, MouseDragEventArgs e) {
			Dispatcher.BeginInvoke((Action<MouseDragEventArgs>)((args) => {
				if(IsRestrictedArea(args.StartCoord)) {
					return;
				}
				Actions.Add(MouseDragAction.Record(args.Button, args.StartCoord, args.EndCoord));
			}), e);
		}

		void MouseMoveHook(object sender, MouseMoveArgs e) {
			Dispatcher.BeginInvoke((Action<MouseMoveArgs>)((args) => {
				if(elementFromPoint != null) {
					elementFromPoint.BreakOperations();
					elementFromPoint = null;
				}

				if(args.Stopped && !IsRestrictedArea(args.Coord)) {
					ShowMouseClickBlocker(args.Coord);
					elementFromPoint = new ElementFromPoint(AutomationElementService, WinApiService, args.Coord, true);
					TestsLaunchingService.HighlightElement(elementFromPoint.TreeOfSpecificUiElement.BoundingRectangle);
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
			if(!mainMenu.miStartStop.IsChecked) {
				if(e.VKCode == KeyboardHook.KeyStartStop) {
					TestsLaunchingService.Stop();
				}
				return;
			}

			if(e.VKCode == KeyboardHook.KeyStartStop) {
				StopHooks();
				return;
			}

			Dispatcher.BeginInvoke((Action<RawKeyEventArgs>)((args) => {
				if(elementFromPoint != null) {
					elementFromPoint.BreakOperations();
					elementFromPoint = null;
				}

				WinAPI.GetCursorPos(out WinAPI.POINT pt);

				if(!IsRestrictedArea(pt)) {
					Actions.Add(KeybdAction.Record(args.VKCode, e.IsUp));
				}
				CloseMouseClickBlocker();
				TestsLaunchingService.CloseHighlighter();
			}), e);
		}

	}
}
