using System.Threading.Tasks;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class DelayAction : BaseAction {
		public int Milliseconds { get; set; }

		public static DelayAction Record(int milliseconds) {
			var instance = CreateInstance<DelayAction>();
			instance.Milliseconds = milliseconds;
			return instance;
		}

		public static async Task Play(int milliseconds) {
			var instance = CreateInstance<DelayAction>();
			instance.Milliseconds = milliseconds;
			await instance.ExecuteAsync();
		}

		public DelayAction(
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService) : base(settingsService, testsLaunchingService, fileService) {
		}

		protected override async ValueTask ExecuteCoreAsync() {
			await DoDelay();
		}

		public override string ToString() {
			return string.Format("{0} Period, s:{1:0.###}", nameof(DelayAction), Milliseconds);
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}({2})", nameof(DelayAction), nameof(DelayAction.Play), Milliseconds);
		}

		async ValueTask DoDelay() {
			await Task.Delay(Milliseconds);
		}
	}
}
