using System;

namespace UsAcRe.Core.Services {
	public interface IWindowsFormsService {
		void BeginInvoke(Delegate method, params object[] args);
	}
}
