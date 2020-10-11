using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

		public static readonly DependencyProperty IsStoppedProperty =
			DependencyProperty.Register(nameof(IsStopped), typeof(bool), typeof(MainMenu), new PropertyMetadata(true));

		public bool IsStopped {
			get { return (bool)GetValue(IsStoppedProperty); }
			set { SetValue(IsStoppedProperty, value); }
		}

		public event ExecutedRoutedEventHandler OnNewProjectCommand;
		public event ExecutedRoutedEventHandler OnOpenProjectCommand;
		public event ExecutedRoutedEventHandler OnSaveProjectCommand;
		public event ExecutedRoutedEventHandler OnExitCommand;
		public event ExecutedRoutedEventHandler OnStartStopCommand;

		public bool SaveEnable { get; set; } = false;

		public MainMenu() {
			InitializeComponent();
			MainMenuModel = new MainMenuModel();
			miNewProject.CommandBindings.Add(new CommandBinding(UICommands.NewProject, (s, e) => OnNewProjectCommand?.Invoke(s, e)));
			miOpenProject.CommandBindings.Add(new CommandBinding(UICommands.OpenProject, (s, e) => OnOpenProjectCommand?.Invoke(s, e)));
			miSaveProject.CommandBindings.Add(new CommandBinding(UICommands.SaveProject,
				(s, e) => OnSaveProjectCommand?.Invoke(s, e),
				(s, e) => e.CanExecute = SaveEnable && !miStartStop.IsChecked
			));
			miExit.CommandBindings.Add(new CommandBinding(UICommands.Exit, (s, e) => OnExitCommand?.Invoke(s, e)));
			miStartStop.CommandBindings.Add(new CommandBinding(UICommands.StartStop, (s, e) => OnStartStopCommand?.Invoke(s, e)));

			if(!DesignerProperties.GetIsInDesignMode(this)) {
				MainMenuModel.AssignControl(miActions, (MainWindow)Application.Current.MainWindow);
			}
		}
	}
}
