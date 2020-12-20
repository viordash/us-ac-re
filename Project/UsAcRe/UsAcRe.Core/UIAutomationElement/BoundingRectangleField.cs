using System;
using System.Windows;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UIAutomationElement {
	public class BoundingRectangleField : IElementField<BoundingRectangleField> {
		public Rect Value { get; set; }

		public BoundingRectangleField(Rect value) {
			Value = value;
		}

		public Func<string> Differences(BoundingRectangleField other, ElementCompareParameters parameters, int attemptNumber) {
			if(parameters.CompareLocation && !DimensionsHelper.AreLocationEquals(Value.Location, other.Value.Location, parameters.LocationToleranceInPercent,
					System.Windows.Forms.Screen.PrimaryScreen.WorkingArea)) {
				return () => string.Format("this.Location != other.Location ({0}) != ({1}), tol:{2}%", Value.Location, other.Value.Location,
						parameters.LocationToleranceInPercent);
			}
			if(parameters.CompareSizes && !DimensionsHelper.AreSizeEquals(Value.Size, other.Value.Size, parameters.SizeToleranceInPercent)) {
				return () => string.Format("this.Size != other.Size ({0}) != ({1}), tol:{2}%", Value.Size, other.Value.Size,
						parameters.SizeToleranceInPercent);
			}
			return null;
		}

		public override string ToString() {
			return Value.ToString();
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
