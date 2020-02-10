
using System;
using System.Drawing;
using System.Windows.Forms;
using UsAcRe.WindowsSystem;

namespace UsAcRe.Highlighter {

	class ScreenRectangle : IDisposable {
		private bool visible;
		private Color color;
		private Rectangle location;

		Form form;

		public ScreenRectangle() {
			form = new Form();

			form.FormBorderStyle = FormBorderStyle.None;
			form.ShowInTaskbar = false;
			form.TopMost = true;
			form.Visible = false;
			form.Left = 0;
			form.Top = 0;
			form.Width = 1;
			form.Height = 1;
			form.Show();
			form.Hide();
			form.Opacity = 0.2;

			//set popup style
			int num1 = WinAPI.GetWindowLong(form.Handle, -20);
			WinAPI.SetWindowLong(form.Handle, -20, num1 | 0x80);
			color = Color.Black;
		}

		public bool Visible {
			get { return visible; }
			set {
				visible = value;

				if(value) {
					WinAPI.ShowWindow(form.Handle, 8);
				} else {
					form.Close();
				}
			}
		}

		public Color Color {
			get { return color; }
			set {
				color = value;
				form.BackColor = value;
			}
		}

		public double Opacity {
			get { return form.Opacity; }
			set { form.Opacity = value; }
		}

		public Rectangle Location {
			get { return location; }
			set {
				location = value;
				Layout();
			}
		}

		private void Layout() {
			WinAPI.SetWindowPos(form.Handle, WinAPI.HWND_TOPMOST, location.X, location.Y, location.Width, location.Height, 0x10);
		}

		#region IDisposable Members

		public void Dispose() {
			form.Dispose();
		}

		#endregion
	}
}
