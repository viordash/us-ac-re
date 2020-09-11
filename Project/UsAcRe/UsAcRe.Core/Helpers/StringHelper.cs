namespace UsAcRe.Core.Helpers {
	public class StringHelper {
		public static bool ImplicitEquals(string text1, string text2) {
			if(string.IsNullOrEmpty(text1)) {
				text1 = string.Empty;
			}
			if(string.IsNullOrEmpty(text2)) {
				text2 = string.Empty;
			}
			return text1 == text2;
		}
	}
}
