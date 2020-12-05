namespace UsAcRe.Core.Services {
	public interface ISettingsService {
		int ClickPositionToleranceInPercent { get; }
		bool CheckByValue { get; }
		int ElementSearchNestingLevel { get; }
	}
}
