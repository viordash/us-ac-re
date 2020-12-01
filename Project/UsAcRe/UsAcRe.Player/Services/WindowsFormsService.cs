using System;
using System.Threading;
using UsAcRe.Core.Services;

namespace UsAcRe.Player.Services {
	public class WindowsFormsService : IWindowsFormsService {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Player");
		readonly Thread staThread;

		public WindowsFormsService() {
			staThread = new Thread(new ThreadStart(() => {
				logger.Trace("WindowsFormsService start UI thread");
				System.Windows.Threading.Dispatcher.Run();
			}));
			staThread.SetApartmentState(ApartmentState.STA);
			staThread.IsBackground = true;
			staThread.Start();
		}

		public void BeginInvoke(Delegate method, params object[] args) {
			System.Windows.Threading.Dispatcher
				.FromThread(staThread)
				.BeginInvoke(method, args);
		}
	}
}
