using System.Linq;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UI.Actions;

namespace UsAcRe.Core.Tests.MouseProcessTests {
	[TestFixture]
	public class ActionPresentationTests {

		[Test]
		public void Retrieve_BackgroundColor_Test() {
			var baseActionAssembly = typeof(BaseAction).Assembly;
			var actionsTypes = baseActionAssembly.GetTypes()
				.Where(t => t.BaseType == (typeof(BaseAction)));
			foreach(var type in actionsTypes) {
				var color = ActionPresentation.BackgroundColor[typeof(DelayAction)];
				Assert.That(color, Is.Not.EqualTo(default(System.Drawing.Color)));
			}
		}
	}
}
