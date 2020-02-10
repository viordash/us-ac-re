using System.Drawing;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public class BoundingRectangleElementHighLighter : ElementHighlighter {

		public static BoundingRectangleElementHighLighter CreateInstance(ElementFromPoint elementFromPoint) {
			return new BoundingRectangleElementHighLighter(elementFromPoint);
		}

		ScreenBoundingRectangle innerRectangle = new ScreenBoundingRectangle();
		ScreenBoundingRectangle outerRectangle = new ScreenBoundingRectangle();

		public BoundingRectangleElementHighLighter(ElementFromPoint elementFromPoint) : base(elementFromPoint) {
			innerRectangle.Color = Color.Red;
			outerRectangle.Color = Color.Yellow;
			innerRectangle.Opacity = 0.6;
			outerRectangle.Opacity = 0.6;
		}

		protected override Rectangle Location {
			set {
				innerRectangle.Location = value;
				var outerRect = value;
				outerRect.Inflate(new Size(innerRectangle.LineWidth, innerRectangle.LineWidth));
				outerRectangle.Location = outerRect;

			}
		}

		protected override void SetVisibility(bool show) {
			innerRectangle.Visible = show;
			outerRectangle.Visible = show;
		}

		protected override void SetToolTip(string toolTipMessage) {

		}

		protected override void OnDispose() {
			innerRectangle.Dispose();
			outerRectangle.Dispose();
		}
	}
}
