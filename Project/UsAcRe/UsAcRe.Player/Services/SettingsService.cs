using UsAcRe.Core.Services;

namespace UsAcRe.Player.Services {
	public class SettingsService : IPlayerSettingsService {
		public int ElementSearchNestingLevel {
			get {
				return 5;
			}
		}

		public int ClickPositionToleranceInPercent {
			get {
				return 50;//			Properties.Settings.Default.ClickPositionToleranceInPercent;
			}
		}

		public bool CheckByValue {
			get {
				return true;
			}
		}

		public string TestResultsPath {
			get {
				return ".\\TestResults";
			}
		}
	}
}
