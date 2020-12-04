using System;
using System.Drawing;
using System.Drawing.Imaging;
using UsAcRe.Core.WindowsSystem;

namespace UsAcRe.Player {
	public class Screenshot {
		public const string PngFileExtension = ".png";
		public const string JpegFileExtension = ".jpg";

		public static void MakePng(string saveFileName) {
			saveFileName += PngFileExtension;
			Screenshot.MakeForCurrentCursorPosition(saveFileName, ImageFormat.Png);
		}

		public static void MakeJpeg(string saveFileName) {
			saveFileName += JpegFileExtension;
			Screenshot.MakeForCurrentCursorPosition(saveFileName, ImageFormat.Jpeg);
		}

		public static void MakeForCurrentCursorPosition(string saveFileName, ImageFormat format) {
			WinAPI.POINT pt;
			WinAPI.GetCursorPos(out pt);
			var screenshot = new Screenshot();
			screenshot.Make(saveFileName, format, new System.Drawing.Point(pt.x, pt.y));
		}

		public void Make(string saveFileName, ImageFormat format, Point screenPosition) {
			Bitmap screenshot = CreateBitmapFromDesktop();
			screenshot.Save(saveFileName, format);
		}

		static Bitmap CreateBitmapFromDesktop() {
			WinAPI.SIZE sz;
			sz.cx = WinAPI.GetSystemMetrics(WinAPI.SM_CXSCREEN);
			sz.cy = WinAPI.GetSystemMetrics(WinAPI.SM_CYSCREEN);

			IntPtr hDesk = WinAPI.GetDesktopWindow();
			IntPtr hSrce = WinAPI.GetWindowDC(hDesk);
			IntPtr hDest = WinAPI.CreateCompatibleDC(hSrce);
			IntPtr hBmp = WinAPI.CreateCompatibleBitmap(hSrce, sz.cx, sz.cy);
			IntPtr hOldBmp = WinAPI.SelectObject(hDest, hBmp);
			bool b = WinAPI.BitBlt(hDest, 0, 0, sz.cx, sz.cy, hSrce, 0, 0, (int)(CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt));
			Bitmap bmp = Bitmap.FromHbitmap(hBmp);
			WinAPI.SelectObject(hDest, hOldBmp);
			WinAPI.DeleteObject(hBmp);
			WinAPI.DeleteDC(hDest);
			WinAPI.ReleaseDC(hDesk, hSrce);
			return bmp;
		}
	}


}
