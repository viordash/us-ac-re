using System.Threading.Tasks;
using CommonServiceLocator;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;

namespace UsAcRe.Player {
	public class Runner {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");

		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }
		ISettingsService SettingsService { get { return ServiceLocator.Current.GetInstance<ISettingsService>(); } }
		IFileService FileService { get { return ServiceLocator.Current.GetInstance<IFileService>(); } }
		IScriptCompiler ScriptCompiler { get { return ServiceLocator.Current.GetInstance<IScriptCompiler>(); } }

		public async Task Start(string filename) {
			logger.Info("Runner.Start: {0}", filename);
			var sourceCode = FileService.ReadAllText(filename);
			//StartKeyboardHooks();
			try {
				using(TestsLaunchingService.Start(false)) {
					await ScriptCompiler.RunTest(sourceCode);
				}
			} catch(TestFailedExeption ex) {
				logger.Error(ex.Message);
				throw;
			}
		}

	}
}
