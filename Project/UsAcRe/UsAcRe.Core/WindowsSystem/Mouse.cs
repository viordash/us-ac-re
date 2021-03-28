using System;
using GuardNet;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.WindowsSystem {
	public class Mouse {
		#region inner classes
		public enum Button {
			Left = 0,
			Middle = 1,
			Right = 2,
			XButton1 = 3,
			XButton2 = 4,
		}
		#endregion

		readonly IWinApiService winApiService;

		public Mouse(IWinApiService winApiService) {
			Guard.NotNull(winApiService, nameof(winApiService));
			this.winApiService = winApiService;
		}

		public void Click(Button mouseButton) {
			Down(mouseButton);
			Up(mouseButton);
		}

		public void DoubleClick(Button mouseButton) {
			Click(mouseButton);
			Click(mouseButton);
		}

		public void Down(Button mouseButton) {
			var inputFlags = GetInputFlags(mouseButton, false, out uint additionalData);
			winApiService.SendMouseInput(0, 0, additionalData, inputFlags);
		}

		public void DragTo(Button mouseButton, System.Drawing.Point point) {
			Down(mouseButton);
			MoveTo(point);
			Up(mouseButton);
		}

		public void MoveTo(System.Drawing.Point point) {
			winApiService.SendMouseInput(point.X, point.Y, 0, WinAPI.SendMouseInputFlags.Move | WinAPI.SendMouseInputFlags.Absolute);
		}

		public void Up(Button mouseButton) {
			var inputFlags = GetInputFlags(mouseButton, true, out uint additionalData);
			winApiService.SendMouseInput(0, 0, additionalData, inputFlags);
		}

		static WinAPI.SendMouseInputFlags GetInputFlags(Button mouseButton, bool isUp, out uint additionalData) {
			additionalData = 0;
			if(mouseButton == Button.Left && isUp) {
				return WinAPI.SendMouseInputFlags.LeftUp;
			}
			if(mouseButton == Button.Left && !isUp) {
				return WinAPI.SendMouseInputFlags.LeftDown;
			}
			if(mouseButton == Button.Right && isUp) {
				return WinAPI.SendMouseInputFlags.RightUp;
			}
			if(mouseButton == Button.Right && !isUp) {
				return WinAPI.SendMouseInputFlags.RightDown;
			}
			if(mouseButton == Button.Middle && isUp) {
				return WinAPI.SendMouseInputFlags.MiddleUp;
			}
			if(mouseButton == Button.Middle && !isUp) {
				return WinAPI.SendMouseInputFlags.MiddleDown;
			}
			if(mouseButton == Button.XButton1 && isUp) {
				additionalData = WinAPI.XBUTTON1;
				return WinAPI.SendMouseInputFlags.XUp;
			}
			if(mouseButton == Button.XButton1 && !isUp) {
				additionalData = WinAPI.XBUTTON1;
				return WinAPI.SendMouseInputFlags.XDown;
			}
			if(mouseButton == Button.XButton2 && isUp) {
				additionalData = WinAPI.XBUTTON2;
				return WinAPI.SendMouseInputFlags.XUp;
			}
			if(mouseButton == Button.XButton2 && !isUp) {
				additionalData = WinAPI.XBUTTON2;
				return WinAPI.SendMouseInputFlags.XDown;
			}
			throw new InvalidOperationException();
		}
	}
}
