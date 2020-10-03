using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using UsAcRe.Recorder.UI.Models;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		MainMenuModel MainMenuModel;

		public MainWindow() {
			InitializeComponent();

			MainMenuModel = new MainMenuModel();
			MainMenuModel.AssignControl(mainMenu, this);
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
