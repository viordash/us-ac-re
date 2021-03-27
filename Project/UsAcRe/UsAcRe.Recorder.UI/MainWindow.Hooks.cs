using System;
using System.Windows;
using UsAcRe.Core.Actions;
using UsAcRe.Core.KeyboardProcess;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.UI.Highlighter;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Core.WindowsSystem;

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
			ActionsList.IsEnabled = false;
			this.ResizeMode = ResizeMode.NoResize;
			MainMenu.IsStopped = false;
			testsLaunchingService.Record();
		}

		void StopHooks() {
			logger.Warn("Stop");
			testsLaunchingService.Stop();
			MouseHook.OnMouseClick -= MouseClickHook;
			MouseHook.OnMouseStartDrag -= MouseStartDragHook;
			MouseHook.OnMouseDrag -= MouseDragHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.Stop();

			StopKeyboadHooks();
			if(MainMenu.miStartStop.IsChecked) {
				MainMenu.miStartStop.IsChecked = false;
			}
			ActionsList.IsEnabled = true;
			this.ResizeMode = ResizeMode.CanResizeWithGrip;
			MainMenu.IsStopped = true;
		}

		bool IsRestrictedArea(System.Drawing.Point point) {
			return IsMouseOver;
		}
		bool IsRestrictedArea(WinAPI.POINT coord) {
			return IsMouseOver;
		}

		void MouseClickHook(object sender, MouseClickEventArgs e) {
			Dispatcher.BeginInvoke((Action<MouseClickEventArgs>)((args) => {
				if(IsRestrictedArea(args.Coord)) {
					return;
				}
				if(elementFromPoint != null) {
					Actions.Add(ElementMatchAction.Record(elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement));
					CloseMouseClickBlocker();
					testsLaunchingService.CloseHighlighter();
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
					testsLaunchingService.CloseHighlighter();
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
					elementFromPoint = new ElementFromPoint(automationElementService, winApiService, args.Coord, true);
					testsLaunchingService.HighlightElement(elementFromPoint.TreeOfSpecificUiElement.BoundingRectangle);
					CloseMouseClickBlocker();
					//logger.Info("elementFromPoint {0}", elementFromPoint == null);
				} else {
					testsLaunchingService.CloseHighlighter();
					CloseMouseClickBlocker();
				}
			}), e);
		}

		void ShowMouseClickBlocker(WinAPI.POINT coord) {
			CloseMouseClickBlocker();
			mouseClickBlocker = new ElementHighlighter(new System.Windows.Rect(coord.x - 3, coord.y - 3, 6, 6));
			mouseClickBlocker.StartHighlighting();
		}

		void CloseMouseClickBlocker() {
			if(mouseClickBlocker != null) {
				mouseClickBlocker.StopHighlighting();
				mouseClickBlocker = null;
			}
		}

		void KeyboardEvent(object sender, RawKeyEventArgs e) {
			if(!MainMenu.miStartStop.IsChecked) {
				if(e.VKCode == KeyboardHook.KeyStartStop) {
					testsLaunchingService.Stop();
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
				testsLaunchingService.CloseHighlighter();
			}), e);
		}

	}
}
