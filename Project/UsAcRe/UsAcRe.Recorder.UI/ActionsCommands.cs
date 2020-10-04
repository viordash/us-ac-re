using System.Windows.Input;

namespace UsAcRe.Recorder.UI {
	public class ActionsCommands {
		public static RoutedCommand Pause { get; }

		static ActionsCommands() {
			Pause = new RoutedCommand("Pause", typeof(MainWindow));
		}

		public static void AssignCommand(CommandBinding commandBinding, MainWindow mainWindow) {
			if(commandBinding.Command == Pause) {
				commandBinding.Executed += mainWindow.OnCommand_SelectAction;
				return;
			}
		}
	}
}
