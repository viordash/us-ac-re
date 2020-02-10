using System;
using System.Timers;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Highlighter {
	public class FadingBoundingRectangleElementHighLighter : BoundingRectangleElementHighLighter {
		readonly System.Windows.Forms.Form mainForm;

		public static FadingBoundingRectangleElementHighLighter CreateInstance(System.Windows.Forms.Form mainForm, ElementFromPoint elementFromPoint) {
			return new FadingBoundingRectangleElementHighLighter(mainForm, elementFromPoint);
		}
		Timer timer;
		public FadingBoundingRectangleElementHighLighter(System.Windows.Forms.Form mainForm, ElementFromPoint elementFromPoint) : base(elementFromPoint) {
			this.mainForm = mainForm;
		}

		protected override void SetVisibility(bool show) {
			base.SetVisibility(show);

			if(show) {
				SetHidingTimer();
			}
		}

		private void SetHidingTimer() {
			if(timer == null) {
				timer = new Timer {
					Interval = 2000,
					AutoReset = false
				};
				timer.Elapsed += timer_Tick;
			}

			timer.Stop();
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e) {
			timer.Stop();
			mainForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () {
				Dispose();
			});
		}

		protected override void OnDispose() {
			if(timer != null) {
				timer.Stop();
				timer.Elapsed -= timer_Tick;
				timer = null;
			}
			base.OnDispose();

		}
	}
}
