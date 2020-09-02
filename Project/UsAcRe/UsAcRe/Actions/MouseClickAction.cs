using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Exceptions;
using UsAcRe.Extensions;
using UsAcRe.MouseProcess;

namespace UsAcRe.Actions {
	public class MouseClickAction : BaseAction {
		public MouseButtonType Button { get; set; }
		public Point ClickedPoint { get; set; }
		public bool DoubleClick { get; set; }

		public MouseClickAction(MouseClickEventArgs args)
			: this(null, args.Button, args.Coord, args.DoubleClick) { }

		public MouseClickAction(BaseAction prevAction, MouseButtonType button, Point clickedPoint, bool doubleClick)
			: base(prevAction) {
			Button = button;
			ClickedPoint = clickedPoint;
			DoubleClick = doubleClick;
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoClick();
		}

		public override string ToString() {
			return string.Format("{0} Type:{1}, Down:{2}, DblClick:{3}", nameof(MouseClickAction), Button, ClickedPoint, DoubleClick);
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}({1}, {2}, {3})", nameof(ActionsExecutor.MouseClick), Button.ForNew(), ClickedPoint.ForNew(), DoubleClick.ForNew());
		}

		async ValueTask DoClick() {
			await Task.Delay(20);
			var clickedPoint = ClickedPoint;

			if(prevAction is MouseClickAction prevMouseAction) {
				if(prevMouseAction.ClickedPoint.WithBoundaries(clickedPoint, settingsService.GetClickPositionToleranceInPercent())) {
					await Task.Delay(MouseHook.MaxDoubleClickTime);
				}
			}

			var actionForDetermineClickPoint = prevAction;
			while(actionForDetermineClickPoint is MouseClickAction prevMouseAct
				&& clickedPoint.WithBoundaries(prevMouseAct.ClickedPoint, settingsService.GetClickPositionToleranceInPercent())) {
				actionForDetermineClickPoint = prevMouseAct.prevAction;
			}

			if(actionForDetermineClickPoint is ElementMatchAction elementMatchAction) {
				if(elementMatchAction.OffsetPoint.HasValue) {
					clickedPoint.Offset((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
				}
			}

			testsLaunchingService.CloseHighlighter();
			MainForm.MoveOutFromPoint(clickedPoint.X, clickedPoint.Y);
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
