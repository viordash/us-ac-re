using System.Threading;
using Moq;
using NUnit.Framework;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;

namespace UsAcRe.Core.UI.Tests.ServicesTests {
	[TestFixture]
	public class TestsLaunchingServiceTests {

		TestsLaunchingService testable;
		Mock<IWindowsFormsService> windowsFormsServiceMock;

		[SetUp]
		public virtual void Setup() {
			windowsFormsServiceMock = new Mock<IWindowsFormsService>();
			testable = new TestsLaunchingService(windowsFormsServiceMock.Object);
		}

		[TearDown]
		public virtual void TearDown() {
		}

		[Test]
		public void Start_Method_Is_Disposable_And_CancellationToken_Is_Canceled_On_ScopeEnd() {
			CancellationToken cancellationToken;
			using(testable.Start(true)) {
				cancellationToken = testable.GetCurrentCancellationToken();
				Assert.That(cancellationToken.IsCancellationRequested, Is.False);
			}
			Assert.That(cancellationToken.IsCancellationRequested, Is.True);

		}
	}
}
