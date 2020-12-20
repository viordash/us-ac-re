using System;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public class AutomationIdField : IElementField<AutomationIdField> {
		public string Value { get; set; }

		public AutomationIdField(string value) {
			Value = value;
		}

		public Func<string> Differences(AutomationIdField other, ElementCompareParameters parameters) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				return () => string.Format("this.AutomationId != other.AutomationId ({0}) != ({1})", Value, other.Value);
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
