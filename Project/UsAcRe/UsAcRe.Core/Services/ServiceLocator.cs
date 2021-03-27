using System;
using NGuard;

namespace UsAcRe.Core.Services {
	public class ServiceLocator {
		private IServiceProvider currentServiceProvider;
		private static IServiceProvider gServiceProvider = null;

		private ServiceLocator(IServiceProvider serviceProvider) {
			currentServiceProvider = serviceProvider;
		}

		public static ServiceLocator Current {
			get {
				if(gServiceProvider == null) {
					throw new InvalidOperationException("ServiceLocationProvider must be set.");
				}
				return new ServiceLocator(gServiceProvider);
			}
		}

		public static void SetLocatorProvider(IServiceProvider serviceProvider) {
			Guard.Requires(serviceProvider, nameof(serviceProvider)).IsNotNull();
			gServiceProvider = serviceProvider;
		}

		public object GetInstance(Type serviceType) {
			return currentServiceProvider.GetService(serviceType);
		}

		public TService GetInstance<TService>() where TService : class {
			return currentServiceProvider.GetService(typeof(TService)) as TService;
		}
	}
}
