﻿using NUnit.Framework;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Player.Actions;

namespace UsAcRe.Player.Tests.ActionsTests {
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
			var action = MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(1, 2), false);
			var sourcePresentation = action.ExecuteAsScriptSource();
			Assert.AreEqual(sourcePresentation, "MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(1, 2), false)");
		}
	}
}