using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsAcRe.Exceptions;
using UsAcRe.Scripts;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Extensions {
	public static class ScriptSourceExtension {
		public static string ForNew(this int val) {
			return string.Format("{0}", val);
		}
		public static string ForNew(this uint val) {
			return string.Format("{0}", val);
		}
		public static string ForNew(this bool val) {
			return string.Format("{0:L}", val).ToLower();
		}
		public static string ForNew(this Enum val) {
			return string.Format("{0}.{1}", val.GetType().Name, val);
		}
		public static string ForNew(this System.Drawing.Point val) {
			var type = val.GetType();
			return string.Format("new {0}.{1}({2}, {3})", type.Namespace, type.Name, val.X, val.Y);
		}
		public static string ForNew(this string val) {
			return string.Format("\"{0}\"", val);
		}
		public static string ForNew(this ElementProgram val) {
			return string.Format("new {0}({1}, {2})", val.GetType().Name, val.Index.ForNew(), val.FileName.ForNew());
		}

		public static string ForNew(this System.Windows.Rect val) {
			var type = val.GetType();
			if(val.IsEmpty) {
				return string.Format("new {0}.{1}()", type.Namespace, type.Name);
			} else {
				return string.Format("new {0}.{1}({2}, {3}, {4}, {5})", type.Namespace, type.Name, val.X, val.Y, val.Width, val.Height);
			}
		}
		public static string ForNew(this UiElement val) {
			return string.Format("new {0}({1}, {2}, {3}, {4}, {5}, {6}, {7})", val.GetType().Name, val.Index.ForNew(), val.Value.ForNew(), val.Name.ForNew(), val.ClassName.ForNew(),
				val.AutomationId.ForNew(), val.ControlTypeId.ForNew(), val.BoundingRectangle.ForNew());
		}
		public static string ForNew(this List<UiElement> val) {
			var type = val.GetType();
			while(type.GenericTypeArguments.Length == 0) {
				type = type.BaseType;
				if(type == null) {
					throw new ScriptComposeException();
				}
			}
			var sb = new StringBuilder();
			sb.AppendFormat("new List<{0}>() {{", type.GenericTypeArguments[0].Name);
			sb.AppendLine();
			foreach(var item in val.AsEnumerable()) {
				sb.AppendFormat("{0}{1},{2}", ScriptBuilder.tab, item.ForNew(), ScriptBuilder.newLine);
			}
			sb.Append("}");
			return sb.ToString();
		}
	}
}
