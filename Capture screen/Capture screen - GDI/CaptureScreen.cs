using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ScreenshotCaptureWithMouse.ScreenCapture
{
    class CaptureScreen
    {
        //This structure shall be used to keep the size of the screen.
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            Process[] process = Process.GetProcessesByName("rf_online.bin");
            SIZE size;
            size.cx = 1280;
            size.cy = 800;
            IntPtr mainWindowHandle = hWnd;
            
            IntPtr hDC = Win32Stuff.GetDC(mainWindowHandle);
            IntPtr hMemDC = GDIStuff.CreateCompatibleDC(hDC);

            IntPtr hBitmap = GDIStuff.CreateCompatibleBitmap(hDC, size.cx, size.cy);

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)GDIStuff.SelectObject
                                       (hMemDC, hBitmap);

                GDIStuff.BitBlt(hMemDC, 0, 0, size.cx , size.cy, hDC,
                                               0, 0, GDIStuff.SRCCOPY);

                GDIStuff.SelectObject(hMemDC, hOld);
                GDIStuff.DeleteDC(hMemDC);
                Win32Stuff.ReleaseDC(mainWindowHandle, hDC);
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                GDIStuff.DeleteObject(hBitmap);
                //GC.Collect(); //garbage collector
                return bmp;
            }
            return null;   
        }

    }
}
