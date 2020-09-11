using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UsAcRe.Core.UIAutomationElement {
	[Serializable]
	public class TreeOfSpecificUiElement : List<UiElement> {

		public ElementProgram Program { get; set; }

		public System.Windows.Rect BoundingRectangle {
			get {
				if(Count > 0) {
					return this.First().BoundingRectangle;
				}
				return System.Windows.Rect.Empty;
			}
		}

		public override string ToString() {
			var sb = new StringBuilder();
			int indent = 0;
			foreach(var item in this) {
				sb.AppendFormat("{0}{1}\r\n", new string('\t', indent++), item);
			}

			sb.AppendFormat($"{Program}\r\n");
			return sb.ToString();
		}
	}
}
