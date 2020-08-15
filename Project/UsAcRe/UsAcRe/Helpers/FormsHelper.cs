using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UsAcRe.Helpers {
	public class FormsHelper {
		public static TForm OpenOrCreateNew<TForm>() where TForm : Form, new() {
			var form = Application.OpenForms.OfType<TForm>().FirstOrDefault();
			if(form == null) {
				form = new TForm();
			}
			return form;
		}

		public static void LoadLocation(Point point, Form form) {
			if(!point.IsEmpty
				&& point.X + form.Width >= 0
				&& point.Y + form.Height >= 0
				&& Screen.PrimaryScreen.Bounds.Right > point.X
				&& Screen.PrimaryScreen.Bounds.Bottom > point.Y) {
				form.Left = point.X;
				form.Top = point.Y;
			}
		}

		public static void LoadSize(Size size, Form form) {
			if(!size.IsEmpty
				&& Screen.PrimaryScreen.Bounds.Size.Width > size.Width
				&& Screen.PrimaryScreen.Bounds.Size.Height > size.Height) {
				form.Width = size.Width;
				form.Height = size.Height;
			}
		}

		//public static void Load(SerializableDictionary<string, FormLayoutModel> dict, Form form) {
		//	var tag = GetSettingsTag(form);
		//	if(dict != null && dict.TryGetValue(tag, out FormLayoutModel formLayoutModel)) {
		//		LoadPosition(formLayoutModel.Point, form);
		//		LoadSize(formLayoutModel.Size, form);
		//	}
		//}

		//public static void Store(SerializableDictionary<string, FormLayoutModel> dict, Form form) {
		//	var tag = GetSettingsTag(form);
		//	dict[tag] = new FormLayoutModel() {
		//		Point = new Point(form.Left, form.Top),
		//		Size = new Size(form.Width, form.Height)
		//	};
		//}
	}
}
