﻿using System.Threading.Tasks;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.Actions {
	public class TextTypingAction : BaseAction {
		readonly IWinApiService winApiService;
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
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService,
			IWinApiService winApiService) : base(settingsService, testsLaunchingService, fileService) {
			this.winApiService = winApiService;
		}

		protected override ValueTask ExecuteCoreAsync() {
			var keyboard = new Keyboard(winApiService);
			keyboard.Type(Text);
			return new ValueTask(Task.CompletedTask);
		}

		public override string ToString() {
			return string.Format("{0} Text:{0}", nameof(TextTypingAction), NamingHelpers.Escape(Text, int.MaxValue));
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("{0}.{1}(\"{2}\")", nameof(TextTypingAction), nameof(TextTypingAction.Play), NamingHelpers.Escape(Text, int.MaxValue));
		}

		public override string ShortDescription() {
			return string.Format("TextTyping, Text:{0}", NamingHelpers.Escape(Text, 20));
		}

	}
}
