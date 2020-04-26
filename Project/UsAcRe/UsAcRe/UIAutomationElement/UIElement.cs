using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Xml.Serialization;
using UsAcRe.Extensions;
using UsAcRe.Scripts;

namespace UsAcRe.UIAutomationElement {
	[Serializable]
	public class UiElement : IScriptUsings {
		public int Index;
		public string Value;
		public string Name;
		public string AutomationId;
		public int ControlTypeId;
		public Rect BoundingRectangle;
		[XmlIgnore]
		public object AutomationElementObj;

		public UiElement() { }

		public UiElement(int index, string value, string name, string automationId, int controlTypeId, Rect boundingRectangle) {
			Index = index;
			Value = value;
			Name = name;
			AutomationId = automationId;
			ControlTypeId = controlTypeId;
			BoundingRectangle = boundingRectangle;
		}

		public override string ToString() {
			return string.Format("[{0}] \"{1}\" \"{2}\" \"{3}\" \"{4}\"", Index, ControlType.LookupById(ControlTypeId).LocalizedControlType,
				Name?.Substring(0, Math.Min(Name.Length, 60)), AutomationId, Value);
		}

		public List<string> UsingsForScriptSource() {
			return new List<string>() {
				Index.ForUsings(),
				Value.ForUsings(),
				Name.ForUsings(),
				AutomationId.ForUsings(),
				ControlTypeId.ForUsings(),
				BoundingRectangle.ForUsings(),
			};
		}
	}
}
