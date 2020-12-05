using UsAcRe.Core.Services;

namespace UsAcRe.Services {
	public class SettingsService : IRecorderSettingsService {
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
	}
}
