using System;
using System.Drawing;

namespace UsAcRe.Core.Extensions {
	public static class PointExtension {
		public static bool WithBoundaries(this Point point, Point other, int tolerance) {
			return Math.Abs(point.X - other.X) <= tolerance && Math.Abs(point.Y - other.Y) <= tolerance;
		}
		public static bool WithBoundaries(this Point point, int otherX, int otherY, int toleranceX, int toleranceY) {
			return Math.Abs(point.X - otherX) <= toleranceX && Math.Abs(point.Y - otherY) <= toleranceY;
		}
		public static bool WithBoundaries(this Point point, int otherX, int otherY, int tolerance) {
			return Math.Abs(point.X - otherX) <= tolerance && Math.Abs(point.Y - otherY) <= tolerance;
		}
	}
}
