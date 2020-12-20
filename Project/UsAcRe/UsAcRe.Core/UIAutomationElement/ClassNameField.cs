using System;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public class ClassNameField : IElementField<ClassNameField> {
		public string Value { get; set; }

		public ClassNameField(string value) {
			Value = value;
		}

		public Func<string> Differences(ClassNameField other, ElementCompareParameters parameters) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				return () => string.Format("this.ClassName != other.ClassName ({0}) != ({1})", Value, other.Value);
			}
			return null;
		}

		public override string ToString() {
			return NamingHelpers.Escape(Value, 60);
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
