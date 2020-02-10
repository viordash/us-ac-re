using System;
using System.Drawing;
using System.Windows;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public abstract class ElementHighlighter : IDisposable {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		readonly ElementFromPoint elementFromPoint;
		bool isHighlighting;
		bool isDisposed;

		public ElementHighlighter(ElementFromPoint elementFromPoint) {
			this.elementFromPoint = elementFromPoint;
		}

		public void StartHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(!isHighlighting) {
				var rectangle = elementFromPoint.BoundingRectangle;
				if(rectangle == Rect.Empty) {
					SetVisibility(false);
					return;
				}
				Location = new Rectangle((int)rectangle.Left, (int)rectangle.Top, (int)rectangle.Width, (int)rectangle.Height);
				SetToolTip();
				SetVisibility(true);
				isHighlighting = true;
			}
		}

		public void StopHighlighting() {
			if(isDisposed) {
				throw new ObjectDisposedException(nameof(ElementHighlighter));
			}

			if(isHighlighting) {
				SetVisibility(false);
				isHighlighting = false;
			}
		}

		public void Dispose() {
			if(!isDisposed) {
				StopHighlighting();
				OnDispose();
				isDisposed = true;
			}
		}

		abstract protected Rectangle Location { set; }
		abstract protected void SetVisibility(bool show);
		abstract protected void SetToolTip(string toolTipMessage);
		abstract protected void OnDispose();

		private void SetToolTip() {
			SetToolTip(elementFromPoint.ToString());
		}
	}
}
