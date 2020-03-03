﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using NGuard;
using UsAcRe.Helpers;
using UsAcRe.UIAutomationElement;

namespace UsAcRe.Services {
	public interface IAutomationElementService {
		UiElement FromPoint(System.Windows.Point pt);
		UiElement FromHandle(IntPtr hwnd);
		List<UiElement> FindAll(UiElement element, TreeScope scope, Condition condition);
		UiElement GetParent(UiElement element);
		IntPtr GetNativeWindowHandle(UiElement element);

		bool Compare(UiElement left, UiElement right);
		string BuildFriendlyInfo(AutomationElement element);
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

		public List<UiElement> FindAll(UiElement element, TreeScope scope, Condition condition) {
			if(!TryGetAutomationElement(element, out AutomationElement automationElement)) {
				return null;
			}
			return automationElement.FindAll(scope, condition)
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

			if(left.BoundingRectangle != right.BoundingRectangle
				|| left.ControlTypeId != right.ControlTypeId
				&& left.Name != right.Name
				&& left.Value != right.Value) {
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

			if(leftAutomationElement.Current.AutomationId == rightAutomationElement.Current.AutomationId) {
				var leftRuntimeId = leftAutomationElement.GetRuntimeId();
				var rightRuntimeId = rightAutomationElement.GetRuntimeId();
				return leftRuntimeId.Length != rightRuntimeId.Length || leftRuntimeId.SequenceEqual(rightRuntimeId);
			}
			return false;
		}

		UiElement ToUiElement(AutomationElement element) {
			try {
				return new UiElement() {
					Value = GetValue(element),
					Name = NamingHelpers.Escape(element.Current.Name, 300),
					ControlTypeId = element.Current.ControlType.Id,
					BoundingRectangle = element.Current.BoundingRectangle,
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
			automationElement = uiElement.AutomationElementObj as AutomationElement;
			return automationElement != null;
		}
	}
}
