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

		public Func<string> Differences(ControlTypeIdField other, ElementCompareParameters parameters, int attemptNumber) {
			if(Value != other.Value) {
				return () => string.Format("this.ControlTypeId != other.ControlTypeId ({0}) != ({1})", Value, other.Value);
			}
			return null;
		}

		public override string ToString() {
			return string.Format("(0x{0:X4}){1}", Value, ControlType.LookupById(Value).LocalizedControlType);
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
