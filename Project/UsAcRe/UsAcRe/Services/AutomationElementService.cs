﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using NGuard;
using UsAcRe.Exceptions;
using UsAcRe.Extensions;
using UsAcRe.Helpers;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Services {
	public interface IAutomationElementService {
		UiElement FromPoint(System.Windows.Point pt);
		UiElement FromHandle(IntPtr hwnd);
		List<UiElement> FindAllValidElements(UiElement element, TreeScope scope);
		UiElement GetParent(UiElement element);
		UiElement GetDesktop();
		IntPtr GetNativeWindowHandle(UiElement element);

		bool Compare(UiElement left, UiElement right);
		bool ElementsIsSimilar(UiElement expected, UiElement actual);
		string BuildFriendlyInfo(AutomationElement element);
		void RetrieveElementValue(UiElement element);
		ElementProgram GetProgram(UiElement element);
		UiElement GetRootElement(ElementProgram program);
		bool TryGetClickablePoint(UiElement element, out Point pt);
	}

	public class AutomationElementService : IAutomationElementService {
		readonly IWinApiService winApiService;

		public AutomationElementService(IWinApiService winApiService) {
			Guard.Requires(winApiService, nameof(winApiService));
			this.winApiService = winApiService;
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

		public bool Compare(UiElement left, UiElement right) {
			if(object.Equals(left, null)) {
				return (object.Equals(right, null));
			}
			if(object.Equals(right, null)) {
				return false;
			}

			if(left.BoundingRectangle != right.BoundingRectangle) {
				return false;
			}

			if(left.ControlTypeId != right.ControlTypeId) {
				return false;
			}

			if(!StringHelper.ImplicitEquals(left.Name, right.Name)) {
				return false;
			}

			if(!StringHelper.ImplicitEquals(left.Value, right.Value)) {
				return false;
			}

			bool leftAutomationElementEmpty = !TryGetAutomationElement(left, out AutomationElement leftAutomationElement);
			bool rightAutomationElementEmpty = !TryGetAutomationElement(right, out AutomationElement rightAutomationElement);
			if(leftAutomationElementEmpty) {
				return rightAutomationElementEmpty;
			}
			if(rightAutomationElementEmpty) {
				return false;
			}


			if(!StringHelper.ImplicitEquals(leftAutomationElement.Current.AutomationId, rightAutomationElement.Current.AutomationId)) {
				return false;
			}

			var leftRuntimeId = leftAutomationElement.GetRuntimeId();
			var rightRuntimeId = rightAutomationElement.GetRuntimeId();
			if(!leftRuntimeId.SequenceEqual(rightRuntimeId)) {
				return false;
			}

			if(!StringHelper.ImplicitEquals(leftAutomationElement.Current.ProviderDescription, rightAutomationElement.Current.ProviderDescription)) {
				return false;
			}
			return true;
		}

		public bool ElementsIsSimilar(UiElement expected, UiElement actual) {
			return expected.ControlTypeId == actual.ControlTypeId
				&& expected.Name == actual.Name
				&& expected.AutomationId == actual.AutomationId
				&& expected.Value == actual.Value;
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

		public UiElement GetRootElement(ElementProgram program) {
			var processes = Process.GetProcesses()
				.Where(x => x.MainWindowHandle != IntPtr.Zero)
				.Where(x => SafeGetProcessFileName(x) == program.FileName)
				.OrderBy(x => x.StartTime)
				.ToList();
			if(processes.Count <= program.Index) {
				throw new TargetProgramNotFoundExeption(program);
			}
			var process = processes[program.Index];
			var element = AutomationElement.FromHandle(process.MainWindowHandle);
			return ToUiElement(element);
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
