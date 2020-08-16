using System;
using System.Linq;
using System.Collections.Generic;
using UsAcRe.Actions;
using System.Text;

namespace UsAcRe.Scripts {
	public class ScriptBuilder {
		public const string newLine = "\r\n";
		public const string tab = "\t";

		readonly ActionsList actions;

		public ScriptBuilder(ActionsList actions) {
			this.actions = actions;
		}

		public string CreateUsingsSection() {
			var ctorsArgs = actions
				.SelectMany(x => GetConstructorArgumentsTypes(x))
				.Distinct();

			var genericArgs = ctorsArgs
				.Where(x => x.GenericTypeArguments.Length > 0)
				.SelectMany(x => x.GenericTypeArguments);

			var args = ctorsArgs
				.Concat(genericArgs)
				.Distinct();

			var usings = args
				.Select(x => x.Namespace)
				.Distinct()
				.OrderBy(x => x)
				.Select(x => string.Format("using {0};", x));

			return string.Join(newLine, usings);
		}

		static IEnumerable<Type> GetConstructorArgumentsTypes<T>(T examinedClass) {
			var ctors = examinedClass.GetType().GetConstructors();
			return ctors
				.Select(x => x.GetParameters())
				.SelectMany(x => x)
				.Select(x => x.ParameterType);
		}

		public string CreateNamespaceSection(string code) {
			return "namespace UsAcRe.TestsScripts {"
				+ newLine
				+ code
				+ newLine
				+ "}";
		}

		public string CreateClassSection(string code) {
			return tab + "public class TestsScript {"
				+ newLine
				+ code
				+ newLine
				+ tab + "}";
		}

		public string CreateExecuteMethodSection(string code) {
			return tab + tab
				+ "public void Execute() {"
				+ newLine
				+ code
				+ newLine
				+ tab + tab + "}";
		}

		public string CreateExecuteMethodBody() {
			var sb = new StringBuilder();
			foreach(var action in actions) {
				var codeLines = action.ExecuteAsScriptSource()
					.Split('\r', '\n')
					.Where(x => !string.IsNullOrEmpty(x))
					.Select(x => string.Format("{0}{0}{0}{1}", tab, x));

				var code = string.Join(newLine, codeLines);
				sb.AppendFormat("{0};{1}", code, newLine);
			}
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
