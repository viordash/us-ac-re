using System.Threading.Tasks;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.MouseProcess {
	public class MouseHover {
		readonly IWinApiService winApiService;
		const int beamCount = 8;
		const int beamLengthStep = 1;
		const int beamLengthStepMaxCount = 8 + 1;

		public MouseHover(IWinApiService winApiService) {
			this.winApiService = winApiService;
		}

		public async Task Perform(System.Windows.Point point, int step, int delay = 20) {
			var gradation = step % (beamCount * beamLengthStepMaxCount);
			var beamLengthNum = gradation % beamLengthStepMaxCount;
			var shiftPos = beamLengthStep * beamLengthNum;
			var x = point.X - (shiftPos / 2);
			var y = point.Y - (shiftPos + (shiftPos / 2));
			var prev = new System.Windows.Point(x, y);

			for(int beamNumb = 0; beamNumb < beamCount; beamNumb++) {
				switch(beamNumb) {
					case 0:
						x += shiftPos;
						break;
					case 1:
						x += shiftPos;
						y += shiftPos;
						break;
					case 2:
						y += shiftPos;
						break;
					case 3:
						x -= shiftPos;
						y += shiftPos;
						break;
					case 4:
						x -= shiftPos;
						break;
					case 5:
						x -= shiftPos;
						y -= shiftPos;
						break;
					case 6:
						y -= shiftPos;
						break;
					case 7:
						x += shiftPos;
						y -= shiftPos;
						break;
				}
				prev = await SmoothMove(prev, new System.Windows.Point(x, y), delay, 8);
			}
		}


		public async Task<System.Windows.Point> SmoothMove(System.Windows.Point from, System.Windows.Point to, int delay, int interpolate) {
			var deltaX = (to.X - from.X) / interpolate;
			var deltaY = (to.Y - from.Y) / interpolate;
			var delayDelta = delay / interpolate;
			if(delayDelta <= 0) {
				delayDelta = 1;
			}
			if(deltaX != 0 || deltaY != 0) {
				for(int i = 0; i < interpolate; i++) {
					from.X += deltaX;
					from.Y += deltaY;
					MoveTo(from);
					await Task.Delay(delayDelta);
				}
			} else {
				MoveTo(to);
				await Task.Delay(delay);
			}
			return from;
		}


		public void MoveTo(System.Windows.Point point) {
			new Mouse(winApiService).MoveTo(new System.Drawing.Point((int)point.X, (int)point.Y));
		}
	}
}
