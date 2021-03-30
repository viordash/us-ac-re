using System;
using Microsoft.Extensions.DependencyInjection;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;
using UsAcRe.Recorder.UI.Models;
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
					.AddSingleton<ActionsListModel, ActionsListModel>()
					.AddSingleton<IScriptBuilder, ScriptBuilder>()
					.AddSingleton<IScriptCompiler, ScriptCompiler>()

					.AddTransient<ISettingsService>((sp) => sp.GetRequiredService<IRecorderSettingsService>())

					.AddTestsActions();

			var serviceProvider = services.BuildServiceProvider();
			ServiceLocator.SetLocatorProvider(serviceProvider);
			return serviceProvider;
		}
	}
}
