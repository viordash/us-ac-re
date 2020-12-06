using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonServiceLocator;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;
using UsAcRe.Core.WindowsSystem;
using UsAcRe.Player.Reporters;
using UsAcRe.Player.Services;

namespace UsAcRe.Player {
	public class Runner {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");
		int hotKey = -1;
		int actionsCount = 0;
		int executedActionsCount = 0;
		string title;

		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }
		IFileService FileService { get { return ServiceLocator.Current.GetInstance<IFileService>(); } }
		IScriptCompiler ScriptCompiler { get { return ServiceLocator.Current.GetInstance<IScriptCompiler>(); } }
		IPlayerSettingsService PlayerSettingsService { get { return ServiceLocator.Current.GetInstance<IPlayerSettingsService>(); } }

		public async Task Start(string filename) {
			logger.Info("Runner.Start: {0}", filename);
			var sourceCode = FileService.ReadAllText(filename);
			var reporter = ReporterFactory.Create(PlayerSettingsService);
			Start();
			try {
				actionsCount = 0;
				executedActionsCount = 0;
				if(TestsLaunchingService is TestsLaunchingService testsLaunchingService) {
					using(TestsLaunchingService.Start(true)) {
						await ScriptCompiler.RunTest(sourceCode);
						actionsCount = TestsLaunchingService.ExecutedActions.Count();
					}
					testsLaunchingService.OnBeforeExecuteAction += (sender, arg) => {
						ExecutionProgress(arg.Action);
					};
					testsLaunchingService.OnAfterExecuteAction += (sender, arg) => {
						ExecutionProgress(arg.Action);
					};
				}


				using(TestsLaunchingService.Start(false)) {
					try {
						await ScriptCompiler.RunTest(sourceCode);
					} catch(ExecuteBaseActionException) {
						if(PlayerSettingsService.Screenshot) {
							TakeScreenshot(filename);
						}
						throw;
					}
				}
			} finally {
				Stop();
			}
		}

		void Start() {
			title = Console.Title;
			WinAPI.SendMessage(Process.GetCurrentProcess().MainWindowHandle, WinAPI.WM_SYSCOMMAND, new IntPtr(WinAPI.SC_MINIMIZE), IntPtr.Zero);
			hotKey = HotKeyManager.RegisterHotKey(Keys.Pause, WinAPI.KeyModifiers.None);
			HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
		}

		void Stop() {
			HotKeyManager.UnregisterHotKey(hotKey);
			WinAPI.SendMessage(Process.GetCurrentProcess().MainWindowHandle, WinAPI.WM_SYSCOMMAND, new IntPtr(WinAPI.SC_RESTORE), IntPtr.Zero);
			Console.Title = title;
		}

		void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e) {
			if(e.Key == Keys.Pause) {
				TestsLaunchingService.Stop();
			}
		}

		void ExecutionProgress(BaseAction testAction) {
			executedActionsCount++;
			Console.Title = $"{executedActionsCount} of {actionsCount}";
			logger.Info("{0,5}|{1}", executedActionsCount, testAction.ToString());
		}

		void TakeScreenshot(string testFilename) {
			var screenshotFileName = string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(testFilename), DateTime.Now.ToString("yyyyMMddHHmm"));
			var screenshotFileFullPath = Path.Combine(PlayerSettingsService.TestResultsPath, screenshotFileName);
			Screenshot.MakePng(screenshotFileFullPath);
			logger.Info("Screenshot saved to '{0}'", screenshotFileName);
		}
	}
}
