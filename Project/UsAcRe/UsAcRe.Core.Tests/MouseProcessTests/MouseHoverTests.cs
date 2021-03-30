using System.Diagnostics;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.Services;
using UsAcRe.Core.Tests.ActionsTests;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Tests.MouseProcessTests {
	[TestFixture]
	public class MouseHoverTests : Testable {

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}


		[Test]
		public async Task Perform_Test() {
			int step = 0;

			winApiServiceMock
				.Setup(x => x.SendMouseInput(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<WinAPI.SendMouseInputFlags>()))
				.Callback<int, int, uint, WinAPI.SendMouseInputFlags>((x, y, data, flags) => {
					new WinApiService().SendMouseInput(x, y, data, flags);
				});


			WinAPI.SetCursorPos(800 - 50, 600 - 50);

			var stopwatch = Stopwatch.StartNew();
			WinAPI.GetCursorPos(out WinAPI.POINT startPt);
			for(int i = 0; i < 4; i++) {
				var point = new System.Windows.Point(startPt.x, startPt.y);
				await new MouseHover(winApiServiceMock.Object).Perform(point, step, 1);
				step += 4;
			}

			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.GreaterThan(4 * 10));

			WinAPI.GetCursorPos(out WinAPI.POINT currPt);
			Assert.That(currPt.x, Is.InRange(startPt.x - 64, startPt.x + 64));
			Assert.That(currPt.y, Is.InRange(startPt.y - 64, startPt.y + 64));
		}
	}
}
