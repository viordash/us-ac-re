using System;
using System.Collections.Generic;

namespace UsAcRe.Core.Actions {

	public class ActionsList : List<BaseAction> {
		public ActionsList(IEnumerable<BaseAction> actions) : this() {
			AddRange(actions);
		}
		public ActionsList() { }
	}

}