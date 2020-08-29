using System.Collections.Generic;
using System.Threading.Tasks;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Actions {
	public static class ActionsExecutor {
		public static Task<BaseAction> Perform {
			get {
				return Task.FromResult<BaseAction>(null);
			}
		}

		public static async Task<BaseAction> ElementMatching(this Task<BaseAction> taskPrevAction, ElementProgram program, List<UiElement> searchPath, int timeoutMs = 20 * 1000) {
			var prevAction = await taskPrevAction;
			var action = new ElementMatchAction(prevAction, new ElementProgram(1, "win32calc.exe"), new List<UiElement>(), 20000);
			await action.ExecuteAsync();
			return action;
		}

		public static async Task<BaseAction> MouseDrag(this Task<BaseAction> taskPrevAction, MouseActionType type, System.Drawing.Point downClickedPoint,
			System.Drawing.Point upClickedPoint) {
			var prevAction = await taskPrevAction;
			var action = new MouseAction(prevAction, type, downClickedPoint, upClickedPoint);
			await action.ExecuteAsync();
			return action;
		}

		public static async Task<BaseAction> MouseClick(this Task<BaseAction> taskPrevAction, MouseActionType type, System.Drawing.Point downClickedPoint) {
			var prevAction = await taskPrevAction;
			var action = new MouseAction(prevAction, type, downClickedPoint);
			await action.ExecuteAsync();
			return action;
		}


	}

	public static class BaseActionExtensions {

	}

}
