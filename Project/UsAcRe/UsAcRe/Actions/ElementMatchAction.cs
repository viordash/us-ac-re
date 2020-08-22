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

namespace UsAcRe.Actions {
	public class ElementMatchAction : BaseAction {
		#region inner classes
		class RequiredElement {
			public UiElement Element { get; set; }
			public UiElement Parent { get; set; }
			public UiElement ParentEquivalentInSearchPath { get; set; }
		}
		#endregion

		public ElementProgram Program { get; set; }
		public List<UiElement> SearchPath { get; set; }
		public UiElement MatchedElement { get => SearchPath?[0]; }
		public int TimeoutMs { get; set; }

		int stepWaitAppear;

		public ElementMatchAction(ElementProgram program, List<UiElement> searchPath, int timeoutMs = 1000 * 1000) {
			Program = program;
			SearchPath = searchPath;
			TimeoutMs = timeoutMs;
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
			return string.Format("await new {0}({1}, {2}, {3}).{4}()", nameof(ElementMatchAction), Program.ForNew(), SearchPath.ForNew(), TimeoutMs.ForNew(),
				nameof(ElementMatchAction.ExecuteAsync));
		}

		async ValueTask DoWorkAsync() {
			await Task.Run(async () => {
				var stopwatch = Stopwatch.StartNew();
				stepWaitAppear = 0;
				while(!cancellationToken.IsCancellationRequested && stopwatch.Elapsed.TotalMilliseconds < TimeoutMs) {
					var requiredElement = GetElement();
					if(requiredElement != null && requiredElement.Element != null) {
						break;
					}
					await WaitAppearElement(requiredElement);
				}
				testsLaunchingService.CloseHighlighter();
			});
		}

		async Task WaitAppearElement(RequiredElement requiredElement) {
			System.Drawing.Point point;

			if(requiredElement != null && requiredElement.Parent != null) {
				point = CalculateRelativePoint(requiredElement);
			} else {
				point = new System.Drawing.Point((int)MatchedElement.BoundingRectangle.X, (int)MatchedElement.BoundingRectangle.Y);
			}
			var rect = MatchedElement.BoundingRectangle;

			testsLaunchingService.ShowHighlighter(rect, MatchedElement.ToShortString());
			await Task.Delay(100);
			MouseHover.Perform(point, stepWaitAppear);
			stepWaitAppear++;
		}

		System.Drawing.Point CalculateRelativePoint(RequiredElement requiredElement) {
			var originalLocation = requiredElement.ParentEquivalentInSearchPath.BoundingRectangle.Location;
			var originalSize = requiredElement.ParentEquivalentInSearchPath.BoundingRectangle.Size;
			var currentLocation = requiredElement.Parent.BoundingRectangle.Location;
			var currentSize = requiredElement.Parent.BoundingRectangle.Size;
			var shiftX = originalLocation.X - currentLocation.X;
			var shiftY = originalLocation.Y - currentLocation.Y;
			var shiftWidth = originalSize.Width - currentSize.Width;
			var shiftHeight = originalSize.Height - currentSize.Height;

			var point = new System.Drawing.Point((int)MatchedElement.BoundingRectangle.X, (int)MatchedElement.BoundingRectangle.Y);

			point.X = point.X - (int)shiftX;
			point.Y = point.Y - (int)shiftY;
			return point;
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
				|| (compareSizes && !AreSizeEquals(element1.BoundingRectangle.Size, element2.BoundingRectangle.Size))) {
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

		bool AreSizeEquals(System.Windows.Size size1, System.Windows.Size size2) {
			if((size1.Height <= 0 || size1.Width <= 0)
				&& (size2.Height <= 0 || size2.Width <= 0)) {
				return true;
			}
			var tolerance = (double)20;//			GetControlSizeToleranceInPercent();
			double heightTolerance = 0;
			double widthTolerance = 0;
			if(tolerance < 0) {
				return true;
			} else if(tolerance > 0) {
				tolerance = 100 / tolerance;
				heightTolerance = Math.Max((int)size1.Height, size2.Height) / tolerance;
				widthTolerance = Math.Max((int)size1.Width, size2.Width) / tolerance;
			}
			return Math.Abs((int)size1.Height - size2.Height) <= heightTolerance
				&& Math.Abs((int)size1.Width - size2.Width) <= widthTolerance;
		}


		bool AreValueEquals(UiElement element1, UiElement element2) {
			return true /*IsCheckByValue()*/
				&& StringEquals(element1.Value, element1.Value);
		}

		bool StringEquals(string s1, string s2) {
			return (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
				|| s1 == s2;
		}
	}
}
