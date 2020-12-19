using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.UIAutomationElement {
	public class NameField : IElementField<NameField> {
		public string Value { get; set; }

		public void Compare(NameField other) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				throw new ElementMismatchExceptions(string.Format("left.Name != right.Name ({0}) != ({1})", Value, other.Value));
			}
		}

		public override string ToString() {
			return NamingHelpers.Escape(Value, 60);
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
