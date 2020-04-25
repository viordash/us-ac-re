using System.Collections.Generic;

namespace UsAcRe.Actions {
	public class ActionsList : List<BaseAction> {
		public ActionsList() { }
	}

	public class ActionsContainer {
		public string Name { get; set; }
		public ActionsList Items;


		public ActionsContainer() {
			Items = new ActionsList();

		}
	}
}
