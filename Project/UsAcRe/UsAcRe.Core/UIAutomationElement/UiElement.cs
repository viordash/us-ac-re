using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Xml.Serialization;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
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

		public OrderedDifference CreateOrderedDifference(Func<string> difference, ref int weight) {
			weight++;
			if(difference == null) {
				return null;
			}
			return new OrderedDifference(weight, difference);
		}

		public OrderedDifference Differences(UiElement other, ElementCompareParameters parameters, IAutomationElementService automationElementService, int attemptNumber) {
			int weight = 0;
			if(object.Equals(other, null)) {
				return CreateOrderedDifference(() => string.Format("other is null"), ref weight);
			}

			var res = CreateOrderedDifference(ControlTypeId.Differences(other.ControlTypeId, parameters, attemptNumber), ref weight)
				?? CreateOrderedDifference(Name.Differences(other.Name, parameters, attemptNumber), ref weight)
				?? CreateOrderedDifference(ClassName.Differences(other.ClassName, parameters, attemptNumber), ref weight)
				?? CreateOrderedDifference(AutomationId.Differences(other.AutomationId, parameters, attemptNumber), ref weight)
				?? CreateOrderedDifference(BoundingRectangle.Differences(other.BoundingRectangle, parameters, attemptNumber), ref weight);
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
				res = CreateOrderedDifference(Value.Differences(other.Value, parameters, attemptNumber), ref weight);
				if(res != null) {
					return res;
				}
			}

			if(!parameters.AutomationElementInternal) {
				return null;
			}

			bool automationElementEmpty = !automationElementService.TryGetAutomationElement(this, out AutomationElement automationElement);
			bool otherAutomationElementEmpty = !automationElementService.TryGetAutomationElement(other, out AutomationElement otherAutomationElement);
			if(automationElementEmpty || otherAutomationElementEmpty) {
				if(automationElementEmpty != otherAutomationElementEmpty) {
					return CreateOrderedDifference(() => string.Format("this or other AutomationElement is empty"), ref weight);
				}
				return null;
			}

			var runtimeId = automationElement.GetRuntimeId();
			var otherRuntimeId = otherAutomationElement.GetRuntimeId();
			if(!runtimeId.SequenceEqual(otherRuntimeId)) {
				return CreateOrderedDifference(() => string.Format("this.GetRuntimeId() != other.GetRuntimeId() ({0}) != ({1})",
					string.Join(", ", runtimeId), string.Join(", ", otherRuntimeId)), ref weight);
			}
			return null;
		}
	}
}
