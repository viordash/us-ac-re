using System.Windows;
using System.Windows.Media;

namespace UsAcRe {
	/// <summary>
	/// Interaction logic for WpfElementToolTip.xaml
	/// </summary>
	public partial class WpfElementToolTip : Window {

		public WpfElementToolTip(string toolTip, double opacity, Color backgroundColor, Point location) {
			InitializeComponent();
			lbToolTip.Content = toolTip;
			Opacity = opacity;
			Left = location.X;
			Top = location.Y;
			lbToolTip.Foreground = new SolidColorBrush(Color.FromArgb(backgroundColor.A, (byte)~backgroundColor.R, (byte)~backgroundColor.G, (byte)~backgroundColor.B));
			Background = new SolidColorBrush(backgroundColor);
		}

		public void OnDispose() {
			Close();
		}
	}
}
