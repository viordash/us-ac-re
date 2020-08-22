using System;
using System.Windows;
using System.Windows.Media;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public class ElementHighlighter : IDisposable {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		readonly Rect boundingRectangle;
		readonly string toolTip;
		bool isHighlighting;
		bool isDisposed;

		WpfElementBounding wpfElementBounding;

		public ElementHighlighter(ElementFromPoint elementFromPoint)
			: this(elementFromPoint.TreeOfSpecificUiElement.BoundingRectangle, null, 1, 0.6, Colors.Yellow, Colors.Red) {
		}

		public ElementHighlighter(Rect boundingRectangle, string toolTip)
			: this(boundingRectangle, toolTip, 2, 0.6, Colors.Green, Colors.LightGreen) {
		}

		public ElementHighlighter(Rect boundingRectangle)
			: this(boundingRectangle, null, 4, 0.6, Colors.Red, Colors.DarkRed) {
		}

		public ElementHighlighter(Rect boundingRectangle, string toolTip, int boundingThickness, double opacity, Color innerColor, Color outerColor) {
			this.boundingRectangle = boundingRectangle;
			this.toolTip = toolTip;
			wpfElementBounding = new WpfElementBounding(boundingThickness, opacity, innerColor, outerColor);
		}

		public void StartHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(!isHighlighting) {
				var rectangle = boundingRectangle;
				if(rectangle == Rect.Empty) {
					wpfElementBounding.SetVisibility(false);
					return;
				}
				wpfElementBounding.BoundRect = rectangle;
				wpfElementBounding.SetToolTip(toolTip);
				wpfElementBounding.SetVisibility(true);

				isHighlighting = true;
				wpfElementBounding.Show();
			}
		}

		public void StopHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(isHighlighting) {
				wpfElementBounding.SetVisibility(false);
				isHighlighting = false;
			}
		}

		public void Dispose() {
			if(!isDisposed) {
				StopHighlighting();
				wpfElementBounding.OnDispose();
				isDisposed = true;
			}
		}
	}
}
