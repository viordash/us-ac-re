using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Xml.Serialization;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.UIAutomationElement {
	[Serializable]
	public class UiElement {
		public int Index;
		public string Value;
		public string Name;
		public string ClassName;

		public AutomationIdField AutomationId { get; set; }

		public ControlTypeIdField ControlTypeId { get; set; }
		public Rect BoundingRectangle;
		[XmlIgnore]
		public object AutomationElementObj;

		public UiElement(int index, string value, string name, string className, string automationId, int controlTypeId, Rect boundingRectangle) {
			Index = index;
			Value = value;
			Name = name;
			ClassName = className;
			AutomationId.Value = automationId;
			ControlTypeId.Value = controlTypeId;
			BoundingRectangle = boundingRectangle;
		}

		public override string ToString() {
			return string.Format("[{0}] \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" {6}", Index, ControlTypeId, NamingHelpers.Escape(Name, 60),
				NamingHelpers.Escape(ClassName, 60), AutomationId, Value, BoundingRectangle);
		}

		public string ToShortString() {
			var strings = new[] { Value, Name, ClassName };
			return string.Format("\"{0}\" \"{1}\"", ControlTypeId, NamingHelpers.Escape(strings.FirstOrDefault(s => !string.IsNullOrEmpty(s)), 30));
		}
	}
}
