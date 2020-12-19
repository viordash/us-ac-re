using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UsAcRe.Core.Exceptions;

namespace UsAcRe.Player.Reporters {
	public class XUnitReporter : IReporter {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");

		#region inner classes
		class TestCase {
			//public string ClassName { get; set; }
			public string Name { get; set; }
			public TimeSpan Time { get; set; }
			public TestFailedException Error { get; set; }
		}
		#endregion

		readonly List<TestCase> testCases = new List<TestCase>();

		public void Fail(string name, TimeSpan time, TestFailedException exception) {
			testCases.Add(new TestCase() {
				Name = name,
				Time = time,
				Error = exception
			});
		}

		public void Success(string name, TimeSpan time) {
			logger.Trace("XUnitReporter: {0}, ({1})", name, time.TotalSeconds.ToString("0.###"));
			testCases.Add(new TestCase() {
				Name = name,
				Time = time
			});
		}

		XElement CreateTestcaseElement(string className, TestCase testCase) {
			var element = new XElement("testcase",
					//new XAttribute("classname", className),
					new XAttribute("name", testCase.Name),
					new XAttribute("time", testCase.Time.TotalSeconds.ToString("0.###")));
			if(testCase.Error != null) {
				var xmlFailure = new XElement("failure", testCase.Error.GetBaseException().Message);
				xmlFailure.Add(new XAttribute("type", testCase.Error.GetType().Name));
				xmlFailure.Add(new XAttribute("message", testCase.Error.Action.FailMessage != null ? testCase.Error.Action.FailMessage() : string.Empty));
				element.Add(xmlFailure);
			}
			return element;
		}

		public string Generate(string suiteName) {
			var xmlTestCases = testCases.Select(x => CreateTestcaseElement(suiteName, x));

			var xdoc = new XDocument(new XElement("testsuite",
				new XAttribute("name", suiteName),
				new XAttribute("tests", testCases.Count),
				new XAttribute("errors", 0),
				new XAttribute("failures", testCases.Where(x => x.Error != null).Count()),
				new XAttribute("skip", 0),
				xmlTestCases
				));
			return xdoc.ToString();
		}
	}
}
