using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NGuard;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class ActionSet : BaseAction {
		readonly IScriptCompiler scriptCompiler;
		public string SourceCodeFileName { get; private set; }

		int count = -1;
		public int Count {
			get {
				if(count < 0) {
					count = GetChildActionsCount();
				}
				return count;
			}
		}

		public static ActionSet Record(string sourceCodeFileName) {
			var instance = CreateInstance<ActionSet>();
			instance.SourceCodeFileName = sourceCodeFileName;
			return instance;
		}

		public static async Task Play(string sourceCodeFileName) {
			var instance = CreateInstance<ActionSet>();
			instance.SourceCodeFileName = sourceCodeFileName;
			await instance.ExecuteAsync();
		}

		public ActionSet(
			IScriptCompiler scriptCompiler,
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService) : base(settingsService, testsLaunchingService, fileService) {
			Guard.Requires(scriptCompiler, nameof(scriptCompiler));
			this.scriptCompiler = scriptCompiler;
		}

		public override string ToString() {
			return string.Format("{0} SourceCode: '{1}', Count: {2}", nameof(ActionSet), SourceCodeFileName, Count);
		}

		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}(@\"{2}\")", nameof(ActionSet), nameof(ActionSet.Play), SourceCodeFileName);
		}

		public override string ShortDescription() {
			return ToString();
		}

		protected override async ValueTask ExecuteCoreAsync() {
			var sourceCode = fileService.ReadAllText(SourceCodeFileName);
			await scriptCompiler.RunTest(sourceCode);
		}

		int GetChildActionsCount() {
			var sourceCode = fileService.ReadAllText(SourceCodeFileName);
			using(testsLaunchingService.Examine()) {
				scriptCompiler.RunTest(sourceCode).Wait();
				return testsLaunchingService.ExecutedActions.Count;
			}
		}
	}
}
