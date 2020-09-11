namespace UsAcRe.Core.UIAutomationElement {
	public class ElementProgram {
		public int Index { get; set; }
		public string FileName { get; set; }

		public ElementProgram(int index, string fileName) {
			Index = index;
			FileName = fileName;
		}
		public override string ToString() {
			return string.Format("[{0}] \"{1}\"", Index, FileName);
		}
	}
}
