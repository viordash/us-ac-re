﻿using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.MouseProcess;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class MouseActionTests : BaseActionTestable {

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		[Test]
		public void ExecuteAsScriptSource_Test() {
			var action = new MouseClickAction(null, MouseButtonType.Left, new System.Drawing.Point(1, 2), false);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "MouseClick(MouseButtonType.Left, new System.Drawing.Point(1, 2), false)");
		}
	}
}
