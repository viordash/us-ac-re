using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Models;

[assembly: HostingStartup(typeof(UsAcRe.Web.Server.Areas.Identity.IdentityHostingStartup))]
namespace UsAcRe.Web.Server.Areas.Identity {
	public class IdentityHostingStartup : IHostingStartup {
		public void Configure(IWebHostBuilder builder) {
			builder.ConfigureServices((context, services) => {
			});
		}
	}
}