using System.Drawing;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public class BoundingRectangleElementHighLighter : ElementHighlighter {

		public static BoundingRectangleElementHighLighter CreateInstance(ElementFromPoint elementFromPoint) {
			return new BoundingRectangleElementHighLighter(elementFromPoint);
		}

		protected ScreenBoundingRectangle rectangle = new ScreenBoundingRectangle();

		public BoundingRectangleElementHighLighter(ElementFromPoint elementFromPoint) : base(elementFromPoint) {
			rectangle.Color = Color.Red;
			rectangle.Opacity = 0.8;
		}

		protected override Rectangle Location {
			get { return rectangle.Location; }
			set { rectangle.Location = value; }
		}

		protected override void SetVisibility(bool show) {
			rectangle.Visible = show;
		}

		protected override void SetToolTip(string toolTipMessage) {

		}

		protected override void OnDispose() {
			rectangle.Dispose();
		}
	}
}
