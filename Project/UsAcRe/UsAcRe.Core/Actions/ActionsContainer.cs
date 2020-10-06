using System.IO;
using NGuard;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;

namespace UsAcRe.Player.Actions {
	public class ActionsContainer {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly ISettingsService settingsService;
		readonly IScriptBuilder scriptBuilder;

		public string Name { get; set; }
		public ActionsList Items;

		public ActionsContainer(
			ISettingsService settingsService,
			IScriptBuilder scriptBuilder) {
			Guard.Requires(settingsService, nameof(settingsService));
			Guard.Requires(scriptBuilder, nameof(scriptBuilder));
			this.settingsService = settingsService;
			this.scriptBuilder = scriptBuilder;
			Items = new ActionsList();
		}

		public void Add(BaseAction actionInfo) {
			Items.Add(actionInfo);
			logger.Info("{0}", actionInfo.ExecuteAsScriptSource());
		}

		public void Store(string fileName) {
			var script = scriptBuilder.Generate(Items);
			File.WriteAllText(fileName, script);
		}
	}
}
