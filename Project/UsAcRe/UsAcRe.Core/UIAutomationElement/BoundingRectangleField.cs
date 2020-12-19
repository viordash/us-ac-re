﻿using System.Windows;
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

		public void Compare(BoundingRectangleField other, ElementCompareParameters parameters) {
			if(parameters.CompareLocation && !DimensionsHelper.AreLocationEquals(Value.Location, other.Value.Location, parameters.LocationToleranceInPercent,
					System.Windows.Forms.Screen.PrimaryScreen.WorkingArea)) {
				throw new ElementMismatchExceptions(string.Format("left.Location != right.Location ({0}) != ({1}), tol:{2}%", Value.Location, other.Value.Location,
						parameters.LocationToleranceInPercent));
			}
			if(parameters.CompareSizes && !DimensionsHelper.AreSizeEquals(Value.Size, other.Value.Size, parameters.SizeToleranceInPercent)) {
				throw new ElementMismatchExceptions(string.Format("left.Size != right.Size ({0}) != ({1}), tol:{2}%", Value.Size, other.Value.Size,
						parameters.SizeToleranceInPercent));
			}
		}

		public override string ToString() {
			return Value.ToString();
		}

		public string ForNew() {
			return Value.ForNew();
		}
	}
}
