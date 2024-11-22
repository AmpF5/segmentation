using System;
using System.Runtime.InteropServices;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace segmentation.Helpers;

public static class PixelHelper {
    public static Color GetPixelColor(int x, int y, int stride, IntPtr pBackBuffer) {
        var pixelOffset = (y * stride) + (x * 4);
        unsafe
        {
            var pixelPointer = (byte*)pBackBuffer + pixelOffset;

            // Read pixel components (BGRA format)
            var b = pixelPointer[0];
            var g = pixelPointer[1];
            var r = pixelPointer[2];
            var a = pixelPointer[3];

            return Color.FromArgb(a, r, g, b);
        }
    }

    public static Color GetPixelColor(WriteableBitmap bitmap, int x, int y) {
        using var pixelBuffer = bitmap.Lock();
        var pixels = new byte[pixelBuffer.RowBytes * bitmap.PixelSize.Height];
        var pixelOffset = (y * pixelBuffer.RowBytes) + (x * 4);

        Marshal.Copy(pixelBuffer.Address, pixels, 0, pixels.Length);
        var b = pixels[pixelOffset];
        var g = pixels[pixelOffset + 1];
        var r = pixels[pixelOffset + 2];
        var a = pixels[pixelOffset + 3];
        
        return Color.FromArgb(r, g, b, a);
    }

    public static void SetPixelColor(int x, int y, int stride, IntPtr pBackBuffer, Color color) {
        var pPixel = pBackBuffer + y * stride + x * 4;
        var colorData = (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
        Marshal.WriteInt32(pPixel, colorData);
    }
}