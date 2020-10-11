using CommonServiceLocator;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;
using UsAcRe.Services;

namespace UsAcRe.Recorder.UI {
	public class Bootstrapper {

		public static UnityContainer Initialize() {
			var container = new UnityContainer();

			container.RegisterType<IAutomationElementService, AutomationElementService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IWinApiService, WinApiService>(new ContainerControlledLifetimeManager());
			container.RegisterType<ITestsLaunchingService, TestsLaunchingService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IWindowsFormsService, WindowsFormsService>(new ContainerControlledLifetimeManager());
			container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IFileService, FileService>(new ContainerControlledLifetimeManager());

			container.RegisterType<IScriptBuilder, ScriptBuilder>(new TransientLifetimeManager());
			container.RegisterType<IScriptCompiler, ScriptCompiler>(new TransientLifetimeManager());

			container.RegisterType<ElementMatchAction, ElementMatchAction>(new TransientLifetimeManager());
			container.RegisterType<KeybdAction, KeybdAction>(new TransientLifetimeManager());
			container.RegisterType<MouseClickAction, MouseClickAction>(new TransientLifetimeManager());
			container.RegisterType<MouseDragAction, MouseDragAction>(new TransientLifetimeManager());
			container.RegisterType<TextTypingAction, TextTypingAction>(new TransientLifetimeManager());
			container.RegisterType<ActionSet, ActionSet>(new TransientLifetimeManager());

			var serviceLocator = new UnityServiceLocator(container);
			ServiceLocator.SetLocatorProvider(() => serviceLocator);    // Warning: do NOT remove serviceLocator local variable, do not inline "new UnityServiceLocator"!
			return container;
		}
	}
}
