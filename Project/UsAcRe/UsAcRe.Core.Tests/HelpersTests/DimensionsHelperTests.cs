using NUnit.Framework;
using UsAcRe.Core.Helpers;

namespace UsAcRe.Core.Tests.MouseProcessTests {
	[TestFixture]
	public class DimensionsHelperTests {

		[Test]
		public void AreLocationEquals_Negative_Tests() {
			var point1 = new System.Windows.Point(1226, -142);
			var point2 = new System.Windows.Point(1226, -142);
			var tolerance = 50.0;
			Assert.That(DimensionsHelper.AreLocationEquals(point1, point2, tolerance));
		}

		[Test]
		public void AreSizeEquals_Tests() {
			var size1 = new System.Windows.Size (100, 142);
			var size2 = new System.Windows.Size(105, 152);
			var tolerance = 50.0;
			Assert.That(DimensionsHelper.AreSizeEquals(size1, size2, tolerance));
		}
	}
}
