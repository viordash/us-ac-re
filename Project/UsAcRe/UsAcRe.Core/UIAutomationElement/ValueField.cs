using System;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public class ValueField : IElementField<ValueField> {
		public string Value { get; set; }

		public ValueField(string value) {
			Value = value;
		}

		public Func<string> Differences(ValueField other, ElementCompareParameters parameters) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				return () => string.Format("this.Value != other.Value ({0}) != ({1})", Value, other.Value);
			}
			return null;
		}

		public override string ToString() {
			return Value;
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
