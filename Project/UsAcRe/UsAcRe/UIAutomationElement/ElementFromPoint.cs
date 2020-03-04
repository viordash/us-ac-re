using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using NGuard;
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

		UiElement specificElement;

		public System.Windows.Rect BoundingRectangle {
			get {
				try {
					if(specificElement != null) {
						return specificElement.BoundingRectangle;
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
				return string.Format($"{nameof(ElementFromPoint)} ({elementCoord.x}, {elementCoord.y}). {specificElement}");
			} else {
				return string.Format($"{nameof(ElementFromPoint)} ({elementCoord.x}, {elementCoord.y}). No element");
			}
		}

		void DetermineElementUnderPoint() {
			specificElement = GetElementFromPoint();
			automationElementService.RetrieveElementValue(specificElement);
			logger.Trace("             DetermineElementUnderPoint 1: {0}; {1}", specificElement, specificElement.BoundingRectangle);
		}

		UiElement GetElementFromPoint() {
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

			var rootWindowHwnd = winApiService.GetRootWindowForElementUnderPoint(elementCoord);
			if(rootWindowHwnd == IntPtr.Zero) {
				return null;
			}

			var rootElement = automationElementService.FromHandle(rootWindowHwnd);

			try {
				var elementsUnderPoint = new List<UiElement>();

				RetreiveChildrenUnderPoint(rootElement, elementsUnderPoint);
				RemoveParents(rootElement, elementsUnderPoint);
				var element = elementsUnderPoint
					.OrderByDescending(x => GetTreeOrder(rootElement, x))
					.ThenByDescending(x => GetZOrder(x))
					.ThenBy(x => x.BoundingRectangle, new BoundingRectangleComp())
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

		void RetreiveChildrenUnderPoint(UiElement elementUnderPoint, List<UiElement> elements) {
			BreakOperationsIfCoordChanged();
			var childElements = GetChildren(elementUnderPoint);
			var elementsUnderPoint = childElements
				.Where(x => x.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
				.ToList();

			var outsideOfPoint = childElements
				.Where(x => !x.BoundingRectangle.Contains(elementCoord.x, elementCoord.y));

			foreach(var item in outsideOfPoint) {
				var suspectedElements = GetChildren(item);

				var suspectedElementsUnderPoint = suspectedElements
					.Where(x => x.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
					.Except(elementsUnderPoint)
					.ToList();
				if(suspectedElementsUnderPoint.Count > 0) {
					elementsUnderPoint.AddRange(suspectedElementsUnderPoint);
					Debug.WriteLine("		suspected: {0}, childs: {1}", item, suspectedElementsUnderPoint.Count());
				}
			}

			Debug.WriteLine("elementsUnderPoint: {0}, childs: {1}", elementUnderPoint, elementsUnderPoint.Count());

			if(elementsUnderPoint.Count > 0) {
				elements.AddRange(elementsUnderPoint);
				foreach(var item in elementsUnderPoint) {
					RetreiveChildrenUnderPoint(item, elements);
				}
			}
		}

		List<UiElement> GetChildren(UiElement element) {
			var controlType = ControlType.LookupById(element.ControlTypeId);
			if(controlType == ControlType.Tree || controlType == ControlType.TreeItem) {
				return automationElementService.FindAllValidElements(element, TreeScope.Descendants);
			} else {
				return automationElementService.FindAllValidElements(element, TreeScope.Children);
			}
		}

		IEnumerable<UiElement> GetChainOfParents(UiElement rootElement, UiElement element) {
			var list = new List<UiElement>();
			do {
				element = automationElementService.GetParent(element);
				list.Add(element);
			} while(element != null && !automationElementService.Compare(rootElement, element));

			return list;
		}

		void RemoveParents(UiElement rootElement, IList<UiElement> elementsUnderPoint) {
			if(elementsUnderPoint == null || elementsUnderPoint.Count == 0) {
				return;
			}
			int i = 0;
			do {
				BreakOperationsIfCoordChanged();
				var element = elementsUnderPoint[i];
				var parents = GetChainOfParents(rootElement, element);
				var parentsInList = elementsUnderPoint
					.Where(x => parents.Any(p => automationElementService.Compare(p, x)))
					.ToList();
				foreach(var parent in parentsInList) {
					BreakOperationsIfCoordChanged();
					elementsUnderPoint.Remove(parent);
				}
			} while(++i < elementsUnderPoint.Count);
		}

		int GetTreeOrder(UiElement rootElement, UiElement element) {
			if(element == null) {
				return -1;
			}
			var _element = element;
			var z = 0;
			while(!automationElementService.Compare(rootElement, _element)) {
				_element = automationElementService.GetParent(_element);
				z++;
			}

			Debug.WriteLine("GetTreeOrder: {0}, z: {1}", element, z);
			return z;
		}

		int GetZOrder(UiElement element) {
			if(element == null) {
				return int.MaxValue;
			}
			var hWnd = automationElementService.GetNativeWindowHandle(element);
			if(hWnd == IntPtr.Zero) {
				return int.MaxValue;
			}
			var lowestHwnd = winApiService.GetWindow(hWnd, WinAPI.GW_HWNDLAST);
			var hwndTmp = lowestHwnd;
			int z = 0;
			while(hwndTmp != IntPtr.Zero) {
				if(hWnd == hwndTmp) {
					Debug.WriteLine("GetZOrder: {0}, z: {1}", element, z);
					return z;
				}
				hwndTmp = winApiService.GetWindow(hwndTmp, WinAPI.GW_HWNDPREV);
				z++;
			}
			return int.MaxValue;
		}

		void BreakOperationsIfCoordChanged() {
			var pt = winApiService.GetMousePosition();
			if(!pt.WithBoundaries(elementCoord, 10)) {
				throw new OperationCanceledException();
			}
		}
	}
}
