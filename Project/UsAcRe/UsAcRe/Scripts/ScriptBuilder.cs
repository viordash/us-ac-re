using System;
using System.Linq;
using System.Collections.Generic;
using UsAcRe.Actions;
using System.Text;
using UsAcRe.Services;
using NGuard;

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
	}
}
