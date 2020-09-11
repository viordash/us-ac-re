using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Tests.MouseProcessTests {
	[TestFixture]
	public class MouseHoverTests {

		[SetUp]
		public void Setup() {

		}

		[TearDown]
		public void TearDown() {

		}


		[Test]
		public async Task Perform_Test() {
			int step = 0;
			WinAPI.POINT currPt;
			WinAPI.POINT startPt;

			WinAPI.SetCursorPos(800 - 50, 600 - 50);

			var stopwatch = Stopwatch.StartNew();
			WinAPI.GetCursorPos(out startPt);
			for(int i = 0; i < 4; i++) {
				var point = new System.Windows.Point(startPt.x, startPt.y);
				await MouseHover.Perform(point, step, 1);
				step += 4;
			}

			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.GreaterThan(4 * 10));

			WinAPI.GetCursorPos(out currPt);
			Assert.That(currPt.x, Is.InRange(startPt.x - 64, startPt.x + 64));
			Assert.That(currPt.y, Is.InRange(startPt.y - 64, startPt.y + 64));
		}
	}
}
