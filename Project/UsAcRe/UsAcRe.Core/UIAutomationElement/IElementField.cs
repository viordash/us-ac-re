using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public interface IElementField<in A> {
		void Compare(A other, ElementCompareParameters parameters);
		string ForNew();
	}
}
