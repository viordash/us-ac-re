using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Automation;
using UsAcRe.Helpers;
using UsAcRe.WindowsSystem;

namespace UsAcRe.UIAutomationElement {
	[Serializable]
	public class ElementFromPoint {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");
		List<UIElementPathItem> PathItems = new List<UIElementPathItem>();


		public ElementFromPoint(Point clickedPoint) {

			var el = RetrieveAutomationElementFromPoint(clickedPoint);

		}

		AutomationElement RetrieveAutomationElementFromPoint(Point clickedPoint) {
			try {
				return AutomationElement.FromPoint(new System.Windows.Point(clickedPoint.X, clickedPoint.Y));
			} catch { }
			var hwnd = WinAPI.WindowFromPoint(new WinAPI.POINT(clickedPoint.X, clickedPoint.Y));
			var elementByHandle = AutomationElement.FromHandle(hwnd);
			return GetElementThatBoundingRectangleContainsPoint(elementByHandle, clickedPoint, 0);
		}

		AutomationElement GetElementThatBoundingRectangleContainsPoint(AutomationElement root, Point clickedPoint, int nestedLevel) {
			if(nestedLevel++ > 10) {
				return null;
			}
			foreach(var el in EnumerateChildElementsBack(root)) {
				logger.Trace("             GetElementThatBoundingRectangleContainsPoint {3}: {0}; {1}; {2}", NamingHelpers.Escape(el.Current.Name, 300), el.Current.ControlType.ProgrammaticName,
						el.Current.BoundingRectangle, nestedLevel);
				if(el.Current.BoundingRectangle.Contains(clickedPoint.X, clickedPoint.Y)) {
					return GetElementThatBoundingRectangleContainsPoint(el, clickedPoint, nestedLevel);
				}
			}
			foreach(var el in EnumerateChildElements(root)) {
				logger.Trace("             GetElementThatBoundingRectangleContainsPoint FORWARD {3}: {0}; {1}; {2}", NamingHelpers.Escape(el.Current.Name, 300), el.Current.ControlType.ProgrammaticName,
						el.Current.BoundingRectangle, nestedLevel);
				if(el.Current.BoundingRectangle.Contains(clickedPoint.X, clickedPoint.Y)) {
					return GetElementThatBoundingRectangleContainsPoint(el, clickedPoint, nestedLevel);
				}
			}
			return nestedLevel == 1 ? null : root;
		}

		IEnumerable<AutomationElement> EnumerateChildElementsBack(AutomationElement root) {
			AutomationElement el = TreeWalker.RawViewWalker.GetLastChild(root);
			while(el != null) {
				yield return el;
				el = TreeWalker.RawViewWalker.GetPreviousSibling(el);
			}
		}

		IEnumerable<AutomationElement> EnumerateChildElements(AutomationElement root) {
			AutomationElement el = TreeWalker.RawViewWalker.GetFirstChild(root);
			while(el != null) {
				yield return el;
				el = TreeWalker.RawViewWalker.GetNextSibling(el);
			}
		}
	}
}
