using System.Threading.Tasks;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class ActionSet : BaseAction {
		public string SourceCodeFileName { get; private set; }

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
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService) : base(settingsService, testsLaunchingService, fileService) {
		}

		public override string ToString() {
			return string.Format("{0} SourceCode: '{1}'", nameof(ActionSet), SourceCodeFileName);
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}(\"{2}\")", nameof(ActionSet), nameof(ActionSet.Play), SourceCodeFileName);
		}

		protected override async ValueTask ExecuteCoreAsync() {
			var sourceCode = fileService.ReadAllText(SourceCodeFileName);
			await ScriptCompiler.RunTest(sourceCode);
		}

		protected override bool CanExecute() {
			return true;
		}
	}
}
