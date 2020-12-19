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
		public ValueField Value { get; set; }
		public NameField Name { get; set; }
		public ClassNameField ClassName { get; set; }
		public AutomationIdField AutomationId { get; set; }
		public ControlTypeIdField ControlTypeId { get; set; }
		public Rect BoundingRectangle;
		[XmlIgnore]
		public object AutomationElementObj;

		public UiElement(int index, string value, string name, string className, string automationId, int controlTypeId, Rect boundingRectangle) {
			Index = index;
			Value.Value = value;
			Name.Value = name;
			ClassName.Value = className;
			AutomationId.Value = automationId;
			ControlTypeId.Value = controlTypeId;
			BoundingRectangle = boundingRectangle;
		}

		public override string ToString() {
			return string.Format("[{0}] \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" {6}", Index, ControlTypeId, Name, ClassName, AutomationId, Value, BoundingRectangle);
		}

		public string ToShortString() {
			var strings = new[] { Value.Value, Name.Value, ClassName.Value };
			return string.Format("\"{0}\" \"{1}\"", ControlTypeId, NamingHelpers.Escape(strings.FirstOrDefault(s => !string.IsNullOrEmpty(s)), 30));
		}
	}
}
