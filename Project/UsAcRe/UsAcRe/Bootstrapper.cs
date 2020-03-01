using CommonServiceLocator;
using Unity;
using Unity.ServiceLocation;

namespace UsAcRe {
	public class Bootstrapper {

		public static UnityContainer Initialize() {
			var container = new UnityContainer();

			var serviceLocator = new UnityServiceLocator(container);
			ServiceLocator.SetLocatorProvider(() => serviceLocator);    // Warning: do NOT remove serviceLocator local variable, do not inline "new UnityServiceLocator"!
			return container;
		}
	}
}
