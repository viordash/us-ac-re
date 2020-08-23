using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.MouseProcess;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Tests.MouseProcessTests {
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

			var stopwatch = Stopwatch.StartNew();
			WinAPI.GetCursorPos(out startPt);
			for(int i = 0; i < 16; i++) {
				var point = new System.Windows.Point(startPt.x, startPt.y);
				await MouseHover.Perform(point, step, 1);
				step++;
			}

			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.GreaterThan(4 * 10));

			WinAPI.GetCursorPos(out currPt);
			Assert.That(currPt.x, Is.InRange(startPt.x - 64, startPt.x + 64));
			Assert.That(currPt.y, Is.InRange(startPt.y - 64, startPt.y + 64));
		}
	}
}
