namespace UsAcRe.Services {

	public interface ISettingsService {
		int GetClickPositionToleranceInPercent();
		bool AnalyzeTextTyping();
		bool CheckByValue();
	}

	public class SettingsService : ISettingsService {

		public bool AnalyzeTextTyping() {
			return true;
		}

		public bool CheckByValue() {
			return true;
		}

		public int GetClickPositionToleranceInPercent() {
			return 50;//			Properties.Settings.Default.ClickPositionToleranceInPercent;
		}
	}
}
