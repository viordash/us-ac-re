using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace UsAcRe.Recorder.UI.Models {
	public class MainMenuItem {
		public string Header { get; private set; }
		public bool IsSeparator { get; set; } = false;
		public ICommand Command { get; private set; }
		public object CommandParameter { get; private set; }
		public ObservableCollection<MainMenuItem> Nodes { get; set; }
		public bool EnableOnStopped { get; set; } = false;

		public MainMenuItem(string header, ICommand command = null, object parameter = null, ObservableCollection<MainMenuItem> nodes = null) {
			Header = header;
			Command = command;
			CommandParameter = parameter;
			Nodes = nodes;
		}
	}

	public class MainMenuModel {
		public ObservableCollection<MainMenuItem> Items { get; set; }

		public MainMenuModel() {
			Items = new ObservableCollection<MainMenuItem>() {
				new MainMenuItem(header: "Add pause",
					nodes: new ObservableCollection<MainMenuItem>() {
						new MainMenuItem( header: "0.5 sec", command: ActionsCommands.Pause, parameter:500 ),
						new MainMenuItem( header: "1 sec", command: ActionsCommands.Pause, parameter:1000 ),
						new MainMenuItem( header: "2 sec", command: ActionsCommands.Pause, parameter:2000 ),
						new MainMenuItem( header: "5 sec", command: ActionsCommands.Pause, parameter:5000 ),
						new MainMenuItem( header: "10 sec", command: ActionsCommands.Pause, parameter:10000 ),
					}),
				new MainMenuItem( "" ) {
					IsSeparator = true
				},
				new MainMenuItem( header: "Include actions", command: ActionsCommands.IncludeSet ) {
					EnableOnStopped = true
				},
			};
		}

		void Assign(MenuItem menuItem, ObservableCollection<MainMenuItem> nodes, MainWindow mainWindow) {
			foreach(var item in nodes) {
				if(item.Command != null) {
					var commandBinding = new CommandBinding();
					commandBinding.Command = item.Command;
					ActionsCommands.AssignCommand(commandBinding, mainWindow);
					menuItem.CommandBindings.Add(commandBinding);
				}
				if(item.Nodes?.Count > 0) {
					Assign(menuItem, item.Nodes, mainWindow);
				}
			}
		}

		public void AssignControl(MenuItem menuItem, MainWindow mainWindow) {
			Assign(menuItem, Items, mainWindow);
			menuItem.ItemsSource = Items;
		}
	}
}
