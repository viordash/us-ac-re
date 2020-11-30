using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonServiceLocator;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;
//using UsAcRe.TestsScripts;

namespace UsAcRe.Player {
	public class Runner {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");

		int hotKey = -1;

		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }
		ISettingsService SettingsService { get { return ServiceLocator.Current.GetInstance<ISettingsService>(); } }
		IFileService FileService { get { return ServiceLocator.Current.GetInstance<IFileService>(); } }
		IScriptCompiler ScriptCompiler { get { return ServiceLocator.Current.GetInstance<IScriptCompiler>(); } }

		public async Task Start(string filename) {
			logger.Info("Runner.Start: {0}", filename);
			var sourceCode = FileService.ReadAllText(filename);
			StartKeyboardHooks();
			try {
				using(TestsLaunchingService.Start(false)) {
					//var ss = new TestsScript();
					//await ss.ExecuteAsync();
					await ScriptCompiler.RunTest(sourceCode);
				}
			} finally {
				StopKeyboadHooks();
			}
		}

		void StartKeyboardHooks() {
			hotKey = HotKeyManager.RegisterHotKey(Keys.Pause, WinAPI.KeyModifiers.None);
			HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
		}

		void StopKeyboadHooks() {
			HotKeyManager.UnregisterHotKey(hotKey);
		}

		void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e) {
			if(e.Key == Keys.Pause) {
				TestsLaunchingService.Stop();
			}
		}
	}
}
