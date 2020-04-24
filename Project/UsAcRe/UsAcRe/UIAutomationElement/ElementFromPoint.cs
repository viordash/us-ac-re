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
		readonly bool detailedRetrieve;

		bool breakOperations;
		public void BreakOperations() {
			breakOperations = true;
		}


		TreeOfSpecificUiElement treeOfSpecificElement = new TreeOfSpecificUiElement();

		public System.Windows.Rect BoundingRectangle {
			get {
				return treeOfSpecificElement.BoundingRectangle;
			}
		}

		public ElementFromPoint(
			IAutomationElementService automationElementService,
			IWinApiService winApiService,
			WinAPI.POINT elementCoord,
			bool detailedRetrieve) {
			Guard.Requires(automationElementService, nameof(automationElementService));
			Guard.Requires(winApiService, nameof(winApiService));

			this.automationElementService = automationElementService;
			this.winApiService = winApiService;
			this.elementCoord = elementCoord;
			this.detailedRetrieve = detailedRetrieve;
			DetermineElementUnderPoint();
		}

		public override string ToString() {
			if(treeOfSpecificElement.Count > 0) {
				return string.Format("{0} ({1}, {2}) |{3}|\r\n{4}", nameof(ElementFromPoint), elementCoord.x, elementCoord.y, treeOfSpecificElement.Count, treeOfSpecificElement);
			} else {
				return string.Format("{0} ({1}, {2}). No element", nameof(ElementFromPoint), elementCoord.x, elementCoord.y);
			}
		}

		void DetermineElementUnderPoint() {
			BuildTreeOfSpecificElement();
			if(treeOfSpecificElement.Count > 0) {
				var specificElement = treeOfSpecificElement.FirstOrDefault();
				automationElementService.RetrieveElementValue(treeOfSpecificElement.FirstOrDefault());
				logger.Trace("             DetermineElementUnderPoint 1: {0}; {1}", specificElement, specificElement.BoundingRectangle);
			}
		}

		void BuildTreeOfSpecificElement() {
			Debug.WriteLine("");
			Debug.WriteLine("------------------------");
			Debug.WriteLine("ElementCoord: {0}", elementCoord);
			Debug.WriteLine("");

			var rootWindowHwnd = winApiService.GetRootWindowForElementUnderPoint(elementCoord);
			if(rootWindowHwnd == IntPtr.Zero) {
				return;
			}

			if(!detailedRetrieve) {
				var element = automationElementService.FromPoint(new System.Windows.Point(elementCoord.x, elementCoord.y));
				if(element != null) {
					treeOfSpecificElement.Insert(0, element);
					return;
				}
			}

			var rootElement = automationElementService.FromHandle(rootWindowHwnd);
			rootElement.Index = 0;

			try {
				treeOfSpecificElement.ProgramName = automationElementService.GetProgramName(rootElement);
				treeOfSpecificElement.Add(rootElement);
				var elementsUnderPoint = new List<UiElement>();

				RetreiveChildrenUnderPoint(rootElement, elementsUnderPoint);

				var tree = BuildElementsTree(rootElement, elementsUnderPoint);
				RemoveParents(tree);

				var sortedElements = tree
					.Where(x => x.Value != null)
					.OrderByDescending(x => GetZOrder(x.Key))
					.ThenByDescending(x => x.Value.Count)
					.ThenBy(x => x.Key.BoundingRectangle, new BoundingRectangleComp())
					.FirstOrDefault();

				if(sortedElements.Key != null) {
					treeOfSpecificElement.InsertRange(0, sortedElements.Value);
					treeOfSpecificElement.Insert(0, sortedElements.Key);
				}
			} catch(Exception ex) {
				if(ex is OperationCanceledException) {
					throw;
				}
			}
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

			Debug.WriteLine($"elementsUnderPoint: {elementUnderPoint}, childs: {elementsUnderPoint.Count()}");

			if(elementsUnderPoint.Count > 0) {

				foreach(var item in elementsUnderPoint) {
					var similars = childElements.Where(x => automationElementService.ElementsIsSimilar(x, item));
					for(int i = 0; i < similars.Count(); i++) {
						if(item == similars.ElementAt(i)) {
							item.Index = i;
						}
					}
				}

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

		Dictionary<UiElement, List<UiElement>> BuildElementsTree(UiElement rootElement, List<UiElement> elementsUnderPoint) {
			var tree = new Dictionary<UiElement, List<UiElement>>();
			foreach(var element in elementsUnderPoint) {
				BreakOperationsIfCoordChanged();
				var elementTree = GetAncestors(rootElement, element);
				tree.Add(element, elementTree);
			}
			return tree;
		}

		List<UiElement> GetAncestors(UiElement rootElement, UiElement element) {
			var list = new List<UiElement>();
			do {
				element = automationElementService.GetParent(element);
				if(element != null && automationElementService.Compare(rootElement, element)) {
					break;
				}
				list.Add(element);
			} while(list.Count < 10000);

			return list;
		}

		void RemoveParents(Dictionary<UiElement, List<UiElement>> tree) {
			var keysToBeDeleted = new List<UiElement>();
			foreach(var element in tree.Keys) {
				foreach(var ancestors in tree.Values) {
					var itemIsParent = ancestors.Any(x => automationElementService.Compare(element, x));
					if(itemIsParent) {
						keysToBeDeleted.Add(element);
					}
				}
			}
			foreach(var key in keysToBeDeleted) {
				var keyLikely = tree.Values.Select(x => x.Where(e => automationElementService.Compare(key, e))).SelectMany(x => x);
				foreach(var item in keyLikely) {
					item.Index = key.Index;
				}
				tree.Remove(key);
			}

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
			if(breakOperations) {
				throw new OperationCanceledException();
			}
		}
	}
}
