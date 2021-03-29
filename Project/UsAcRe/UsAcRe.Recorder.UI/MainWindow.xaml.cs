using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GuardNet;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI;
using UsAcRe.Core.UI.Helpers;
using UsAcRe.Core.UI.Highlighter;
using UsAcRe.Core.UI.Services;
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

		readonly IAutomationElementService automationElementService;
		readonly IWinApiService winApiService;
		readonly ITestsLaunchingService testsLaunchingService;
		readonly IDialogService dialogService;
		readonly IFileService fileService;
		readonly IScriptBuilder scriptBuilder;
		readonly IScriptCompiler scriptCompiler;
		readonly IWindowsFormsService windowsFormsService;

		public MainWindow(
			IAutomationElementService automationElementService,
			IWinApiService winApiService,
			ITestsLaunchingService testsLaunchingService,
			IDialogService dialogService,
			IFileService fileService,
			IScriptBuilder scriptBuilder,
			IScriptCompiler scriptCompiler,
			IWindowsFormsService windowsFormsService,
			ActionsListModel actionsListModel
			) {
			Guard.NotNull(automationElementService, nameof(automationElementService));
			Guard.NotNull(winApiService, nameof(winApiService));
			Guard.NotNull(testsLaunchingService, nameof(testsLaunchingService));
			Guard.NotNull(dialogService, nameof(dialogService));
			Guard.NotNull(fileService, nameof(fileService));
			Guard.NotNull(scriptBuilder, nameof(scriptBuilder));
			Guard.NotNull(scriptCompiler, nameof(scriptCompiler));
			Guard.NotNull(windowsFormsService, nameof(windowsFormsService));
			Guard.NotNull(actionsListModel, nameof(actionsListModel));

			this.automationElementService = automationElementService;
			this.winApiService = winApiService;
			this.testsLaunchingService = testsLaunchingService;
			this.dialogService = dialogService;
			this.fileService = fileService;
			this.scriptBuilder = scriptBuilder;
			this.scriptCompiler = scriptCompiler;
			this.windowsFormsService = windowsFormsService;
			this.Actions = actionsListModel;

			InitializeComponent();
			if(!DesignerProperties.GetIsInDesignMode(this)) {
				Actions.ActionsListChanged += (s, e) => {
					MainMenu.SaveEnable = e.Items.Count > 0;
					ActionsList.ListActions.ItemsSource = e.Items;
				};
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

		private void closeButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			this.DragMove();
		}

		internal void OnCommand_NewProject(object sender, ExecutedRoutedEventArgs e) {
			if(dialogService.Confirmation("Create new project?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK) {
				return;
			}
			Actions.Clear();
		}

		internal async void OnCommand_OpenProject(object sender, ExecutedRoutedEventArgs e) {
			var fileName = dialogService.OpenFileDialog(Constants.TestsFileFilter);

			if(string.IsNullOrEmpty(fileName)) {
				return;
			}
			var sourceCode = fileService.ReadAllText(fileName);
			try {
				using(testsLaunchingService.Start()) {
					await scriptCompiler.RunTest(sourceCode);
					Actions.AddRange(testsLaunchingService.ExecutedActions);
				}
			} catch(TestFailedException ex) {
				logger.Error(ex.Message);
				throw;
			}
		}

		internal void OnCommand_SaveProject(object sender, ExecutedRoutedEventArgs e) {
			var fileName = dialogService.SaveFileDialog(Constants.TestsFileFilter);
			if(string.IsNullOrEmpty(fileName)) {
				return;
			}
			Actions.Store(fileName);
		}

		internal void OnCommand_Exit(object sender, ExecutedRoutedEventArgs e) {
			this.Close();
		}

		internal void OnCommand_StartStop(object sender, ExecutedRoutedEventArgs e) {
			if(MainMenu.miStartStop.IsChecked) {
				StartHooks();
			} else {
				StopHooks();
			}
		}

		internal void OnCommand_SelectAction(object sender, ExecutedRoutedEventArgs e) {
			Debug.WriteLine("OnCommand_SelectAction {0} {1}", sender, e);
			if(e.Command == ActionsCommands.Pause && e.Parameter is int milliseconds) {
				Actions.Add(DelayAction.Record(milliseconds));
				return;
			}
			if(e.Command == ActionsCommands.IncludeSet) {
				var fileName = dialogService.OpenFileDialog(Constants.TestsFileFilter);
				if(string.IsNullOrEmpty(fileName)) {
					return;
				}
				Actions.Add(ActionSet.Record(fileName));
				return;
			}
		}
	}



}
