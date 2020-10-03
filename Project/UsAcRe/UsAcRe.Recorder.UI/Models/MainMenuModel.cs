using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace UsAcRe.Recorder.UI.Models {
	public class MainMenuItem {
		public string Header { get; set; }
		public bool IsSeparator { get; set; }
		public ICommand Command { get; set; }
		public ObservableCollection<MainMenuItem> Nodes { get; set; }
	}

	public class MainMenuModel {
		public ObservableCollection<MainMenuItem> Items { get; set; }

		public MainMenuModel() {
			Items = new ObservableCollection<MainMenuItem>() {
				new MainMenuItem() {
					Header = "File",
					Nodes = new ObservableCollection<MainMenuItem>() {
						new MainMenuItem() { Header = "New Project", Command = UICommands.NewProject },
						new MainMenuItem() { Header = "Open Project", Command = UICommands.OpenProject },
						new MainMenuItem() { IsSeparator = true },
						new MainMenuItem() { Header = "Exit", Command = UICommands.Exit }
					}
				},
				new MainMenuItem() { Header = "Start", Command = UICommands.StartStop },
			};
		}



		void Assign(Menu menu, ObservableCollection<MainMenuItem> nodes, MainWindow mainWindow) {
			foreach(var item in nodes) {
				if(item.Command != null) {
					var commandBinding = new CommandBinding();
					commandBinding.Command = item.Command;
					UICommands.AssignCommand(commandBinding, mainWindow);
					menu.CommandBindings.Add(commandBinding);
				}
				if(item.Nodes?.Count > 0) {
					Assign(menu, item.Nodes, mainWindow);
				}
			}
		}

		public void AssignControl(Menu menu, MainWindow mainWindow) {
			Assign(menu, Items, mainWindow);
			menu.ItemsSource = Items;
		}
	}
}
