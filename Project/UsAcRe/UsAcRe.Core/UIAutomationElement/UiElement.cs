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
		public BoundingRectangleField BoundingRectangle { get; private set; }
		[XmlIgnore]
		public object AutomationElementObj;

		public UiElement(int index, string value, string name, string className, string automationId, int controlTypeId, Rect boundingRectangle) {
			Index = index;

			Value = new ValueField(value);
			Name = new NameField(name);
			ClassName = new ClassNameField(className);
			ControlTypeId = new ControlTypeIdField(controlTypeId);
			AutomationId = new AutomationIdField(automationId);
			BoundingRectangle = new BoundingRectangleField(boundingRectangle);
		}

		public override string ToString() {
			return string.Format("[{0}] \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" {6}", Index, ControlTypeId, Name, ClassName, AutomationId, Value, BoundingRectangle);
		}

		public string ToShortString() {
			var strings = new[] { Value.Value, Name.Value, ClassName.Value };
			return string.Format("\"{0}\" \"{1}\"", ControlTypeId, NamingHelpers.Escape(strings.FirstOrDefault(s => !string.IsNullOrEmpty(s)), 30));
		}

		public Func<string> Differences(UiElement other, ElementCompareParameters parameters, IAutomationElementService automationElementService) {
			if(object.Equals(other, null)) {
				return () => string.Format("other is null");
			}

			var res = ControlTypeId.Differences(other.ControlTypeId, parameters)
				?? Name.Differences(other.Name, parameters)
				?? AutomationId.Differences(other.AutomationId, parameters)
				?? BoundingRectangle.Differences(other.BoundingRectangle, parameters);
			if(res != null) {
				return res;
			}

			if(parameters.CheckByValue) {
				bool empty = string.IsNullOrEmpty(Value.Value);
				bool otherEmpty = string.IsNullOrEmpty(other.Value.Value);
				if(empty && otherEmpty) {
					return null;
				}
				if(empty) {
					automationElementService.RetrieveElementValue(this);
				}
				if(otherEmpty) {
					automationElementService.RetrieveElementValue(other);
				}
				return Value.Differences(other.Value, parameters);
			}

			if(!parameters.AutomationElementInternal) {
				return null;
			}


			bool automationElementEmpty = !automationElementService.TryGetAutomationElement(this, out AutomationElement automationElement);
			bool otherAutomationElementEmpty = !automationElementService.TryGetAutomationElement(other, out AutomationElement otherAutomationElement);
			if(automationElementEmpty != otherAutomationElementEmpty
				&& (automationElementEmpty || otherAutomationElementEmpty)) {
				return () => string.Format("this or other AutomationElement is empty");
			}

			var runtimeId = automationElement.GetRuntimeId();
			var otherRuntimeId = otherAutomationElement.GetRuntimeId();
			if(!runtimeId.SequenceEqual(otherRuntimeId)) {
				return () => string.Format("this.GetRuntimeId() != other.GetRuntimeId() ({0}) != ({1})",
					string.Join(", ", runtimeId), string.Join(", ", otherRuntimeId));
			}

			return null;
		}
	}
}
