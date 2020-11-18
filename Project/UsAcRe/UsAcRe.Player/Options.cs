using CommandLine;

namespace UsAcRe.Player {
	public class Options {
		[Value(0, Required = true, HelpText = "Input filename.")]
		public string Filename { get; set; }
	}
}
