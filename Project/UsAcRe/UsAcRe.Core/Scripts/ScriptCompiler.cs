using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Scripts {

	public class ScriptCompiler : IScriptCompiler {
		static NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		#region inner classes
		internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext {
			public SimpleUnloadableAssemblyLoadContext()
				: base(true) {
			}

			protected override Assembly Load(AssemblyName assemblyName) {
				return null;
			}
		}
		#endregion


		static string GeTypeAssemblyLocation<T>() {
			return typeof(T).Assembly.Location;
		}

		internal static IEnumerable<string> GetMandatoryAssemblies() {
			return new string[] {
				GeTypeAssemblyLocation<System.Windows.Forms.Form>(),
				GeTypeAssemblyLocation<System.Drawing.Point>(),
				GeTypeAssemblyLocation<UsAcRe.Core.Actions.BaseAction>()
			};
		}

		static byte[] Compile(string sourceCode) {
			using(var ms = new MemoryStream()) {
				var result = GenerateCode(sourceCode).Emit(ms);
				if(!result.Success) {
					var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
					foreach(var diagnostic in failures) {
						logger.Error(diagnostic);
					}
					throw new ApplicationException("Error occured during compilation");
				}
				ms.Seek(0, SeekOrigin.Begin);
				return ms.ToArray();
			}
		}

		static CSharpCompilation GenerateCode(string sourceCode) {
			var codeString = SourceText.From(sourceCode);
			var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

			var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

			var assemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(a => !a.IsDynamic)
				.Select(a => a.Location)
				.Concat(GetMandatoryAssemblies())
				.Distinct();

			var references = assemblies.Select(x => MetadataReference.CreateFromFile(x));

			return CSharpCompilation.Create("Hello.dll",
				new[] { parsedSyntaxTree },
				references: references,
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
					optimizationLevel: OptimizationLevel.Release,
					assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
		}


		static async Task Execute(byte[] compiledAssembly) {
			var assemblyLoadContextWeakRef = await LoadAndExecute(compiledAssembly);
			for(var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++) {
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static async Task<WeakReference> LoadAndExecute(byte[] compiledAssembly) {
			using(var asm = new MemoryStream(compiledAssembly)) {
				var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

				var assembly = assemblyLoadContext.LoadFromStream(asm);
				await CreateAndInvoke(assembly);
				assemblyLoadContext.Unload();

				return new WeakReference(assemblyLoadContext);
			}
		}

		//static Assembly PrepareAssembly(string sourceCode) {
		//	var codeProvider = CodeDomProvider.CreateProvider("CSharp");
		//	var compilerparams = new CompilerParameters();
		//	compilerparams.GenerateExecutable = false;
		//	compilerparams.GenerateInMemory = true;

		//	var assemblies = AppDomain.CurrentDomain
		//					.GetAssemblies()
		//					.Where(a => !a.IsDynamic)
		//					.Select(a => a.Location)
		//					.Concat(GetMandatoryAssemblies())
		//					.Distinct();

		//	compilerparams.ReferencedAssemblies.AddRange(assemblies.ToArray());

		//	// Invoke compilation of the source file.
		//	var compilerResults = codeProvider.CompileAssemblyFromSource(compilerparams, sourceCode);

		//	if(compilerResults.Errors.HasErrors) {
		//		foreach(CompilerError compileError in compilerResults.Errors) {
		//			logger.Error("Line {0},{1}\t: {2}\n", compileError.Line, compileError.Column, compileError.ErrorText);
		//		}
		//		throw new ApplicationException("Error occured during compilation");
		//	}
		//	return compilerResults.CompiledAssembly;
		//}

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
			var compiled = Compile(sourceCode);
			await Execute(compiled);
		}
	}
}
