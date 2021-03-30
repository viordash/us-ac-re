using System;
using System.Drawing;
using System.Threading.Tasks;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Actions {
	public class KeybdAction : BaseAction {
		readonly IWinApiService winApiService;
		public VirtualKeyCodes VKCode { get; set; }
		public bool IsUp { get; set; }

		public static KeybdAction Record(VirtualKeyCodes vKCode, bool isUp) {
			var instance = CreateInstance<KeybdAction>();
			instance.VKCode = vKCode;
			instance.IsUp = isUp;
			return instance;
		}

		public static async Task Play(VirtualKeyCodes vKCode, bool isUp) {
			var instance = CreateInstance<KeybdAction>();
			instance.VKCode = vKCode;
			instance.IsUp = isUp;
			await instance.ExecuteAsync();
		}

		public KeybdAction(
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService,
			IWinApiService winApiService) : base(settingsService, testsLaunchingService, fileService) {
			this.winApiService = winApiService;
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoKeyPress();
		}

		public override string ToString() {
			return string.Format("{0} Code:{1}, Down:{2}, Up:{3}", nameof(KeybdAction), VKCode, IsUp ? "Up" : "  ", Convert.ToChar(VKCode));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}({2}, {3})", nameof(KeybdAction), nameof(KeybdAction.Play), VKCode.ForNew(), IsUp.ForNew());
		}

		public override string ShortDescription() {
			return string.Format("Keybd, Code:{0}", VKCode);
		}

		ValueTask DoKeyPress() {
			var keyboard = new Keyboard(winApiService);
			if(!this.IsUp) {
				keyboard.Press(VKCode);
			} else if(this.IsUp) {
				keyboard.Release(VKCode);
			}
			return new ValueTask(Task.CompletedTask);
		}
	}
}
