using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UsAcRe.Core.Actions;
using UsAcRe.Core.UI.Actions;

namespace UsAcRe.Core.Tests.MouseProcessTests {
	[TestFixture]
	public class ActionPresentationTests {

		[Test]
		public void BackgroundColor_Is_Exists_And_Unique_Test() {
			var baseActionAssembly = typeof(BaseAction).Assembly;
			var actionsTypes = baseActionAssembly.GetTypes()
				.Where(t => t.BaseType == (typeof(BaseAction)));
			var colors = new List<System.Drawing.Color>();
			foreach(var type in actionsTypes) {
				var color = ActionPresentation.BackgroundColor[type];
				Assert.That(color, Is.Not.EqualTo(default(System.Drawing.Color)));
				Assert.That(colors, Has.No.Member(color));
				colors.Add(color);
			}
		}
	}
}
