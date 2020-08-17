using System;
using UsAcRe.Extensions;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Actions {
	public class KeybdAction : BaseAction {
		public VirtualKeyCodes VKCode { get; set; }
		public bool IsUp { get; set; }

		public KeybdAction(VirtualKeyCodes vKCode, bool isUp) {
			VKCode = vKCode;
			IsUp = isUp;
		}

		protected override void ExecuteCore() {
			//SafeAction(() => DoClick());
		}

		public override string ToString() {
			return string.Format("{0} Code:{0:D3}, Down:{2}, Up:{3}", nameof(KeybdAction), VKCode, IsUp ? "Up" : "  ", Convert.ToChar(VKCode));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("new {0}({1}, {2}).{3}()", nameof(KeybdAction), VKCode.ForNew(), IsUp.ForNew(), nameof(KeybdAction.Execute));
		}

	}
}
