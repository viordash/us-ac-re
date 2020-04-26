using System;
using System.Linq;
using System.Collections.Generic;
using UsAcRe.Actions;

namespace UsAcRe.Scripts {
	public class ScriptBuilder {
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

			return string.Join("\r\n", usings);
		}

		static IEnumerable<Type> GetConstructorArgumentsTypes<T>(T examinedClass) {
			var ctors = examinedClass.GetType().GetConstructors();
			return ctors
				.Select(x => x.GetParameters())
				.SelectMany(x => x)
				.Select(x => x.ParameterType);
		}
	}
}
