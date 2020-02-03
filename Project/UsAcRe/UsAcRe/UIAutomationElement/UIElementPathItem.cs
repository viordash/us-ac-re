﻿using System;
using System.Windows.Automation;

namespace UsAcRe.UIAutomationElement {
	[Serializable]
	public class UIElementPathItem {
		public string Name;
		public AutomationIdentifier ControlType;
		public int Index;
		public System.Drawing.Point Location;
		public System.Drawing.Size Size;
		public bool SearchOrderBack;
		public override string ToString() {
			return string.Format("\"{0}\" [{1}]; \"{2}\"", ControlType.ProgrammaticName, Index, Name.Substring(0, Math.Min(Name.Length, 60)));
		}
	}
}
