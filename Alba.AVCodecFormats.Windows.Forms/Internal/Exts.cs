using System.Drawing.Imaging;
using FFMediaToolkit.Graphics;
using static System.Drawing.Imaging.PixelFormat;

namespace Alba.AVCodecFormats.Drawing.Internal;

internal static class Exts
{
    public static ImagePixelFormat ToImagePixelFormat(this PixelFormat @this) =>
        @this switch {
            Format32bppArgb or Format32bppPArgb or Format32bppRgb => ImagePixelFormat.Argb32,
            Format24bppRgb => ImagePixelFormat.Rgb24,
            Format16bppGrayScale => ImagePixelFormat.Gray16,
            _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, $"Unsupported pixel format: {@this}."),
        };
}