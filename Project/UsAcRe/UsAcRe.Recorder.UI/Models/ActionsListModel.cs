using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using GuardNet;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;
using UsAcRe.Core.UI.Actions;
using UsAcRe.Core.UI.Services;

namespace UsAcRe.Recorder.UI.Models {
	public class ActionsListItem {
		public BaseAction Action { get; private set; }
		public string Code { get { return Action?.ExecuteAsScriptSource(); } }
		public Brush BackgroundColor { get; private set; }

		public ActionsListItem(BaseAction action) {
			Action = action;
			var color = ActionPresentation.BackgroundColor[action.GetType()];
			BackgroundColor = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}
	}

	public class ActionsListChangedEventArgs : EventArgs {
		public ObservableCollection<ActionsListItem> Items { get; private set; }
		public ActionsListChangedEventArgs(ObservableCollection<ActionsListItem> items) {
			Items = items;
		}
	}

	public class ActionsListModel {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly IScriptBuilder scriptBuilder;
		readonly IFileService fileService;
		ObservableCollection<ActionsListItem> Items;

		public string Name { get; set; }

		public delegate void ActionsListChangedEventHandler(object sender, ActionsListChangedEventArgs e);
		public event ActionsListChangedEventHandler ActionsListChanged;

		public ActionsListModel(
			IScriptBuilder scriptBuilder,
			IFileService fileService) {
			Guard.NotNull(scriptBuilder, nameof(scriptBuilder));
			Guard.NotNull(fileService, nameof(fileService));
			this.scriptBuilder = scriptBuilder;
			this.fileService = fileService;
			Items = new ObservableCollection<ActionsListItem>();
		}

		public void Add(BaseAction actionInfo) {
			Items.Add(new ActionsListItem(actionInfo));
			ActionsListChanged?.Invoke(this, new ActionsListChangedEventArgs(Items));
			logger.Info("{0}", actionInfo.ExecuteAsScriptSource());
		}

		public void AddRange(IEnumerable<BaseAction> actions) {
			var actionItems = Items
				.Concat(actions.Select(x => new ActionsListItem(x)));
			Items = new ObservableCollection<ActionsListItem>(actionItems);
			ActionsListChanged?.Invoke(this, new ActionsListChangedEventArgs(Items));
			logger.Info("AddRange {0}", actions.Count());
		}

		public void Store(string fileName) {
			var actionsList = new ActionsList(Items.Select(x => x.Action));
			var script = scriptBuilder.Generate(actionsList);
			fileService.WriteAllText(fileName, script);
		}

		public void Clear() {
			Items.Clear();
		}
	}

}