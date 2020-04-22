using System.Windows;

namespace UsAcRe.ClickBlocker {
	public partial class MouseClickBlocker : Window {
		public MouseClickBlocker(double Size) {
			InitializeComponent();
			this.Width = this.Height = MainViewbox.Width = MainViewbox.Height = Size;
			Rotation.CenterX = Rotation.CenterY = Size / 2.0;
		}

		public double Size {
			get { return this.Width; }
			set {
				this.Width = this.Height = MainViewbox.Width = MainViewbox.Height = value;
				Rotation.CenterX = Rotation.CenterY = value / 2.0;
			}
		}

		public void Rotate(double angle) {
			Rotation.Angle += angle;
		}

		//FOR TESTS--------------------------------------------------------------------------------
		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Rotate(10);
		}
		//-----------------------------------------------------------------------------------------
	}
}