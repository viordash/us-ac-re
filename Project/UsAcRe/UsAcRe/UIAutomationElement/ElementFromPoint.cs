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
		class BoundingRectangleComp : IComparer<System.Windows.Rect> {
			public int Compare(System.Windows.Rect x, System.Windows.Rect y) {
				return (x.Height + x.Width).CompareTo(y.Height + y.Width);
			}
		}
		#endregion

		CacheRequest CacheRequest;
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


		AutomationElement GetElementFromPoint() {
			Debug.WriteLine("");
			Debug.WriteLine("------------------------");
			Debug.WriteLine("ElementCoord: {0}", ElementCoord);
			Debug.WriteLine("");
			var hwnd = WinAPI.WindowFromPoint(new WinAPI.POINT(ElementCoord.X, ElementCoord.Y));
			if(hwnd == IntPtr.Zero) {
				return null;
			}

			var rootWindowHwnd = GetRootWindow(hwnd);
			if(rootWindowHwnd == IntPtr.Zero) {
				return null;
			}

			var rootElement = AutomationElement.FromHandle(rootWindowHwnd);

			try {
				CacheRequest = new CacheRequest();
				CacheRequest.Add(AutomationElement.BoundingRectangleProperty);
				CacheRequest.Add(AutomationElement.NativeWindowHandleProperty);

				using(CacheRequest.Activate()) {
					var rect = new System.Windows.Rect(0, 0, 0, 0);
					var condBoundingRectangle = new PropertyCondition(AutomationElement.BoundingRectangleProperty, rect);
					var condOffscreen = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
					var cond = new AndCondition(new NotCondition(condBoundingRectangle), condOffscreen);

					var elementsUnderPoint = new List<AutomationElement>();
					RetreiveChildrenUnderPoint(rootElement, cond, elementsUnderPoint);
					RemoveParents(rootElement, elementsUnderPoint);
					var element = elementsUnderPoint
						.OrderByDescending(x => GetZOrder(x))
						.ThenBy(x => x.Cached.BoundingRectangle, new BoundingRectangleComp())
						.FirstOrDefault();

					if(element != null) {
						return element;
					}
				}
			} catch { }
			return rootElement;
		}

		void RetreiveChildrenUnderPoint(AutomationElement elementUnderPoint, Condition condition, List<AutomationElement> elements) {
			var childElements = elementUnderPoint.FindAll(TreeScope.Children, condition);
			var elementsUnderPoint = childElements.OfType<AutomationElement>()
				.Where(x => x.Cached.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y))
				.ToList();

			var outsideOfPoint = childElements.OfType<AutomationElement>()
				.Where(x => !x.Cached.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y));

			foreach(var item in outsideOfPoint) {
				var suspectedElements = item.FindAll(TreeScope.Children, condition);
				var suspectedElementsUnderPoint = suspectedElements.OfType<AutomationElement>()
					.Where(x => x.Cached.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y))
					.Except(elementsUnderPoint)
					.ToList();
				if(suspectedElementsUnderPoint.Count > 0) {
					elementsUnderPoint.AddRange(suspectedElementsUnderPoint);
					Debug.WriteLine("		suspected: {0} {1} '{2}', childs: {3}", NamingHelpers.Escape(item.Current.AutomationId, 300),
						item.Current.ClassName, item.Current.ControlType.ProgrammaticName, suspectedElementsUnderPoint.Count());
				}
			}

			Debug.WriteLine("elementsUnderPoint: {0} {1} '{2}' '{3}', childs: {4}", NamingHelpers.Escape(elementUnderPoint.Current.Name, 300),
					NamingHelpers.Escape(elementUnderPoint.Current.AutomationId, 300), elementUnderPoint.Current.ClassName,
					elementUnderPoint.Current.ControlType.ProgrammaticName, elementsUnderPoint.Count());

			if(elementsUnderPoint.Count > 0) {
				elements.AddRange(elementsUnderPoint);
				foreach(var item in elementsUnderPoint) {
					RetreiveChildrenUnderPoint(item, condition, elements);
				}
			}
		}

		IEnumerable<AutomationElement> GetChainOfParents(AutomationElement rootElement, AutomationElement element) {
			var list = new List<AutomationElement>();
			do {
				element = TreeWalker.RawViewWalker.GetParent(element, CacheRequest);
				list.Add(element);
			} while(element != null && !Automation.Compare(rootElement, element));

			return list;
		}

		void RemoveParents(AutomationElement rootElement, IList<AutomationElement> elementsUnderPoint) {
			if(elementsUnderPoint == null || elementsUnderPoint.Count == 0) {
				return;
			}
			int i = 0;
			do {
				var element = elementsUnderPoint[i];
				var parents = GetChainOfParents(rootElement, element);
				var parentsInList = elementsUnderPoint
					.Where(x => parents.Any(p => Automation.Compare(p, x)))
					.ToList();
				foreach(var parent in parentsInList) {
					elementsUnderPoint.Remove(parent);
				}
			} while(++i < elementsUnderPoint.Count);
		}

		int GetZOrder(AutomationElement element) {
			if(element == null) {
				return -1;
			}
			var hWnd = new IntPtr(element.Cached.NativeWindowHandle);
			if(hWnd == IntPtr.Zero) {
				var parentEl = TreeWalker.RawViewWalker.GetParent(element, CacheRequest);
				return GetZOrder(parentEl);
			}
			return GetZOrder(hWnd);
		}

		int GetZOrder(IntPtr hWnd) {
			var lowestHwnd = WinAPI.GetWindow(hWnd, WinAPI.GW_HWNDLAST);
			var z = 0;
			var hwndTmp = lowestHwnd;
			while(hwndTmp != IntPtr.Zero) {
				if(hWnd == hwndTmp) {
					//Debug.WriteLine("{0:X8} z:{1}", hWnd.ToInt32(), z);
					return z;
				}

				hwndTmp = WinAPI.GetWindow(hwndTmp, WinAPI.GW_HWNDPREV);
				z++;
			}
			return -1;
		}


		IntPtr GetRootWindow(IntPtr selectedWindow) {
			var rootHwnd = WinAPI.GetAncestor(selectedWindow, WinAPI.GA_ROOT);
			return rootHwnd;
		}
	}
}
