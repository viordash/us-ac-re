using System.Windows.Automation;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;

namespace UsAcRe.Core.UIAutomationElement {
	public class ControlTypeIdField : IElementField<ControlTypeIdField> {
		public int Value { get; set; }

		public ControlTypeIdField(int value) {
			Value = value;
		}

		public void Compare(ControlTypeIdField other) {
			if(Value != other.Value) {
				throw new ElementMismatchExceptions(string.Format("left.ControlTypeId != right.ControlTypeId ({0}) != ({1})", Value, other.Value));
			}
		}

		public override string ToString() {
			return ControlType.LookupById(Value).LocalizedControlType;
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
