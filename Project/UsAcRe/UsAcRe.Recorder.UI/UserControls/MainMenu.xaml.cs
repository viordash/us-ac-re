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
			miNewProject.CommandBindings.Add(new CommandBinding(UICommands.NewProject, OnCommand_NewProject));
			miOpenProject.CommandBindings.Add(new CommandBinding(UICommands.OpenProject, OnCommand_OpenProject));
			miExit.CommandBindings.Add(new CommandBinding(UICommands.Exit, OnCommand_Exit));
			miStartStop.CommandBindings.Add(new CommandBinding(UICommands.StartStop, OnCommand_StartStop));

			miActions.ItemsSource = MainMenuModel.Items;
			MainMenuModel.AssignControl(miActions, (MainWindow)Application.Current.MainWindow);
		}

		internal void OnCommand_NewProject(object sender, ExecutedRoutedEventArgs e) {
			OnNewProjectCommand?.Invoke(sender, e);
		}

		internal void OnCommand_OpenProject(object sender, ExecutedRoutedEventArgs e) {
			OnOpenProjectCommand?.Invoke(sender, e);
		}

		internal void OnCommand_Exit(object sender, ExecutedRoutedEventArgs e) {
			OnExitCommand?.Invoke(sender, e);
		}

		internal void OnCommand_StartStop(object sender, ExecutedRoutedEventArgs e) {
			OnStartStopCommand?.Invoke(sender, e);
		}
	}
}
