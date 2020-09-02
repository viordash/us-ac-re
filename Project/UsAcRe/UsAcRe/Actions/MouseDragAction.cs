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
			var startCoord = StartCoord;
			var endCoord = EndCoord;

			if(prevAction is ElementMatchAction elementMatchAction) {
				if(elementMatchAction.OffsetPoint.HasValue) {
					startCoord.Offset((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
					endCoord.Offset((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
				}
			}


			testsLaunchingService.CloseHighlighter();
			MainForm.MoveOutFromPoint(startCoord.X, startCoord.Y);
			switch(Button) {
				case MouseButtonType.Left:
					DragTo(MouseButton.Left, startCoord, endCoord);
					break;
				case MouseButtonType.Right:
					DragTo(MouseButton.Right, startCoord, endCoord);
					break;
				case MouseButtonType.Middle:
					DragTo(MouseButton.Middle, startCoord, endCoord);
					break;
				default:
					throw new SevereException(this, nameof(DoDrag));
			}
		}

		void DragTo(MouseButton mouseButton, Point startCoord, Point endCoord) {
			Mouse.MoveTo(startCoord);
			Mouse.Down(mouseButton);
			bool xPointReached = false, yPointReached = false;
			int counter = 0;
			do {
				if(startCoord.X < endCoord.X) {
					startCoord.X++;
				} else if(startCoord.X > endCoord.X) {
					startCoord.X--;
				} else {
					xPointReached = true;
				}
				if(startCoord.Y < endCoord.Y) {
					startCoord.Y++;
				} else if(startCoord.Y > endCoord.Y) {
					startCoord.Y--;
				} else {
					yPointReached = true;
				}
				Mouse.MoveTo(startCoord);
				if(counter++ % 10 == 0) {
					Thread.Sleep(1);
				}
			} while(!xPointReached || !yPointReached);
			Mouse.Up(mouseButton);
		}
	}
}
