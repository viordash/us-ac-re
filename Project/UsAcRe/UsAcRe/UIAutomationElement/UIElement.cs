using System;
using System.Windows;
using System.Windows.Automation;
using System.Xml.Serialization;

namespace UsAcRe.UIAutomationElement {
	[Serializable]
	public class UiElement {
		public string Value;
		public string Name;
		public int ControlTypeId;
		public Rect BoundingRectangle;
		[XmlIgnore]
		public object AutomationElementObj;
		public override string ToString() {
			return string.Format("\"{0}\" [{1}]; \"{2}\"", ControlType.LookupById(ControlTypeId).LocalizedControlType, Name?.Substring(0, Math.Min(Name.Length, 60)), Value);
		}
	}
}
