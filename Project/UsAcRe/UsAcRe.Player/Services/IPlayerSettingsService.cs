using UsAcRe.Core.Services;

namespace UsAcRe.Player.Services {
	public interface IPlayerSettingsService : ISettingsService {
		string TestResultsPath { get; }
		bool Screenshot { get; }
	}
}
