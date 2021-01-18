using CommandLine;
using UsAcRe.Player.Reporters;

namespace UsAcRe.Player {
	public class Options {
		[Value(0, Required = true, HelpText = "Input filename.")]
		public string Filename { get; set; }

		[Option('p', "results-path", Required = false, HelpText = "test results path", Default = ".\\Results")]
		public string TestResultsPath { get; set; }

		[Option('r', "reporter", Required = false, HelpText = "test report formats [xunit]", Default = ReporterType.xUnit)]
		public ReporterType Reporter { get; set; }

		[Option('v', "check-by-value", Required = false, HelpText = "check value when comparing", Default = true)]
		public bool CheckByValue { get; set; }

		[Option('c', "click-tolerance", Required = false, HelpText = "click position tolerance, percent", Default = 50)]
		public int ClickPositionToleranceInPercent { get; set; }

		[Option('l', "location-tolerance", Required = false, HelpText = "location change tolerance, percent", Default = null)]
		public int? LocationToleranceInPercent { get; set; }

		[Option('s', "screenshot", Required = false, HelpText = "take screenshot when test fails", Default = true)]
		public bool Screenshot { get; set; }
	}
}
