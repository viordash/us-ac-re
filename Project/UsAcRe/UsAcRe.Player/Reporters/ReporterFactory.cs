using UsAcRe.Core.Exceptions;
using UsAcRe.Player.Services;

namespace UsAcRe.Player.Reporters {
	public class ReporterFactory {

		public static IReporter Create(IPlayerSettingsService settingsService) {
			switch(settingsService.Reporter) {
				case ReporterType.xUnit:
					return new XUnitReporter();
				default:
					throw new PlayerException("wrong test reporter type");
			}
		}
	}
}
