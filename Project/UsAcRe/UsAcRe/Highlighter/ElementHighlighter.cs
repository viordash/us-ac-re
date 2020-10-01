using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace UsAcRe.Highlighter {
	public class ElementHighlighter : IDisposable {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		bool isDisposed;

		WpfElementBounding wpfElementBounding;
		WpfElementToolTip wpfElementToolTip;

		public ElementHighlighter(Rect boundingRectangle, string toolTip)
			: this(boundingRectangle, toolTip, 2, 0.6, Colors.Black, Colors.Orange) {
		}

		public ElementHighlighter(Rect boundingRectangle)
			: this(boundingRectangle, null, 4, 0.6, Colors.Red, Colors.DarkRed) {
		}

		public ElementHighlighter(Rect boundingRectangle, string toolTip, int boundingThickness, double opacity, Color innerColor, Color outerColor) {
			wpfElementBounding = new WpfElementBounding(boundingThickness, opacity, innerColor, outerColor, boundingRectangle);

			wpfElementToolTip = !string.IsNullOrEmpty(toolTip)
				? new WpfElementToolTip(toolTip, opacity * 1.2, innerColor, outerColor, GetTooltipLocation(boundingRectangle, GetTooltipSize(toolTip)))
				: null;
		}

		Size GetTooltipSize(string toolTip) {
			var font = new System.Drawing.Font("Segoe UI", 9.0F);
			var toolTipSize = TextRenderer.MeasureText(toolTip, font);
			return new Size(toolTipSize.Width, toolTipSize.Height);
		}

		Point GetTooltipLocation(Rect boundingRectangle, Size tooltipSize) {
			var toolTipLocation = new Point();

			if(boundingRectangle.Y < Screen.PrimaryScreen.Bounds.Height / 4) {
				toolTipLocation.Y = boundingRectangle.Bottom + tooltipSize.Height * 1;
			} else {
				toolTipLocation.Y = boundingRectangle.Top - tooltipSize.Height * 2;
			}

			if(boundingRectangle.X < Screen.PrimaryScreen.Bounds.Width / 4) {
				toolTipLocation.X = boundingRectangle.Right + (tooltipSize.Width * 0.02);
			} else {
				toolTipLocation.X = boundingRectangle.Left - (tooltipSize.Width * 0.9);
			}
			return toolTipLocation;
		}

		public void StartHighlighting() {
			wpfElementBounding.Show();
			wpfElementToolTip?.Show();
		}

		public void StopHighlighting() {
			wpfElementBounding.Hide();
			wpfElementToolTip?.Hide();
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
