using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.Actions {
	public class TextTypingAction : BaseAction {
		public string Text { get; set; }

		public static TextTypingAction CreateInstance(string text) {
			var instance = CreateInstance<TextTypingAction>();
			instance.Text = text;
			return instance;
		}

		public TextTypingAction(ITestsLaunchingService testsLaunchingService) : base(testsLaunchingService) {
		}

		protected override ValueTask ExecuteCoreAsync() {
			Keyboard.Type(Text);
			return new ValueTask(Task.CompletedTask);
		}

		public override string ToString() {
			return string.Format("{0} Text:{0}", nameof(TextTypingAction), NamingHelpers.Escape(Text, int.MaxValue));
		}
		public override string ExecuteAsScriptSource() {
			return null;//string.Format("{0}(\"{1}\")", nameof(ActionsExecutor.TextType), NamingHelpers.Escape(Text, int.MaxValue));
		}


	}
}
