using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using NGuard;
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

	public class ActionsListModel {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly IScriptBuilder scriptBuilder;
		readonly IFileService fileService;

		public string Name { get; set; }
		public ObservableCollection<ActionsListItem> Items;

		public ActionsListModel(
			IScriptBuilder scriptBuilder,
			IFileService fileService) {
			Guard.Requires(scriptBuilder, nameof(scriptBuilder));
			Guard.Requires(fileService, nameof(fileService));
			this.scriptBuilder = scriptBuilder;
			this.fileService = fileService;
			Items = new ObservableCollection<ActionsListItem>();
		}

		public void Add(BaseAction actionInfo) {
			Items.Add(new ActionsListItem(actionInfo));
			logger.Info("{0}", actionInfo.ExecuteAsScriptSource());
		}

		public void AddRange(IEnumerable<BaseAction> actions) {
			foreach(var actionInfo in actions) {
				Add(actionInfo);
			}
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