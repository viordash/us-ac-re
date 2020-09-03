using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using UsAcRe.Exceptions;
using UsAcRe.Extensions;
using UsAcRe.Helpers;
using UsAcRe.MouseProcess;
using UsAcRe.UIAutomationElement;

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
		public System.Windows.Forms.AnchorStyles Anchor { get; private set; }
		public UiElement MatchedElement { get => SearchPath?[0]; }
		public int TimeoutMs { get; private set; }
		public System.Windows.Point? OffsetPoint { get; private set; }

		int stepWaitAppear;

		public ElementMatchAction(ElementFromPoint elementFromPoint)
			: this(null, elementFromPoint.TreeOfSpecificUiElement.Program, elementFromPoint.TreeOfSpecificUiElement) { }

		public ElementMatchAction(BaseAction prevAction, ElementProgram program, List<UiElement> searchPath, int timeoutMs = 20 * 1000) : base(prevAction) {
			Program = program;
			SearchPath = searchPath;
			Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
			TimeoutMs = timeoutMs;
			OffsetPoint = null;
		}

		protected override async ValueTask ExecuteCoreAsync() {
			stepWaitAppear = 0;
			OffsetPoint = null;
			var stopwatch = Stopwatch.StartNew();
			while(!cancellationToken.IsCancellationRequested && stopwatch.Elapsed.TotalMilliseconds < TimeoutMs) {
				var requiredElement = GetElement();
				if(requiredElement?.Element != null) {
					testsLaunchingService.OpenHighlighter(requiredElement.Element.BoundingRectangle, null);
					OffsetPoint = GetClickablePointOffset(MatchedElement, requiredElement.Element);
					MouseHover.MoveTo(requiredElement.Element.BoundingRectangle.Location);
					break;
				}
				await WaitAppearElement(requiredElement);
			}

			if(!OffsetPoint.HasValue) {
				throw new TestFailedExeption(this);
			}
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
			return string.Format("{0}({1}, {2}, {3})", nameof(ActionsExecutor.ElementMatching), Program.ForNew(), SearchPath.ForNew(), TimeoutMs.ForNew());
		}

		async Task WaitAppearElement(RequiredElement requiredElement) {
			System.Windows.Rect rect;

			if(requiredElement?.Parent != null) {
				var offset = GetClickablePointOffset(requiredElement.ParentEquivalentInSearchPath, requiredElement.Parent);
				rect = MatchedElement.BoundingRectangle;
				rect.Offset(offset.X, offset.Y);
			} else {
				rect = MatchedElement.BoundingRectangle;
			}
			var clickableRect = rect;
			clickableRect.Offset(clickableRect.Width / 2, clickableRect.Height / 2);
			testsLaunchingService.CloseHighlighter();
			await MouseHover.Perform(clickableRect.Location, stepWaitAppear, 10);
			testsLaunchingService.OpenHighlighter(rect, MatchedElement.ToShortString());
			await Task.Delay(200);
			stepWaitAppear++;
		}

		System.Windows.Point GetClickablePointOffset(UiElement original, UiElement current) {
			var point = new System.Windows.Point();
			if(Anchor == System.Windows.Forms.AnchorStyles.None) {
				return point;
			}
			var originalRect = original.BoundingRectangle;
			var originalLocation = originalRect.Location;
			var originalSize = originalRect.Size;

			var currentRect = current.BoundingRectangle;
			var currentLocation = currentRect.Location;
			var currentSize = currentRect.Size;

			point.X = currentLocation.X - originalLocation.X;
			point.Y = currentLocation.Y - originalLocation.Y;
			var offsetWidth = currentSize.Width - originalSize.Width;
			var offsetHeight = currentSize.Height - originalSize.Height;

			if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Left) && Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Right)) {
				point.X = point.X - (offsetWidth / 2);
			} else if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Right)) {
				point.X = point.X - offsetWidth;
			}
			if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Top) && Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Bottom)) {
				point.Y = point.Y - (offsetHeight / 2);
			} else if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Bottom)) {
				point.Y = point.Y - offsetHeight;
			}
			return point;
		}

		UiElement GetRootElementFromDesktop() {
			var desktop = automationElementService.GetDesktop();
			var childs = GetChildren(desktop);

			var rootElement = SearchRequiredElement(SearchPath.Last(), childs);
			return rootElement;
		}

		RequiredElement GetElement() {
			UiElement rootElement;
			if(System.IO.Path.GetFileName(Program.FileName).ToLower() == "explorer.exe") {
				var desktop = automationElementService.GetDesktop();
				rootElement = GetRootElementFromDesktop();
			} else {
				rootElement = automationElementService.GetRootElement(Program);
			}

			if(rootElement == null) {
				return null;
			}

			var parentEquivalentInSearchPath = SearchPath[SearchPath.Count - 1];
			if(!AreUiElementsEquals(rootElement, parentEquivalentInSearchPath, false)) {
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
						settingsService.GetClickPositionToleranceInPercent()))) {
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
