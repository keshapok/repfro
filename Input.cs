using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Drawing;

namespace bot
{
    class Input
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        #region Mouse Fields 
        public const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        public const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        public const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        public const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        public const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
        public const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        public const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */
        #endregion

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        #region Keyboard Fields
        public const int KEYBOARDEVENTF_KEYDOWN = 0x0000; //KeyDown
        public const int KEYBOARDEVENTF_KEYUP = 0x0002; //KeyUp
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
        
            return lpPoint;
        }

        public static void SmoothMouseMove(int absoluteX, int absoluteY, int speed)
        {
            int currentX = Convert.ToInt32(Input.GetCursorPosition().X);
            int currentY = Convert.ToInt32(Input.GetCursorPosition().Y);

            while ((currentX != absoluteX) | (currentY != absoluteY))
            {
                int xOffset = 0;
                int yOffset = 0;

                if (currentX != absoluteX)
                {

                    if (absoluteX - currentX > 0)
                    {
                        xOffset = 1;
                    }
                    else
                    {
                        xOffset = -1;
                    }
                }

                if (currentY != absoluteY)
                {

                    if (absoluteY - currentY > 0)
                    {
                        yOffset = 1;
                    }
                    else
                    {
                        yOffset = -1;
                    }
                }

                int aX = currentX + xOffset;
                int aY = currentY + yOffset;
                Input.SetCursorPos(aX, aY);

                System.Threading.Thread.Sleep(speed);

                currentX = Convert.ToInt32(Input.GetCursorPosition().X);
                currentY = Convert.ToInt32(Input.GetCursorPosition().Y);
            }
        }
    }
}
