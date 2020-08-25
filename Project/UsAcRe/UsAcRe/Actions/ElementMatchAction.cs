using System.Collections.Generic;
using System.Text;
using System.Linq;
using UsAcRe.Extensions;
using UsAcRe.UIAutomationElement;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Automation;
using System;
using UsAcRe.MouseProcess;
using UsAcRe.Exceptions;
using UsAcRe.Helpers;

namespace UsAcRe.Actions {
	public class ElementMatchAction : BaseAction {
		#region inner classes
		class RequiredElement {
			public UiElement Element { get; set; }
			public UiElement Parent { get; set; }
			public UiElement ParentEquivalentInSearchPath { get; set; }
		}
		#endregion

		public ElementProgram Program { get; private set; }
		public List<UiElement> SearchPath { get; private set; }
		public UiElement MatchedElement { get => SearchPath?[0]; }
		public int TimeoutMs { get; private set; }
		public System.Windows.Point? ClickablePoint { get; private set; }

		int stepWaitAppear;

		public ElementMatchAction(ElementProgram program, List<UiElement> searchPath, int timeoutMs = 20 * 1000) {
			Program = program;
			SearchPath = searchPath;
			TimeoutMs = timeoutMs;
			ClickablePoint = null;
		}

		protected override async Task ExecuteCoreAsync() {
			await SafeActionAsync(DoWorkAsync);
		}

		public override string ToString() {
			var sb = new StringBuilder();
			sb.Append(nameof(ElementMatchAction));
			sb.AppendFormat($" {Program}");
			foreach(var item in SearchPath.AsEnumerable().Reverse()) {
				sb.AppendFormat(" -> {0}", item);
			}
			return sb.ToString();
		}
		public override string ExecuteAsScriptSource() {
			return string.Format("new {0}({1}, {2}, {3}).{4}(prevAction)", nameof(ElementMatchAction), Program.ForNew(), SearchPath.ForNew(), TimeoutMs.ForNew(),
				nameof(ElementMatchAction.ExecuteAsync));
		}

		async ValueTask DoWorkAsync() {
			await Task.Run(async () => {
				stepWaitAppear = 0;
				ClickablePoint = null;
				var stopwatch = Stopwatch.StartNew();
				while(!cancellationToken.IsCancellationRequested && stopwatch.Elapsed.TotalMilliseconds < TimeoutMs) {
					var requiredElement = GetElement();
					if(requiredElement != null
					&& requiredElement.Element != null) {
						ClickablePoint = GetClickablePoint(requiredElement, requiredElement.Element.BoundingRectangle);
						testsLaunchingService.OpenHighlighter(requiredElement.Element.BoundingRectangle, null);
						MouseHover.MoveTo(ClickablePoint.Value);
						break;
					}
					await WaitAppearElement(requiredElement);
				}

				if(!ClickablePoint.HasValue) {
					throw new TestFailedExeption(this);
				}
			});
		}

		async Task WaitAppearElement(RequiredElement requiredElement) {
			System.Windows.Rect rect;

			if(requiredElement?.Parent != null) {
				rect = GetRelativeRectangle(requiredElement);
			} else {
				rect = MatchedElement.BoundingRectangle;
			}
			var clickablePoint = GetClickablePoint(requiredElement, rect);
			testsLaunchingService.CloseHighlighter();
			await MouseHover.Perform(clickablePoint, stepWaitAppear, 50);
			testsLaunchingService.OpenHighlighter(rect, MatchedElement.ToShortString());
			await Task.Delay(100);
			stepWaitAppear++;
		}

		System.Windows.Rect GetRelativeRectangle(RequiredElement requiredElement) {
			var originalLocation = requiredElement.ParentEquivalentInSearchPath.BoundingRectangle.Location;
			var originalSize = requiredElement.ParentEquivalentInSearchPath.BoundingRectangle.Size;
			var currentLocation = requiredElement.Parent.BoundingRectangle.Location;
			var currentSize = requiredElement.Parent.BoundingRectangle.Size;
			var offsetX = currentLocation.X - originalLocation.X;
			var offsetY = currentLocation.Y - originalLocation.Y;

			var rect = MatchedElement.BoundingRectangle;
			rect.Offset(offsetX, offsetY);
			return rect;
		}

		System.Windows.Point GetClickablePoint(RequiredElement requiredElement, System.Windows.Rect boundingRect) {
			if(requiredElement?.Element != null && automationElementService.TryGetClickablePoint(requiredElement.Element, out System.Windows.Point point)) {
				return point;
			}
			boundingRect.Offset(boundingRect.Width / 2, boundingRect.Height / 2);
			return boundingRect.Location;
		}

		RequiredElement GetElement() {
			var rootElement = automationElementService.GetRootElement(Program);
			if(rootElement == null) {
				return null;
			}
			var parentEquivalentInSearchPath = SearchPath[SearchPath.Count - 1];
			if(!AreUiElementsEquals(rootElement, parentEquivalentInSearchPath, true)) {
				return null;
			}

			var requiredElement = new RequiredElement() {
				Element = rootElement
			};
			var childs = GetChildren(rootElement);

			for(int i = SearchPath.Count - 2; i >= 0; i--) {
				var element = SearchPath[i];
				requiredElement.Parent = requiredElement.Element;
				requiredElement.ParentEquivalentInSearchPath = parentEquivalentInSearchPath;
				parentEquivalentInSearchPath = element;

				requiredElement.Element = SearchRequiredElement(element, childs);
				if(requiredElement.Element == null) {
					break;
				}
				childs = GetChildren(requiredElement.Element);
			}
			return requiredElement;
		}

		bool AreUiElementsEquals(UiElement element1, UiElement element2, bool compareSizes) {
			if(element1.ControlTypeId != element2.ControlTypeId
				|| !AreValueEquals(element1, element2)
				|| !TextMatched(element1.Name, element2.Name)
				|| !StringEquals(element1.ClassName, element2.ClassName)
				|| !StringEquals(element1.AutomationId, element2.AutomationId)
				|| (compareSizes && !DimensionsHelper.AreSizeEquals(element1.BoundingRectangle.Size, element2.BoundingRectangle.Size,
						GetClickPositionToleranceInPercent()))) {
				return false;
			}
			return true;
		}

		UiElement SearchRequiredElement(UiElement searchedElement, List<UiElement> childs) {
			foreach(var element in childs) {
				if(!AreUiElementsEquals(element, searchedElement, true)) {
					continue;
				}
				return element;
			}
			return null;
		}

		List<UiElement> GetChildren(UiElement element) {
			var controlType = ControlType.LookupById(element.ControlTypeId);
			if(controlType == ControlType.Tree || controlType == ControlType.TreeItem) {
				return automationElementService.FindAllValidElements(element, TreeScope.Descendants);
			} else {
				return automationElementService.FindAllValidElements(element, TreeScope.Children);
			}
		}

		bool TextMatched(string text, string sSearchTerm) {
			bool isMatchCase = true;//			IsMatchCase();
			bool isMatchWholeWord = true;//			IsMatchWholeWord() || string.IsNullOrEmpty(text);
			if(StringEquals(text, sSearchTerm)) {
				return true;
			}
			if(string.IsNullOrEmpty(text) || string.IsNullOrEmpty(sSearchTerm)) {
				return false;
			}
			if(text.Equals(sSearchTerm, StringComparison.OrdinalIgnoreCase)) {
				return true;
			}
			if(isMatchWholeWord || string.IsNullOrEmpty(sSearchTerm)) {
				return false;
			}
			var comparisonType = isMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if(!isMatchWholeWord && sSearchTerm.IndexOf(text, comparisonType) >= 0) {
				return true;
			}
			if(!isMatchWholeWord && text.IndexOf(sSearchTerm, comparisonType) >= 0) {
				return true;
			}
			return false;
		}

		bool AreValueEquals(UiElement element1, UiElement element2) {
			automationElementService.RetrieveElementValue(element1);
			return true /*IsCheckByValue()*/
				&& StringEquals(element1.Value, element2.Value);
		}

		bool StringEquals(string s1, string s2) {
			return (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
				|| s1 == s2;
		}
	}
}
