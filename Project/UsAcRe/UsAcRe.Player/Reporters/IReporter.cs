using System;

namespace UsAcRe.Player.Reporters {
	public interface IReporter {
		string Add(string testcase, Exception exception);
	}
}
