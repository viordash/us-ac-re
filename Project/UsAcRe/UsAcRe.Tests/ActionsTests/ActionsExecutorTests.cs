using System.Collections.Generic;
using NUnit.Framework;
using UsAcRe.Actions;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Tests.ActionsTests {
	[TestFixture]
	public class ActionsExecutorTests {

		[Test, Explicit]
		public async void TestMethod1() {

			await ActionsExecutor
				.Perform
				.ElementMatching(new ElementProgram(1, "win32calc.exe"), new List<UiElement>(), 10000)
				.ElementMatching(new ElementProgram(1, "win32calc.exe"), new List<UiElement>(), 10000)
				.MouseClick(MouseActionType.LeftClick, new System.Drawing.Point(2032, 295))
				.ElementMatching(new ElementProgram(1, "win32calc.exe"), new List<UiElement>(), 10000)
				.MouseDrag(MouseActionType.LeftClick, new System.Drawing.Point(2032, 295), new System.Drawing.Point(2032, 200))
				;

		}
	}
}
