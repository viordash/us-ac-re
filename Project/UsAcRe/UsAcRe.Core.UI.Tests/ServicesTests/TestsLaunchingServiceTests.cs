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
			using(testable.Start()) {
				cancellationToken = testable.CurrentCancellationToken;
				Assert.That(cancellationToken.IsCancellationRequested, Is.False);
			}
			Assert.That(cancellationToken.IsCancellationRequested, Is.True);
		}

		[Test]
		public void GetCurrentCancellationToken_Throws_Error_If_Not_Already_Started() {
			Assert.Throws<InvalidOperationException>(() => { var x = testable.CurrentCancellationToken; });
		}

		[Test]
		public void Start_After_Already_Started_Throws_Error() {
			CancellationToken cancellationToken;
			using(testable.Start()) {
				cancellationToken = testable.CurrentCancellationToken;
				Assert.Throws<InvalidOperationException>(() => testable.Start());
			}
			Assert.That(cancellationToken.IsCancellationRequested, Is.True);
		}

		[Test]
		public void Examine_After_Started_Use_Nested_Scope() {
			CancellationToken cancellationToken;
			using(testable.Start()) {
				cancellationToken = testable.CurrentCancellationToken;
				var executedActions = testable.ExecutedActions;

				using(testable.Examine()) {
					Assert.AreEqual(cancellationToken, testable.CurrentCancellationToken);
					Assert.AreNotSame(executedActions, testable.ExecutedActions);
				}
			}
			Assert.That(cancellationToken.IsCancellationRequested, Is.True);
		}
	}
}
