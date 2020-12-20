using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using NGuard;
using UsAcRe.Core.Extensions;
using UsAcRe.Core.Helpers;
using UsAcRe.Core.UIAutomationElement;

namespace UsAcRe.Core.Services {
	public interface IAutomationElementService {
		UiElement FromPoint(Point pt);
		UiElement FromHandle(IntPtr hwnd);
		List<UiElement> FindAllValidElements(UiElement element, TreeScope scope);
		UiElement GetParent(UiElement element);
		UiElement GetDesktop();
		IntPtr GetNativeWindowHandle(UiElement element);

		bool Compare(UiElement left, UiElement right, ElementCompareParameters parameters);
		string BuildFriendlyInfo(AutomationElement element);
		void RetrieveElementValue(UiElement element);
		bool TryGetAutomationElement(UiElement uiElement, out AutomationElement automationElement);
		ElementProgram GetProgram(UiElement element);
		UiElement GetRootElement(ElementProgram program, bool windowHandleFromWinApi);
		bool TryGetClickablePoint(UiElement element, out Point pt);
	}

	public class AutomationElementService : IAutomationElementService {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");
		readonly IWinApiService winApiService;
		readonly ISettingsService settingsService;

		public AutomationElementService(
			IWinApiService winApiService,
			ISettingsService settingsService) {
			Guard.Requires(winApiService, nameof(winApiService));
			Guard.Requires(settingsService, nameof(settingsService));
			this.winApiService = winApiService;
			this.settingsService = settingsService;
		}

		public UiElement FromPoint(Point pt) {
			return ToUiElement(AutomationElement.FromPoint(pt));
		}

		public UiElement FromHandle(IntPtr hwnd) {
			return ToUiElement(AutomationElement.FromHandle(hwnd));
		}

		public List<UiElement> FindAllValidElements(UiElement element, TreeScope scope) {
			if(!TryGetAutomationElement(element, out AutomationElement automationElement)) {
				return null;
			}

			var cond = Condition.TrueCondition;
			return automationElement.FindAll(scope, cond)
				.OfType<AutomationElement>()
				.Select(x => ToUiElement(x))
				.ToList();
		}

		public UiElement GetParent(UiElement element) {
			if(!TryGetAutomationElement(element, out AutomationElement automationElement)) {
				return null;
			}
			return ToUiElement(TreeWalker.RawViewWalker.GetParent(automationElement));
		}

		public UiElement GetDesktop() {
			return ToUiElement(AutomationElement.RootElement);
		}

		public IntPtr GetNativeWindowHandle(UiElement element) {
			if(!TryGetAutomationElement(element, out AutomationElement automationElement)) {
				return IntPtr.Zero;
			}
			return new IntPtr(automationElement.Current.NativeWindowHandle);
		}

		public string BuildFriendlyInfo(AutomationElement element) {
			return string.Format("'{0}' '{1}' '{2}' '{3}'", NamingHelpers.Escape(element.Current.Name, 300), NamingHelpers.Escape(element.Current.AutomationId, 300),
				element.Current.ClassName, element.Current.ControlType.ProgrammaticName);
		}


		public bool Compare(UiElement left, UiElement right, ElementCompareParameters parameters) {
			return left.Differences(right, parameters, this) == null;
		}

		UiElement ToUiElement(AutomationElement element) {
			if(element == null) {
				return null;
			}
			try {
				return new UiElement(-1, null, element.Current.Name?.MaxLength(100), element.Current.ClassName,
					element.Current.AutomationId.MaxLength(100), element.Current.ControlType.Id, element.Current.BoundingRectangle) {
					AutomationElementObj = element
				};
			} catch {
				return null;
			}
		}

		string GetValue(AutomationElement element) {
			if(!element.Current.IsPassword && element.TryGetCurrentPattern(ValuePattern.Pattern, out object patternObj)) {
				var valuePattern = (ValuePattern)patternObj;
				var s = valuePattern.Current.Value;
				return s.Substring(0, Math.Min(s.Length, 300));
			} /*else if(element.TryGetCurrentPattern(TextPattern.Pattern, out patternObj)) {
                var textPattern = (TextPattern)patternObj;
                return textPattern.DocumentRange.GetText(-1).TrimEnd('\r'); // often there is an extra '\r' hanging off the end.
            }*/ else {
				return null;
			}
		}

		public bool TryGetAutomationElement(UiElement uiElement, out AutomationElement automationElement) {
			automationElement = uiElement?.AutomationElementObj as AutomationElement;
			return automationElement != null;
		}

		public void RetrieveElementValue(UiElement element) {
			if(TryGetAutomationElement(element, out AutomationElement automationElement)) {
				element.Value.Value = GetValue(automationElement);
			}
		}

		public ElementProgram GetProgram(UiElement element) {
			if(!TryGetAutomationElement(element, out AutomationElement automationElement)) {
				throw new InvalidOperationException();
			}

			var selectedProcess = Process.GetProcessById(automationElement.Current.ProcessId);
			var processes = Process.GetProcesses()
				.Where(x => x.MainWindowHandle != IntPtr.Zero)
				.Where(x => x.ProcessName == selectedProcess.ProcessName)
				.OrderBy(x => x.StartTime)
				.ToList();
			var index = processes.FindIndex(x => x.Id == selectedProcess.Id);

			var elementProgram = new ElementProgram(index, selectedProcess.ProcessName);
			return elementProgram;
		}

		public UiElement GetRootElement(ElementProgram program, bool windowHandleFromWinApi) {
			var processes = Process.GetProcesses()
				.Where(x => x.MainWindowHandle != IntPtr.Zero)
				.Where(x => x.ProcessName == program.FileName)
				.OrderBy(x => x.StartTime)
				.ToList();
			if(processes.Count <= program.Index) {
				logger.Warn("TargetProgramNotFoundExeption '{0}'", program.ToString());
				return null;
			}
			var process = processes[program.Index];
			if(!process.WaitForInputIdle(100)) {
				logger.Warn("WaitForInputIdle timeout '{0}'", program.ToString());
				return null;
			}
			var hwnd = !windowHandleFromWinApi
				? process.MainWindowHandle
				: winApiService.GetWindowHandle(process.Id);
			return ToUiElement(AutomationElement.FromHandle(hwnd));
		}

		public bool TryGetClickablePoint(UiElement element, out Point pt) {
			if(TryGetAutomationElement(element, out AutomationElement automationElement)) {
				try {
					pt = automationElement.GetClickablePoint();
					return true;
				} catch(NoClickablePointException) { } catch(ElementNotAvailableException) { }
			}
			pt = new Point();
			return false;
		}
	}
}
