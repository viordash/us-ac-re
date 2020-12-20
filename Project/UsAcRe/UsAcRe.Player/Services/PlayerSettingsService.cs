using UsAcRe.Player.Reporters;

namespace UsAcRe.Player.Services {
	public class PlayerSettingsService : IPlayerSettingsService {
		readonly Options options;
		public PlayerSettingsService(Options options) {
			this.options = options;
		}

		public int ElementSearchNestingLevel {
			get {
				return options.ElementSearchNestingLevel;
			}
		}

		public int ClickPositionToleranceInPercent {
			get {
				return options.ClickPositionToleranceInPercent;
			}
		}

		public int? LocationToleranceInPercent {
			get {
				return options.LocationToleranceInPercent;
			}
		}

		public bool CheckByValue {
			get {
				return options.CheckByValue;
			}
		}

		public string TestResultsPath {
			get {
				return options.TestResultsPath;
			}
		}

		public bool Screenshot {
			get {
				return options.Screenshot;
			}
		}

		public ReporterType Reporter {
			get {
				return options.Reporter;
			}
		}


	}
}
