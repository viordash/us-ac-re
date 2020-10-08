using System.Windows.Input;

namespace UsAcRe.Recorder.UI {
	public class UICommands {
		public static RoutedCommand NewProject { get; }
		public static RoutedCommand OpenProject { get; }
		public static RoutedCommand SaveProject { get; }
		public static RoutedCommand Exit { get; }
		public static RoutedCommand StartStop { get; }

		static UICommands() {
			NewProject = new RoutedCommand("NewProject", typeof(MainWindow));
			OpenProject = new RoutedCommand("OpenProject", typeof(MainWindow));
			SaveProject = new RoutedCommand("SaveProject", typeof(MainWindow));
			Exit = new RoutedCommand("Exit", typeof(MainWindow));
			StartStop = new RoutedCommand("StartStop", typeof(MainWindow));
		}

		public static void AssignCommand(CommandBinding commandBinding, MainWindow mainWindow) {
			if(commandBinding.Command == NewProject) {
				commandBinding.Executed += mainWindow.OnCommand_NewProject;
				return;
			}
			if(commandBinding.Command == OpenProject) {
				commandBinding.Executed += mainWindow.OnCommand_OpenProject;
				return;
			}
			if(commandBinding.Command == SaveProject) {
				commandBinding.Executed += mainWindow.OnCommand_SaveProject;
				return;
			}
			if(commandBinding.Command == Exit) {
				commandBinding.Executed += mainWindow.OnCommand_Exit;
				return;
			}
			if(commandBinding.Command == StartStop) {
				commandBinding.Executed += mainWindow.OnCommand_StartStop;
				return;
			}
		}
	}
}
