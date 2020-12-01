using UsAcRe.Core.Services;

namespace UsAcRe.Player.Services {
	public class SettingsService : ISettingsService {
		int ISettingsService.ElementSearchNestingLevel {
			get {
				return 5;
			}
		}

		int ISettingsService.ClickPositionToleranceInPercent {
			get {
				return 50;//			Properties.Settings.Default.ClickPositionToleranceInPercent;
			}
		}

		bool ISettingsService.AnalyzeTextTyping {
			get {
				return true;
			}
		}

		bool ISettingsService.CheckByValue {
			get {
				return true;
			}
		}
	}
}
