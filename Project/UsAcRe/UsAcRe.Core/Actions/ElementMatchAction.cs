using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using NGuard;
using UsAcRe.Core.Exceptions;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.MouseProcess;
using UsAcRe.Core.Services;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Actions {
	public class ElementMatchAction : BaseAction {

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

		public static async Task Play(ElementProgram program, List<UiElement> searchPath, int timeoutMs = TestActionConstants.defaultTimeoutMs,
					AnchorStyles anchor = TestActionConstants.defaultAnchor) {
			var instance = CreateInstance<ElementMatchAction>();
			instance.Program = program;
			instance.SearchPath = searchPath;
			instance.Anchor = anchor;
			instance.TimeoutMs = timeoutMs;
			await instance.ExecuteAsync();
		}

		public ElementProgram Program { get; private set; }
		public List<UiElement> SearchPath { get; private set; }
		public AnchorStyles Anchor { get; set; } = TestActionConstants.defaultAnchor;
		public UiElement MatchedElement { get => SearchPath?[0]; }
		public int TimeoutMs { get; set; } = TestActionConstants.defaultTimeoutMs;
		public System.Windows.Point? OffsetPoint { get; private set; } = null;
		int stepWaitAppear;

		readonly IAutomationElementService automationElementService;

		public ElementMatchAction(
			IAutomationElementService automationElementService,
			ISettingsService settingsService,
			ITestsLaunchingService testsLaunchingService,
			IFileService fileService) : base(settingsService, testsLaunchingService, fileService) {
			Guard.Requires(automationElementService, nameof(automationElementService));
			this.automationElementService = automationElementService;
		}

		protected override async ValueTask ExecuteCoreAsync() {
			stepWaitAppear = 0;
			OffsetPoint = null;
			var stopwatch = Stopwatch.StartNew();
			while(stopwatch.Elapsed.TotalMilliseconds < TimeoutMs) {
				if(cancellationToken.IsCancellationRequested) {
					throw new OperationCanceledException(this.ToString());
				}
				var requiredElement = GetElement();
				if(requiredElement?.Element != null) {
					OffsetPoint = GetClickablePointOffset(MatchedElement, requiredElement.Element);
					MouseHover.MoveTo(requiredElement.Element.BoundingRectangle.Value.Location);
					break;
				}
				await WaitAppearElement(requiredElement);
			}

			if(!OffsetPoint.HasValue) {
				throw new TestFailedException(this);
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
			sb.AppendFormat("{0}.{1}({2}, {3}", nameof(ElementMatchAction), nameof(ElementMatchAction.Play), Program?.ForNew(), SearchPath.ForNew());
			if(TimeoutMs != TestActionConstants.defaultTimeoutMs) {
				sb.AppendFormat(", {0}", TimeoutMs.ForNew());
			}
			if(Anchor != TestActionConstants.defaultAnchor) {
				sb.AppendFormat(", {0}", Anchor.ForNew());
			}
			sb.Append(")");
			return sb.ToString();
		}

		public override string ShortDescription() {
			var targetElem = SearchPath.FirstOrDefault();
			return string.Format("Element: '{0}' {1}", Path.GetFileNameWithoutExtension(Program.FileName), targetElem?.ToShortString());
		}

		async Task WaitAppearElement(RequiredElement requiredElement) {
			System.Windows.Rect rect;

			if(requiredElement?.Parent != null) {
				var offset = GetClickablePointOffset(requiredElement.ParentEquivalentInSearchPath, requiredElement.Parent);
				rect = MatchedElement.BoundingRectangle.Value;
				rect.Offset(offset.X, offset.Y);
			} else {
				rect = MatchedElement.BoundingRectangle.Value;
			}
			var clickableRect = rect;
			clickableRect.Offset(clickableRect.Width / 2, clickableRect.Height / 2);
			testsLaunchingService.CloseHighlighter();
			await MouseHover.Perform(clickableRect.Location, stepWaitAppear, 10);
			testsLaunchingService.OpenHighlighter(rect, MatchedElement.ToShortString());
			var period = Math.Min(200 + (stepWaitAppear * 100), 1000);
			await Task.Delay(period);
			stepWaitAppear++;
		}

		System.Windows.Point GetClickablePointOffset(UiElement original, UiElement current) {
			var point = new System.Windows.Point();
			if(Anchor == System.Windows.Forms.AnchorStyles.None) {
				return point;
			}
			var originalRect = original.BoundingRectangle;
			var originalLocation = originalRect.Value.Location;
			var originalSize = originalRect.Value.Size;

			var currentRect = current.BoundingRectangle;
			var currentLocation = currentRect.Value.Location;
			var currentSize = currentRect.Value.Size;

			point.X = currentLocation.X - originalLocation.X;
			point.Y = currentLocation.Y - originalLocation.Y;
			var offsetWidth = currentSize.Width - originalSize.Width;
			var offsetHeight = currentSize.Height - originalSize.Height;

			if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Left) && Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Right)) {
				point.X -= (offsetWidth / 2);
			} else if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Right)) {
				point.X -= offsetWidth;
			}
			if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Top) && Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Bottom)) {
				point.Y -= (offsetHeight / 2);
			} else if(Anchor.HasFlag(System.Windows.Forms.AnchorStyles.Bottom)) {
				point.Y -= offsetHeight;
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
			if(System.IO.Path.GetFileName(Program.FileName).ToLower() == "explorer") {
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

			var compareParameters = new ElementCompareParameters() {
				AutomationElementInternal = false,
				Anchor = TestActionConstants.defaultAnchor,
				CompareLocation = false,
				CompareSizes = false,
				NameIsMatchCase = true,
				NameIsMatchWholeWord = true,
				CheckByValue = false
			};

			FailMessage = rootElement.Differences(parentEquivalentInSearchPath, compareParameters, automationElementService);
			if(FailMessage != null) {
				logger.Debug("attempt to retrieve the rootElement with help of window handle from WinApi");
				rootElement = GetRootElement(true);
				if(rootElement == null) {
					return null;
				}
				FailMessage = rootElement.Differences(parentEquivalentInSearchPath, compareParameters, automationElementService);
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
				var orderedDifference = element.Differences(searchedElement, new ElementCompareParameters() {
					AutomationElementInternal = false,
					Anchor = TestActionConstants.defaultAnchor,
					CompareLocation = settingsService.LocationToleranceInPercent.HasValue && settingsService.LocationToleranceInPercent.Value > 0,
					CompareSizes = true,
					SizeToleranceInPercent = settingsService.ClickPositionToleranceInPercent,
					LocationToleranceInPercent = settingsService.LocationToleranceInPercent.HasValue
						? settingsService.LocationToleranceInPercent.Value
						: -1,
					NameIsMatchCase = true,
					NameIsMatchWholeWord = true,
					CheckByValue = settingsService.CheckByValue && isTargetedElementWithPresentedValue
				}, automationElementService);

				if(orderedDifference != null) {
					if(FailMessage == null || FailMessage.Weight < orderedDifference.Weight) {
						FailMessage = orderedDifference;
						Debug.WriteLine($"               -------  {FailMessage.Difference()}");
					}
					continue;
				}
				if(searchedElement.Index > 0) {
					var similars = childs
						.Where(x => automationElementService.Compare(x, element, ElementCompareParameters.ForSimilars()))
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
			var controlType = ControlType.LookupById(element.ControlTypeId.Value);
			if(controlType == ControlType.Tree || controlType == ControlType.TreeItem) {
				return automationElementService.FindAllValidElements(element, TreeScope.Descendants);
			} else {
				return automationElementService.FindAllValidElements(element, TreeScope.Children);
			}
		}
	}
}
