using System;
using System.Drawing;
using System.Windows;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public class ElementHighlighter : IDisposable {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		readonly ElementFromPoint elementFromPoint;
		bool isHighlighting;
		bool isDisposed;

        WpfElementBounding wpfElementBounding;

		public ElementHighlighter(ElementFromPoint elementFromPoint) {
			this.elementFromPoint = elementFromPoint;

            //creating wpf element bounding with thickness and opacity
            wpfElementBounding = new WpfElementBounding(1, 0.6);
        }

		public void StartHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(!isHighlighting) {
                var rectangle = elementFromPoint.BoundingRectangle;
				if(rectangle == Rect.Empty) {
                    wpfElementBounding.SetVisibility(false);
					return;
				}
                wpfElementBounding.Location = new Rect((int)rectangle.Left, (int)rectangle.Top, (int)rectangle.Width, (int)rectangle.Height);
                wpfElementBounding.SetToolTip(elementFromPoint.ToString());
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
