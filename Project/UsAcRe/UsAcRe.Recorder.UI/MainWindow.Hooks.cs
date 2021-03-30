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
			if(IsRestrictedArea(e.Coord)) {
				return;
			}
			if(elementFromPoint != null) {
				Actions.Add(ElementMatchAction.Record(elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement));
				CloseMouseClickBlocker();
				testsLaunchingService.CloseHighlighter();
				elementFromPoint = null;
			}
			Actions.Add(MouseClickAction.Record(e.Button, e.Coord, e.DoubleClick));
		}

		void MouseStartDragHook(object sender, MouseStartDragEventArgs e) {
			if(IsRestrictedArea(e.Coord)) {
				return;
			}
			if(elementFromPoint != null) {
				Actions.Add(ElementMatchAction.Record(elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement));
				CloseMouseClickBlocker();
				testsLaunchingService.CloseHighlighter();
				elementFromPoint = null;
			}
		}

		void MouseDragHook(object sender, MouseDragEventArgs e) {
			if(IsRestrictedArea(e.StartCoord)) {
				return;
			}
			Actions.Add(MouseDragAction.Record(e.Button, e.StartCoord, e.EndCoord));
		}

		void MouseMoveHook(object sender, MouseMoveArgs e) {
			if(elementFromPoint != null) {
				elementFromPoint.BreakOperations();
				elementFromPoint = null;
			}

			if(e.Stopped && !IsRestrictedArea(e.Coord)) {
				ShowMouseClickBlocker(e.Coord);
				var tmpElem = new ElementFromPoint(automationElementService, winApiService, e.Coord, true);
				testsLaunchingService.HighlightElement(tmpElem.TreeOfSpecificUiElement.BoundingRectangle);
				elementFromPoint = tmpElem;
				CloseMouseClickBlocker();
			} else {
				testsLaunchingService.CloseHighlighter();
				CloseMouseClickBlocker();
			}
		}

		void ShowMouseClickBlocker(WinAPI.POINT coord) {
			windowsFormsService.BeginInvoke((Action)(() => {
				mouseClickBlocker?.StopHighlighting();
				mouseClickBlocker = new ElementHighlighter(new System.Windows.Rect(coord.x - 3, coord.y - 3, 6, 6));
				mouseClickBlocker.StartHighlighting();
			}));
		}

		void CloseMouseClickBlocker() {
			windowsFormsService.BeginInvoke((Action)(() => {
				if(mouseClickBlocker != null) {
					mouseClickBlocker.StopHighlighting();
					mouseClickBlocker = null;
				}
			}));
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

			if(elementFromPoint != null) {
				elementFromPoint.BreakOperations();
				elementFromPoint = null;
			}

			WinAPI.GetCursorPos(out WinAPI.POINT pt);

			if(!IsRestrictedArea(pt)) {
				Actions.Add(KeybdAction.Record(e.VKCode, e.IsUp));
			}
			CloseMouseClickBlocker();
			testsLaunchingService.CloseHighlighter();
		}

	}
}
