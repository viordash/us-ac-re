using UsAcRe.Core.Services;
using UsAcRe.Player.Reporters;

namespace UsAcRe.Player.Services {
	public interface IPlayerSettingsService : ISettingsService {
		string TestResultsPath { get; }
		bool Screenshot { get; }
		ReporterType Reporter { get; }
	}
}
