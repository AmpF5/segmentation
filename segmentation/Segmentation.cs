using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using segmentation.Helpers;

namespace segmentation;

public static class Segmentation {
    public static async Task<WriteableBitmap> FloodFill(IStorageFile originalImage, Point pt, Color targetColor, Color replacementColor)
    {
        var writeableBitmap = WriteableBitmap.Decode(await originalImage.OpenReadAsync());
        var width = writeableBitmap.PixelSize.Width;
        var height = writeableBitmap.PixelSize.Height;
        var stride = (writeableBitmap.PixelSize.Width * 32 + 7) / 8;
        using var backBuffer = writeableBitmap.Lock();
        var pBackBuffer = backBuffer.Address;
        
        var pixels = new Stack<Point>(width * height / 4);
        pixels.Push(pt);

        while (pixels.Count > 0)
        {
            var a = pixels.Pop();
            if (a.X < width && a.X > 0 && a.Y < height && a.Y > 0)
            {
                if (PixelHelper.GetPixelColor((int)a.X, (int)a.Y, stride, pBackBuffer) == targetColor)
                {
                    PixelHelper.SetPixelColor((int)a.X, (int)a.Y, stride, pBackBuffer, replacementColor);
                    pixels.Push(new Point(a.X - 1, a.Y));
                    pixels.Push(new Point(a.X + 1, a.Y));
                    pixels.Push(new Point(a.X, a.Y - 1));
                    pixels.Push(new Point(a.X, a.Y + 1));
                }
            }
        }
        return writeableBitmap;
    }
}