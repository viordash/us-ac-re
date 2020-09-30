using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using NGuard;
using UsAcRe.Core.Exceptions;
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

		bool CompareInSiblings(UiElement left, UiElement right, ElementCompareParameters parameters);
		bool Compare(UiElement left, UiElement right, ElementCompareParameters parameters);
		string BuildFriendlyInfo(AutomationElement element);
		void RetrieveElementValue(UiElement element);
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

			var rect = new Rect(0, 0, 0, 0);
			var condBoundingRectangle = new PropertyCondition(AutomationElement.BoundingRectangleProperty, rect);
			var condOffscreen = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
			var cond = new AndCondition(new NotCondition(condBoundingRectangle), condOffscreen);
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


		public bool CompareInSiblings(UiElement left, UiElement right, ElementCompareParameters parameters) {
			return Compare(left, right, int.MaxValue, parameters, false);
		}

		public bool Compare(UiElement left, UiElement right, ElementCompareParameters parameters) {
			return Compare(left, right, int.MaxValue, parameters, true);
		}

		bool CompareValue(UiElement left, UiElement right) {
			bool leftEmpty = string.IsNullOrEmpty(left.Value);
			bool rightEmpty = string.IsNullOrEmpty(right.Value);
			if(leftEmpty && rightEmpty) {
				return true;
			}
			if(leftEmpty) {
				RetrieveElementValue(left);
			}
			if(rightEmpty) {
				RetrieveElementValue(right);
			}
			return StringHelper.ImplicitEquals(left.Value, right.Value);
		}

		bool Compare(UiElement left, UiElement right, int nestedLevel, ElementCompareParameters parameters, bool logging) {
			if(object.Equals(left, null)) {
				return (object.Equals(right, null));
			}
			if(object.Equals(right, null)) {
				return false;
			}
			//logger.Debug("Compare ({0}) ({1})", left, right);
			if(left.ControlTypeId != right.ControlTypeId) {
				if(logging) {
					logger.Debug("left.ControlTypeId != right.ControlTypeId ({0}) != ({1})", left.ControlTypeId, right.ControlTypeId);
				}
				return false;
			}

			if(!StringHelper.ImplicitEquals(left.Name, right.Name)) {
				if(logging) {
					logger.Debug("left.Name != right.Name ({0}) != ({1})", left.Name, right.Name);
				}
				return false;
			}

			if(!StringHelper.ImplicitEquals(left.AutomationId, right.AutomationId)) {
				if(logging) {
					logger.Debug("left.AutomationId != right.AutomationId ({0}) != ({1})", left.AutomationId, right.AutomationId);
				}
				return false;
			}

			if(parameters.CompareLocation
				&& !DimensionsHelper.AreLocationEquals(left.BoundingRectangle.Location, right.BoundingRectangle.Location, parameters.LocationToleranceInPercent)) {
				if(logging) {
					logger.Debug("DimensionsHelper.AreLocationEquals ({0}) != ({1}), {2}%", left.BoundingRectangle.Location, right.BoundingRectangle.Location, parameters.LocationToleranceInPercent);
				}
				return false;
			}

			if(parameters.CompareSizes
				&& !DimensionsHelper.AreSizeEquals(left.BoundingRectangle.Size, right.BoundingRectangle.Size, parameters.SizeToleranceInPercent)) {
				if(logging) {
					logger.Debug("DimensionsHelper.AreSizeEquals ({0}) != ({1}), {2}%", left.BoundingRectangle.Size, right.BoundingRectangle.Size, parameters.SizeToleranceInPercent);
				}
				return false;
			}

			if(parameters.CheckByValue && !CompareValue(left, right)) {
				if(logging) {
					logger.Debug("left.Value != right.Value ({0}) != ({1})", left.Value, right.Value);
				}
				return false;
			}

			if(!parameters.AutomationElementInternal) {
				return true;
			}

			bool leftAutomationElementEmpty = !TryGetAutomationElement(left, out AutomationElement leftAutomationElement);
			bool rightAutomationElementEmpty = !TryGetAutomationElement(right, out AutomationElement rightAutomationElement);
			if(leftAutomationElementEmpty) {
				return rightAutomationElementEmpty;
			}
			if(rightAutomationElementEmpty) {
				return false;
			}

			var leftRuntimeId = leftAutomationElement.GetRuntimeId();
			var rightRuntimeId = rightAutomationElement.GetRuntimeId();
			if(!leftRuntimeId.SequenceEqual(rightRuntimeId)) {
				if(logging) {
					logger.Debug("left.GetRuntimeId() != right.GetRuntimeId() ({0}) != ({1})", string.Join(", ", leftRuntimeId), string.Join(", ", rightRuntimeId));
				}
				return false;
			}

			if(!StringHelper.ImplicitEquals(leftAutomationElement.Current.ProviderDescription, rightAutomationElement.Current.ProviderDescription)) {
				if(logging) {
					logger.Debug("left.ProviderDescription != right.ProviderDescription ({0}) != ({1})", leftAutomationElement.Current.ProviderDescription, rightAutomationElement.Current.ProviderDescription);
				}
				return false;
			}

			if(nestedLevel > settingsService.ElementSearchNestingLevel) {
				return true;
			}
			var leftParent = TreeWalker.RawViewWalker.GetParent(leftAutomationElement);
			var rightParent = TreeWalker.RawViewWalker.GetParent(rightAutomationElement);
			return Compare(ToUiElement(leftParent), ToUiElement(rightParent), nestedLevel + 1, parameters, logging);
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

		bool TryGetAutomationElement(UiElement uiElement, out AutomationElement automationElement) {
			automationElement = uiElement?.AutomationElementObj as AutomationElement;
			return automationElement != null;
		}

		public void RetrieveElementValue(UiElement element) {
			if(TryGetAutomationElement(element, out AutomationElement automationElement)) {
				element.Value = GetValue(automationElement);
			}
		}

		public ElementProgram GetProgram(UiElement element) {
			if(!TryGetAutomationElement(element, out AutomationElement automationElement)) {
				throw new InvalidOperationException();
			}

			var selectedProcess = Process.GetProcessById(automationElement.Current.ProcessId);
			var processes = Process.GetProcesses()
				.Where(x => x.ProcessName == selectedProcess.ProcessName)
				.OrderBy(x => x.StartTime)
				.ToDictionary(x => x.Id, x => Path.GetFileName(x.MainModule.FileName));

			var exePath = Path.GetFileName(selectedProcess.MainModule.FileName);
			var singleFileProcesses = processes
				.Where(x => x.Value == exePath)
				.ToList();
			var index = singleFileProcesses.FindIndex(x => x.Key == selectedProcess.Id);

			var elementProgram = new ElementProgram(index, exePath);
			return elementProgram;
		}

		string SafeGetProcessFileName(Process process) {
			try {
				return Path.GetFileName(process.MainModule.FileName);
			} catch(Win32Exception ex) {
				if((uint)ex.ErrorCode != 0x80004005) {
					throw;
				}
			}
			return null;
		}

		public UiElement GetRootElement(ElementProgram program, bool windowHandleFromWinApi) {
			var processes = Process.GetProcesses()
				.Where(x => x.MainWindowHandle != IntPtr.Zero)
				.Where(x => SafeGetProcessFileName(x) == program.FileName)
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
