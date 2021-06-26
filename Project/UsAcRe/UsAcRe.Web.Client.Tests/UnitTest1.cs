using NUnit.Framework;
using Radzen;

namespace UsAcRe.Web.Client.Tests {
	public class Tests {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void Test1() {
			var loadDataArgs = new LoadDataArgs() { };
			Assert.That(loadDataArgs, Is.Not.Null);
		}
	}
}