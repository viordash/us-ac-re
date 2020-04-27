using System.Collections.Generic;

namespace UsAcRe.Actions {
	public class ActionsList : List<BaseAction> {
		public ActionsList() { }
	}

	public class ActionsContainer {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		public string Name { get; set; }
		public ActionsList Items;


		public ActionsContainer() {
			Items = new ActionsList();
		}

		public void Add(BaseAction actionInfo) {
			Items.Add(actionInfo);
			logger.Info("{0}", actionInfo.ExecuteAsScriptSource());
		}
	}
}
