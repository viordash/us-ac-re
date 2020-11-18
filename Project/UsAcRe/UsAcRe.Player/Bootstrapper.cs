using CommonServiceLocator;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Services;

namespace UsAcRe.Player {
	public class Bootstrapper {

		public static UnityContainer Initialize() {
			var container = new UnityContainer();
			container.RegisterType<ITestsLaunchingService, TestsLaunchingService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IWindowsFormsService, WindowsFormsService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IFileService, FileService>(new ContainerControlledLifetimeManager());

			container.RegisterType<IScriptCompiler, ScriptCompiler>(new TransientLifetimeManager());

			var serviceLocator = new UnityServiceLocator(container);
			ServiceLocator.SetLocatorProvider(() => serviceLocator);    // Warning: do NOT remove serviceLocator local variable, do not inline "new UnityServiceLocator"!
			return container;
		}
	}
}
