using System.Drawing;
using System.Windows.Forms;

namespace UsAcRe.Core.UI.Helpers {
	public class WindowsHelper {
		public static void LoadLocation(Point point, System.Windows.Window window) {
			if(!point.IsEmpty
				&& point.X + window.Width >= 0
				&& point.Y + window.Height >= 0
				&& Screen.PrimaryScreen.Bounds.Right > point.X
				&& Screen.PrimaryScreen.Bounds.Bottom > point.Y) {
				window.Left = point.X;
				window.Top = point.Y;
			}
		}

		public static void LoadSize(Size size, System.Windows.Window window) {
			if(!size.IsEmpty
				&& Screen.PrimaryScreen.Bounds.Size.Width > size.Width
				&& Screen.PrimaryScreen.Bounds.Size.Height > size.Height) {
				window.Width = size.Width;
				window.Height = size.Height;
			}
		}
	}
}
