using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.UIAutomationElement {
	public class AutomationIdField : IElementField<AutomationIdField> {
		public string Value { get; set; }

		public AutomationIdField(string value) {
			Value = value;
		}

		public void Compare(AutomationIdField other) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				throw new ElementMismatchExceptions(string.Format("left.AutomationId != right.AutomationId ({0}) != ({1})", Value, other.Value));
			}
		}

		public override string ToString() {
			return Value;
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
