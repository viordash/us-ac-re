using System;
using CommandLine;
using CommandLine.Text;

namespace UsAcRe.Player {
	class Program {
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
				  Console.WriteLine(helpText);
			  });

		}

		static void Run(Options options) {
			Console.WriteLine(options.Filename);
		}
	}
}
