using System.Threading.Tasks;
using Microsoft.Test.Input;

namespace UsAcRe.MouseProcess {
	public class MouseHover {
		const int beamCount = 8;
		const int beamLengthStep = 2;
		const int beamLengthStepMaxCount = 16 + 1;

		public static async Task Perform(System.Windows.Point point, int step) {
			for(int beamNumb = 1; beamNumb < beamCount; beamNumb++) {
				var gradation = step % (beamCount * beamLengthStepMaxCount);
				var beamLengthNum = gradation % beamLengthStepMaxCount;
				var x = point.X;
				var y = point.Y;
				var shiftPos = beamLengthStep * beamLengthNum;
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
				Mouse.MoveTo(new System.Drawing.Point((int)x, (int)y));
				await Task.Delay(100);
			}
		}
	}
}
