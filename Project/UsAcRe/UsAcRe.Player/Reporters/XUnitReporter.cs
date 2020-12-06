using System;
using System.Collections.Generic;
using UsAcRe.Core.Exceptions;

namespace UsAcRe.Player.Reporters {
	public class XUnitReporter : IReporter {
		readonly NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");

		#region inner classes
		class TestCase {
			public string ClassName { get; set; }
			public string Name { get; set; }
			public TimeSpan Time { get; set; }
			public Exception Error { get; set; }
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
			logger.Warn("{0}|{1}", name, time.ToString());
			testCases.Add(new TestCase() {
				Name = name,
				Time = time
			});
		}


		public string Generate(string suiteName) {
			throw new NotImplementedException();
		}
	}
}
