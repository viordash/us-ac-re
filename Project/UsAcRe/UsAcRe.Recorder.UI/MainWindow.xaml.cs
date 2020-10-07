using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Helpers;
using UsAcRe.Core.UI.Highlighter;
using UsAcRe.Player.Actions;
using UsAcRe.Recorder.UI.Models;
using UsAcRe.Recorder.UI.Properties;
using UsAcRe.Recorder.UIAutomationElement;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		readonly ActionsContainer Actions;

		ElementFromPoint elementFromPoint = null;
		ElementHighlighter mouseClickBlocker = null;

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }
		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }
		ISettingsService SettingsService { get { return ServiceLocator.Current.GetInstance<ISettingsService>(); } }

		public MainWindow() {
			InitializeComponent();
			Actions = new ActionsContainer(SettingsService, new ScriptBuilder(SettingsService));
		}

		private void Window_Initialized(object sender, System.EventArgs e) {
			WindowsHelper.LoadLocation(Settings.Default.MainFormLocation, this);
			WindowsHelper.LoadSize(Settings.Default.MainFormSize, this);
		}

		private void Window_Closed(object sender, System.EventArgs e) {
			Settings.Default.MainFormLocation = new System.Drawing.Point((int)Left, (int)Top);
			Settings.Default.MainFormSize = Bounds.Size;
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
			if(mainMenu.miStartStop.IsChecked) {
				Actions.Items.Clear();
				StartHooks();
			} else {
				StopHooks();
			}
		}

		internal void OnCommand_SelectAction(object sender, ExecutedRoutedEventArgs e) {
			Debug.WriteLine("OnCommand_SelectAction {0} {1}", sender, e);
		}

		private void MainMenu_OnNewProjectCommand(object sender, ExecutedRoutedEventArgs e) {

		}
	}



}
