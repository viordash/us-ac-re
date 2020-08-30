using System;
using System.Collections.Generic;
using System.Linq;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Extensions {
	public static class VirtualKeyCodesExtensions {
		public static bool IsPrintable(this VirtualKeyCodes keyCode) {
			return PrintableCodes.Any(x => x == keyCode);
		}

		public static bool TryGetKeyValue(this VirtualKeyCodes keyCode, out char value) {
			value = default;
			if(!IsPrintable(keyCode)) {
				return false;
			}
			value = char.ToLower(Convert.ToChar(keyCode));
			return true;
		}


		static IEnumerable<VirtualKeyCodes> PrintableCodes = new VirtualKeyCodes[] {
			VirtualKeyCodes.VK_TAB,
			VirtualKeyCodes.VK_SPACE,
			VirtualKeyCodes.K_0,
			VirtualKeyCodes.K_1,
			VirtualKeyCodes.K_2,
			VirtualKeyCodes.K_3,
			VirtualKeyCodes.K_4,
			VirtualKeyCodes.K_5,
			VirtualKeyCodes.K_6,
			VirtualKeyCodes.K_7,
			VirtualKeyCodes.K_8,
			VirtualKeyCodes.K_9,
			VirtualKeyCodes.K_A,
			VirtualKeyCodes.K_B,
			VirtualKeyCodes.K_C,
			VirtualKeyCodes.K_D,
			VirtualKeyCodes.K_E,
			VirtualKeyCodes.K_F,
			VirtualKeyCodes.K_G,
			VirtualKeyCodes.K_H,
			VirtualKeyCodes.K_I,
			VirtualKeyCodes.K_J,
			VirtualKeyCodes.K_K,
			VirtualKeyCodes.K_L,
			VirtualKeyCodes.K_M,
			VirtualKeyCodes.K_N,
			VirtualKeyCodes.K_O,
			VirtualKeyCodes.K_P,
			VirtualKeyCodes.K_Q,
			VirtualKeyCodes.K_R,
			VirtualKeyCodes.K_S,
			VirtualKeyCodes.K_T,
			VirtualKeyCodes.K_U,
			VirtualKeyCodes.K_V,
			VirtualKeyCodes.K_W,
			VirtualKeyCodes.K_X,
			VirtualKeyCodes.K_Y,
			VirtualKeyCodes.K_Z
		};
	}
}
