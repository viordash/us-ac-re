using System.Threading.Tasks;
using Microsoft.Test.Input;

namespace UsAcRe.MouseProcess {
	public class MouseHover {
		const int beamCount = 8;
		const int beamLengthStep = 2;
		const int beamLengthStepMaxCount = 16 + 1;

		public static async Task Perform(System.Windows.Point point, int step, int delay = 20) {
			var gradation = step % (beamCount * beamLengthStepMaxCount);
			var beamLengthNum = gradation % beamLengthStepMaxCount;
			var shiftPos = beamLengthStep * beamLengthNum;
			var x = point.X - (shiftPos / 2);
			var y = point.Y - (shiftPos + (shiftPos / 2));
			var prevX = x;
			var prevY = y;

			for(int beamNumb = 0; beamNumb < beamCount; beamNumb++) {
				switch(beamNumb) {
					case 0:
						x = x + shiftPos;
						break;
					case 1:
						x = x + shiftPos;
						y = y + shiftPos;
						break;
					case 2:
						y = y + shiftPos;
						break;
					case 3:
						x = x - shiftPos;
						y = y + shiftPos;
						break;
					case 4:
						x = x - shiftPos;
						break;
					case 5:
						x = x - shiftPos;
						y = y - shiftPos;
						break;
					case 6:
						y = y - shiftPos;
						break;
					case 7:
						x = x + shiftPos;
						y = y - shiftPos;
						break;
				}

				const int interpolate = 4;
				var deltaX = (x - prevX) / interpolate;
				var deltaY = (y - prevY) / interpolate;
				var delayDelta = delay / interpolate;
				if(delayDelta <= 0) {
					delayDelta = 1;
				}
				if(deltaX >= 1 && deltaY >= 1) {
					for(int i = 0; i < interpolate; i++) {
						prevX += deltaX;
						prevY += deltaY;
						Mouse.MoveTo(new System.Drawing.Point((int)prevX, (int)prevY));
						await Task.Delay(delayDelta);
					}
				} else {
					Mouse.MoveTo(new System.Drawing.Point((int)x, (int)y));
					await Task.Delay(delay);
				}
				prevX = x;
				prevY = y;
			}
		}
	}
}
