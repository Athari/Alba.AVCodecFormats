using System.Runtime.InteropServices;
using FFMediaToolkit.Graphics;
using ImageMagick;

namespace Alba.AVCodecFormats.Magick.NET.Internal;

internal static class Exts
{
    public static ImagePixelFormat ToImagePixelFormat(this StorageType @this, string mapping) =>
        (@this, mapping) switch {
            (StorageType.Short, "RGBA") => ImagePixelFormat.Rgba64,
            (StorageType.Short, "R") => ImagePixelFormat.Gray16,
            (StorageType.Char, "RGBA") => ImagePixelFormat.Rgba32,
            (StorageType.Char, "ARGB") => ImagePixelFormat.Argb32,
            (StorageType.Char, "BGRA") => ImagePixelFormat.Bgra32,
            (StorageType.Char, "RGB") => ImagePixelFormat.Rgb24,
            (StorageType.Char, "BGR") => ImagePixelFormat.Bgr24,
            (StorageType.Char, "R") => ImagePixelFormat.Gray8,
            _ => throw new ArgumentException($"Unsupported storage type and pixel mapping: {@this} {mapping}."),
        };

    public static int ToByteSize<T>(this StorageType @this)
        where T : struct, IConvertible =>
        @this switch {
            StorageType.Char => 1,
            StorageType.Short => 2,
            StorageType.Float => 4,
            StorageType.Int32 => 4,
            StorageType.Int64 => 8,
            StorageType.Double => 8,
            StorageType.Quantum => Marshal.SizeOf<T>(),
            _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null),
        };
}