using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Xml.Serialization;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	[Serializable]
	public class UiElement {
		public int Index;
		public ValueField Value { get; private set; }
		public NameField Name { get; private set; }
		public ClassNameField ClassName { get; private set; }
		public AutomationIdField AutomationId { get; private set; }
		public ControlTypeIdField ControlTypeId { get; private set; }
		public Rect BoundingRectangle;
		[XmlIgnore]
		public object AutomationElementObj;

		public UiElement(int index, string value, string name, string className, string automationId, int controlTypeId, Rect boundingRectangle) {
			Index = index;
			BoundingRectangle = boundingRectangle;

			Value = new ValueField(value);
			Name = new NameField(name);
			ClassName = new ClassNameField(className);
			ControlTypeId = new ControlTypeIdField(controlTypeId);
			AutomationId = new AutomationIdField(automationId);
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
