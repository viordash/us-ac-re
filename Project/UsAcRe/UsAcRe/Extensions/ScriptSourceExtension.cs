using System;

namespace UsAcRe.Extensions {
	public static class ScriptSourceExtension {
		public static string ForUsings(this Enum val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}
		public static string ForUsings(this System.Drawing.Point val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}

		public static string ForNew(this Enum val) {
			return string.Format("{0}.{1}", val.GetType().Name, val);
		}
		public static string ForNew(this System.Drawing.Point val) {
			return string.Format("new {0}({1}, {2})", val.GetType().Name, val.X, val.Y);
		}
	}
}
