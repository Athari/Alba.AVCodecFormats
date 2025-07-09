using System.Drawing;
using FFMediaToolkit.Graphics;
using SkiaSharp;

namespace Alba.AVCodecFormats.SkiaSharp.Internal;

internal static class Exts
{
    public static ImagePixelFormat ToImagePixelFormat(this SKColorType @this) =>
        @this switch {
            SKColorType.Rgba8888 or SKColorType.Srgba8888 or SKColorType.Rgb888x => ImagePixelFormat.Rgba32,
            SKColorType.Bgra8888 => ImagePixelFormat.Bgra32,
            SKColorType.Gray8 or SKColorType.Alpha8 => ImagePixelFormat.Gray8,
            SKColorType.Alpha16 => ImagePixelFormat.Gray16,
            SKColorType.Rgba16161616 => ImagePixelFormat.Rgba64,
            _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, $"Unsupported pixel format: {@this}."),
        };

    public static SKSizeI ToPixelSize(this Size @this) =>
        new(@this.Width, @this.Height);
}