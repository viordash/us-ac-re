using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Recorder.KeyboardProcess {
	public delegate void RawKeyEventHandler(object sender, RawKeyEventArgs args);
	public class RawKeyEventArgs : EventArgs {
		public VirtualKeyCodes VKCode;
		public bool IsUp;
		public int MessageTimeStamp;

		public RawKeyEventArgs(VirtualKeyCodes vKCode, bool isUp, int messageTimeStamp) {
			this.VKCode = vKCode;
			this.IsUp = isUp;
			this.MessageTimeStamp = messageTimeStamp;
		}
	}

	public static class KeyboardHook {
		public static VirtualKeyCodes KeyStartStop = VirtualKeyCodes.VK_PAUSE;
		public static VirtualKeyCodes KeyTestControl = VirtualKeyCodes.VK_SNAPSHOT;
		public static event RawKeyEventHandler KeyAction = delegate { };
		private static WinAPI.LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;

		public static void Start() {
			_hookID = SetHook(_proc);
		}

		public static void Stop() {
			WinAPI.UnhookWindowsHookEx(_hookID);
		}

		private static IntPtr SetHook(WinAPI.LowLevelKeyboardProc proc) {
			using(Process curProcess = Process.GetCurrentProcess())
			using(ProcessModule curModule = curProcess.MainModule) {
				return WinAPI.SetWindowsHookEx(WinAPI.WH_KEYBOARD_LL, proc, WinAPI.GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		static uint prevVKCode = 0;
		static bool prevKeyUP = false;
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
			if(nCode >= 0) {
				WinAPI.KBDLLHOOKSTRUCT hookStruct = (WinAPI.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.KBDLLHOOKSTRUCT));
				//if(hookStruct.vkCode == KeyStartStop || hookStruct.vkCode == KeyTestControl) { //skip keys
				//    KeyAction(null, new RawKeyEventArgs(hookStruct.vkCode, (int)wParam == WindowsMessages.WM_SYSKEYUP 
				//        || (int)wParam == WindowsMessages.WM_KEYUP, hookStruct.time));
				//    return new IntPtr(1);
				//}
				switch((int)wParam) {
					case WindowsMessages.WM_SYSKEYDOWN:
					case WindowsMessages.WM_KEYDOWN:
						if((VirtualKeyCodes)hookStruct.vkCode == KeyStartStop || (VirtualKeyCodes)hookStruct.vkCode == KeyTestControl) { //skip keys
							KeyAction(null, new RawKeyEventArgs((VirtualKeyCodes)hookStruct.vkCode, false, (int)hookStruct.time));
							return new IntPtr(1);
						}
						if(prevVKCode != hookStruct.vkCode || prevKeyUP != false) {
							KeyAction(null, new RawKeyEventArgs((VirtualKeyCodes)hookStruct.vkCode, false, (int)hookStruct.time));
						}
						prevVKCode = hookStruct.vkCode;
						prevKeyUP = false;
						break;
					case WindowsMessages.WM_SYSKEYUP:
					case WindowsMessages.WM_KEYUP:
						if((VirtualKeyCodes)hookStruct.vkCode == KeyStartStop || (VirtualKeyCodes)hookStruct.vkCode == KeyTestControl) { //skip keys
							KeyAction(null, new RawKeyEventArgs((VirtualKeyCodes)hookStruct.vkCode, true, (int)hookStruct.time));
							return new IntPtr(1);
						}
						if(prevVKCode != hookStruct.vkCode || prevKeyUP != true) {
							KeyAction(null, new RawKeyEventArgs((VirtualKeyCodes)hookStruct.vkCode, true, (int)hookStruct.time));
						}
						prevVKCode = hookStruct.vkCode;
						prevKeyUP = true;
						break;
				}
			}
			return WinAPI.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}
	}
}
