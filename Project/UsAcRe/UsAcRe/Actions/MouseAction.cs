using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Exceptions;
using UsAcRe.Extensions;
using UsAcRe.Helpers;
using UsAcRe.MouseProcess;

namespace UsAcRe.Actions {
	public class MouseAction : BaseAction {
		public MouseActionType ActionType { get; set; }
		public Point DownClickedPoint { get; set; }
		public Point UpClickedPoint { get; set; }

		public MouseAction(MouseActionType type, Point downClickedPoint, Point upClickedPoint)
			: this(null, type, downClickedPoint, upClickedPoint) { }

		public MouseAction(BaseAction prevAction, MouseActionType type, Point downClickedPoint, Point upClickedPoint)
			: this(prevAction, type, downClickedPoint) {
			UpClickedPoint = upClickedPoint;
		}

		public MouseAction(MouseActionType type, Point downClickedPoint)
			: this(null, type, downClickedPoint) { }

		public MouseAction(BaseAction prevAction, MouseActionType type, Point downClickedPoint) : base(prevAction) {
			ActionType = type;
			DownClickedPoint = downClickedPoint;
			UpClickedPoint = Point.Empty;
		}

		protected override async Task ExecuteCoreAsync() {
			await SafeActionAsync(DoClick);
		}

		protected override async Task DelayBeforeExecute() {
			await Task.Delay(20);

			if(prevAction is MouseAction prevMouseAction) {
				if(DimensionsHelper.IsClickPointInSamePosition(prevMouseAction.DownClickedPoint, DownClickedPoint, GetClickPositionToleranceInPercent())) {
					await Task.Delay(MouseHook.DoubleClickTime);
				}
			}
		}

		public override string ToString() {
			return string.Format("{0} Type:{1}, Down:{2}, Up:{3}", nameof(MouseAction), ActionType, DownClickedPoint, UpClickedPoint);
		}
		public override string ExecuteAsScriptSource() {
			if(UpClickedPoint.IsEmpty) {
				return string.Format("new {0}({1}, {2}).{3}(prevAction)", nameof(MouseAction), ActionType.ForNew(), DownClickedPoint.ForNew(),
					nameof(MouseAction.ExecuteAsync));
			} else {
				return string.Format("new {0}({1}, {2}, {3}).{4}(prevAction)", nameof(MouseAction), ActionType.ForNew(), DownClickedPoint.ForNew(), UpClickedPoint.ForNew(),
					nameof(MouseAction.ExecuteAsync));
			}
		}


		ValueTask DoClick() {
			var downClickedPoint = DownClickedPoint;

			if(UpClickedPoint.IsEmpty || DimensionsHelper.IsClickPointInSamePosition(UpClickedPoint, downClickedPoint, GetClickPositionToleranceInPercent())) {
				var actionForDetermineClickPoint = prevAction;

				while(actionForDetermineClickPoint is MouseAction prevMouseAction
					&& DimensionsHelper.IsClickPointInSamePosition(downClickedPoint, prevMouseAction.DownClickedPoint, GetClickPositionToleranceInPercent())) {
					actionForDetermineClickPoint = prevMouseAction.prevAction;
				}

				if(actionForDetermineClickPoint is ElementMatchAction elementMatchAction) {
					if(elementMatchAction.OffsetPoint.HasValue) {
						downClickedPoint.Offset((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
					}
				}
			}

			testsLaunchingService.CloseHighlighter();
			MainForm.MoveOutFromPoint(downClickedPoint.X, downClickedPoint.Y);
			switch(ActionType) {
				case MouseActionType.LeftClick:
					Mouse_MoveTo(downClickedPoint.X, downClickedPoint.Y);
					Mouse.Click(MouseButton.Left);
					break;
				case MouseActionType.RightClick:
					Mouse_MoveTo(downClickedPoint.X, downClickedPoint.Y);
					Mouse.Click(MouseButton.Right);
					break;
				case MouseActionType.MiddleClick:
					Mouse_MoveTo(downClickedPoint.X, downClickedPoint.Y);
					Mouse.Click(MouseButton.Middle);
					break;
				case MouseActionType.LeftDoubleClick:
					Mouse_MoveTo(downClickedPoint.X, downClickedPoint.Y);
					Mouse.DoubleClick(MouseButton.Left);
					break;
				case MouseActionType.LeftDrag:
					DragTo(MouseButton.Left, DownClickedPoint);
					break;
				case MouseActionType.RightDrag:
					DragTo(MouseButton.Right, DownClickedPoint);
					break;
				case MouseActionType.MiddleDrag:
					DragTo(MouseButton.Middle, DownClickedPoint);
					break;
				default:
					throw new SevereException(this, nameof(DoClick));
			}
			return new ValueTask(Task.CompletedTask);
		}

		void Mouse_MoveTo(int x, int y) {
			Mouse.MoveTo(new Point(x, y));
		}

		void DragTo(MouseButton mouseButton, Point downClickablePoint) {
			Point nextClickablePoint = UpClickedPoint;
			Mouse.MoveTo(downClickablePoint);
			Mouse.Down(mouseButton);
			bool xPointReached = false, yPointReached = false;
			int counter = 0;
			do {
				if(downClickablePoint.X < UpClickedPoint.X) {
					downClickablePoint.X++;
				} else if(downClickablePoint.X > UpClickedPoint.X) {
					downClickablePoint.X--;
				} else {
					xPointReached = true;
				}
				if(downClickablePoint.Y < UpClickedPoint.Y) {
					downClickablePoint.Y++;
				} else if(downClickablePoint.Y > UpClickedPoint.Y) {
					downClickablePoint.Y--;
				} else {
					yPointReached = true;
				}
				Mouse.MoveTo(downClickablePoint);
				if(counter++ % 10 == 0) {
					Thread.Sleep(1);
				}
			} while(!xPointReached || !yPointReached);
			Mouse.Up(mouseButton);
		}


	}
}
