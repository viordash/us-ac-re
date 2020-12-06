using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Tests.ActionsTests {
	[TestFixture]
	public class BaseActionTests : Testable {
		class TestAction : BaseAction {
			readonly Func<BaseAction, ValueTask> action;

			public TestAction(Func<BaseAction, ValueTask> action, ISettingsService settingsService, ITestsLaunchingService testsLaunchingService, IFileService fileService)
				: base(settingsService, testsLaunchingService, fileService) {
				this.action = action;
			}

			public override string ExecuteAsScriptSource() {
				throw new System.NotImplementedException();
			}

			protected override async ValueTask ExecuteCoreAsync() {
				await action(this);
			}
		}

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		[Test]
		public async Task Calc_Duration_Test() {
			var cancellationToken = new CancellationToken(false);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			var testAction = new TestAction(async (act) => {
				await Task.Delay(220);
			},
				settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object
			);

			var stopwatch = Stopwatch.StartNew();
			await testAction.ExecuteAsync();
			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.GreaterThan(200));
			Assert.That(testAction.Duration, Is.GreaterThan(TimeSpan.FromMilliseconds(200)).And.LessThan(TimeSpan.FromMilliseconds(300)));
		}

		[Test]
		public void Calc_Duration_When_Failed_Test() {
			var cancellationToken = new CancellationToken(false);
			testsLaunchingServiceMock
				.Setup(x => x.GetCurrentCancellationToken())
				.Returns(() => {
					return cancellationToken;
				});

			var testAction = new TestAction(async (act) => {
				await Task.Delay(333);
				throw new TestFailedException(act);
			},
				settingsServiceMock.Object, testsLaunchingServiceMock.Object, fileServiceMock.Object
			);

			var stopwatch = Stopwatch.StartNew();
			Assert.ThrowsAsync<TestFailedException>(async () => await testAction.ExecuteAsync());
			var elapsed = stopwatch.Elapsed.TotalMilliseconds;
			Assert.That(elapsed, Is.GreaterThan(330));
			Assert.That(testAction.Duration, Is.GreaterThan(TimeSpan.FromMilliseconds(300)).And.LessThan(TimeSpan.FromMilliseconds(400)));
		}
	}
}
