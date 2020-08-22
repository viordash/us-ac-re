using System;
using System.Windows;
using System.Windows.Media;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public class ElementHighlighter : IDisposable {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		readonly string toolTip;
		bool isHighlighting;
		bool isDisposed;

		WpfElementBounding wpfElementBounding;
		WpfElementToolTip wpfElementToolTip;

		public ElementHighlighter(ElementFromPoint elementFromPoint)
			: this(elementFromPoint.TreeOfSpecificUiElement.BoundingRectangle, null, 1, 0.6, Colors.Yellow, Colors.Red) {
		}

		public ElementHighlighter(Rect boundingRectangle, string toolTip)
			: this(boundingRectangle, toolTip, 2, 0.6, Colors.LightBlue, Colors.LightGreen) {
		}

		public ElementHighlighter(Rect boundingRectangle)
			: this(boundingRectangle, null, 4, 0.6, Colors.Red, Colors.DarkRed) {
		}

		public ElementHighlighter(Rect boundingRectangle, string toolTip, int boundingThickness, double opacity, Color innerColor, Color outerColor) {
			this.toolTip = toolTip;
			wpfElementBounding = new WpfElementBounding(boundingThickness, opacity, innerColor, outerColor, boundingRectangle);

			var toolTipLocation = boundingRectangle.BottomLeft;
			toolTipLocation.Offset(0, boundingRectangle.Height * 0.2);
			wpfElementToolTip = !string.IsNullOrEmpty(toolTip)
				? new WpfElementToolTip(toolTip, opacity * 1.2, outerColor, toolTipLocation)
				: null;
		}

		public void StartHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(!isHighlighting) {
				isHighlighting = true;
				wpfElementBounding.Show();
				wpfElementToolTip?.Show();
			}
		}

		public void StopHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(isHighlighting) {
				wpfElementBounding.Hide();
				wpfElementToolTip?.Hide();
				isHighlighting = false;
			}
		}

		public void Dispose() {
			if(!isDisposed) {
				StopHighlighting();
				wpfElementBounding.OnDispose();
				wpfElementToolTip?.OnDispose();
				isDisposed = true;
			}
		}
	}
}
