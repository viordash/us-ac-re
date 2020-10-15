using System.Windows;

namespace UsAcRe.Player.UI {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		App() {
			Bootstrapper.Initialize();
			InitializeComponent();
		}
	}
}
