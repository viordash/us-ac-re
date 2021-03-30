using System.Collections.Generic;
using System.Linq;
using GuardNet;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.WindowsSystem {
	public class Keyboard {
		readonly IWinApiService winApiService;
		static readonly List<VirtualKeyCodes> ExtentedKeys = new() {
			VirtualKeyCodes.VK_PRIOR,
			VirtualKeyCodes.VK_NEXT,
			VirtualKeyCodes.VK_END,
			VirtualKeyCodes.VK_HOME,
			VirtualKeyCodes.VK_LEFT,
			VirtualKeyCodes.VK_UP,
			VirtualKeyCodes.VK_RIGHT,
			VirtualKeyCodes.VK_DOWN,
			VirtualKeyCodes.VK_SNAPSHOT,
			VirtualKeyCodes.VK_INSERT,
			VirtualKeyCodes.VK_DELETE,

			VirtualKeyCodes.VK_LWIN,
			VirtualKeyCodes.VK_RWIN,
			VirtualKeyCodes.VK_APPS,
			VirtualKeyCodes.VK_DIVIDE,
			VirtualKeyCodes.VK_NUMLOCK,
			VirtualKeyCodes.VK_RCONTROL,
			VirtualKeyCodes.VK_RMENU
		};

		public Keyboard(IWinApiService winApiService) {
			Guard.NotNull(winApiService, nameof(winApiService));
			this.winApiService = winApiService;
		}

		public void Press(VirtualKeyCodes vKCode) {
			winApiService.SendKeyboardKey(vKCode, true, ExtentedKeys.Contains(vKCode), false);
		}

		public void Release(VirtualKeyCodes vKCode) {
			winApiService.SendKeyboardKey(vKCode, false, ExtentedKeys.Contains(vKCode), false);
		}

		public void Type(string text) {
			foreach(char c in text) {
				bool isUnicode = c > 0xFE;
				if(isUnicode) {
					winApiService.SendKeyboardKey((VirtualKeyCodes)c, true, false, true);
					winApiService.SendKeyboardKey((VirtualKeyCodes)c, false, false, true);
				} else {
					var vKeyValue = winApiService.GetKeyScan(c);
					var key = (VirtualKeyCodes)(vKeyValue & WinAPI.VKeyCharMask);

					var modifierKeys = new List<VirtualKeyCodes>();
					if((vKeyValue & WinAPI.VKeyShiftMask) == WinAPI.VKeyShiftMask) {
						modifierKeys.Add(VirtualKeyCodes.VK_SHIFT);
					}
					if((vKeyValue & WinAPI.VKeyCtrlMask) == WinAPI.VKeyCtrlMask) {
						modifierKeys.Add(VirtualKeyCodes.VK_CONTROL);
					}
					if((vKeyValue & WinAPI.VKeyAltMask) == WinAPI.VKeyAltMask) {
						modifierKeys.Add(VirtualKeyCodes.VK_MENU);
					}
					Type(key, modifierKeys);
				}
			}
		}

		void Type(VirtualKeyCodes key, IEnumerable<VirtualKeyCodes> modifierKeys) {
			foreach(var modiferKey in modifierKeys) {
				Press(modiferKey);
			}

			Press(key);
			Release(key);

			foreach(var modifierKey in modifierKeys.Reverse()) {
				Release(modifierKey);
			}
		}
	}
}
