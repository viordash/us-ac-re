using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Actions {
	public class KeybdAction : BaseAction {
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
			ITestsLaunchingService testsLaunchingService) : base(settingsService, testsLaunchingService) {
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoKeyPress();
		}

		public override string ToString() {
			return string.Format("{0} Code:{0:D3}, Down:{2}, Up:{3}", nameof(KeybdAction), VKCode, IsUp ? "Up" : "  ", Convert.ToChar(VKCode));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}({2}, {3})", nameof(KeybdAction), nameof(KeybdAction.Play), VKCode.ForNew(), IsUp.ForNew());
		}

		ValueTask DoKeyPress() {
			if(!this.IsUp) {
				Keyboard.Press((Key)this.VKCode);
			} else if(this.IsUp) {
				Keyboard.Release((Key)this.VKCode);
			}
			return new ValueTask(Task.CompletedTask);
		}
	}
}
