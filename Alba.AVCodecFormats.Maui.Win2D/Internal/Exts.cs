using System.Drawing;
using Windows.Graphics;
using Windows.Graphics.DirectX;
using FFMediaToolkit.Graphics;
using static Windows.Graphics.DirectX.DirectXPixelFormat;

namespace Alba.AVCodecFormats.Maui.Graphics.Win2D.Internal;

internal static class Exts
{
    public static ImagePixelFormat ToImagePixelFormat(this DirectXPixelFormat @this) =>
        @this switch {
            B8G8R8A8UIntNormalized or B8G8R8A8UIntNormalizedSrgb or B8G8R8A8Typeless or
                B8G8R8X8UIntNormalized or B8G8R8X8UIntNormalizedSrgb or B8G8R8X8Typeless => ImagePixelFormat.Bgra32,
            R8G8B8A8Int or R8G8B8A8IntNormalized or R8G8B8A8Typeless or
                R8G8B8A8UInt or R8G8B8A8UIntNormalized or R8G8B8A8UIntNormalizedSrgb => ImagePixelFormat.Rgba32,
            R16G16B16A16Int or R16G16B16A16IntNormalized or R16G16B16A16Typeless or
                R16G16B16A16UInt or R16G16B16A16UIntNormalized => ImagePixelFormat.Rgba64,
            R16Int or R16IntNormalized or R16Typeless or R16UInt or R16UIntNormalized => ImagePixelFormat.Gray16,
            R8Int or R8IntNormalized or R8Typeless or R8UInt or R8UIntNormalized => ImagePixelFormat.Gray8,
            _ => throw new ArgumentException($"Unsupported pixel format: {@this}."),
        };

    public static SizeInt32 ToPixelSize(this Size @this) =>
        new(@this.Width, @this.Height);
}