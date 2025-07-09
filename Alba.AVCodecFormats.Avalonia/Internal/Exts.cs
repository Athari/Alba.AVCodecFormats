using Avalonia;
using Avalonia.Platform;
using FFMediaToolkit.Graphics;
using DrawingSize = System.Drawing.Size;

namespace Alba.AVCodecFormats.Avalonia.Internal;

internal static class Exts
{
    public static ImagePixelFormat ToImagePixelFormat(this PixelFormat @this)
    {
        if (@this == PixelFormats.Rgba8888)
            return ImagePixelFormat.Rgba32;
        else if (@this == PixelFormats.Rgb24)
            return ImagePixelFormat.Rgb24;
        else if (@this == PixelFormats.Rgba64)
            return ImagePixelFormat.Rgba64;
        else if (@this == PixelFormats.Bgra8888)
            return ImagePixelFormat.Bgra32;
        else if (@this == PixelFormats.Bgr24)
            return ImagePixelFormat.Bgr24;
        else if (@this == PixelFormats.Gray8)
            return ImagePixelFormat.Gray8;
        else if (@this == PixelFormats.Gray16)
            return ImagePixelFormat.Gray16;
        else
            throw new ArgumentException($"Unsupported pixel format: {@this}.");
    }

    public static PixelSize ToPixelSize(this DrawingSize @this) =>
        new(@this.Width, @this.Height);
}