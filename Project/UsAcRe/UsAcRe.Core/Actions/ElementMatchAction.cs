using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using NGuard;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.Services;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Actions {
	public class ElementMatchAction : BaseAction {
		const int defaultTimeoutMs = 20 * 1000;
		public const AnchorStyles defaultAnchor = AnchorStyles.Top | AnchorStyles.Left;

		#region inner classes
		class RequiredElement {
			public UiElement Element { get; set; }
			public UiElement Parent { get; set; }
			public UiElement ParentEquivalentInSearchPath { get; set; }
		}
		#endregion

		public static ElementMatchAction Record(ElementProgram program, List<UiElement> searchPath) {
			var instance = CreateInstance<ElementMatchAction>();
			instance.Program = program;
			instance.SearchPath = searchPath;
			return instance;
		}

		public static async Task Play(ElementProgram program, List<UiElement> searchPath, int timeoutMs = defaultTimeoutMs, AnchorStyles anchor = defaultAnchor) {
			var instance = CreateInstance<ElementMatchAction>();
			instance.Program = program;
			instance.SearchPath = searchPath;
			instance.Anchor = anchor;
			instance.TimeoutMs = timeoutMs;
			await instance.ExecuteAsync();
		}

		public ElementProgram Program { get; private set; }
		public List<UiElement> SearchPath { get; private set; }
		public AnchorStyles Anchor { get; set; } = defaultAnchor;
		public UiElement MatchedElement { get => SearchPath?[0]; }
		public int TimeoutMs { get; set; } = defaultTimeoutMs;
		public System.Windows.Point? OffsetPoint { get; private set; } = null;
		int stepWaitAppear;

		readonly IAutomationElementService automationElementService;

		public ElementMatchAction(
			IAutomationElementService automationElementService,
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService) : base(settingsService, testsLaunchingService) {
			Guard.Requires(automationElementService, nameof(automationElementService));
			this.automationElementService = automationElementService;
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
			var sb = new StringBuilder();
			sb.AppendFormat("{0}.{1}({2}, {3}", nameof(ElementMatchAction), nameof(ElementMatchAction.Play), Program.ForNew(), SearchPath.ForNew());
			if(TimeoutMs != defaultTimeoutMs) {
				sb.AppendFormat(", {0}", TimeoutMs.ForNew());
			}
			if(Anchor != defaultAnchor) {
				sb.AppendFormat(", {0}", Anchor.ForNew());
			}
			sb.Append(")");
			return sb.ToString();
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

		UiElement GetRootElement(bool windowHandleFromWinApi) {
			UiElement rootElement;
			if(System.IO.Path.GetFileName(Program.FileName).ToLower() == "explorer.exe") {
				rootElement = GetRootElementFromDesktop();
			} else {
				rootElement = automationElementService.GetRootElement(Program, windowHandleFromWinApi);
			}
			return rootElement;
		}

		RequiredElement GetElement() {
			var parentEquivalentInSearchPath = SearchPath[SearchPath.Count - 1];
			var rootElement = GetRootElement(false);
			if(rootElement == null) {
				return null;
			}

			if(!automationElementService.Compare(rootElement, parentEquivalentInSearchPath, new ElementCompareParameters() {
				AutomationElementInternal = false,
				Anchor = defaultAnchor,
				CompareLocation = false,
				CompareSizes = false,
				NameIsMatchCase = true,
				NameIsMatchWholeWord = true,
				CheckByValue = false
			})) {
				logger.Debug("attempt to retrieve the rootElement with help of window handle from WinApi");
				rootElement = GetRootElement(true);
				if(rootElement == null) {
					return null;
				}
				if(!automationElementService.Compare(rootElement, parentEquivalentInSearchPath, new ElementCompareParameters() {
					AutomationElementInternal = false,
					Anchor = defaultAnchor,
					CompareLocation = false,
					CompareSizes = false,
					NameIsMatchCase = true,
					NameIsMatchWholeWord = true,
					CheckByValue = false
				})) {
					return null;
				}
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

				//logger.Debug(" ---    search element {0}", element);

				requiredElement.Element = SearchRequiredElement(element, childs);
				if(requiredElement.Element == null) {
					break;
				}
				childs = GetChildren(requiredElement.Element);
			}
			return requiredElement;
		}

		UiElement SearchRequiredElement(UiElement searchedElement, List<UiElement> childs) {
			bool isTargetedElementWithPresentedValue = searchedElement == SearchPath[0];
			foreach(var element in childs) {
				if(!automationElementService.Compare(element, searchedElement, new ElementCompareParameters() {
					AutomationElementInternal = false,
					Anchor = defaultAnchor,
					CompareLocation = true,
					LocationToleranceInPercent = settingsService.ClickPositionToleranceInPercent,
					CompareSizes = true,
					SizeToleranceInPercent = settingsService.ClickPositionToleranceInPercent,
					NameIsMatchCase = true,
					NameIsMatchWholeWord = true,
					CheckByValue = settingsService.CheckByValue && isTargetedElementWithPresentedValue
				})) {
					continue;
				}
				if(searchedElement.Index > 0) {
					var similars = childs
						.Where(x => automationElementService.CompareInSiblings(x, element, ElementCompareParameters.ForSimilars()))
						.ToList();
					if(similars.Count > searchedElement.Index) {
						return similars[searchedElement.Index];
					}
				} else {
					return element;
				}
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

		bool AreValueEquals(UiElement element1, UiElement element2) {
			return settingsService.CheckByValue
				&& StringEquals(element1.Value, element2.Value);
		}

		bool StringEquals(string s1, string s2) {
			return (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
				|| s1 == s2;
		}
	}
}
