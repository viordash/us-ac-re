using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Automation;
using UsAcRe.Helpers;
using UsAcRe.WindowsSystem;

namespace UsAcRe.UIAutomationElement {
	public class ElementFromPoint {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		AutomationElement Element;
		public Point ElementCoord { get; }
		public System.Windows.Rect BoundingRectangle {
			get {
				try {
					if(Element != null) {
						return (System.Windows.Rect)Element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
					}
				} catch(Exception ex) {
					logger.Error(ex);
				}
				return System.Windows.Rect.Empty;
			}
		}

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
			Element = GetElementFromPoint();
			if(Element != null) {
				logger.Trace("             DetermineElementUnderPoint 1: {0}; {1}; {2}", NamingHelpers.Escape(Element.Current.AutomationId, 300),
					Element.Current.ControlType.ProgrammaticName, Element.Current.BoundingRectangle);
			}
		}

		#region inner classes
		class MatchedElement {
			public AutomationElement Element;
			public int ZOrder;
			public System.Windows.Rect BoundingRectangle;

			public override string ToString() {
				return string.Format("{0}; Z:{1}; Elem:{2} {3}", nameof(MatchedElement), ZOrder, NamingHelpers.Escape(Element.Current.AutomationId, 300),
					Element.Current.ControlType.ProgrammaticName);
			}
		}

		class BoundingRectangleComp : IComparer<System.Windows.Rect> {
			public int Compare(System.Windows.Rect x, System.Windows.Rect y) {
				return (x.Height + x.Width).CompareTo(y.Height + y.Width);
			}
		}
		#endregion

		//--------------------------
		AutomationElement GetFirstSiblingElement(AutomationElement elementUnderPoint) {
			var parentEl = TreeWalker.RawViewWalker.GetParent(elementUnderPoint);
			bool parentIsDesktop = AutomationElement.RootElement == parentEl;
			if(!parentIsDesktop) {
				elementUnderPoint = TreeWalker.RawViewWalker.GetFirstChild(parentEl);
			} else {
				elementUnderPoint = TreeWalker.RawViewWalker.GetFirstChild(elementUnderPoint);
			}

			return elementUnderPoint;
		}

		List<MatchedElement> ObtainSiblingsUnderPoint(AutomationElement startElement) {

			var matchedElements = new List<MatchedElement>();
			try {
				var iterElement = startElement;
				do {
					var boundingRectangle = iterElement.Current.BoundingRectangle;
					if(!boundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
						continue;
					}
					if(matchedElements.Any(x => Automation.Compare(x.Element, iterElement))) {
						continue;
					}

					matchedElements.Add(new MatchedElement() {
						Element = iterElement,
						ZOrder = GetZOrder(iterElement),
						BoundingRectangle = boundingRectangle
					});
				} while((iterElement = TreeWalker.ControlViewWalker.GetNextSibling(iterElement)) != null);
			} catch(NullReferenceException) {

			}
			return matchedElements;
		}

		bool RawCompareElement(AutomationElement parentElement, int parentZOrder, MatchedElement matchedElement) {
			if(parentElement.Current.BoundingRectangle == matchedElement.Element.Current.BoundingRectangle
				&& parentElement.Current.ControlType == matchedElement.Element.Current.ControlType
				&& parentElement.Current.Name == matchedElement.Element.Current.Name
				&& parentElement.Current.AutomationId == matchedElement.Element.Current.AutomationId
				&& parentZOrder == matchedElement.ZOrder) {
				return true;
			}
			return false;
		}

		List<MatchedElement> ObtainChildsUnderPoint(AutomationElement parentElement) {
			AutomationElement childElement;
			try {
				childElement = TreeWalker.RawViewWalker.GetFirstChild(parentElement);
			} catch(NullReferenceException) {
				childElement = null;
			}
			if(childElement == null) {
				return null;
			}
			var parentElementZOrder = GetZOrder(parentElement);
			var matchedElements = ObtainSiblingsUnderPoint(childElement);
			var childElements = new List<MatchedElement>();
			foreach(var child in matchedElements) {
				if(RawCompareElement(parentElement, parentElementZOrder, child)) {
					continue;
				}
				var elements = ObtainChildsUnderPoint(child.Element);
				if(elements != null) {
					childElements.AddRange(elements);
				}
			}

			var filteredElements = childElements
				.Where(child => !matchedElements.Any(x => Automation.Compare(x.Element, child.Element)));
			matchedElements.AddRange(filteredElements);
			return matchedElements;
		}

		AutomationElement GetElementFromPoint() {
			var hwnd = WinAPI.WindowFromPoint(new WinAPI.POINT(ElementCoord.X, ElementCoord.Y));
			if(hwnd == null) {
				return null;
			}
			var elementUnderPoint = AutomationElement.FromHandle(hwnd);

			var element = GetFirstSiblingElement(elementUnderPoint);
			if(element == null) {
				return null;
			}

			var siblingsUnderPoint = ObtainSiblingsUnderPoint(element);
			var matchedElements = new List<MatchedElement>(siblingsUnderPoint);
			foreach(var item in siblingsUnderPoint) {
				var elements = ObtainChildsUnderPoint(item.Element);
				if(elements != null) {
					matchedElements.AddRange(elements);
				}
			}

			element = matchedElements
				.OrderBy(x => x.ZOrder)
				.ThenBy(x => x.BoundingRectangle, new BoundingRectangleComp())
				.Select(x => x.Element)
				.FirstOrDefault();
			if(element != null) {
				return element;
			} else {
				return elementUnderPoint;
			}
		}

		int GetZOrder(AutomationElement element) {
			if(element == null) {
				return -1;
			}
			var hWnd = new IntPtr(element.Current.NativeWindowHandle);
			if(hWnd == IntPtr.Zero) {
				return -1;
			}

			var lowestHwnd = WinAPI.GetWindow(hWnd, WinAPI.GW_HWNDLAST);

			var z = 0;
			var hwndTmp = lowestHwnd;
			while(hwndTmp != IntPtr.Zero) {
				if(hWnd == hwndTmp) {
					return z;
				}

				hwndTmp = WinAPI.GetWindow(hwndTmp, WinAPI.GW_HWNDPREV);
				z++;
			}

			return -1;
		}
	}
}
