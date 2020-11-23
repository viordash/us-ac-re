using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Scripts {
	public class ScriptCompiler : IScriptCompiler {
		static NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		static string GetypeAssemblyLocation<T>() {
			return typeof(T).Assembly.Location;
		}

		internal static IEnumerable<string> GetMandatoryAssemblies() {
			return new string[] {
				GetypeAssemblyLocation<System.Windows.Forms.Form>(),
				GetypeAssemblyLocation<System.Drawing.Point>(),
				GetypeAssemblyLocation<UsAcRe.Core.Actions.BaseAction>()
			};
		}

		static Assembly PrepareAssembly(string sourceCode) {
			var codeProvider = CodeDomProvider.CreateProvider("CSharp");
			var compilerparams = new CompilerParameters();
			compilerparams.GenerateExecutable = false;
			compilerparams.GenerateInMemory = true;

			var assemblies = AppDomain.CurrentDomain
							.GetAssemblies()
							.Where(a => !a.IsDynamic)
							.Select(a => a.Location)
							.Concat(GetMandatoryAssemblies())
							.Distinct();

			compilerparams.ReferencedAssemblies.AddRange(assemblies.ToArray());


			// Invoke compilation of the source file.
			var compilerResults = codeProvider.CompileAssemblyFromSource(compilerparams, sourceCode);

			if(compilerResults.Errors.HasErrors) {
				foreach(CompilerError compileError in compilerResults.Errors) {
					logger.Error("Line {0},{1}\t: {2}\n", compileError.Line, compileError.Column, compileError.ErrorText);
				}
				throw new ApplicationException("Error occured during compilation");
			}
			return compilerResults.CompiledAssembly;
		}

		static async Task CreateAndInvoke(Assembly assembly) {
			var instance = assembly.CreateInstance(ScriptConstants.TestsNamespace + "." + ScriptConstants.TestsClassName);
			if(instance == null) {
				throw new ApplicationException("There is no " + ScriptConstants.TestsClassName + " class in the compiled Assembly");
			}
			Type type = instance.GetType();
			MethodInfo method = type.GetMethod(nameof(BaseAction.ExecuteAsync));
			if(method == null) {
				throw new ApplicationException("There is no TestsScript.Execute method in the compiled class");
			}
			await (Task)method.Invoke(instance, null);
		}

		public async Task RunTest(string sourceCode) {
			var assembly = PrepareAssembly(sourceCode);
			await CreateAndInvoke(assembly);
		}
	}
}
