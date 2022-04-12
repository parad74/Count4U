using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Count4U
{
	public class WindowStyler
	{
		private const Int32 GWL_STYLE = -16;
		private const Int32 GWL_EXSTYLE = -20;
		private const UInt32 WS_SYSMENU = 0x80000;
		private const UInt32 WS_MAXIMIZEBOX = 0x00010000;
		private const UInt32 WS_MINIMIZEBOX = 0x00020000;

		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
		private static extern UInt32 GetWindowLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		private static extern UInt32 SetWindowLongPtr32(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
		private static extern UInt32 GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
		private static extern UInt32 SetWindowLongPtr64(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

		public static UInt32 GetWindowLong(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size == 8)
				return GetWindowLongPtr64(hWnd, nIndex);
			else
				return GetWindowLongPtr32(hWnd, nIndex);
		}

		public static UInt32 SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong)
		{
			if (IntPtr.Size == 8)
				return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
			else
				return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
		}

		public static void SetWindowStyle(Window wnd, uint style)
		{
			lock (wnd)
			{
				IntPtr hWnd = new WindowInteropHelper(wnd).Handle;
				UInt32 windowStyle = GetWindowLong(hWnd, GWL_STYLE);
				windowStyle = windowStyle | style;
				SetWindowLong(hWnd, GWL_STYLE, windowStyle);
			}
		}

		public static void ResetWindowStyle(Window wnd, uint style)
		{
			lock (wnd)
			{
				IntPtr hWnd = new WindowInteropHelper(wnd).Handle;
				UInt32 windowStyle = GetWindowLong(hWnd, GWL_STYLE);
				windowStyle = windowStyle & ~style;
				SetWindowLong(hWnd, GWL_STYLE, windowStyle);
			}
		}

		public static void EnableMaximize(Window wnd)
		{
			SetWindowStyle(wnd, WS_MAXIMIZEBOX);
		}

		public static void DisableMaximize(Window wnd)
		{
			ResetWindowStyle(wnd, WS_MAXIMIZEBOX);
		}

		public static void EnableMinimize(Window wnd)
		{
			SetWindowStyle(wnd, WS_MINIMIZEBOX);
		}

		public static void DisableMinimize(Window wnd)
		{
			ResetWindowStyle(wnd, WS_MINIMIZEBOX);
		}
	}
}
