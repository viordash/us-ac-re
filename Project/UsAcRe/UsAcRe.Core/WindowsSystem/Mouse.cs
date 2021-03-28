using System;
using System.Threading.Tasks;
using GuardNet;
using UsAcRe.Core.MouseProcess;
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

		public async ValueTask Click(Button mouseButton) {
			await Down(mouseButton);
			await Up(mouseButton);
		}

		public async ValueTask DoubleClick(Button mouseButton) {
			await Click(mouseButton);
			await Task.Delay(MouseProcessConstants.DoubleClickTime / 2);
			await Click(mouseButton);
		}

		public ValueTask Down(Button mouseButton) {
			var inputFlags = GetInputFlags(mouseButton, false, out uint additionalData);
			winApiService.SendMouseInput(0, 0, additionalData, inputFlags);
			return new ValueTask(Task.CompletedTask);
		}

		public ValueTask MoveTo(int x, int y) {
			winApiService.SendMouseInput(x, y, 0, WinAPI.SendMouseInputFlags.Move | WinAPI.SendMouseInputFlags.Absolute);
			return new ValueTask(Task.CompletedTask);
		}

		public ValueTask Up(Button mouseButton) {
			var inputFlags = GetInputFlags(mouseButton, true, out uint additionalData);
			winApiService.SendMouseInput(0, 0, additionalData, inputFlags);
			return new ValueTask(Task.CompletedTask);
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
