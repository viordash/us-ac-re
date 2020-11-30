﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
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

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }
		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }
		ISettingsService SettingsService { get { return ServiceLocator.Current.GetInstance<ISettingsService>(); } }
		IDialogService DialogService { get { return ServiceLocator.Current.GetInstance<IDialogService>(); } }
		IFileService FileService { get { return ServiceLocator.Current.GetInstance<IFileService>(); } }
		IScriptBuilder ScriptBuilder { get { return ServiceLocator.Current.GetInstance<IScriptBuilder>(); } }
		IScriptCompiler ScriptCompiler { get { return ServiceLocator.Current.GetInstance<IScriptCompiler>(); } }

		public MainWindow() {
			InitializeComponent();
			if(!DesignerProperties.GetIsInDesignMode(this)) {
				Actions = new ActionsListModel(ScriptBuilder, FileService);
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
			if(DialogService.Confirmation("Create new project?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK) {
				return;
			}
			Actions.Clear();
		}

		internal async void OnCommand_OpenProject(object sender, ExecutedRoutedEventArgs e) {
			var fileName = DialogService.OpenFileDialog(Constants.TestsFileFilter);

			if(string.IsNullOrEmpty(fileName)) {
				return;
			}
			var sourceCode = FileService.ReadAllText(fileName);
			try {
				using(TestsLaunchingService.Start(true)) {
					await ScriptCompiler.RunTest(sourceCode);
					Actions.AddRange(TestsLaunchingService.ExecutedActions);
				}
			} catch(TestFailedException ex) {
				logger.Error(ex.Message);
				throw;
			}
		}

		internal void OnCommand_SaveProject(object sender, ExecutedRoutedEventArgs e) {
			var fileName = DialogService.SaveFileDialog(Constants.TestsFileFilter);
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
				var fileName = DialogService.OpenFileDialog(Constants.TestsFileFilter);
				if(string.IsNullOrEmpty(fileName)) {
					return;
				}
				Actions.Add(ActionSet.Record(fileName));
				return;
			}
		}
	}



}
