using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.UIAutomationElement {
	public class ValueField : IElementField<ValueField> {
		public string Value { get; set; }

		public void Compare(ValueField other) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				throw new ElementMismatchExceptions(string.Format("left.Value != right.Value ({0}) != ({1})", Value, other.Value));
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
