namespace UsAcRe.Core.UIAutomationElement {
	public interface IElementField<in A> {
		void Compare(A other);
		string ForNew();
	}
}
