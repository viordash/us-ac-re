using System.Collections.Generic;
using System.Drawing;
using System.Windows.Automation;
using UsAcRe.Helpers;
using UsAcRe.WindowsSystem;

namespace UsAcRe.UIAutomationElement {

	public class ElementFromPoint {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		AutomationElement Element;
		public Point ElementCoord { get; set; }

		public ElementFromPoint(Point elementCoord) {
			ElementCoord = elementCoord;
			DetermineElementUnderPoint();
		}

		public override string ToString() {
			if(Element != null) {
				return string.Format($"{nameof(ElementFromPoint)} ({ElementCoord.X}, {ElementCoord.Y}). Name: {NamingHelpers.Escape(Element.Current.Name, 50)}, " +
					$"{Element.Current.ControlType.ProgrammaticName}, {Element.Current.BoundingRectangle}");
			} else {
				return string.Format($"{nameof(ElementFromPoint)} ({ElementCoord.X}, {ElementCoord.Y}). No element");
			}
		}

		void DetermineElementUnderPoint() {
			try {
				logger.Info("DetermineElementUnderPoint: {0}", ElementCoord);
				Element = AutomationElement.FromPoint(new System.Windows.Point(ElementCoord.X, ElementCoord.Y));
			} catch {
				var hwnd = WinAPI.WindowFromPoint(new WinAPI.POINT(ElementCoord.X, ElementCoord.Y));
				var elementByHandle = AutomationElement.FromHandle(hwnd);
				Element = GetElementThatBoundingRectangleContainsPoint(elementByHandle, 0);
			}
		}

		AutomationElement GetElementThatBoundingRectangleContainsPoint(AutomationElement root, int nestedLevel) {
			if(nestedLevel++ > 10) {
				return null;
			}
			foreach(var el in EnumerateChildElementsBack(root)) {
				logger.Trace("             GetElementThatBoundingRectangleContainsPoint {3}: {0}; {1}; {2}", NamingHelpers.Escape(el.Current.Name, 300), el.Current.ControlType.ProgrammaticName,
						el.Current.BoundingRectangle, nestedLevel);
				if(el.Current.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
					return GetElementThatBoundingRectangleContainsPoint(el, nestedLevel);
				}
			}
			foreach(var el in EnumerateChildElements(root)) {
				logger.Trace("             GetElementThatBoundingRectangleContainsPoint FORWARD {3}: {0}; {1}; {2}", NamingHelpers.Escape(el.Current.Name, 300), el.Current.ControlType.ProgrammaticName,
						el.Current.BoundingRectangle, nestedLevel);
				if(el.Current.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
					return GetElementThatBoundingRectangleContainsPoint(el, nestedLevel);
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
