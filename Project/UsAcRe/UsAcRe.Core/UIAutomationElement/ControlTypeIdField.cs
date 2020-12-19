using System;
using System.Windows.Automation;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public class ControlTypeIdField : IElementField<ControlTypeIdField> {
		public int Value { get; set; }

		public ControlTypeIdField(int value) {
			Value = value;
		}

		public Func<string> Differences(ControlTypeIdField other, ElementCompareParameters parameters) {
			if(Value != other.Value) {
				return () => string.Format("left.ControlTypeId != right.ControlTypeId ({0}) != ({1})", Value, other.Value);
			}
			return null;
		}

		public override string ToString() {
			return ControlType.LookupById(Value).LocalizedControlType;
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
