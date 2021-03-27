using System;
using Microsoft.Extensions.DependencyInjection;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;
using UsAcRe.Services;

namespace UsAcRe.Recorder.UI {
	public class Startup {
		public static IServiceProvider BuildServiceProvider() {
			var services = new ServiceCollection();

			services.AddSingleton<IAutomationElementService, AutomationElementService>()
					.AddSingleton<IWinApiService, WinApiService>()
					.AddSingleton<ITestsLaunchingService, TestsLaunchingService>()
					.AddSingleton<IWindowsFormsService, WindowsFormsService>()
					.AddSingleton<IRecorderSettingsService, SettingsService>()
					.AddSingleton<IDialogService, DialogService>()
					.AddSingleton<IFileService, FileService>()
					.AddSingleton<MainWindow>()

					.AddTransient<ISettingsService>((sp) => sp.GetRequiredService<IRecorderSettingsService>())
					.AddTransient<IScriptBuilder, ScriptBuilder>()
					.AddTransient<IScriptCompiler, ScriptCompiler>()
					.AddTransient<ElementMatchAction, ElementMatchAction>()
					.AddTransient<KeybdAction, KeybdAction>()
					.AddTransient<MouseClickAction, MouseClickAction>()
					.AddTransient<MouseDragAction, MouseDragAction>()
					.AddTransient<TextTypingAction, TextTypingAction>()
					.AddTransient<ActionSet, ActionSet>();

			var serviceProvider = services.BuildServiceProvider();
			ServiceLocator.SetLocatorProvider(serviceProvider);
			return serviceProvider;
		}
	}
}
