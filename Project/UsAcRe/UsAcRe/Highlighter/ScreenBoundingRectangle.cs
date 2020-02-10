using System;
using System.Drawing;

namespace UsAcRe.Highlighter {
	public class ScreenBoundingRectangle : IDisposable {
		const int width = 2;
		private bool visible;
		private Color color;
		private Rectangle location;

		private string tooltipText;

		private ScreenRectangle leftRectangle, bottomRectangle, rightRectangle, topRectangle;
		private ScreenRectangle[] rectangles;

		public ScreenBoundingRectangle() {
			leftRectangle = new ScreenRectangle();
			topRectangle = new ScreenRectangle();
			rightRectangle = new ScreenRectangle();
			bottomRectangle = new ScreenRectangle();

			rectangles = new ScreenRectangle[] { leftRectangle, topRectangle, rightRectangle, bottomRectangle };
		}

		public bool Visible {
			get { return visible; }
			set {
				visible = leftRectangle.Visible = rightRectangle.Visible = topRectangle.Visible = bottomRectangle.Visible = value;
			}
		}

		public Color Color {
			get { return color; }
			set {
				color = leftRectangle.Color = rightRectangle.Color = topRectangle.Color = bottomRectangle.Color = value;
			}
		}

		public double Opacity {
			get { return leftRectangle.Opacity; }
			set {
				leftRectangle.Opacity = rightRectangle.Opacity = topRectangle.Opacity = bottomRectangle.Opacity = value;
			}
		}

		public int LineWidth {
			get { return width; }
		}

		public Rectangle Location {
			get { return location; }
			set {
				location = value;
				Layout();
			}
		}


		public string ToolTipText {
			get { return tooltipText; }
			set { tooltipText = value; }
		}

		private void Layout() {
			leftRectangle.Location = new Rectangle(location.Left - width, location.Top, width, location.Height);
			topRectangle.Location = new Rectangle(location.Left - width, location.Top - width, location.Width + (2 * width), width);
			rightRectangle.Location = new Rectangle(location.Left + location.Width, location.Top, width, location.Height);
			bottomRectangle.Location = new Rectangle(location.Left - width, location.Top + location.Height, location.Width + (2 * width), width);
		}

		#region IDisposable Members

		public void Dispose() {
			foreach(ScreenRectangle rectangle in rectangles) {
				rectangle.Dispose();
			}
		}

		#endregion
	}
}
