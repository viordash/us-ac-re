using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Xml.Serialization;
using UsAcRe.Helpers;

namespace UsAcRe.UIAutomationElement {
	[Serializable]
	public class UiElement {
		public int Index;
		public string Value;
		public string Name;
		public string ClassName;
		public string AutomationId;
		public int ControlTypeId;
		public Rect BoundingRectangle;
		[XmlIgnore]
		public object AutomationElementObj;

		public UiElement(int index, string value, string name, string className, string automationId, int controlTypeId, Rect boundingRectangle) {
			Index = index;
			Value = value;
			Name = name;
			ClassName = className;
			AutomationId = automationId;
			ControlTypeId = controlTypeId;
			BoundingRectangle = boundingRectangle;
		}

		public override string ToString() {
			return string.Format("[{0}] \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" {6}", Index, ControlType.LookupById(ControlTypeId).LocalizedControlType,
				NamingHelpers.Escape(Name, 60), NamingHelpers.Escape(ClassName, 60), AutomationId, Value, BoundingRectangle);
		}

		public string ToShortString() {
			var strings = new[] { Value, Name, ClassName };
			return string.Format("\"{0}\" \"{1}\"", ControlType.LookupById(ControlTypeId).LocalizedControlType,
				 NamingHelpers.Escape(strings.FirstOrDefault(s => !string.IsNullOrEmpty(s)), 30));
		}
	}
}
