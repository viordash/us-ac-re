using System;
using CommandLine;
using CommandLine.Text;

namespace UsAcRe.Player {
	class Program {
		static NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");

		static void Main(string[] args) {
			var parser = new Parser(with => with.HelpWriter = null);
			var parserResult = parser.ParseArguments<Options>(args);
			parserResult
			  .WithParsed(opt => Run(opt))
			  .WithNotParsed(x => {
				  var helpText = HelpText.AutoBuild(parserResult, h => {
					  h.AutoHelp = false;
					  h.AutoVersion = true;
					  return HelpText.DefaultParsingErrorsHandler(parserResult, h);
				  }, e => e);
				  logger.Warn(helpText);
			  });
			Console.ReadKey();
		}

		static void Run(Options options) {
			logger.Fatal(options.Filename);
			logger.Error(options.Filename);
			logger.Warn(options.Filename);
			logger.Info(options.Filename);
			logger.Debug(options.Filename);
			logger.Trace(options.Filename);
		}
	}
}
