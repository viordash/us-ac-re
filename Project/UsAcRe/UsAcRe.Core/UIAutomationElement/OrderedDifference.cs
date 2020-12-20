using System;

namespace UsAcRe.Core.UIAutomationElement {
	public class OrderedDifference {
		public int Weight { get; set; }
		public Func<string> Difference { get; set; }

		public OrderedDifference(int weight, Func<string> difference) {
			Weight = weight;
			Difference = difference;
		}
	}
}
