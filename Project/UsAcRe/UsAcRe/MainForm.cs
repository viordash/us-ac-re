using System;
using System.Diagnostics;
using System.Windows.Forms;
using CommonServiceLocator;
using NLog.Windows.Forms;
using UsAcRe.Highlighter;
using UsAcRe.Mouse;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		ElementHighlighter elementHighlighter = null;
		ElementFromPoint elementFromPoint = null;
		ElementHighlighter mouseClickBlocker = null;

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }

		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if(logger == null) {
				logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			CloseMouseClickBlocker();
			CloseHighlighter();
		}

		private void btnStart_Click(object sender, EventArgs e) {
			if(btnStart.Checked) {
				logger.Warn("Start");
				StartHooks();
			} else {
				StopHooks();
				logger.Warn("Stop");
			}
		}

		void StartHooks() {
			MouseHook.Start();
			MouseHook.OnMouseEvent -= MouseEventHook;
			MouseHook.OnMouseEvent += MouseEventHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.OnMouseMove += MouseMoveHook;
		}

		void StopHooks() {
			MouseHook.OnMouseEvent -= MouseEventHook;
			MouseHook.OnMouseMove -= MouseMoveHook;
			MouseHook.Stop();

		}

		void MouseEventHook(object sender, Mouse.MouseEventArgs e) {
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


		void ShowHighlighter() {
			CloseHighlighter();
			elementHighlighter = new ElementHighlighter(elementFromPoint);
			elementHighlighter.StartHighlighting();
		}

		void CloseHighlighter() {
			if(elementHighlighter != null) {
				var highlighter = elementHighlighter;
				highlighter.StopHighlighting();
				elementHighlighter = null;
			}
		}

		void ShowMouseClickBlocker(WinAPI.POINT coord) {
			CloseMouseClickBlocker();
			mouseClickBlocker = new ElementHighlighter(new System.Windows.Rect(coord.x - 3, coord.y - 3, 6, 6), string.Empty);// MouseClickBlocker();
																															  //mouseClickBlocker.Show(coord);
			mouseClickBlocker.StartHighlighting();
			Debug.WriteLine($"ShowMouseClickBlocker :   coord:{coord}");
		}

		void CloseMouseClickBlocker() {
			if(mouseClickBlocker != null) {
				mouseClickBlocker.StopHighlighting();//				mouseClickBlocker.Hide();
				mouseClickBlocker = null;
			}
		}

		void MouseMoveHook(object sender, Mouse.MouseMoveArgs e) {
			BeginInvoke((Action<Mouse.MouseMoveArgs>)((args) => {
				if(elementFromPoint != null) {
					elementFromPoint.BreakOperations();
					elementFromPoint = null;
				}

				if(args.Stopped) {
					ShowMouseClickBlocker(args.Coord);
					elementFromPoint = new ElementFromPoint(AutomationElementService, WinApiService, args.Coord, true);
					CloseMouseClickBlocker();
					ShowHighlighter();
					logger.Info(elementFromPoint);
				} else {
					CloseMouseClickBlocker();
					CloseHighlighter();
				}

			}), e);
		}

		private void miClearLog_Click(object sender, EventArgs e) {
			txtLog.Clear();
		}
	}
}
