using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using NGuard;
using NLog;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Services;
using UsAcRe.Core.UIAutomationElement;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Recorder.UIAutomationElement {
	public class ElementFromPoint {
		private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

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
			//Debug.WriteLine("");
			//Debug.WriteLine("------------------------");
			//Debug.WriteLine("ElementCoord: {0}", elementCoord);
			//Debug.WriteLine("");

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
			var similars = desktopChildren.Where(x => automationElementService.CompareInSiblings(x, rootElement, ElementCompareParameters.ForSimilars(), out string message));
			for(int i = 0; i < similars.Count(); i++) {
				if(automationElementService.Compare(rootElement, similars.ElementAt(i), ElementCompareParameters.ForExact(), out string message)) {
					rootElement.Index = i;
					break;
				}
			}

			//Debug.WriteLine($"rootElement: {rootElement}");

			try {
				TreeOfSpecificUiElement.Program = automationElementService.GetProgram(rootElement);

				var elements = new List<TreeElement>();

				var rootParent = automationElementService.GetParent(rootElement);
				while(rootParent != null && !automationElementService.Compare(rootParent, desktop, ElementCompareParameters.ForExact(), out string message)) {
					BreakOperationsIfCoordChanged();
					var childElements = GetChildren(rootParent);
					var elementsUnderPoint = childElements
						.Where(x => x.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
						.ToList();

					//Debug.WriteLine($"inserted rootParent: {rootParent}");
					elements.Add(new TreeElement(rootParent, elementsUnderPoint));
					rootParent = automationElementService.GetParent(rootParent);
				}

				RetreiveElementsUnderPoint(rootElement, elements);

				var targetedElement = SortElementsByPointProximity(elements, rootWindowHwnd);
				if(targetedElement != null) {
					Debug.WriteLine($"targetedElement: {targetedElement}");
					var tree = BuildElementTree(targetedElement, desktop);
					TreeOfSpecificUiElement.AddRange(tree);
				}
			} catch(Exception ex) {
				if(ex is OperationCanceledException) {
					throw;
				}
			}
		}

		UiElement GetAncestor(UiElement targetedElement, UiElement startingPoint) {
			BreakOperationsIfCoordChanged();
			Debug.WriteLine("                        ----  ");
			Debug.WriteLine($"GetAncestor targeted: {targetedElement}, strtPnt: {startingPoint}");
			var parent = automationElementService.GetParent(startingPoint);
			if(parent == null) {
				return null;
			}
			Debug.WriteLine($"GetAncestor, parent: {parent}");

			var childElements = GetChildren(parent);
			var similars = childElements
				.Where(x => automationElementService.CompareInSiblings(x, targetedElement, ElementCompareParameters.ForSimilars(), out string message))
				.ToList();

			for(int i = 0; i < similars.Count; i++) {
				if(automationElementService.Compare(targetedElement, similars[i], ElementCompareParameters.ForExact(), out string message)) {
					targetedElement.Index = i;
					return parent;
				}
				Debug.WriteLine($"similars: {similars[i]}");
			}
			Debug.WriteLine($"GetAncestor, Parent not found: {targetedElement}");
			return GetAncestor(targetedElement, parent);
		}

		List<UiElement> BuildElementTree(UiElement targetedElement, UiElement desktop) {
			var tree = new List<UiElement>();
			tree.Add(targetedElement);
			while(true) {
				var parent = GetAncestor(targetedElement, targetedElement);
				if(parent == null) {
					bool attemptToSearchElementInAncestor = tree.Count >= 2;
					if(attemptToSearchElementInAncestor) {
						targetedElement = tree[tree.Count - 2];
						var startingPoint = tree[tree.Count - 1];
						parent = GetAncestor(targetedElement, startingPoint);
						if(parent != null) {
							tree.Remove(startingPoint);
						}
					}

					if(parent == null) {
						throw new RetrieveElementExceptions($"Not found ancestor for {targetedElement}");
					}
				}

				if(automationElementService.Compare(parent, desktop, ElementCompareParameters.ForExact(), out string message)) {
					return tree;
				}
				tree.Add(parent);
				targetedElement = parent;
			}
		}

		protected UiElement SortElementsByPointProximity(IEnumerable<TreeElement> elements, IntPtr rootWindow) {
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
			return proximityElement.Element;
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
					.Except(elementsUnderPoint)
					.Where(x => x.BoundingRectangle.Contains(elementCoord.x, elementCoord.y))
					.ToList();
				if(suspectedElementsUnderPoint.Count > 0) {
					elementsUnderPoint.AddRange(suspectedElementsUnderPoint);
					Debug.WriteLine("		suspected: {0}, childs: {1}", item, suspectedElementsUnderPoint.Count());
				}
			}

			//Debug.WriteLine($"elementsUnderPoint: {elementUnderPoint}, childs: {elementsUnderPoint.Count()}");
			elements.Add(new TreeElement(elementUnderPoint, elementsUnderPoint));

			if(elementsUnderPoint.Count > 0) {
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
