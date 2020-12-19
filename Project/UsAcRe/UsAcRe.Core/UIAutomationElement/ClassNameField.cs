using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.UIAutomationElement {
	public class ClassNameField : IElementField<ClassNameField> {
		public string Value { get; set; }

		public ClassNameField(string value) {
			Value = value;
		}

		public void Compare(ClassNameField other) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				throw new ElementMismatchExceptions(string.Format("left.ClassName != right.ClassName ({0}) != ({1})", Value, other.Value));
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
