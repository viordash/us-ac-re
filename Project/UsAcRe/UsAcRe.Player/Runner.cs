using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonServiceLocator;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;
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

		public async Task Start(string filename) {
			logger.Info("Runner.Start: {0}", filename);
			var sourceCode = FileService.ReadAllText(filename);
			Start();
			try {
				actionsCount = 0;
				executedActionsCount = 0;
				if(TestsLaunchingService is PlayerLaunchingService playerLaunchingService) {
					using(TestsLaunchingService.Start(true)) {
						await ScriptCompiler.RunTest(sourceCode);
						actionsCount = TestsLaunchingService.ExecutedActions.Count();
					}
					playerLaunchingService.OnBeforeExecuteAction += (sender, arg) => {
						ExecutionProgress(arg.Action);
					};
				}

				using(TestsLaunchingService.Start(false)) {
					await ScriptCompiler.RunTest(sourceCode);
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
	}
}
