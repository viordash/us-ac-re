using System;
using System.Threading;
using System.Windows.Forms;
using CommonServiceLocator;
using NLog.Windows.Forms;
using UsAcRe.Highlighter;
using UsAcRe.Mouse;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;
using UsAcRe.ClickBlocker;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		bool stopSearchElement;
		ElementHighlighter elementHighlighter = null;

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }

		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);

			//----------------------------------------------
			MouseClickBlocker mouseClickBlocker = new MouseClickBlocker(30);
			mouseClickBlocker.Show();
			mouseClickBlocker.Size = 30;
			mouseClickBlocker.Rotate(10);
			//----------------------------------------------
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if(logger == null) {
				logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			stopSearchElement = true;
			CloseHighlighter();
		}

		private void btnStart_Click(object sender, EventArgs e) {
			if(btnStart.Checked) {
				logger.Warn("Start");
				stopSearchElement = false;
				//SearchElement();
				StartHooks();
			} else {
				stopSearchElement = true;
				StopHooks();
				logger.Warn("Stop");
			}
		}

		void SearchElement() {
			new Thread(delegate () {
				var lastMouseMoved = DateTime.Now;
				bool moved = false;
				var prevPoint = new WinAPI.POINT();
				while(!stopSearchElement) {
					var pt = WinApiService.GetMousePosition();
					if(!pt.WithBoundaries(prevPoint, 10)) {
						lastMouseMoved = DateTime.Now;
						prevPoint = pt;
						moved = true;
						CloseHighlighter();
					} else if(moved && (DateTime.Now - lastMouseMoved).TotalMilliseconds >= 500) {
						try {
							var elementFromPoint = new ElementFromPoint(AutomationElementService, WinApiService, pt, false);
							BeginInvoke((MethodInvoker)delegate () {
								CloseHighlighter();
								elementHighlighter = new ElementHighlighter(elementFromPoint);
								elementHighlighter.StartHighlighting();
							});

							logger.Info(elementFromPoint);
						} catch { }
						moved = false;
					}
				}
			}).Start();
		}

		void CloseHighlighter() {
			if(elementHighlighter != null) {
				var highlighter = elementHighlighter;
				BeginInvoke((MethodInvoker)delegate () {
					highlighter.StopHighlighting();
				});
				elementHighlighter = null;
			}
		}

		void StartHooks() {
			MouseHook.Start();
			MouseHook.OnMouseEvent -= MouseEvent;
			MouseHook.OnMouseEvent += MouseEvent;
		}

		void StopHooks() {
			MouseHook.OnMouseEvent -= MouseEvent;
			MouseHook.Stop();

		}

		void MouseEvent(object sender, Mouse.MouseEventArgs e) {
			BeginInvoke((Action<Mouse.MouseEventArgs>)((args) => {
				if(args.Event == null) {
					return;
				}
				if(Bounds.Contains(args.Event.DownClickedPoint.X, args.Event.DownClickedPoint.Y)
					|| (Bounds.Contains(args.Event.UpClickedPoint.X, args.Event.UpClickedPoint.Y))) {
					return;
				}
				logger.Info($"MouseEvent: {args.Event.Type}, down:{args.Event.DownClickedPoint}, up:{args.Event.UpClickedPoint}");
			}), e);
		}
	}
}
