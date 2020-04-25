using System;
using System.Drawing;
using System.Threading;
using Microsoft.Test.Input;
using UsAcRe.MouseProcess;
using System.ComponentModel;

namespace UsAcRe.Actions {

	public class MouseAction : BaseAction {
		MouseActionType actionType;
		Point downClickedPoint;
		Point upClickedPoint;

		public MouseActionType ActionType {
			get { return actionType; }
			set {
				actionType = value;
				Modified();
			}
		}
		public Point DownClickedPoint {
			get { return downClickedPoint; }
			set {
				downClickedPoint = value;
				Modified();
			}
		}
		public Point UpClickedPoint {
			get { return upClickedPoint; }
			set {
				upClickedPoint = value;
				Modified();
			}
		}

		public override int ExecuteTimeoutMs {
			get { return 60 * 1000; }
		}

		public override void Execute() {
			throw new System.NotImplementedException();
		}

		bool DoClick() {
			MainForm.MoveOutFromPoint(DownClickedPoint.X, DownClickedPoint.Y);
			switch(ActionType) {
				case MouseActionType.LeftClick:
					Mouse_MoveTo(DownClickedPoint.X, DownClickedPoint.Y);
					Mouse.Click(MouseButton.Left);
					return true;
				case MouseActionType.RightClick:
					Mouse_MoveTo(DownClickedPoint.X, DownClickedPoint.Y);
					Mouse.Click(MouseButton.Right);
					return true;
				case MouseActionType.MiddleClick:
					Mouse_MoveTo(DownClickedPoint.X, DownClickedPoint.Y);
					Mouse.Click(MouseButton.Middle);
					return true;
				case MouseActionType.LeftDoubleClick:
					Mouse_MoveTo(DownClickedPoint.X, DownClickedPoint.Y);
					Mouse.DoubleClick(MouseButton.Left);
					return true;
				case MouseActionType.LeftDrag:
					SafeAction(() => DragTo(MouseButton.Left, DownClickedPoint));
					return true;
				case MouseActionType.RightDrag:
					SafeAction(() => DragTo(MouseButton.Right, DownClickedPoint));
					return true;
				case MouseActionType.MiddleDrag:
					SafeAction(() => DragTo(MouseButton.Middle, DownClickedPoint));
					return true;
				default:
					throw new ExecuteMouseActionException(this, nameof(DoClick));
			}
		}

		void Mouse_MoveTo(int x, int y) {
			SafeAction(() => Mouse.MoveTo(new Point(x, y)));
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

		void SafeAction(Action action) {
			try {
				action();
			} catch(Win32Exception ex) {
				if((uint)ex.ErrorCode != 0x80004005) {
					throw;
				}
			}
		}
	}
}
