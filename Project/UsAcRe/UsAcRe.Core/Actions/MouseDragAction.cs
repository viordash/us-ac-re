using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class MouseDragAction : BaseAction {
		public MouseButtonType Button { get; set; }
		public Point StartCoord { get; set; }
		public Point EndCoord { get; set; }

		public static MouseDragAction Record(MouseButtonType button, Point startCoord, Point endCoord) {
			var instance = CreateInstance<MouseDragAction>();
			instance.Button = button;
			instance.StartCoord = startCoord;
			instance.EndCoord = endCoord;
			return instance;
		}

		public static async Task Play(MouseButtonType button, Point startCoord, Point endCoord) {
			var instance = CreateInstance<MouseDragAction>();
			instance.Button = button;
			instance.StartCoord = startCoord;
			instance.EndCoord = endCoord;
			await instance.ExecuteAsync();
		}

		public MouseDragAction(
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService) : base(settingsService, testsLaunchingService, fileService) {
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoDrag();
		}

		public override string ToString() {
			return string.Format("{0} Button:{1}, Down:{2}, Up:{3}", nameof(MouseDragAction), Button, StartCoord, EndCoord);
		}

		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}({2}, {3}, {4})", nameof(MouseDragAction), nameof(MouseDragAction.Play), Button.ForNew(), StartCoord.ForNew(), EndCoord.ForNew());
		}

		public override string ShortDescription() {
			return string.Format("MouseDrag, Button:{0}, Down:{1}, Up:{2}", Button, StartCoord, EndCoord);
		}

		async ValueTask DoDrag() {
			await Task.Delay(20);
			var startCoord = StartCoord;
			var endCoord = EndCoord;

			if(testsLaunchingService.LastAction is ElementMatchAction elementMatchAction) {
				if(elementMatchAction.OffsetPoint.HasValue) {
					startCoord.Offset((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
					endCoord.Offset((int)elementMatchAction.OffsetPoint.Value.X, (int)elementMatchAction.OffsetPoint.Value.Y);
				}
			}

			testsLaunchingService.CloseHighlighter();
			//MainForm.MoveOutFromPoint(startCoord.X, startCoord.Y);
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
