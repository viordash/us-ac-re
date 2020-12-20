using System;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public interface IElementField<in A> {
		Func<string> Differences(A other, ElementCompareParameters parameters);
		string ForNew();
	}
}
