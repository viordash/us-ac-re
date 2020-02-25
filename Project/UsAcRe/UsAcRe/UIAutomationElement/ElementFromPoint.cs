using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using UsAcRe.Helpers;
using UsAcRe.WindowsSystem;

namespace UsAcRe.UIAutomationElement {
	public class ElementFromPoint {
		NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.Trace");

		#region inner classes
		class MatchedElement {
			public AutomationElement Element;
			//public int ZOrder;
			public System.Windows.Rect BoundingRectangle;
		}

		class ElementsUderPoint {
			public IntPtr Handle;
			public System.Windows.Rect BoundingRectangle;
			public string ClassName;
			public string WindowName;
			public int ZOrder;
		}

		class BoundingRectangleComp : IComparer<System.Windows.Rect> {
			public int Compare(System.Windows.Rect x, System.Windows.Rect y) {
				return (x.Height + x.Width).CompareTo(y.Height + y.Width);
			}
		}
		#endregion

		AutomationElement Element;
		public Point ElementCoord;
		public int Iterations;
		public System.Windows.Rect BoundingRectangle {
			get {
				try {
					if(Element != null) {
						return Element.Current.BoundingRectangle;
					}
				} catch(Exception ex) {
					logger.Error(ex);
				}
				return System.Windows.Rect.Empty;
			}
		}

		List<MatchedElement> treeElements = new List<MatchedElement>();

		public ElementFromPoint(Point elementCoord) {
			Iterations = 0;
			ElementCoord = elementCoord;
			DetermineElementUnderPoint();
		}

		public override string ToString() {
			if(Element != null) {
				return string.Format($"{Iterations}| {nameof(ElementFromPoint)} ({ElementCoord.X}, {ElementCoord.Y}). Name: {NamingHelpers.Escape(Element.Current.Name, 50)}, " +
					$"{Element.Current.ControlType.ProgrammaticName}, {Element.Current.BoundingRectangle}");
			} else {
				return string.Format($"{nameof(ElementFromPoint)} ({ElementCoord.X}, {ElementCoord.Y}). No element");
			}
		}

		void DetermineElementUnderPoint() {
			Element = GetElementFromPoint();
			if(Element != null) {
				logger.Trace("             DetermineElementUnderPoint 1: {0}; {1}; {2}", NamingHelpers.Escape(Element.Current.AutomationId, 300),
					Element.Current.ControlType.ProgrammaticName, Element.Current.BoundingRectangle);
			}
		}

		AutomationElement GetFirstSiblingElement(AutomationElement elementUnderPoint) {
			var parentEl = TreeWalker.RawViewWalker.GetParent(elementUnderPoint);
			bool parentIsDesktop = AutomationElement.RootElement == parentEl;
			if(!parentIsDesktop) {
				elementUnderPoint = TreeWalker.RawViewWalker.GetFirstChild(parentEl);
			} else {
				elementUnderPoint = TreeWalker.RawViewWalker.GetFirstChild(elementUnderPoint);
			}

			return elementUnderPoint;
		}

		AutomationElement GetFirstChild(AutomationElement parentElement) {
			//try {
			return TreeWalker.RawViewWalker.GetFirstChild(parentElement);
			//} catch(NullReferenceException) {
			//}
			//return null;
		}

		bool ChildIsUnderPoint(AutomationElement parentElement) {
			AutomationElement childElement = GetFirstChild(parentElement);
			if(childElement == null) {
				return false;
			}

			try {
				var iterElement = childElement;
				do {
					var boundingRectangle = iterElement.Current.BoundingRectangle;
					if(boundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
						return true;
					}
					Iterations++;
				} while((iterElement = TreeWalker.ControlViewWalker.GetNextSibling(iterElement)) != null);
			} catch(NullReferenceException) {

			}
			return false;
		}

		List<MatchedElement> ObtainSiblingsUnderPoint(AutomationElement startElement, AutomationElement parentElement, int parentZOrder) {
			var matchedElements = new List<MatchedElement>();
			try {
				var iterElement = startElement;
				do {
					var zOrder = GetZOrder(iterElement);
					var boundingRectangle = iterElement.Current.BoundingRectangle;
					if(!boundingRectangle.Contains(ElementCoord.X, ElementCoord.Y)) {
						bool preventOfInfinityLoop = RawCompareElement(parentElement, parentZOrder, iterElement, zOrder);
						if(preventOfInfinityLoop) {
							continue;
						}
						if(!ChildIsUnderPoint(iterElement)) {
							continue;
						} else {
							Debug.WriteLine("ChildIsUnderPoint 1: {0}; {1}; {2}", NamingHelpers.Escape(Element.Current.AutomationId, 300),
																Element.Current.ControlType.ProgrammaticName, Element.Current.BoundingRectangle);
						}
					}
					if(matchedElements.Any(x => Automation.Compare(x.Element, iterElement))) {
						continue;
					}

					matchedElements.Add(new MatchedElement() {
						Element = iterElement,
						//ZOrder = zOrder,
						BoundingRectangle = boundingRectangle
					});
				} while((iterElement = TreeWalker.ControlViewWalker.GetNextSibling(iterElement)) != null);
			} catch(NullReferenceException) {

			}
			return matchedElements;
		}

		bool RawCompareElement(AutomationElement parentElement, int parentZOrder, AutomationElement element, int zOrder) {
			if(parentElement.Current.BoundingRectangle == element.Current.BoundingRectangle
				&& parentElement.Current.ControlType == element.Current.ControlType
				&& parentElement.Current.Name == element.Current.Name
				&& parentElement.Current.AutomationId == element.Current.AutomationId
				&& parentZOrder == zOrder) {
				return true;
			}
			return false;
		}

		List<MatchedElement> ObtainChildsUnderPoint(AutomationElement parentElement) {
			AutomationElement childElement = GetFirstChild(parentElement);
			if(childElement == null) {
				return null;
			}
			var parentElementZOrder = GetZOrder(parentElement);
			var matchedElements = ObtainSiblingsUnderPoint(childElement, parentElement, parentElementZOrder);
			var childElements = new List<MatchedElement>();
			foreach(var child in matchedElements) {
				//bool preventOfInfinityLoop = RawCompareElement(parentElement, parentElementZOrder, child.Element, child.ZOrder);
				//if(preventOfInfinityLoop) {
				//	continue;
				//}
				var elements = ObtainChildsUnderPoint(child.Element);
				if(elements != null) {
					childElements.AddRange(elements);
				}
			}

			var filteredElements = childElements
				.Where(child => !matchedElements.Any(x => Automation.Compare(x.Element, child.Element)));
			matchedElements.AddRange(filteredElements);
			return matchedElements;
		}

		bool RawCompareElement(AutomationElement parentElement, AutomationElement element) {
			if(parentElement.Current.BoundingRectangle == element.Current.BoundingRectangle
				&& parentElement.Current.ControlType == element.Current.ControlType
				&& parentElement.Current.Name == element.Current.Name
				&& parentElement.Current.AutomationId == element.Current.AutomationId) {
				return true;
			}
			return false;
		}

		void ObtainChildElements(AutomationElement startElement) {
			try {
				var iterElement = startElement;
				do {
					if(treeElements.Any(x => RawCompareElement(x.Element, iterElement))) {
						continue;
					}

					//if(elements.Any(x => Automation.Compare(x.Element, iterElement))) {
					//	continue;
					//}

					if(!iterElement.Current.BoundingRectangle.IsEmpty) {
						treeElements.Add(new MatchedElement() {
							Element = iterElement,
							BoundingRectangle = iterElement.Current.BoundingRectangle
						});
					}

					AutomationElement childElement = GetFirstChild(iterElement);
					if(childElement != null) {
						ObtainChildElements(childElement);
					}
					Iterations++;
					Debug.WriteLine("Iterations: {0}", Iterations);
				} while((iterElement = TreeWalker.ControlViewWalker.GetNextSibling(iterElement)) != null);
			} catch(Exception) {
			}
		}

		AutomationElement GetElementFromPoint() {
			Debug.WriteLine("");
			Debug.WriteLine("------------------------");
			Debug.WriteLine("ElementCoord: {0}", ElementCoord);
			Debug.WriteLine("");
			var hwnd = WinAPI.WindowFromPoint(new WinAPI.POINT(ElementCoord.X, ElementCoord.Y));
			if(hwnd == IntPtr.Zero) {
				return null;
			}

			//	var elementUnderPoint1 = AutomationElement.FromHandle(hwnd);

			var rootWindowHwnd = GetRootWindow(hwnd);
			if(rootWindowHwnd == IntPtr.Zero) {
				return null;
			}

			var childs = GetAllChildHandles(rootWindowHwnd);

			var elementHandle = childs
				.OrderByDescending(x => GetZOrder(x.Handle))
				.ThenBy(x => x.BoundingRectangle, new BoundingRectangleComp())
				.Select(x => x.Handle)
				.FirstOrDefault();

			if(elementHandle == IntPtr.Zero) {
				elementHandle = rootWindowHwnd;
			}
			var elementUnderPoint = AutomationElement.FromHandle(elementHandle);
			Debug.WriteLine("{0:X8} elementUnderPoint: {1}; {2}; {3}", elementHandle.ToInt32(), NamingHelpers.Escape(elementUnderPoint.Current.AutomationId, 300),
									elementUnderPoint.Current.ControlType.ProgrammaticName, elementUnderPoint.Current.BoundingRectangle);
			return elementUnderPoint;

			//var element = GetFirstSiblingElement(elementUnderPoint);
			//if(element == null) {
			//	return null;
			//}

			//ObtainChildElements(element);

			//var elementsUnderPoint = treeElements
			//	.Where(x => x.BoundingRectangle.Contains(ElementCoord.X, ElementCoord.Y))
			//	.ToList();

			//element = elementsUnderPoint
			//	.OrderByDescending(x => GetZOrder(x.Element))
			//	.ThenBy(x => x.BoundingRectangle, new BoundingRectangleComp())
			//	.Select(x => x.Element)
			//	.FirstOrDefault();
			//if(element != null) {
			//	return element;
			//} else {
			//	return elementUnderPoint;
			//}
		}

		int GetZOrder(AutomationElement element) {
			if(element == null) {
				return -1;
			}
			var hWnd = new IntPtr(element.Current.NativeWindowHandle);
			if(hWnd == IntPtr.Zero) {
				var parentEl = TreeWalker.RawViewWalker.GetParent(element);
				return GetZOrder(parentEl);
			}
			return GetZOrder(hWnd);
		}

		int GetZOrder(IntPtr hWnd) {
			var lowestHwnd = WinAPI.GetWindow(hWnd, WinAPI.GW_HWNDLAST);

			var z = 0;
			var hwndTmp = lowestHwnd;
			while(hwndTmp != IntPtr.Zero) {
				if(hWnd == hwndTmp) {
					//Debug.WriteLine("{0:X8} z:{1}", hWnd.ToInt32(), z);
					return z;
				}

				hwndTmp = WinAPI.GetWindow(hwndTmp, WinAPI.GW_HWNDPREV);
				z++;
			}
			return -1;
		}


		IntPtr GetRootWindow(IntPtr selectedWindow) {
			var rootHwnd = WinAPI.GetAncestor(selectedWindow, WinAPI.GA_ROOT);
			return rootHwnd;
		}

		List<ElementsUderPoint> GetAllChildHandles(IntPtr rootWindow) {
			var childHandles = new List<ElementsUderPoint>();

			var gcChildhandlesList = GCHandle.Alloc(childHandles);
			var pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

			try {
				WinAPI.EnumChildWindows(rootWindow, new WinAPI.EnumWindowProc(EnumWindow), pointerChildHandlesList);
			} finally {
				gcChildhandlesList.Free();
			}
			return childHandles;
		}

		private bool EnumWindow(IntPtr hWnd, IntPtr lParam) {
			GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

			if(gcChildhandlesList == null || gcChildhandlesList.Target == null) {
				return false;
			}

			var info = new WinAPI.WINDOWINFO();
			info.cbSize = (uint)Marshal.SizeOf(info);
			WinAPI.GetWindowInfo(hWnd, ref info);

			var className = GetWindowClass(hWnd);
			var windowName = GetWindowText(hWnd);
			var zOrder = GetZOrder(hWnd);
			Debug.WriteLine("{0:X8} '{1}' '{2}' {3} z:{4}", hWnd.ToInt32(), windowName, className, info.rcWindow, zOrder);

			if(info.rcWindow.Contains(ElementCoord.X, ElementCoord.Y)) {
				var childHandles = gcChildhandlesList.Target as List<ElementsUderPoint>;
				childHandles.Add(new ElementsUderPoint() {
					Handle = hWnd,
					BoundingRectangle = new System.Windows.Rect(info.rcWindow.left, info.rcWindow.top,
							info.rcWindow.right - info.rcWindow.left, info.rcWindow.bottom - info.rcWindow.top),
					ClassName = className,
					WindowName = windowName,
					ZOrder = GetZOrder(hWnd)
				});
			}
			return true;
		}

		string GetWindowText(IntPtr hWnd) {
			int len = WinAPI.GetWindowTextLength(hWnd) + 1;
			var sb = new StringBuilder(len);
			len = WinAPI.GetWindowText(hWnd, sb, len);
			return sb.ToString(0, len);
		}

		string GetWindowClass(IntPtr hWnd) {
			int len = 260;
			var sb = new StringBuilder(len);
			len = WinAPI.GetClassName(hWnd, sb, len);
			return sb.ToString(0, len);
		}
	}
}
