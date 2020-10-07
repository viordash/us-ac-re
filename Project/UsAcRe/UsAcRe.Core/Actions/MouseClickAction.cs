using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class MouseClickAction : BaseAction {
		public MouseButtonType Button { get; set; }
		public Point ClickedPoint { get; set; }
		public bool DoubleClick { get; set; }
		Point? Offset { get; set; } = null;

		public static MouseClickAction Record(MouseButtonType button, Point clickedPoint, bool doubleClick) {
			var instance = CreateInstance<MouseClickAction>();
			instance.Button = button;
			instance.ClickedPoint = clickedPoint;
			instance.DoubleClick = doubleClick;
			return instance;
		}

		public static async Task Play(MouseButtonType button, Point clickedPoint, bool doubleClick) {
			var instance = CreateInstance<MouseClickAction>();
			instance.Button = button;
			instance.ClickedPoint = clickedPoint;
			instance.DoubleClick = doubleClick;
			await instance.ExecuteAsync();
		}

		public MouseClickAction(
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService) : base(settingsService, testsLaunchingService) {
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoClick();
		}

		public override string ToString() {
			return string.Format("{0} Type:{1}, Down:{2}, DblClick:{3}", nameof(MouseClickAction), Button, ClickedPoint, DoubleClick);
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}({2}, {3}, {4})", nameof(MouseClickAction), nameof(MouseClickAction.Play), Button.ForNew(), ClickedPoint.ForNew(), DoubleClick.ForNew());
		}

		async ValueTask DoClick() {
			await Task.Delay(20);
			var clickedPoint = ClickedPoint;

			if(testsLaunchingService.LastAction is MouseClickAction prevMouseAction) {
				if(prevMouseAction.ClickedPoint.WithBoundaries(clickedPoint, settingsService.ClickPositionToleranceInPercent)) {
					await Task.Delay(MouseProcessConstants.MaxDoubleClickTime);
				}
			}

			var actionForDetermineClickPoint = testsLaunchingService.LastAction;
			if(actionForDetermineClickPoint is MouseClickAction prevMouseAct) {
				Offset = prevMouseAct.Offset;
			} else if(actionForDetermineClickPoint is ElementMatchAction elementMatchAction) {
				Offset = new Point((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
			}
			if(Offset.HasValue) {
				clickedPoint.Offset(Offset.Value);
			}

			testsLaunchingService.CloseHighlighter();
			//MainForm.MoveOutFromPoint(clickedPoint.X, clickedPoint.Y);
			switch(Button) {
				case MouseButtonType.Left:
					Mouse_MoveTo(clickedPoint.X, clickedPoint.Y);
					if(DoubleClick) {
						Mouse.DoubleClick(MouseButton.Left);
					} else {
						Mouse.Click(MouseButton.Left);
					}
					break;
				case MouseButtonType.Right:
					Mouse_MoveTo(clickedPoint.X, clickedPoint.Y);
					if(DoubleClick) {
						Mouse.DoubleClick(MouseButton.Right);
					} else {
						Mouse.Click(MouseButton.Right);
					}
					break;
				case MouseButtonType.Middle:
					Mouse_MoveTo(clickedPoint.X, clickedPoint.Y);
					if(DoubleClick) {
						Mouse.DoubleClick(MouseButton.Middle);
					} else {
						Mouse.Click(MouseButton.Middle);
					}
					break;
				default:
					throw new SevereException(this, nameof(DoClick));
			}
		}

		void Mouse_MoveTo(int x, int y) {
			Mouse.MoveTo(new Point(x, y));
		}
	}
}
