using System.Windows.Forms;
using UsAcRe.Core.Actions;

namespace UsAcRe.Core.Services {
	public class ElementCompareParameters {
		public bool AutomationElementInternal { get; set; }
		public AnchorStyles Anchor { get; set; }
		public bool CompareLocation { get; set; }
		public double LocationToleranceInPercent { get; set; }
		public bool CompareSizes { get; set; }
		public double SizeToleranceInPercent { get; set; }
		public bool NameIsMatchCase { get; set; }
		public bool NameIsMatchWholeWord { get; set; }
		public bool CheckByValue { get; set; }

		public static ElementCompareParameters ForExact() {
			return new ElementCompareParameters() {
				AutomationElementInternal = true,
				Anchor = TestActionConstants.defaultAnchor,
				CompareLocation = true,
				LocationToleranceInPercent = 0,
				CompareSizes = true,
				SizeToleranceInPercent = 0,
				NameIsMatchCase = true,
				NameIsMatchWholeWord = true,
				CheckByValue = true
			};
		}

		public static ElementCompareParameters ForSimilars() {
			return new ElementCompareParameters() {
				AutomationElementInternal = false,
				Anchor = TestActionConstants.defaultAnchor,
				CompareLocation = false,
				LocationToleranceInPercent = 0,
				CompareSizes = false,
				SizeToleranceInPercent = 0,
				NameIsMatchCase = true,
				NameIsMatchWholeWord = true,
				CheckByValue = true
			};
		}
	}
}
