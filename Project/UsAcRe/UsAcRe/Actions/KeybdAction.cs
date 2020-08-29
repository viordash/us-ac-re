using System;
using System.Threading.Tasks;
using UsAcRe.Extensions;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Actions {
	public class KeybdAction : BaseAction {
		public VirtualKeyCodes VKCode { get; set; }
		public bool IsUp { get; set; }


		public KeybdAction(VirtualKeyCodes vKCode, bool isUp)
			: this(null, vKCode, isUp) { }

		public KeybdAction(BaseAction prevAction, VirtualKeyCodes vKCode, bool isUp) : base(prevAction) {
			VKCode = vKCode;
			IsUp = isUp;
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await Task.Delay(10);
		}

		public override string ToString() {
			return string.Format("{0} Code:{0:D3}, Down:{2}, Up:{3}", nameof(KeybdAction), VKCode, IsUp ? "Up" : "  ", Convert.ToChar(VKCode));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}({1}, {2})", nameof(ActionsExecutor.Keyboard), VKCode.ForNew(), IsUp.ForNew());
		}

	}
}
