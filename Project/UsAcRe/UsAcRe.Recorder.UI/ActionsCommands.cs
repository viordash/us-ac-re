using System.Windows.Input;

namespace UsAcRe.Recorder.UI {
	public class ActionsCommands {
		public static RoutedCommand Pause { get; }
		public static RoutedCommand IncludeSet { get; }

		static ActionsCommands() {
			Pause = new RoutedCommand("Pause", typeof(MainWindow));
			IncludeSet = new RoutedCommand("IncludeSet", typeof(MainWindow));
		}

		public static void AssignCommand(CommandBinding commandBinding, MainWindow mainWindow) {
			if(commandBinding.Command == Pause) {
				commandBinding.Executed += mainWindow.OnCommand_SelectAction;
				return;
			}
			if(commandBinding.Command == IncludeSet) {
				commandBinding.Executed += mainWindow.OnCommand_SelectAction;
				return;
			}
		}
	}
}
