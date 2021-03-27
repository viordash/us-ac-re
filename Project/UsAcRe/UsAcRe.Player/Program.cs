using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace UsAcRe.Player {
	[SupportedOSPlatform("windows")]
	class Program {
		static NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");

		static async Task Main(string[] args) {
			NLog.LogManager.Configuration.Variables["logDirectory"] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

			var parser = new Parser(with => {
				with.HelpWriter = null;
				with.CaseInsensitiveEnumValues = true;
			});
			var parserResult = parser.ParseArguments<Options>(args);

			await parserResult
			  .WithNotParsed(x => {
				  var helpText = HelpText.AutoBuild(parserResult, h => {
					  h.AutoHelp = false;
					  h.AutoVersion = true;
					  h.AdditionalNewLineAfterOption = false;
					  return HelpText.DefaultParsingErrorsHandler(parserResult, h);
				  }, e => e);
				  logger.Info(helpText);
				  System.Environment.Exit(-1);
			  })
			  .WithParsedAsync(opt => {
				  logger.Info(HeadingInfo.Default);
				  logger.Info(CopyrightInfo.Default);
				  logger.Info(string.Join(", ", args));
				  Startup.BuildServiceProvider(opt);
				  return Run(opt);
			  });

			Console.ReadKey();
		}

		static async Task Run(Options options) {
			try {
				await new Runner().Start(options.Filename);
			} catch(Exception ex) {
				logger.Error(ex.GetBaseException().Message);
			}
		}
	}
}
