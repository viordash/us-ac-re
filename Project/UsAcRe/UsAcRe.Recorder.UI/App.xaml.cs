using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		readonly IServiceProvider serviceProvider;
		App() {
			serviceProvider = UI.Startup.BuildServiceProvider();
			InitializeComponent();
		}

		private void OnStartup(object sender, StartupEventArgs e) {
			var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
		}

	}
}
