using System.Windows;
using System.Windows.Media;

namespace UsAcRe {
	/// <summary>
	/// Interaction logic for WpfElementBounding.xaml
	/// </summary>
	public partial class WpfElementBounding : Window {
		private int boundingThickness = 1;
		private Rect boundRect;

		public WpfElementBounding(int boundingThickness, double opacity, Color innerColor, Color outerColor) {
			InitializeComponent();
			this.boundingThickness = boundingThickness;
			outerBounding.BorderThickness = new Thickness(boundingThickness);
			innerBounding.BorderThickness = new Thickness(boundingThickness);
			outerBounding.BorderBrush = new SolidColorBrush(innerColor);
			innerBounding.BorderBrush = new SolidColorBrush(outerColor);
			Opacity = opacity;
		}

		public Rect BoundRect {
			get { return boundRect; }
			set {
				boundRect = value;
				Left = boundRect.Left - (2 * boundingThickness);
				Width = boundRect.Width + (4 * boundingThickness);
				Top = boundRect.Top - (2 * boundingThickness);
				Height = boundRect.Height + (4 * boundingThickness);
			}
		}

		public void SetToolTip(string toolTipMessage) {
		}

		public void SetVisibility(bool show) {
			if(show) {
				Show();
			} else {
				Hide();
			}
		}

		public void OnDispose() {
			Close();
		}
	}
}
