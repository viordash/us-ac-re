using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Helpers;
using UsAcRe.Core.UI.Highlighter;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Recorder.UI.Models;
using UsAcRe.Recorder.UI.Properties;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		readonly ActionsListModel Actions;

		ElementFromPoint elementFromPoint = null;
		ElementHighlighter mouseClickBlocker = null;

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }
		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }
		ISettingsService SettingsService { get { return ServiceLocator.Current.GetInstance<ISettingsService>(); } }

		public MainWindow() {
			InitializeComponent();
			if(!DesignerProperties.GetIsInDesignMode(this)) {
				Actions = new ActionsListModel(SettingsService, new ScriptBuilder(SettingsService));
				ActionsList.ListActions.ItemsSource = Actions.Items;

				//Actions.Add(ElementMatchAction.Record(
				//	new ElementProgram(0, "notepad++.exe"), new List<UiElement>() {
				//		new UiElement(0, "", "Context", "#32768", "", 50009, new System.Windows.Rect(1537, 173, 243, 422)),
				//	}));

				//Actions.Add(ElementMatchAction.Record(
				//	new ElementProgram(0, "notepad++.exe"), new List<UiElement>() {
				//	new UiElement(0, "", "New", "", "", 50000, new System.Windows.Rect(1427, 107, 23, 22)),
				//	new UiElement(0, "", "", "ToolbarWindow32", "", 50021, new System.Windows.Rect(1427, 107, 855, 25)),
				//	new UiElement(0, "", "", "ReBarWindow32", "", 50033, new System.Windows.Rect(1425, 107, 857, 25)),
				//	new UiElement(0, "", "*new 1 - Notepad++", "Notepad++", "", 50032, new System.Windows.Rect(1417, 56, 873, 428)),
				//}));

				//Actions.Add(MouseClickAction.Record(MouseButtonType.Right, new System.Drawing.Point(1537, 173), false));

				//Actions.Add(ElementMatchAction.Record(
				//	new ElementProgram(0, "notepad++.exe"), new List<UiElement>() {
				//		new UiElement(0, "", "Context", "#32768", "", 50009, new System.Windows.Rect(1537, 173, 243, 422)),
				//	}));

				//Actions.Add(MouseClickAction.Record(MouseButtonType.Left, new System.Drawing.Point(2258, 76), false));
			}
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
			if(MainMenu.miStartStop.IsChecked) {
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
