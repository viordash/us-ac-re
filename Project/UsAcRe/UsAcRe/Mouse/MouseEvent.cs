using System.Drawing;

namespace UsAcRe.Mouse {
	public class MouseEvent {
		public MouseEvent() {
		}

		public MouseEvent(MouseEvent mouseEvent) {
			Type = mouseEvent.Type;
			DownClickedPoint = mouseEvent.DownClickedPoint;
			UpClickedPoint = mouseEvent.UpClickedPoint;
		}
		public Point DownClickedPoint { get; set; }
		public Point UpClickedPoint { get; set; }
		public MouseActionType Type { get; set; }
	}
}
