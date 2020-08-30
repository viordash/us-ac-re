namespace UsAcRe.Services {

	public interface ISettingsService {
		int GetClickPositionToleranceInPercent();
		bool AnalyzeTextTyping();
	}

	public class SettingsService : ISettingsService {

		public bool AnalyzeTextTyping() {
			return true;
		}

		public int GetClickPositionToleranceInPercent() {
			return 50;//			Properties.Settings.Default.ClickPositionToleranceInPercent;
		}
	}
}
