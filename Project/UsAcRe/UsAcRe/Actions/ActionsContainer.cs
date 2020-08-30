using System.Collections.Generic;
using System.IO;
using CommonServiceLocator;
using UsAcRe.Scripts;
using UsAcRe.Services;

namespace UsAcRe.Actions {
	public class ActionsList : List<BaseAction> {
		public ActionsList() { }
	}

	public class ActionsContainer {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		ISettingsService settingsService { get { return ServiceLocator.Current.GetInstance<ISettingsService>(); } }

		public string Name { get; set; }
		public ActionsList Items;

		public ActionsContainer() {
			Items = new ActionsList();
		}

		public void Add(BaseAction actionInfo) {
			Items.Add(actionInfo);
			logger.Info("{0}", actionInfo.ExecuteAsScriptSource());
		}

		public void Store(string fileName) {
			var scriptBuilder = new ScriptBuilder(Items, settingsService);
			var script = scriptBuilder.Generate();
			File.WriteAllText(fileName, script);
		}
	}
}
