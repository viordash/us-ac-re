using System;

namespace UsAcRe.Extensions {
	public static class StringExtension {
		public static string MaxLength(this string input, int length) {
			if(string.IsNullOrEmpty(input)) {
				return null;
			}
			return input.Substring(0, Math.Min(length, input.Length));
		}
	}
}
