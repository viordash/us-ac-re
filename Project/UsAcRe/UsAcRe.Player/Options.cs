using CommandLine;

namespace UsAcRe.Player {
	public class Options {
		[Value(0, Required = true, HelpText = "Input filename.")]
		public string Filename { get; set; }

		[Option("results-path", Required = false, HelpText = "test results path", Default = ".\\Results")]
		public string TestResultsPath { get; set; }

		[Option("check-by-value", Required = false, HelpText = "check value on comparing", Default = true)]
		public bool CheckByValue { get; set; }

		[Option("click-tolerance", Required = false, HelpText = "click position tolerance, percent", Default = 50)]
		public int ClickPositionToleranceInPercent { get; set; }

		[Option("location-tolerance", Required = false, HelpText = "location change tolerance, percent", Default = null)]
		public int? LocationToleranceInPercent { get; set; }

		[Option("search-nesting-level", Required = false, HelpText = "element search nesting level", Default = 5)]
		public int ElementSearchNestingLevel { get; set; }

		[Option("screenshot", Required = false, HelpText = "take screenshot when test fails", Default = true)]
		public bool Screenshot { get; set; }
	}
}
