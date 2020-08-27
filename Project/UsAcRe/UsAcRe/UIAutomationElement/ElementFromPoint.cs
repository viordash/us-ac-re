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

		public class TreeElement {
			public UiElement Element { get; private set; }
			public List<UiElement> Childs { get; } = new List<UiElement>();
			public TreeElement(UiElement element, List<UiElement> childs) {
				Element = element;
				Childs.AddRange(childs);
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
			var similars = desktopChildren.Where(x => automationElementService.ElementsIsSimilar(x, rootElement));
			for(int i = 0; i < similars.Count(); i++) {
				if(automationElementService.Compare(rootElement, similars.ElementAt(i))) {
					rootElement.Index = i;
					break;
				}
			}

			Debug.WriteLine($"rootElement: {rootElement}");

			try {
				TreeOfSpecificUiElement.Program = automationElementService.GetProgram(rootElement);

				var elements = new List<TreeElement>();

				var rootParent = automationElementService.GetParent(rootElement);
				while(rootParent != null && !automationElementService.Compare(rootParent, desktop)) {
					BreakOperationsIfCoordChanged();
					var childElements = GetChildren(rootParent);
					var elementsUnderPoint = childElements
						.Where(x => x.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
						.ToList();

					Debug.WriteLine($"inserted rootParent: {rootParent}");
					elements.Add(new TreeElement(rootParent, elementsUnderPoint));
					rootParent = automationElementService.GetParent(rootParent);
				}

				RetreiveElementsUnderPoint(rootElement, elements);

				var targetedElement = SortElementsByPointProximity(elements, rootWindowHwnd);
				if(targetedElement != null) {
					Debug.WriteLine($"targetedElement: {targetedElement.Element}");
					var tree = BuildElementTree(targetedElement, elements, rootWindowHwnd);
					TreeOfSpecificUiElement.Add(tree.Key);
					TreeOfSpecificUiElement.AddRange(tree.Value);
				}
			} catch(Exception ex) {
				if(ex is OperationCanceledException) {
					throw;
				}
			}
		}

		KeyValuePair<UiElement, List<UiElement>> BuildElementTree(TreeElement targetedElement, List<TreeElement> elements, IntPtr rootWindow) {
			var tree = new KeyValuePair<UiElement, List<UiElement>>(targetedElement.Element, new List<UiElement>());

			while(true) {
				BreakOperationsIfCoordChanged();
				var ancestors = elements
					.Where(x =>
						x.Childs.Any(c => automationElementService.Compare(c, targetedElement.Element)));

				if(ancestors == null || !ancestors.Any()) {
					break;
				}

				targetedElement = SortElementsByPointProximity(ancestors, rootWindow);
				tree.Value.Add(targetedElement.Element);
			}
			return tree;
		}

		protected TreeElement SortElementsByPointProximity(IEnumerable<TreeElement> elements, IntPtr rootWindow) {
			var elementsByChilds = elements
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
				.OrderBy(x => x.Element.BoundingRectangle, new BoundingRectangleComp())
				.FirstOrDefault();
			return proximityElement;
		}

		void RetreiveElementsUnderPoint(UiElement elementUnderPoint, List<TreeElement> elements) {
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
			elements.Add(new TreeElement(elementUnderPoint, elementsUnderPoint));

			if(elementsUnderPoint.Count > 0) {
				foreach(var item in elementsUnderPoint) {
					var similars = childElements.Where(x => automationElementService.ElementsIsSimilar(x, item));
					for(int i = 0; i < similars.Count(); i++) {
						if(item == similars.ElementAt(i)) {
							item.Index = i;
							break;
						}
					}
				}

				foreach(var item in elementsUnderPoint) {
					RetreiveElementsUnderPoint(item, elements);
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
			Debug.WriteLine("zOrder: {0}, z: {1} {2}", element, childWindowFromPoint, hWnd);
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
