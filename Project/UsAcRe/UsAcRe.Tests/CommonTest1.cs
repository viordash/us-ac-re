using System;
using NUnit.Framework;

namespace UsAcRe.Tests.UIAutomationElement {
	[TestFixture]
	public class CommonTest1 {
		[Test]
		public void TestInterpolation() {
			var arg1 = "{test1}";
			var arg2 = "{test2}";
			string res1 = string.Empty;
			string res2 = string.Empty;
			Assert.Throws<FormatException>(() => { res1 = string.Format($"{arg1} {arg2}"); });
			Assert.DoesNotThrow(() => { res2 = string.Format("{0} {1}", arg1, arg2); });
			Assert.IsEmpty(res1);
			Assert.IsNotEmpty(res2);
		}
	}
}
