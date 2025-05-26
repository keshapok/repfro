using System;
using System.Diagnostics;
using System.Threading;

namespace bot
{
    class WorkWithProcess
    {
        public static void BringProcessWindowToFront(Process proc)
        {
            if (proc == null)
                return;
            IntPtr handle = proc.MainWindowHandle;
            int i = 0;

            while (!NativeMethodsForWindow.IsWindowInForeground(handle))
            {
                if (i == 0)
                {
                    // Initial sleep if target window is not in foreground - just to let things settle
                    Thread.Sleep(1);
                }

                if (NativeMethodsForWindow.IsIconic(handle))
                {
                    // Minimized so send restore
                    NativeMethodsForWindow.ShowWindow(handle, NativeMethodsForWindow.WindowShowStyle.Restore);
                }
                else
                {
                    // Already Maximized or Restored so just bring to front
                    NativeMethodsForWindow.SetForegroundWindow(handle);
                }
                Thread.Sleep(1);

                // Check if the target process main window is now in the foreground
                if (NativeMethodsForWindow.IsWindowInForeground(handle))
                {
                    // Leave enough time for screen to redraw
                    Thread.Sleep(5);
                    return;
                }

                // Prevent an infinite loop
                if (i > 620)
                {
                    throw new Exception("Could not set process window to the foreground");
                }
                i++;
            }
        }
    }
}
