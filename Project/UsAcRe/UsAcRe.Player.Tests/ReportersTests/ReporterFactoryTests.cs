using Moq;
using NUnit.Framework;
using UsAcRe.Core.Exceptions;
using UsAcRe.Player.Reporters;
using UsAcRe.Player.Services;

namespace UsAcRe.Player.Tests.ReportersTests {
	[TestFixture]
	public class ReporterFactoryTests {
		Mock<IPlayerSettingsService> settingsServiceMock;

		[SetUp]
		public virtual void Setup() {
			settingsServiceMock = new Mock<IPlayerSettingsService>();
		}

		[TearDown]
		public virtual void TearDown() {
		}

		[Test]
		public void Create_Wrong_ReportType_Throws_PlayerException_Test() {
			settingsServiceMock
				.SetupGet(x => x.Reporter)
				.Returns(() => {
					return (ReporterType)10000;
				});
			Assert.Throws<PlayerException>(() => ReporterFactory.Create(settingsServiceMock.Object));
		}

		[Test]
		public void Create_XUnit_Test() {
			settingsServiceMock
				.SetupGet(x => x.Reporter)
				.Returns(() => {
					return ReporterType.xUnit;
				});
			var reporter = ReporterFactory.Create(settingsServiceMock.Object);
			Assert.That(reporter, Is.InstanceOf<XUnitReporter>());
		}
	}
}
