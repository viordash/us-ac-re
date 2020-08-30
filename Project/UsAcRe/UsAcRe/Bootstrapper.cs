using CommonServiceLocator;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using UsAcRe.Services;

namespace UsAcRe {
	public class Bootstrapper {

		public static UnityContainer Initialize() {
			var container = new UnityContainer();

			container.RegisterType<IAutomationElementService, AutomationElementService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IWinApiService, WinApiService>(new ContainerControlledLifetimeManager());
			container.RegisterType<ITestsLaunchingService, TestsLaunchingService>(new ContainerControlledLifetimeManager());
			container.RegisterType<IWindowsFormsService, WindowsFormsService>(new ContainerControlledLifetimeManager());
			container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());

			var serviceLocator = new UnityServiceLocator(container);
			ServiceLocator.SetLocatorProvider(() => serviceLocator);    // Warning: do NOT remove serviceLocator local variable, do not inline "new UnityServiceLocator"!
			return container;
		}
	}
}
