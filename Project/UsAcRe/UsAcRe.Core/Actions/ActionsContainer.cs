using GuardNet;
using UsAcRe.Core.Actions;
using UsAcRe.Core.Scripts;
using UsAcRe.Core.Services;

namespace UsAcRe.Player.Actions {
	public class ActionsContainer {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
		readonly IScriptBuilder scriptBuilder;
		readonly IFileService fileService;

		public string Name { get; set; }
		public ActionsList Items;

		public ActionsContainer(
			IScriptBuilder scriptBuilder,
			IFileService fileService) {
			Guard.NotNull(scriptBuilder, nameof(scriptBuilder));
			Guard.NotNull(fileService, nameof(fileService));
			this.scriptBuilder = scriptBuilder;
			this.fileService = fileService;
			Items = new ActionsList();
		}

		public void Add(BaseAction actionInfo) {
			Items.Add(actionInfo);
			logger.Info("{0}", actionInfo.ExecuteAsScriptSource());
		}

		public void Store(string fileName) {
			var script = scriptBuilder.Generate(Items);
			fileService.WriteAllText(fileName, script);
		}
	}
}
