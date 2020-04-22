using System.Windows;
using System.Windows.Media;

namespace UsAcRe {
	/// <summary>
	/// Interaction logic for WpfElementBounding.xaml
	/// </summary>
	public partial class WpfElementBounding : Window {
		private int boundingThickness = 1;
		private Rect location;

		public WpfElementBounding(int boundingThickness, double opacity, Color innerColor, Color outerColor) {
			InitializeComponent();
			this.boundingThickness = boundingThickness;
			outerBounding.BorderThickness = new Thickness(boundingThickness);
			innerBounding.BorderThickness = new Thickness(boundingThickness);
			outerBounding.BorderBrush = new SolidColorBrush(innerColor );
			innerBounding.BorderBrush = new SolidColorBrush(outerColor);
			Opacity = opacity;
		}

		public Rect Location {
			get { return location; }
			set {
				location = value;
				Left = location.Left - (2 * boundingThickness);
				Width = location.Width + (4 * boundingThickness);
				Top = location.Top - (2 * boundingThickness);
				Height = location.Height + (4 * boundingThickness);
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
