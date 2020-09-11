using System;

namespace UsAcRe.Core.Extensions {
	public static class ServiceProviderExtension {
		public static TService GetInstance<TService>(this IServiceProvider serviceProvider) {
			var instance = serviceProvider.GetService(typeof(TService));
			return (TService)instance;
		}
	}
}
