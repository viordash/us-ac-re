using System.Windows;
using System.Windows.Media;

namespace UsAcRe.Core.UI.Highlighter {
	/// <summary>
	/// Interaction logic for WpfElementToolTip.xaml
	/// </summary>
	public partial class WpfElementToolTip : Window {

		public WpfElementToolTip(string toolTip, double opacity, Color foregroundColor, Color backgroundColor, Point location) {
			InitializeComponent();
			lbToolTip.Content = toolTip;
			Opacity = opacity;
			Left = location.X;
			Top = location.Y;
			lbToolTip.Foreground = new SolidColorBrush(foregroundColor);
			Background = new SolidColorBrush(backgroundColor);
		}

		public void OnDispose() {
			Close();
		}
	}
}
