using System;
using System.Drawing;

namespace UsAcRe.Helpers {
	public class DimensionsHelper {
		public static bool IsClickPointInSamePosition(Point point1, Point point2, double tolerance) {
			double xTolerance = 0;
			double yTolerance = 0;
			if(tolerance < 0) {
				return true;
			} else if(tolerance > 0) {
				if(point1.IsEmpty || point2.IsEmpty) {
					return false;
				}
				tolerance = 100 / tolerance;

				xTolerance = Math.Max(point1.X, point2.X) / tolerance;
				yTolerance = Math.Max(point1.Y, point2.Y) / tolerance;
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
				heightTolerance = Math.Max((int)size1.Height, size2.Height) / tolerance;
				widthTolerance = Math.Max((int)size1.Width, size2.Width) / tolerance;
			}
			return Math.Abs((int)size1.Height - size2.Height) <= heightTolerance
				&& Math.Abs((int)size1.Width - size2.Width) <= widthTolerance;
		}
	}
}
