using System.Windows;
using System.Windows.Media;

namespace UsAcRe.Core.UI.Highlighter {
	/// <summary>
	/// Interaction logic for WpfElementBounding.xaml
	/// </summary>
	public partial class WpfElementBounding : Window {
		private int boundingThickness = 1;

		public WpfElementBounding(int boundingThickness, double opacity, Color innerColor, Color outerColor, Rect boundRect) {
			InitializeComponent();
			this.boundingThickness = boundingThickness;
			outerBounding.BorderThickness = new Thickness(boundingThickness);
			innerBounding.BorderThickness = new Thickness(boundingThickness);
			outerBounding.BorderBrush = new SolidColorBrush(innerColor);
			innerBounding.BorderBrush = new SolidColorBrush(outerColor);
			Opacity = opacity;

			Left = boundRect.Left - (2 * boundingThickness);
			Width = boundRect.Width + (4 * boundingThickness);
			Top = boundRect.Top - (2 * boundingThickness);
			Height = boundRect.Height + (4 * boundingThickness);
		}

		public void OnDispose() {
			Close();
		}
	}
}
