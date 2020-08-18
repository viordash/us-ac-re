using System.Threading;

namespace UsAcRe.Services {
	public interface ITestsLaunchingService {
		CancellationToken GetCurrentCancellationToken();
		void Start();
		void Stop();
	}

	public class TestsLaunchingService : ITestsLaunchingService {
		CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

		public TestsLaunchingService() {
			cancelTokenSource = null;
		}

		public CancellationToken GetCurrentCancellationToken() {
			if(cancelTokenSource == null) {
				return new CancellationToken(true);
			}
			return cancelTokenSource.Token;
		}

		public void Start() {
			if(cancelTokenSource != null) {
				cancelTokenSource.Cancel();
				cancelTokenSource.Dispose();
			}
			cancelTokenSource = new CancellationTokenSource();
		}

		public void Stop() {
			if(cancelTokenSource != null) {
				cancelTokenSource.Cancel();
				cancelTokenSource = null;
			}
		}
	}
}
