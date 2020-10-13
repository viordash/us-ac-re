using System;
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

		[Test]
		public void GetCurrentCancellationToken_Throws_Error_If_Not_Already_Started() {
			Assert.Throws<InvalidOperationException>(() => testable.GetCurrentCancellationToken());
		}

		[Test]
		public void Start_After_Already_Started_Throws_Error() {
			CancellationToken cancellationToken;
			using(testable.Start(true)) {
				cancellationToken = testable.GetCurrentCancellationToken();
				Assert.Throws<InvalidOperationException>(() => testable.Start(true));
			}
			Assert.That(cancellationToken.IsCancellationRequested, Is.True);
		}
	}
}
