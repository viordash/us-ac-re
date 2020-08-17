using System.Collections.Generic;
using System.Text;
using System.Linq;
using UsAcRe.Extensions;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Actions {
	public class ElementMatchAction : BaseAction {
		public ElementProgram Program { get; set; }
		public List<UiElement> SearchPath { get; set; }
		public int TimeoutMs { get; set; }

		public ElementMatchAction(ElementProgram program, List<UiElement> searchPath, int timeoutMs = 10 * 1000) {
			Program = program;
			SearchPath = searchPath;
			TimeoutMs = timeoutMs;
		}

		protected override void ExecuteCore() {
			//SafeAction(() => DoClick());
		}

		public override string ToString() {
			var sb = new StringBuilder();
			sb.Append(nameof(ElementMatchAction));
			sb.AppendFormat($" {Program}");
			foreach(var item in SearchPath.AsEnumerable().Reverse()) {
				sb.AppendFormat(" -> {0}", item);
			}
			return sb.ToString();
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("new {0}({1}, {2}, {3}).{4}()", nameof(ElementMatchAction), Program.ForNew(), SearchPath.ForNew(), TimeoutMs.ForNew(),
				nameof(ElementMatchAction.Execute));
		}
	}
}
