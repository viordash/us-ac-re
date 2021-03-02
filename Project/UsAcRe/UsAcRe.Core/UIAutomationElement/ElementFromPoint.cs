using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using NGuard;
using NLog;
using UsAcRe.Core.Services;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Core.UIAutomationElement {
	public class ElementFromPoint {
		private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

		#region inner classes
		public class TreeItem {
			public UiElement Element { get; private set; }
			public TreeItem Parent { get; private set; }
			public List<UiElement> Childs { get; set; }
			public TreeItem(UiElement element, TreeItem parent) {
				Element = element;
				Parent = parent;
			}
		}

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

		public TreeOfSpecificUiElement TreeOfSpecificUiElement { get; } = new TreeOfSpecificUiElement();

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
			if(TreeOfSpecificUiElement.Count > 0) {
				return string.Format("{0} ({1}, {2}) |{3}|\r\n{4}", nameof(ElementFromPoint), elementCoord.x, elementCoord.y, TreeOfSpecificUiElement.Count, TreeOfSpecificUiElement);
			} else {
				return string.Format("{0} ({1}, {2}). No element", nameof(ElementFromPoint), elementCoord.x, elementCoord.y);
			}
		}

		void DetermineElementUnderPoint() {
			BuildTreeOfSpecificElement();
			if(TreeOfSpecificUiElement.Count > 0) {
				var specificElement = TreeOfSpecificUiElement.FirstOrDefault();
				automationElementService.RetrieveElementValue(TreeOfSpecificUiElement.FirstOrDefault());
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
					TreeOfSpecificUiElement.Insert(0, element);
					return;
				}
			}

			var rootElement = automationElementService.FromHandle(rootWindowHwnd);
			var desktop = automationElementService.GetDesktop();
			var desktopChildren = GetChildren(desktop);

			rootElement.Index = 0;
			var similars = desktopChildren.Where(x => automationElementService.Compare(x, rootElement, ElementCompareParameters.ForSimilars()));
			for(int i = 0; i < similars.Count(); i++) {
				if(automationElementService.Compare(rootElement, similars.ElementAt(i), ElementCompareParameters.ForExact())) {
					rootElement.Index = i;
					break;
				}
			}

			try {
				TreeOfSpecificUiElement.Program = automationElementService.GetProgram(rootElement);

				var rootParent = rootElement;
				do {
					BreakOperationsIfCoordChanged();
					rootElement = rootParent;
					rootParent = automationElementService.GetParent(rootElement);
				} while(rootParent != null && !automationElementService.Compare(rootParent, desktop, ElementCompareParameters.ForExact()));

				var treeItems = new List<TreeItem>();
				var rootTreeElement = new TreeItem(rootElement, null);
				treeItems.Add(rootTreeElement);

				BuildElementsTree(rootTreeElement, treeItems);
				var filteredElements = FilterByUnderPoint(treeItems);
				var targetedElement = SortElementsByPointProximity(filteredElements, rootWindowHwnd);

				if(targetedElement != null) {
					Debug.WriteLine($"targetedElement: {targetedElement}");
					var tree = BuildElementTree(treeItems, targetedElement, rootElement);
					TreeOfSpecificUiElement.AddRange(tree);
				}
			} catch(Exception ex) {
				if(ex is OperationCanceledException) {
					throw;
				}
			}
		}

		void BuildElementsTree(TreeItem parent, List<TreeItem> elements) {
			BreakOperationsIfCoordChanged();
			Debug.WriteLine("");
			parent.Childs = GetChildren(parent.Element);
			foreach(var item in parent.Childs) {
				Debug.WriteLine($"tree: item:{item},\r\n\t\t\t{parent}");
				var treeItem = new TreeItem(item, parent);
				elements.Add(treeItem);
				BuildElementsTree(treeItem, elements);
			}
		}

		List<TreeItem> FilterByUnderPoint(List<TreeItem> treeItems) {
			BreakOperationsIfCoordChanged();
			Debug.WriteLine("");
			Debug.WriteLine("                        ----  ");
			Debug.WriteLine("FilterByUnderPoint:");

			var elementsUnderPoint = treeItems
				.Where(x => x.Element.BoundingRectangle.Value.Contains(elementCoord.x, elementCoord.y))
				.ToList();

			foreach(var item in elementsUnderPoint) {
				Debug.WriteLine($"underPoint: item:{item.Element},\r\n\t\t\t{item.Parent}");
			}
			return elementsUnderPoint;
		}

		protected UiElement SortElementsByPointProximity(List<TreeItem> filteredElements, IntPtr rootWindow) {
			var elementsByChilds = filteredElements
				.OrderBy(x => x.Childs.Count)
				.ToList();

			if(!elementsByChilds.Any()) {
				return null;
			}

			var elementWithLeastChilds = elementsByChilds.First();
			var elementsWithLeastChilds = elementsByChilds
				.Where(x => x.Childs.Count == elementWithLeastChilds.Childs.Count);

			var elementsByZOrder = elementsWithLeastChilds
				.Select(x => Tuple.Create(GetZOrder(x.Element, rootWindow), x))
				.OrderBy(x => x.Item1)
				.ToList();

			if(!elementsByZOrder.Any()) {
				return null;
			}
			var topElement = elementsByZOrder.First();
			var topElements = elementsByZOrder
				.Where(x => x.Item1 == topElement.Item1)
				.Select(x => x.Item2);

			var proximityElement = topElements
				.OrderBy(x => x.Element.BoundingRectangle.Value, new BoundingRectangleComp())
				.FirstOrDefault();
			return proximityElement.Element;
		}

		List<UiElement> BuildElementTree(List<TreeItem> treeItems, UiElement targetedElement, UiElement rootElement) {
			Debug.WriteLine("");
			Debug.WriteLine("                        ----  ");
			Debug.WriteLine("BuildElementTree:");
			var tree = new List<UiElement>();
			tree.Add(targetedElement);
			while(true) {
				var element = treeItems.Where(x => x.Element == targetedElement).SingleOrDefault();
				if(element == null || element.Parent == null) {
					break;
				}

				var similars = element.Parent.Childs
					.Where(x => automationElementService.Compare(x, targetedElement, ElementCompareParameters.ForSimilars()))
					.ToList();

				for(int i = 0; i < similars.Count; i++) {
					Debug.WriteLine($"similars: {similars[i]}");
					if(targetedElement == similars[i]) {
						targetedElement.Index = i;
						break;
					}
				}

				Debug.WriteLine($"parent: item:{element.Element},\r\n\t\t\t{element.Parent}");

				targetedElement = element.Parent.Element;
				tree.Add(targetedElement);

				if(targetedElement == rootElement) {
					break;
				}
			}
			return tree;
		}

		List<UiElement> GetChildren(UiElement element) {
			var controlType = ControlType.LookupById(element.ControlTypeId.Value);
			if(controlType == ControlType.Tree || controlType == ControlType.TreeItem) {
				return automationElementService.FindAllValidElements(element, TreeScope.Descendants);
			} else {
				return automationElementService.FindAllValidElements(element, TreeScope.Children);
			}
		}

		protected virtual int GetZOrder(UiElement element, IntPtr rootWindow) {
			if(element == null) {
				return int.MaxValue;
			}
			var hWnd = automationElementService.GetNativeWindowHandle(element);
			if(hWnd == IntPtr.Zero) {
				return int.MaxValue;
			}

			var coord = elementCoord;
			winApiService.ScreenToClient(rootWindow, ref coord);

			var childWindowFromPoint = winApiService.RealChildWindowFromPoint(rootWindow, coord);
			if(childWindowFromPoint == IntPtr.Zero) {
				return int.MaxValue;
			}
			//Debug.WriteLine("zOrder: {0}, z: {1} {2}", element, childWindowFromPoint, hWnd);
			if(childWindowFromPoint == hWnd) {
				return 0;
			} else {
				return int.MaxValue;
			}
		}

		void BreakOperationsIfCoordChanged() {
			if(breakOperations) {
				throw new OperationCanceledException();
			}
		}
	}
}
