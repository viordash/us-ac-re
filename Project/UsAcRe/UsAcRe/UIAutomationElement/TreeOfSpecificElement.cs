using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UsAcRe.UIAutomationElement {
	[Serializable]
	public class TreeOfSpecificUiElement : List<UiElement> {

		public string ProgramName { get; set; }

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
				sb.AppendFormat($"{new string('\t', indent++)}{item}\r\n");
			}
			return sb.ToString();
		}
	}
}
