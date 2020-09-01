using System.Drawing;

namespace UsAcRe.MouseProcess {
	public class MouseEvent {
		public MouseEvent() {
		}

		public MouseEvent(MouseEvent mouseEvent) : this() {
			Type = mouseEvent.Type;
			DownClickedPoint = mouseEvent.DownClickedPoint;
			UpClickedPoint = mouseEvent.UpClickedPoint;
		}

		public MouseEvent(MouseClickEventArgs args) : this() {
			if(args.DoubleClick) {
				Type = args.Button == MouseButtonType.Left
					? MouseActionType.LeftDoubleClick
					: args.Button == MouseButtonType.Right
					? MouseActionType.RightDoubleClick
					: MouseActionType.MiddleDoubleClick;
			} else {
				Type = args.Button == MouseButtonType.Left
					? MouseActionType.LeftClick
					: args.Button == MouseButtonType.Right
					? MouseActionType.RightClick
					: MouseActionType.MiddleClick;
			}
			DownClickedPoint = args.Coord;
			UpClickedPoint = default;
		}

		public MouseEvent(MouseButtonType button, Point startCoord, Point endCoord) : this() {
			Type = button == MouseButtonType.Left
				? MouseActionType.LeftDrag
				: button == MouseButtonType.Right
				? MouseActionType.RightDrag
				: MouseActionType.MiddleDrag;

			DownClickedPoint = startCoord;
			UpClickedPoint = endCoord;
		}

		public Point DownClickedPoint { get; set; }
		public Point UpClickedPoint { get; set; }
		public MouseActionType Type { get; set; }
	}
}
