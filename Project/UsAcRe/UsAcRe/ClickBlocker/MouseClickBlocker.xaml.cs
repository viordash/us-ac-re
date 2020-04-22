using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace UsAcRe.ClickBlocker {
	public partial class MouseClickBlocker : Window {

		public MouseClickBlocker(double Size) {
			InitializeComponent();
			Width = Height = MainViewbox.Width = MainViewbox.Height = Size;
			Rotation.CenterX = Rotation.CenterY = Size / 2.0;
			Animate();
		}

		public double Size {
			get { return Width; }
			set {
				Width = Height = MainViewbox.Width = MainViewbox.Height = value;
				Rotation.CenterX = Rotation.CenterY = value / 2.0;
			}
		}

		public void Animate() {
			var doubleAnimation = new DoubleAnimation() {
				From = 0.15,
				To = 0.35,
				AutoReverse = true,
				Duration = new Duration(TimeSpan.FromSeconds(0.3)),
				RepeatBehavior = new RepeatBehavior(1),
				DecelerationRatio = 0.3
			};

			var storyboard = new Storyboard();
			storyboard.Children.Add(doubleAnimation);
			Storyboard.SetTargetName(doubleAnimation, ClickBlockerWindow.Name);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(OpacityProperty));
			Storyboard.SetDesiredFrameRate(doubleAnimation, 60);
			storyboard.Begin(this, true);
		}



		//FOR TESTS--------------------------------------------------------------------------------
		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Animate();
		}
		//-----------------------------------------------------------------------------------------
	}
}