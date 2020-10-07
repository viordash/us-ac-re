using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using UsAcRe.Core.UI.Helpers;
using UsAcRe.Recorder.UI.Models;
using UsAcRe.Recorder.UI.Properties;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		readonly MainMenuModel MainMenuModel;

		public MainWindow() {
			InitializeComponent();

			MainMenuModel = new MainMenuModel();

			miNewProject.CommandBindings.Add(new CommandBinding(UICommands.NewProject, OnCommand_NewProject));
			miOpenProject.CommandBindings.Add(new CommandBinding(UICommands.OpenProject, OnCommand_OpenProject));
			miExit.CommandBindings.Add(new CommandBinding(UICommands.Exit, OnCommand_Exit));
			miStartStop.CommandBindings.Add(new CommandBinding(UICommands.StartStop, OnCommand_StartStop));
			miActions.ItemsSource = MainMenuModel.Items;
			MainMenuModel.AssignControl(miActions, this);
		}

		private void Window_Initialized(object sender, System.EventArgs e) {
			WindowsHelper.LoadLocation(Settings.Default.MainFormLocation, this);
			WindowsHelper.LoadSize(Settings.Default.MainFormSize, this);
		}

		private void Window_Closed(object sender, System.EventArgs e) {
			Settings.Default.MainFormLocation = new System.Drawing.Point((int)Left, (int)Top);
			Settings.Default.MainFormSize = new System.Drawing.Size((int)Width, (int)Height);
			Settings.Default.Save();
		}

		private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {

		}

		private void closeButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Debug.WriteLine("Window_MouseLeftButtonDown {0} {1}", sender, e);
			this.DragMove();
		}

		internal void OnCommand_NewProject(object sender, ExecutedRoutedEventArgs e) {
			Debug.WriteLine("OnCommand_NewProject {0} {1}", sender, e);
		}

		internal void OnCommand_OpenProject(object sender, ExecutedRoutedEventArgs e) {
			Debug.WriteLine("OnCommand_OpenProject {0} {1}", sender, e);
		}

		internal void OnCommand_Exit(object sender, ExecutedRoutedEventArgs e) {
			this.Close();
		}

		internal void OnCommand_StartStop(object sender, ExecutedRoutedEventArgs e) {
			Debug.WriteLine("OnCommand_StartStop {0} {1}", sender, e);
		}

		internal void OnCommand_SelectAction(object sender, ExecutedRoutedEventArgs e) {
			Debug.WriteLine("OnCommand_SelectAction {0} {1}", sender, e);
		}
	}



}
