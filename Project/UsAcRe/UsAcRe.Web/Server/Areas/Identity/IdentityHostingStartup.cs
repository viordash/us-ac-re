using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(UsAcRe.Web.Server.Areas.Identity.IdentityHostingStartup))]
namespace UsAcRe.Web.Server.Areas.Identity {
	public class IdentityHostingStartup : IHostingStartup {
		public void Configure(IWebHostBuilder builder) {
			builder.ConfigureServices((context, services) => {
			});
		}
	}
}