using System;
using System.Windows.Forms;
using CommonServiceLocator;
using NLog.Windows.Forms;
using UsAcRe.Actions;
using UsAcRe.Highlighter;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		ElementHighlighter elementHighlighter = null;
		ElementFromPoint elementFromPoint = null;
		ElementHighlighter mouseClickBlocker = null;

		ActionsContainer Actions = new ActionsContainer();

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
				StartHooks();
			} else {
				StopHooks();
			}
		}

		private void miClearLog_Click(object sender, EventArgs e) {
			txtLog.Clear();
		}

		public static void MoveOutFromPoint(int X, int Y) {
			if(Program.MainFormHandle == IntPtr.Zero)
				return;
			WinAPI.RECT rect = new WinAPI.RECT();
			if(!WinAPI.GetWindowRect(Program.MainFormHandle, ref rect))
				return;
			bool mouseClickInMainForm = rect.top <= Y && rect.bottom >= Y && rect.left <= X && rect.right >= X;
			if(!mouseClickInMainForm) {
				return;
			}
			var workScreen = Screen.FromPoint(new System.Drawing.Point(X, Y));
			int newX = X;
			int newY = Y;
			int formWidth = (int)(rect.right - rect.left);
			int formHeight = (int)(rect.bottom - rect.top);
			if((newX + formWidth) > workScreen.Bounds.Right) {
				newX = newX - formWidth - 20;
			} else {
				newX += 20;
			}
			if((newY + formHeight) > workScreen.Bounds.Bottom) {
				newY = newY - formHeight - 20;
			} else {
				newY += 20;
			}

			WinAPI.SetWindowPos(Program.MainFormHandle, WinAPI.HWND_TOPMOST, newX, newY, formWidth, formHeight, 0x10);
		}
	}
}
