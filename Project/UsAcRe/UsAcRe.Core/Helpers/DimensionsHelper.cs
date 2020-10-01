using System;
using System.Drawing;

namespace UsAcRe.Core.Helpers {
	public class DimensionsHelper {
		public static bool AreLocationEquals(System.Windows.Point point1, System.Windows.Point point2, double tolerance,
			Rectangle workingArea) {
			double xTolerance = 0;
			double yTolerance = 0;
			if(tolerance < 0) {
				return true;
			} else if(tolerance > 0) {
				var screen = workingArea;
				tolerance = 100 / tolerance;
				xTolerance = screen.Width / tolerance;
				yTolerance = screen.Height / tolerance;
			}
			return Math.Abs(point1.X - point2.X) <= xTolerance
				&& Math.Abs(point1.Y - point2.Y) <= yTolerance;
		}

		public static bool AreSizeEquals(System.Windows.Size size1, System.Windows.Size size2, double tolerance) {
			if((size1.Height <= 0 || size1.Width <= 0)
				&& (size2.Height <= 0 || size2.Width <= 0)) {
				return true;
			}
			double heightTolerance = 0;
			double widthTolerance = 0;
			if(tolerance < 0) {
				return true;
			} else if(tolerance > 0) {
				tolerance = 100 / tolerance;
				heightTolerance = Math.Abs(Math.Max(size1.Height, size2.Height)) / tolerance;
				widthTolerance = Math.Abs(Math.Max(size1.Width, size2.Width)) / tolerance;
			}
			return Math.Abs(size1.Height - size2.Height) <= heightTolerance
				&& Math.Abs(size1.Width - size2.Width) <= widthTolerance;
		}
	}
}
