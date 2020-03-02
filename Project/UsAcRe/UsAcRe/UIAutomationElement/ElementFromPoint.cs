using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using NGuard;
using UsAcRe.Helpers;
using UsAcRe.Services;
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

		readonly IAutomationElementService automationElementService;
		readonly IWinApiService winApiService;
		readonly WinAPI.POINT elementCoord;
		readonly bool detailedSearch;

		CacheRequest cacheRequest;
		AutomationElement specificElement;

		public System.Windows.Rect BoundingRectangle {
			get {
				try {
					if(specificElement != null) {
						return specificElement.Current.BoundingRectangle;
					}
				} catch(Exception ex) {
					logger.Error(ex);
				}
				return System.Windows.Rect.Empty;
			}
		}

		public ElementFromPoint(
			IAutomationElementService automationElementService,
			IWinApiService winApiService,
			WinAPI.POINT elementCoord,
			bool detailedSearch) {
			Guard.Requires(automationElementService, nameof(automationElementService));
			Guard.Requires(winApiService, nameof(winApiService));

			this.automationElementService = automationElementService;
			this.winApiService = winApiService;
			this.elementCoord = elementCoord;
			this.detailedSearch = detailedSearch;
			DetermineElementUnderPoint();
		}

		public override string ToString() {
			if(specificElement != null) {
				return string.Format($"{nameof(ElementFromPoint)} ({elementCoord.x}, {elementCoord.y}). Name: {NamingHelpers.Escape(specificElement.Current.Name, 50)}, " +
					$"{specificElement.Current.ControlType.ProgrammaticName}, {specificElement.Current.BoundingRectangle}");
			} else {
				return string.Format($"{nameof(ElementFromPoint)} ({elementCoord.x}, {elementCoord.y}). No element");
			}
		}

		void DetermineElementUnderPoint() {
			specificElement = GetElementFromPoint();
			logger.Trace("             DetermineElementUnderPoint 1: {0}; {1}", automationElementService.BuildFriendlyInfo(specificElement), specificElement.Current.BoundingRectangle);
		}


		AutomationElement GetElementFromPoint() {
			Debug.WriteLine("");
			Debug.WriteLine("------------------------");
			Debug.WriteLine("ElementCoord: {0}", elementCoord);
			Debug.WriteLine("");

			if(!detailedSearch) {
				try {
					var elementWithPoint = automationElementService.FromPoint(new System.Windows.Point(elementCoord.x, elementCoord.y));
					return elementWithPoint;
				} catch { }
			}

			var hwnd = winApiService.GetWindow(elementCoord);
			if(hwnd == IntPtr.Zero) {
				return null;
			}

			var rootWindowHwnd = GetRootWindow(hwnd);
			if(rootWindowHwnd == IntPtr.Zero) {
				return null;
			}

			cacheRequest = new CacheRequest();
			cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
			cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
			cacheRequest.Add(AutomationElement.ControlTypeProperty);

			using(cacheRequest.Activate()) {
				var rootElement = automationElementService.FromHandle(rootWindowHwnd);
				var rect = new System.Windows.Rect(0, 0, 0, 0);
				var condBoundingRectangle = new PropertyCondition(AutomationElement.BoundingRectangleProperty, rect);
				var condOffscreen = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
				var cond = new AndCondition(new NotCondition(condBoundingRectangle), condOffscreen);

				try {
					var elementsUnderPoint = new List<AutomationElement>();

					RetreiveChildrenUnderPoint(rootElement, cond, elementsUnderPoint);
					RemoveParents(rootElement, elementsUnderPoint);
					var element = elementsUnderPoint
						.OrderByDescending(x => GetTreeOrder(rootElement, x))
						.ThenByDescending(x => GetZOrder(x))
						.ThenBy(x => x.Cached.BoundingRectangle, new BoundingRectangleComp())
						.FirstOrDefault();

					if(element != null) {
						return element;
					}

				} catch(Exception ex) {
					if(ex is OperationCanceledException) {
						throw;
					}
				}
				return rootElement;
			}
		}

		void RetreiveChildrenUnderPoint(AutomationElement elementUnderPoint, Condition condition, List<AutomationElement> elements) {
			BreakOperationsIfCoordChanged();
			var childElements = GetChildren(elementUnderPoint, condition);
			var elementsUnderPoint = childElements.OfType<AutomationElement>()
				.Where(x => x.Cached.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
				.ToList();

			var outsideOfPoint = childElements.OfType<AutomationElement>()
				.Where(x => !x.Cached.BoundingRectangle.Contains(elementCoord.x, elementCoord.y));

			foreach(var item in outsideOfPoint) {
				var suspectedElements = GetChildren(item, condition);

				var suspectedElementsUnderPoint = suspectedElements.OfType<AutomationElement>()
					.Where(x => x.Cached.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
					.Except(elementsUnderPoint)
					.ToList();
				if(suspectedElementsUnderPoint.Count > 0) {
					elementsUnderPoint.AddRange(suspectedElementsUnderPoint);
					Debug.WriteLine("		suspected: {0}, childs: {1}", automationElementService.BuildFriendlyInfo(item), suspectedElementsUnderPoint.Count());
				}
			}

			Debug.WriteLine("elementsUnderPoint: {0}, childs: {1}", automationElementService.BuildFriendlyInfo(elementUnderPoint), elementsUnderPoint.Count());

			if(elementsUnderPoint.Count > 0) {
				elements.AddRange(elementsUnderPoint);
				foreach(var item in elementsUnderPoint) {
					RetreiveChildrenUnderPoint(item, condition, elements);
				}
			}
		}

		AutomationElementCollection GetChildren(AutomationElement element, Condition condition) {
			var controlType = element.Cached.ControlType;
			if(controlType == ControlType.Tree || controlType == ControlType.TreeItem) {
				return automationElementService.FindAll(element, TreeScope.Descendants, condition);
			} else {
				return automationElementService.FindAll(element, TreeScope.Children, condition);
			}
		}

		IEnumerable<AutomationElement> GetChainOfParents(AutomationElement rootElement, AutomationElement element) {
			var list = new List<AutomationElement>();
			do {
				element = automationElementService.GetParent(element, cacheRequest);
				list.Add(element);
			} while(element != null && !automationElementService.Compare(rootElement, element));

			return list;
		}

		void RemoveParents(AutomationElement rootElement, IList<AutomationElement> elementsUnderPoint) {
			if(elementsUnderPoint == null || elementsUnderPoint.Count == 0) {
				return;
			}
			int i = 0;
			do {
				BreakOperationsIfCoordChanged();
				var element = elementsUnderPoint[i];
				var parents = GetChainOfParents(rootElement, element);
				var parentsInList = elementsUnderPoint
					.Where(x => parents.Any(p => Automation.Compare(p, x)))
					.ToList();
				foreach(var parent in parentsInList) {
					BreakOperationsIfCoordChanged();
					elementsUnderPoint.Remove(parent);
				}
			} while(++i < elementsUnderPoint.Count);
		}

		int GetTreeOrder(AutomationElement rootElement, AutomationElement element) {
			if(element == null) {
				return -1;
			}
			var _element = element;
			var z = 0;
			while(!automationElementService.Compare(rootElement, _element)) {
				_element = TreeWalker.RawViewWalker.GetParent(_element, cacheRequest);
				z++;
			}

			Debug.WriteLine("GetTreeOrder: {0}, z: {1}", automationElementService.BuildFriendlyInfo(element), z);
			return z;
		}

		int GetZOrder(AutomationElement element) {
			if(element == null) {
				return int.MaxValue;
			}
			var hWnd = new IntPtr(element.Cached.NativeWindowHandle);
			if(hWnd == IntPtr.Zero) {
				return int.MaxValue;
			}
			var lowestHwnd = winApiService.GetWindow(hWnd, WinAPI.GW_HWNDLAST);
			var hwndTmp = lowestHwnd;
			int z = 0;
			while(hwndTmp != IntPtr.Zero) {
				if(hWnd == hwndTmp) {
					Debug.WriteLine("GetZOrder: {0}, z: {1}", automationElementService.BuildFriendlyInfo(element), z);
					return z;
				}
				hwndTmp = winApiService.GetWindow(hwndTmp, WinAPI.GW_HWNDPREV);
				z++;
			}
			return int.MaxValue;
		}

		IntPtr GetRootWindow(IntPtr selectedWindow) {
			var rootHwnd = winApiService.GetAncestor(selectedWindow, WinAPI.GA_ROOT);
			return rootHwnd;
		}

		void BreakOperationsIfCoordChanged() {
			var pt = winApiService.GetMousePosition();
			if(!pt.WithBoundaries(elementCoord, 10)) {
				throw new OperationCanceledException();
			}
		}
	}
}
