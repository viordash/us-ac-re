using System;
using System.Text;

namespace UsAcRe.Helpers {
	public class NamingHelpers {
		public static string Escape(string name, int maxLen) {
			if(string.IsNullOrEmpty(name)) {
				return null;
			}
			var sb = new StringBuilder(name, 0, Math.Min(name.Length, maxLen), 0);
			for(int i = 0; i < sb.Length; i++) {
				var ch = sb[i];
				var i16 = Convert.ToUInt16(ch);
				if(i16 == 0x0D) {
					sb[i] = '\\';
					sb.Insert(i + 1, "r");
				} else if(i16 == 0x0A) {
					sb[i] = '\\';
					sb.Insert(i + 1, "n");
				} else if(i16 == 0x09) {
					sb[i] = '\\';
					sb.Insert(i + 1, "t");
				} else if(i16 < 0x20) {
					sb[i] = '\\';
					sb.Insert(i + 1, string.Format("0x{0:X2}", i16));
				}
			}
			return sb.ToString();
		}
	}
}
