using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGuard;
using UsAcRe.Actions;
using UsAcRe.Core.Extensions;
using UsAcRe.Services;

namespace UsAcRe.Scripts {
	public class ScriptBuilder {
		public const string newLine = "\r\n";
		public const string tab = "\t";
		public const string TestsNamespace = "UsAcRe.TestsScripts";
		public const string TestsClassName = "TestsScript";

		readonly ActionsList actions;
		readonly ISettingsService settingsService;

		public ScriptBuilder(
			ActionsList actions,
			ISettingsService settingsService) {
			Guard.Requires(actions, nameof(actions));
			Guard.Requires(settingsService, nameof(settingsService));
			this.actions = actions;
			this.settingsService = settingsService;
		}

		static void ObtainCtorArgumentsTypes(Type type, List<Type> ctorArgsTypes) {
			var ctors = type.GetConstructors();
			var ctorsArgs = ctors
				.Select(x => x.GetParameters())
				.SelectMany(x => x)
				.Select(x => x.ParameterType);

			var genericArgs = ctorsArgs
				.Where(x => x.GenericTypeArguments.Length > 0)
				.SelectMany(x => x.GenericTypeArguments);

			var types = ctorsArgs
				.Concat(genericArgs)
				.Where(x => !ctorArgsTypes.Contains(x))
				.ToList();

			ctorArgsTypes.AddRange(types);

			foreach(var argType in types) {
				ObtainCtorArgumentsTypes(argType, ctorArgsTypes);
			}
		}

		public string CreateUsingsSection() {
			var actionsTypes = actions
				.Select(x => x.GetType())
				.Distinct();

			var types = new List<Type>();
			foreach(var item in actionsTypes) {
				ObtainCtorArgumentsTypes(item, types);
			}

			var method = typeof(BaseAction).GetMethod(nameof(BaseAction.ExecuteAsync));
			var executeAsyncMethodTypes = method?.GetParameters()
				.Select(x => x.ParameterType)
				.ToList();

			executeAsyncMethodTypes.Add(method?.ReturnParameter.ParameterType);

			var ctorsArgs = types
				.Distinct();

			var usings = ctorsArgs
				.Concat(actionsTypes)
				.Concat(executeAsyncMethodTypes)
				.Select(x => x.Namespace)
				.Distinct()
				.OrderBy(x => x)
				.Select(x => string.Format("using {0};", x));

			return string.Join(newLine, usings);
		}

		public string CreateNamespaceSection(string code) {
			return "namespace " + TestsNamespace + " {"
				+ newLine
				+ code
				+ newLine
				+ "}";
		}

		public string CreateClassSection(string code) {
			return tab + "public class " + TestsClassName + " {"
				+ newLine
				+ code
				+ newLine
				+ tab + "}";
		}

		public string CreateExecuteMethodSection(string code) {
			return tab + tab
				+ "public async Task " + nameof(BaseAction.ExecuteAsync) + "() {"
				+ newLine
				+ tab + tab + tab + "await ActionsExecutor.Perform"
				+ newLine
				+ code
				+ newLine
				+ tab + tab + "}";
		}

		public string CreateExecuteMethodBody() {
			var sb = new StringBuilder();
			CombineTextTypingActions();
			var lastAction = actions.LastOrDefault();
			foreach(var action in actions) {
				var codeLines = ("." + action.ExecuteAsScriptSource())
					.Split('\r', '\n')
					.Where(x => !string.IsNullOrEmpty(x))
					.Select(x => string.Format("{0}{0}{0}{0}{1}", tab, x));

				var code = string.Join(newLine, codeLines);

				var lastItem = action == lastAction;
				if(lastItem) {
					sb.Append(code);
				} else {
					sb.AppendFormat("{0}{1}", code, newLine);
				}
			}
			sb.Append(";");
			return sb.ToString();
		}

		public string Generate() {
			var executeMethod = CreateExecuteMethodSection(CreateExecuteMethodBody());
			var classSection = CreateClassSection(executeMethod);

			var sb = new StringBuilder();
			sb.Append(CreateUsingsSection());
			sb.AppendLine();
			sb.AppendLine();
			sb.Append(CreateNamespaceSection(classSection));
			sb.AppendLine();
			return sb.ToString();
		}

		internal void CombineTextTypingActions() {
			var keysSeqList = new List<(int start, int end)>();
			int? startKeysSeq = null;
			KeybdAction prevKeybdAction = null;
			int prevIndex = -1;

			foreach(var action in actions.Select((value, index) => (value, index))) {
				if(action.value is KeybdAction keybdAction
					&& keybdAction.VKCode.IsPrintable()
					&& keybdAction.IsUp != prevKeybdAction?.IsUp) {
					if(!keybdAction.IsUp && !startKeysSeq.HasValue) {
						startKeysSeq = action.index;
					}
				} else {
					if(prevKeybdAction != null && prevKeybdAction.IsUp && startKeysSeq.HasValue) {
						keysSeqList.Add((startKeysSeq.Value, prevIndex));
					}
					startKeysSeq = null;
				}
				prevKeybdAction = action.value as KeybdAction;
				prevIndex = action.index;
			}
			if(prevKeybdAction != null && prevKeybdAction.IsUp && startKeysSeq.HasValue) {
				keysSeqList.Add((startKeysSeq.Value, prevIndex));
			}

			foreach(var keysSeq in keysSeqList) {
				var size = (keysSeq.end - keysSeq.start) + 1;
				if(size <= 2) {
					continue;
				}
				var keysActions = actions
					.Skip(keysSeq.start)
					.Take(size)
					.OfType<KeybdAction>()
					.Where(x => x.IsUp)
					.Select(x => x.VKCode);

				if(keysActions.Count() * 2 == size) {
					var sb = new StringBuilder();
					foreach(var key in keysActions) {
						if(key.TryGetKeyValue(out char value)) {
							sb.Append(value);
						}
					}
					var textTypingAction = new TextTypingAction(sb.ToString());
					actions[keysSeq.start] = textTypingAction;
					for(int i = keysSeq.start + 1; i <= keysSeq.end; i++) {
						actions[i] = null;
					}
				}
			}
			actions.RemoveAll(x => x == null);
		}
	}
}
