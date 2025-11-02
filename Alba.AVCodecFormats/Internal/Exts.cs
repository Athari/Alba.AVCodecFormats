using System.Collections.ObjectModel;
using FFMediaToolkit.Graphics;
using FFmpeg.AutoGen;

namespace Alba.AVCodecFormats.Internal;

internal static class Exts
{
    public static int ToByteSize(this ImagePixelFormat @this) =>
        @this switch {
            ImagePixelFormat.Rgba64 or (ImagePixelFormat)AVPixelFormat.AV_PIX_FMT_BGRA64LE => 8,
            (ImagePixelFormat)AVPixelFormat.AV_PIX_FMT_RGB48LE or (ImagePixelFormat)AVPixelFormat.AV_PIX_FMT_BGR48LE => 6,
            ImagePixelFormat.Bgra32 or ImagePixelFormat.Rgba32 or ImagePixelFormat.Argb32 => 4,
            ImagePixelFormat.Bgr24 or ImagePixelFormat.Rgb24 => 3,
            ImagePixelFormat.Gray16 => 2,
            ImagePixelFormat.Gray8 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, $"Unsupported pixel format: {@this}."),
        };

    public static ReadOnlyDictionary<string, TValue> ToReadOnlyDictionaryIC<TValue>(
        this IEnumerable<KeyValuePair<string, TValue>> @this)
    {
        // ToDictionary uses Add; we use indexer to be safe.
        var dic = new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in @this)
            dic[key] = value;
        return new(dic);
    }
}