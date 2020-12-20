namespace UsAcRe.Core.Services {
	public interface ISettingsService {
		int ClickPositionToleranceInPercent { get; }
		int? LocationToleranceInPercent { get; }
		bool CheckByValue { get; }
		int WeakFieldsComparisonsNumber { get; }
	}
}
