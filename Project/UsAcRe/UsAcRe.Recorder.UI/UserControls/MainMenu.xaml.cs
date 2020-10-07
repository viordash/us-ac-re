using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UsAcRe.Recorder.UI.Models;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for MainMenu.xaml
	/// </summary>
	public partial class MainMenu : UserControl {
		readonly MainMenuModel MainMenuModel;

		public event ExecutedRoutedEventHandler OnNewProjectCommand;
		public event ExecutedRoutedEventHandler OnOpenProjectCommand;
		public event ExecutedRoutedEventHandler OnExitCommand;
		public event ExecutedRoutedEventHandler OnStartStopCommand;

		public MainMenu() {
			InitializeComponent();
			MainMenuModel = new MainMenuModel();
			miNewProject.CommandBindings.Add(new CommandBinding(UICommands.NewProject, (s, e) => OnNewProjectCommand?.Invoke(s, e)));
			miOpenProject.CommandBindings.Add(new CommandBinding(UICommands.OpenProject, (s, e) => OnOpenProjectCommand?.Invoke(s, e)));
			miExit.CommandBindings.Add(new CommandBinding(UICommands.Exit, (s, e) => OnExitCommand?.Invoke(s, e)));
			miStartStop.CommandBindings.Add(new CommandBinding(UICommands.StartStop, (s, e) => OnStartStopCommand?.Invoke(s, e)));
			miActions.ItemsSource = MainMenuModel.Items;
		}

		private void mainMenu_Loaded(object sender, RoutedEventArgs e) {
			MainMenuModel.AssignControl(miActions, (MainWindow)Application.Current.MainWindow);
		}
	}
}
