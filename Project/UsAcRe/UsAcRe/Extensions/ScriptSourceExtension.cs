using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Extensions {
	public static class ScriptSourceExtension {
		public static string ForUsings(this int val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}
		public static string ForUsings(this Enum val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}
		public static string ForUsings(this System.Drawing.Point val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}
		public static string ForUsings(this string val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}
		public static string ForUsings<T>(this List<T> val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}
		public static string ForUsings(this System.Windows.Rect val) {
			return string.Format("using {0};", val.GetType().Namespace);
		}

		public static string ForNew(this int val) {
			return string.Format("{0}", val);
		}
		public static string ForNew(this Enum val) {
			return string.Format("{0}.{1}", val.GetType().Name, val);
		}
		public static string ForNew(this System.Drawing.Point val) {
			return string.Format("new {0}({1}, {2})", val.GetType().Name, val.X, val.Y);
		}
		public static string ForNew(this string val) {
			return string.Format("\"{0}\"", val);
		}

		public static string ForNew(this System.Windows.Rect val) {
			if(val.IsEmpty) {
				return string.Format("new {0}()", val.GetType().Name);
			} else {
				return string.Format("new {0}({1}, {2}, {3}, {4})", val.GetType().Name, val.X, val.Y, val.Width, val.Height);
			}
		}
		public static string ForNew(this UiElement val) {
			return string.Format("new {0}({1}, {2}, {3}, {4}, {5}, {6})", val.GetType().Name, val.Index.ForNew(), val.Value.ForNew(), val.Name.ForNew(), val.AutomationId.ForNew(), 
				val.ControlTypeId.ForNew(), val.BoundingRectangle.ForNew());
		}
		public static string ForNew(this List<UiElement> val) {
			var sb = new StringBuilder();
			sb.AppendFormat("new List<{0}>() {{", val.GetType().GenericTypeArguments[0].Name);
			sb.AppendLine();
			foreach(var item in val.AsEnumerable().Reverse()) {
				sb.AppendFormat("  {0},", item.ForNew());
				sb.AppendLine();
			}
			sb.Append("}");
			return sb.ToString();
		}
	}
}
