using System;
using System.Collections.Generic;
using System.Drawing;
using SharpDX.Direct3D9;

namespace bot
{
    public static class Direct3DCapture
    {
        private static Direct3D _direct3D9 = new Direct3D();
        private static Dictionary<IntPtr, Device> _direct3DDeviceCache = new Dictionary<IntPtr, Device>();

        /// <summary>
        /// Capture the entire client area of a window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            return CaptureRegionDirect3D(hWnd, NativeMethodsForWindow.GetAbsoluteClientRect(hWnd));
        }

        /// <summary>
        /// Capture a region of the screen using Direct3D
        /// </summary>
        /// <param name="handle">The handle of a window</param>
        /// <param name="region">The region to capture (in screen coordinates)</param>
        /// <returns>A bitmap containing the captured region, this should be disposed of appropriately when finished with it</returns>
        public static Bitmap CaptureRegionDirect3D(IntPtr handle, Rectangle region)
        {
            IntPtr hWnd = handle;
            Bitmap bitmap = null;

            // We are only supporting the primary display adapter for Direct3D mode
            AdapterInformation adapterInfo = _direct3D9.Adapters[0];
            Device device;

            #region Get Direct3D Device
            // Retrieve the existing Direct3D device if we already created one for the given handle
            if (_direct3DDeviceCache.ContainsKey(hWnd))
            {
                device = _direct3DDeviceCache[hWnd];
            }
            // We need to create a new device
            else
            {
                // Setup the device creation parameters
                PresentParameters parameters = new PresentParameters();
                parameters.BackBufferFormat = adapterInfo.CurrentDisplayMode.Format;
                Rectangle clientRect = NativeMethodsForWindow.GetAbsoluteClientRect(hWnd);
                parameters.BackBufferHeight = clientRect.Height;
                parameters.BackBufferWidth = clientRect.Width;
                parameters.MultiSampleType = MultisampleType.None;
                parameters.SwapEffect = SwapEffect.Discard;
                parameters.DeviceWindowHandle = hWnd;
                parameters.PresentationInterval = PresentInterval.Default;
                parameters.FullScreenRefreshRateInHz = 0;

                // Create the Direct3D device
                device = new Device(_direct3D9, adapterInfo.Adapter, DeviceType.Hardware, hWnd, CreateFlags.SoftwareVertexProcessing, parameters);
                _direct3DDeviceCache.Add(hWnd, device);
            }
            #endregion

            // Capture the screen and copy the region into a Bitmap
            using (Surface surface = Surface.CreateOffscreenPlain(device, adapterInfo.CurrentDisplayMode.Width, adapterInfo.CurrentDisplayMode.Height, Format.A8R8G8B8, Pool.SystemMemory))
            {
                device.GetFrontBufferData(0, surface);

                // where they previously expected a RECT type structure for their Rectangle
                bitmap = new Bitmap(Surface.ToStream(surface, ImageFileFormat.Bmp, new SharpDX.Mathematics.Interop.RawRectangle(region.Left, region.Top, region.Right, region.Bottom)));
                // Previous SlimDX bug workaround: new Rectangle(region.Left, region.Top, region.Right, region.Bottom)));
            }

            return bitmap;
        }
    }
}
