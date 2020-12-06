using System;
using UsAcRe.Core.Exceptions;

namespace UsAcRe.Player.Reporters {
	public interface IReporter {
		void Fail(string name, TimeSpan time, TestFailedException exception);
		void Success(string name, TimeSpan time);
		string Generate(string suiteName);
	}
}
