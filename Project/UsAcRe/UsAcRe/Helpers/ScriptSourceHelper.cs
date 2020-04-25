using System;

namespace UsAcRe.Helpers {
	public class ScriptSourceHelper {
		public static string ToNew(Enum enumVal) {
			return string.Format("{0}.{1}", enumVal.GetType().FullName, enumVal);
		}
		public static string ToNew(System.Drawing.Point point) {
			return string.Format("new {0}({1}, {2})", point.GetType().FullName, point.X, point.Y);
		}
	}
}
