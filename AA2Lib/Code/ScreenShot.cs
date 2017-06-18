// --------------------------------------------------
// AA2Lib - ScreenShot.cs
// --------------------------------------------------

using System;
using System.Drawing;

namespace AA2Lib.Code
{
    internal static class ScreenShot
    {
        public static Bitmap Capture(Rectangle area)
        {
            IntPtr desktopHandle = NativeMethods.GetDesktopWindow();
            IntPtr desktopDC = NativeMethods.GetWindowDC(desktopHandle);
            IntPtr destinationDC = NativeMethods.CreateCompatibleDC(desktopDC);
            IntPtr bitmapHandle = NativeMethods.CreateCompatibleBitmap(desktopDC, area.Width, area.Height);
            IntPtr oldBitmapHandle = NativeMethods.SelectObject(destinationDC, bitmapHandle);

            const CopyPixelOperation operation = CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt;
            NativeMethods.BitBlt(destinationDC, 0, 0, area.Width, area.Height, desktopDC, area.X, area.Y, operation);

            Bitmap desktopCapture = Image.FromHbitmap(bitmapHandle);
            NativeMethods.SelectObject(destinationDC, oldBitmapHandle);
            NativeMethods.DeleteObject(bitmapHandle);
            NativeMethods.DeleteDC(destinationDC);
            NativeMethods.ReleaseDC(desktopHandle, desktopDC);

            return desktopCapture;
        }
    }
}