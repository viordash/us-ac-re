using System;
using Microsoft.Extensions.DependencyInjection;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;
using UsAcRe.Player.Services;
using WindowsFormsService = UsAcRe.Player.Services.WindowsFormsService;

namespace UsAcRe.Player {
	public class Startup {
		public static IServiceProvider BuildServiceProvider(Options options) {
			var services = new ServiceCollection();

			services.AddSingleton<IAutomationElementService, AutomationElementService>()
					.AddSingleton<IWinApiService, WinApiService>()
					.AddSingleton<ITestsLaunchingService, TestsLaunchingService>()
					.AddSingleton<IWindowsFormsService, WindowsFormsService>()
					.AddSingleton<IFileService, FileService>()
					.AddSingleton<IPlayerSettingsService>(sp => new PlayerSettingsService(options))
					.AddSingleton<IWinApiService, WinApiService>()

					.AddTransient<ISettingsService>(sp => sp.GetRequiredService<IPlayerSettingsService>())
					.AddTransient<IScriptCompiler, ScriptCompiler>()

					.AddTestsActions();

			var serviceProvider = services.BuildServiceProvider();
			ServiceLocator.SetLocatorProvider(serviceProvider);
			return serviceProvider;
		}
	}
}
