using System;
using System.Collections.Generic;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.UI.Actions {
	public class ActionPresentation {
		public static Dictionary<Type, System.Drawing.Color> BackgroundColor = new Dictionary<Type, System.Drawing.Color>() {
			{ typeof(ElementMatchAction), System.Drawing.Color.LightCoral},
			{ typeof(KeybdAction), System.Drawing.Color.LightGreen},
			{ typeof(TextTypingAction), System.Drawing.Color.LightSeaGreen},
			{ typeof(MouseClickAction), System.Drawing.Color.LightBlue},
			{ typeof(MouseDragAction), System.Drawing.Color.LightCyan},
			{ typeof(DelayAction), System.Drawing.Color.AliceBlue},
		};
	}
}
