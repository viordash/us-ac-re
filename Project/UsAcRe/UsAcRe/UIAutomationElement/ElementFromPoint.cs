using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Automation;
using UsAcRe.Helpers;
using UsAcRe.WindowsSystem;

namespace UsAcRe.UIAutomationElement {
	public class ElementFromPoint {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

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

		AutomationElement Element;
		public Point ElementCoord;
		public int Iterations;
		public System.Windows.Rect BoundingRectangle {
			get {
				try {
					if(Element != null) {
						return Element.Current.BoundingRectangle;
					}
				} catch(Exception ex) {
					logger.Error(ex);
				}
				return System.Windows.Rect.Empty;
			}
		}

		public ElementFromPoint(Point elementCoord) {
			Iterations = 0;
			ElementCoord = elementCoord;
			DetermineElementUnderPoint();
		}

		public override string ToString() {
			if(Element != null) {
				return string.Format($"{Iterations}| {nameof(ElementFromPoint)} ({ElementCoord.X}, {ElementCoord.Y}). Name: {NamingHelpers.Escape(Element.Current.Name, 50)}, " +
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

		AutomationElement GetFirstChild(AutomationElement parentElement) {
			try {
				return TreeWalker.RawViewWalker.GetFirstChild(parentElement);
			} catch(NullReferenceException) {
			}
			return null;
		}

		bool ChildIsUnderPoint(AutomationElement parentElement) {
			AutomationElement childElement = GetFirstChild(parentElement);
			if(childElement == null) {
				return false;
			}

			try {
				var iterElement = childElement;
				do {
					var boundingRectangle = iterElement.Current.BoundingRectangle;
					if(boundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
						return true;
					}
					Iterations++;
				} while((iterElement = TreeWalker.ControlViewWalker.GetNextSibling(iterElement)) != null);
			} catch(NullReferenceException) {

			}
			return false;
		}

		List<MatchedElement> ObtainSiblingsUnderPoint(AutomationElement startElement, AutomationElement parentElement, int parentZOrder) {
			var matchedElements = new List<MatchedElement>();
			try {
				var iterElement = startElement;
				do {
					var zOrder = GetZOrder(iterElement);
					var boundingRectangle = iterElement.Current.BoundingRectangle;
					if(!boundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
						bool preventOfInfinityLoop = RawCompareElement(parentElement, parentZOrder, iterElement, zOrder);
						if(preventOfInfinityLoop) {
							continue;
						}
						if(!ChildIsUnderPoint(iterElement)) {
							continue;
						} else {
							Debug.WriteLine("ChildIsUnderPoint 1: {0}; {1}; {2}", NamingHelpers.Escape(Element.Current.AutomationId, 300),
																Element.Current.ControlType.ProgrammaticName, Element.Current.BoundingRectangle);
						}
					}
					if(matchedElements.Any(x => Automation.Compare(x.Element, iterElement))) {
						continue;
					}

					matchedElements.Add(new MatchedElement() {
						Element = iterElement,
						ZOrder = zOrder,
						BoundingRectangle = boundingRectangle
					});
				} while((iterElement = TreeWalker.ControlViewWalker.GetNextSibling(iterElement)) != null);
			} catch(NullReferenceException) {

			}
			return matchedElements;
		}

		bool RawCompareElement(AutomationElement parentElement, int parentZOrder, AutomationElement element, int zOrder) {
			if(parentElement.Current.BoundingRectangle == element.Current.BoundingRectangle
				&& parentElement.Current.ControlType == element.Current.ControlType
				&& parentElement.Current.Name == element.Current.Name
				&& parentElement.Current.AutomationId == element.Current.AutomationId
				&& parentZOrder == zOrder) {
				return true;
			}
			return false;
		}

		List<MatchedElement> ObtainChildsUnderPoint(AutomationElement parentElement) {
			AutomationElement childElement = GetFirstChild(parentElement);
			if(childElement == null) {
				return null;
			}
			var parentElementZOrder = GetZOrder(parentElement);
			var matchedElements = ObtainSiblingsUnderPoint(childElement, parentElement, parentElementZOrder);
			var childElements = new List<MatchedElement>();
			foreach(var child in matchedElements) {
				bool preventOfInfinityLoop = RawCompareElement(parentElement, parentElementZOrder, child.Element, child.ZOrder);
				if(preventOfInfinityLoop) {
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

			var siblingsUnderPoint = ObtainSiblingsUnderPoint(element, null, -1);
			var matchedElements = new List<MatchedElement>(siblingsUnderPoint);
			foreach(var item in siblingsUnderPoint) {
				var elements = ObtainChildsUnderPoint(item.Element);
				if(elements != null) {
					matchedElements.AddRange(elements);
				}
			}

			element = matchedElements
				.Where(x => x.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y))
				.OrderByDescending(x => x.ZOrder)
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
				var parentEl = TreeWalker.RawViewWalker.GetParent(element);
				return GetZOrder(parentEl);
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
