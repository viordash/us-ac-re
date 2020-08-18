using System.Collections.Generic;
using System.Text;
using System.Linq;
using UsAcRe.Extensions;
using UsAcRe.UIAutomationElement;
using System.Threading.Tasks;
using System.Diagnostics;

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

		protected override async Task ExecuteCoreAsync() {
			await SafeActionAsync(DoWorkAsync);
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
			return string.Format("await new {0}({1}, {2}, {3}).{4}()", nameof(ElementMatchAction), Program.ForNew(), SearchPath.ForNew(), TimeoutMs.ForNew(),
				nameof(ElementMatchAction.ExecuteAsync));
		}

		UiElement GetElement() {
			var rootElement = automationElementService.GetRootElement(Program);
			if(rootElement == null) {
				return null;
			}
			return null;
		}

		async ValueTask DoWorkAsync() {
			await Task.Run(() => {
				int i = 0;
				var stopwatch = Stopwatch.StartNew();
				while(!cancellationToken.IsCancellationRequested && stopwatch.Elapsed.TotalMilliseconds < TimeoutMs) {


					++i;
				}
			});
		}
	}
}
