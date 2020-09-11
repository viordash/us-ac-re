using System.Collections.Generic;
using System.Threading.Tasks;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Actions {
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

		public static async Task<BaseAction> MouseClick(this Task<BaseAction> taskPrevAction, MouseButtonType button, System.Drawing.Point clickedPoint, bool doubleClick) {
			var prevAction = await taskPrevAction;
			var action = new MouseClickAction(prevAction, button, clickedPoint, doubleClick);
			await action.ExecuteAsync();
			return action;
		}

		public static async Task<BaseAction> MouseDrag(this Task<BaseAction> taskPrevAction, MouseButtonType button, System.Drawing.Point startCoord,
				System.Drawing.Point endCoord) {
			var prevAction = await taskPrevAction;
			var action = new MouseDragAction(prevAction, button, startCoord, endCoord);
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
