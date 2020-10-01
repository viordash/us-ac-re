using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NGuard;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Player.Actions;

namespace UsAcRe.Player.Scripts {
	public class ScriptBuilder : IScriptBuilder {
		readonly ISettingsService settingsService;

		public ScriptBuilder(ISettingsService settingsService) {
			Guard.Requires(settingsService, nameof(settingsService));
			this.settingsService = settingsService;
		}

		static void ObtainCtorArgumentsTypes(Type type, List<Type> types) {
			var ctors = type.GetConstructors();
			var ctorsArgs = ctors
				.Select(x => x.GetParameters())
				.SelectMany(x => x)
				.Select(x => x.ParameterType);

			var genericArgs = ctorsArgs
				.Where(x => x.GenericTypeArguments.Length > 0)
				.SelectMany(x => x.GenericTypeArguments);

			var combineTypes = ctorsArgs
				.Concat(genericArgs)
				.Where(x => !types.Contains(x))
				.ToList();

			types.AddRange(combineTypes);

			foreach(var argType in combineTypes) {
				ObtainCtorArgumentsTypes(argType, types);
			}
		}

		static void ObtainMethodArgumentsTypes(MethodInfo methodInfo, List<Type> types) {
			var parameters = methodInfo.GetParameters();
			var parametersTypes = parameters
				.Select(x => x.ParameterType);

			var genericTypes = parametersTypes
				.Where(x => x.GenericTypeArguments.Length > 0)
				.SelectMany(x => x.GenericTypeArguments);

			var combineTypes = parametersTypes
				.Concat(genericTypes)
				.Where(x => !types.Contains(x))
				.ToList();

			types.AddRange(combineTypes);

			foreach(var argType in combineTypes) {
				ObtainCtorArgumentsTypes(argType, types);
			}
		}

		internal string CreateUsingsSection(ActionsList actions) {
			var actionsTypes = actions
				.Select(x => x.GetType())
				.Distinct();

			var types = new List<Type>();
			foreach(var item in actionsTypes) {
				ObtainCtorArgumentsTypes(item, types);
			}

			foreach(var action in actions) {
				var methodRecord = action.GetType().GetMethod("Record");
				if(methodRecord == null) {
					throw new ScriptComposeException();
				}
				ObtainMethodArgumentsTypes(methodRecord, types);
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

			return string.Join(ScriptConstants.NewLine, usings);
		}

		internal string CreateNamespaceSection(string code) {
			return "namespace " + ScriptConstants.TestsNamespace + " {"
				+ ScriptConstants.NewLine
				+ code
				+ ScriptConstants.NewLine
				+ "}";
		}

		internal string CreateClassSection(string code) {
			return ScriptConstants.Tab + "public class " + ScriptConstants.TestsClassName + " {"
				+ ScriptConstants.NewLine
				+ code
				+ ScriptConstants.NewLine
				+ ScriptConstants.Tab + "}";
		}

		internal string CreateExecuteMethodSection(string code) {
			return ScriptConstants.Tab + ScriptConstants.Tab
				+ "public async Task " + nameof(BaseAction.ExecuteAsync) + "() {"
				+ ScriptConstants.NewLine
				+ code
				+ ScriptConstants.NewLine
				+ ScriptConstants.Tab + ScriptConstants.Tab + "}";
		}

		internal string CreateExecuteMethodBody(ActionsList actions) {
			var sb = new StringBuilder();
			CombineTextTypingActions(actions);
			foreach(var action in actions) {
				var codeLines = ("await " + action.ExecuteAsScriptSource())
					.Split('\r', '\n')
					.Where(x => !string.IsNullOrEmpty(x))
					.Select(x => string.Format("{0}{0}{0}{1}", ScriptConstants.Tab, x));

				var code = string.Join(ScriptConstants.NewLine, codeLines);

				sb.AppendFormat("{0};{1}", code, ScriptConstants.NewLine);
			}
			return sb.ToString();
		}

		public string Generate(ActionsList actions) {
			var executeMethod = CreateExecuteMethodSection(CreateExecuteMethodBody(actions));
			var classSection = CreateClassSection(executeMethod);

			var sb = new StringBuilder();
			sb.Append(CreateUsingsSection(actions));
			sb.AppendLine();
			sb.AppendLine();
			sb.Append(CreateNamespaceSection(classSection));
			sb.AppendLine();
			return sb.ToString();
		}

		internal void CombineTextTypingActions(ActionsList actions) {
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
					var textTypingAction = TextTypingAction.Record(sb.ToString());
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
