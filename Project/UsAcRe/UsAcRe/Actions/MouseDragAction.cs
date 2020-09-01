using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Exceptions;
using UsAcRe.Extensions;
using UsAcRe.MouseProcess;

namespace UsAcRe.Actions {
	public class MouseDragAction : BaseAction {
		public MouseButtonType Button { get; set; }
		public Point StartCoord { get; set; }
		public Point EndCoord { get; set; }

		public MouseDragAction(MouseDragEventArgs args) : this(null, args.Button, args.StartCoord, args.EndCoord) { }

		public MouseDragAction(BaseAction prevAction, MouseButtonType button, Point startCoord, Point endCoord)
			: base(prevAction) {
			Button = button;
			StartCoord = startCoord;
			EndCoord = endCoord;
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoDrag();
		}

		public override string ToString() {
			return string.Format("{0} Button:{1}, Down:{2}, Up:{3}", nameof(MouseDragAction), Button, StartCoord, EndCoord);
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}({1}, {2}, {3})", nameof(ActionsExecutor.MouseDrag), Button.ForNew(), StartCoord.ForNew(), EndCoord.ForNew());

		}

		async ValueTask DoDrag() {
			await Task.Delay(20);
			var downClickedPoint = StartCoord;

			testsLaunchingService.CloseHighlighter();
			MainForm.MoveOutFromPoint(downClickedPoint.X, downClickedPoint.Y);
			switch(Button) {
				case MouseButtonType.Left:
					DragTo(MouseButton.Left, StartCoord);
					break;
				case MouseButtonType.Right:
					DragTo(MouseButton.Right, StartCoord);
					break;
				case MouseButtonType.Middle:
					DragTo(MouseButton.Middle, StartCoord);
					break;
				default:
					throw new SevereException(this, nameof(DoDrag));
			}
		}

		void DragTo(MouseButton mouseButton, Point downClickablePoint) {
			Point nextClickablePoint = EndCoord;
			Mouse.MoveTo(downClickablePoint);
			Mouse.Down(mouseButton);
			bool xPointReached = false, yPointReached = false;
			int counter = 0;
			do {
				if(downClickablePoint.X < EndCoord.X) {
					downClickablePoint.X++;
				} else if(downClickablePoint.X > EndCoord.X) {
					downClickablePoint.X--;
				} else {
					xPointReached = true;
				}
				if(downClickablePoint.Y < EndCoord.Y) {
					downClickablePoint.Y++;
				} else if(downClickablePoint.Y > EndCoord.Y) {
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
