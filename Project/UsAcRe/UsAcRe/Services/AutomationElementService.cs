using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using NGuard;
using UsAcRe.Helpers;

namespace UsAcRe.Services {
	public interface IAutomationElementService {
		AutomationElement FromPoint(System.Windows.Point pt);
		AutomationElement FromHandle(IntPtr hwnd);
		AutomationElementCollection FindAll(AutomationElement element, TreeScope scope, Condition condition);
		AutomationElement GetParent(AutomationElement element);

		bool Compare(AutomationElement left, AutomationElement right);
		string BuildFriendlyInfo(AutomationElement element);
	}

	public class AutomationElementService : IAutomationElementService {
		readonly IWinApiService winApiService;

		public AutomationElementService(IWinApiService winApiService) {
			Guard.Requires(winApiService, nameof(winApiService));
			this.winApiService = winApiService;
		}

		public AutomationElement FromHandle(IntPtr hwnd) {
			return AutomationElement.FromHandle(hwnd);
		}

		public AutomationElement FromPoint(Point pt) {
			return AutomationElement.FromPoint(pt);
		}

		public AutomationElementCollection FindAll(AutomationElement element, TreeScope scope, Condition condition) {
			return element.FindAll(scope, condition);
		}

		public AutomationElement GetParent(AutomationElement element) {
			return TreeWalker.RawViewWalker.GetParent(element);
		}

		public string BuildFriendlyInfo(AutomationElement element) {
			return string.Format("'{0}' '{1}' '{2}' '{3}'", NamingHelpers.Escape(element.Current.Name, 300), NamingHelpers.Escape(element.Current.AutomationId, 300),
				element.Current.ClassName, element.Current.ControlType.ProgrammaticName);
		}

		public bool Compare(AutomationElement left, AutomationElement right) {
			if(object.Equals(left, null)) {
				return (object.Equals(right, null));
			}
			if(object.Equals(right, null)) {
				return (object.Equals(left, null));
			}

			if(left.Current.BoundingRectangle == right.Current.BoundingRectangle
				&& left.Current.ControlType == right.Current.ControlType
				&& left.Current.Name == right.Current.Name
				&& left.Current.AutomationId == right.Current.AutomationId) {
				var leftRuntimeId = left.GetRuntimeId();
				var rightRuntimeId = right.GetRuntimeId();
				return leftRuntimeId.Length != rightRuntimeId.Length || leftRuntimeId.SequenceEqual(rightRuntimeId);
			}
			return false;
		}
	}
}
