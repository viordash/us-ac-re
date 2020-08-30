using System.Threading.Tasks;
using Microsoft.Test.Input;
using UsAcRe.Helpers;

namespace UsAcRe.Actions {
	public class TextTypingAction : BaseAction {
		public string Text { get; set; }


		public TextTypingAction(string text)
			: this(null, text) { }

		public TextTypingAction(BaseAction prevAction, string text) : base(prevAction) {
			Text = text;
		}

		protected override ValueTask ExecuteCoreAsync() {
			Keyboard.Type(Text);
			return new ValueTask(Task.CompletedTask);
		}

		public override string ToString() {
			return string.Format("{0} Text:{0}", nameof(TextTypingAction), NamingHelpers.Escape(Text, int.MaxValue));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}(\"{1}\")", nameof(ActionsExecutor.TextType), NamingHelpers.Escape(Text, int.MaxValue));
		}


	}
}
