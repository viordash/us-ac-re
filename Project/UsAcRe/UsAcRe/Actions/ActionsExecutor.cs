using System.Collections.Generic;
using System.Threading.Tasks;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Actions {
	public static class ActionsExecutor {
		public static Task<BaseAction> Perform {
			get {
				return Task.FromResult<BaseAction>(null);
			}
		}

		public static async Task<BaseAction> ElementMatching(this Task<BaseAction> taskPrevAction, ElementProgram program, List<UiElement> searchPath, int timeoutMs = 20 * 1000) {
			var prevAction = await taskPrevAction;
			var action = new ElementMatchAction(prevAction, program, searchPath, timeoutMs);
			await action.ExecuteAsync();
			return action;
		}

		public static async Task<BaseAction> Mouse(this Task<BaseAction> taskPrevAction, MouseActionType type, System.Drawing.Point downClickedPoint,
			System.Drawing.Point upClickedPoint = default) {
			var prevAction = await taskPrevAction;
			var action = new MouseAction(prevAction, type, downClickedPoint, upClickedPoint);
			await action.ExecuteAsync();
			return action;
		}

		public static async Task<BaseAction> Keyboard(this Task<BaseAction> taskPrevAction, VirtualKeyCodes vKCode, bool isUp) {
			var prevAction = await taskPrevAction;
			var action = new KeybdAction(prevAction, vKCode, isUp);
			await action.ExecuteAsync();
			return action;
		}

		public static async Task<BaseAction> TextType(this Task<BaseAction> taskPrevAction, string text) {
			var prevAction = await taskPrevAction;
			var action = new TextTypingAction(prevAction, text);
			await action.ExecuteAsync();
			return action;
		}
	}

}
