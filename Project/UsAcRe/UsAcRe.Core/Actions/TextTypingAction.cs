using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class TextTypingAction : BaseAction {
		public string Text { get; set; }

		public static TextTypingAction Record(string text) {
			var instance = CreateInstance<TextTypingAction>();
			instance.Text = text;
			return instance;
		}

		public static async Task Play(string text) {
			var instance = CreateInstance<TextTypingAction>();
			instance.Text = text;
			await instance.ExecuteAsync();
		}

		public TextTypingAction(
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService) : base(settingsService, testsLaunchingService) {
		}

		protected override ValueTask ExecuteCoreAsync() {
			Keyboard.Type(Text);
			return new ValueTask(Task.CompletedTask);
		}

		public override string ToString() {
			return string.Format("{0} Text:{0}", nameof(TextTypingAction), NamingHelpers.Escape(Text, int.MaxValue));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("await {0}.{1}(\"{2}\");", nameof(TextTypingAction), nameof(TextTypingAction.Play), NamingHelpers.Escape(Text, int.MaxValue));
		}


	}
}
