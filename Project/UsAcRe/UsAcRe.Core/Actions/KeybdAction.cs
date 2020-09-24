using System;
using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Actions {
	public class KeybdAction : BaseAction {
		public VirtualKeyCodes VKCode { get; set; }
		public bool IsUp { get; set; }

		public static KeybdAction CreateInstance(VirtualKeyCodes vKCode, bool isUp) {
			var instance = CreateInstance<KeybdAction>();
			instance.VKCode = vKCode;
			instance.IsUp = isUp;
			return instance;
		}

		public KeybdAction(ITestsLaunchingService testsLaunchingService) : base(testsLaunchingService) {
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoKeyPress();
		}

		public override string ToString() {
			return string.Format("{0} Code:{0:D3}, Down:{2}, Up:{3}", nameof(KeybdAction), VKCode, IsUp ? "Up" : "  ", Convert.ToChar(VKCode));
		}
		public override string ExecuteAsScriptSource() {
			return null;//string.Format("{0}({1}, {2})", nameof(ActionsExecutor.Keyboard), VKCode.ForNew(), IsUp.ForNew());
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
