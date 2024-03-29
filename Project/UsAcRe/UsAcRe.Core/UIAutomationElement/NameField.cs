﻿using System;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public class NameField : IElementField<NameField> {
		public string Value { get; set; }

		public NameField(string value) {
			Value = value;
		}

		public Func<string> Differences(NameField other, ElementCompareParameters parameters, int attemptNumber) {
			if(!StringHelper.ImplicitEquals(Value, other.Value)) {
				return () => string.Format("this.Name != other.Name ({0}) != ({1})", Value, other.Value);
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
