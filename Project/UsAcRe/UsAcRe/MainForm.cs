using System;
using System.IO;
using System.Windows.Forms;
using CommonServiceLocator;
using NLog.Windows.Forms;
using UsAcRe.Actions;
using UsAcRe.Helpers;
using UsAcRe.Highlighter;
using UsAcRe.Properties;
using UsAcRe.Scripts;
using UsAcRe.Services;
using UsAcRe.UIAutomationElement;
using UsAcRe.WindowsSystem;

namespace UsAcRe {
	public partial class MainForm : Form {
		NLog.Logger logger;
		ElementFromPoint elementFromPoint = null;
		ElementHighlighter mouseClickBlocker = null;

		ActionsContainer Actions = new ActionsContainer();

		IAutomationElementService AutomationElementService { get { return ServiceLocator.Current.GetInstance<IAutomationElementService>(); } }
		IWinApiService WinApiService { get { return ServiceLocator.Current.GetInstance<IWinApiService>(); } }
		ITestsLaunchingService TestsLaunchingService { get { return ServiceLocator.Current.GetInstance<ITestsLaunchingService>(); } }

		public MainForm() {
			InitializeComponent();
			RichTextBoxTarget.ReInitializeAllTextboxes(this);
		}

		private void MainForm_Load(object sender, EventArgs e) {
			if(logger == null) {
				logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");
			}
			FormsHelper.LoadLocation(Settings.Default.MainFormLocation, this);
			FormsHelper.LoadSize(Settings.Default.MainFormSize, this);

		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			Settings.Default.MainFormLocation = Location;
			Settings.Default.MainFormSize = Size;
			Settings.Default.Save();

			CloseMouseClickBlocker();
			TestsLaunchingService.CloseHighlighter();
		}

		private void btnStart_Click(object sender, EventArgs e) {
			if(btnStart.Checked) {
				Actions.Items.Clear();
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

		private void btnStoreActions_Click(object sender, EventArgs e) {
			if(saveFileDialog1.ShowDialog() != DialogResult.OK) {
				return;
			}
			Actions.Store(saveFileDialog1.FileName);
		}

		private void btnRunTest_Click(object sender, EventArgs e) {
			if(openFileDialog1.ShowDialog() != DialogResult.OK) {
				return;
			}
			var sourceCode = File.ReadAllText(openFileDialog1.FileName);
			txtLog.Clear();
			txtLog.Text = sourceCode;
			ScriptCompiler.RunTest(sourceCode);
		}
	}
}
