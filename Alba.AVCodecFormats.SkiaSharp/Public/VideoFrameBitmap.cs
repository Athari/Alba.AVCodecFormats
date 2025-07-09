using JetBrains.Annotations;
using SkiaSharp;

namespace Alba.AVCodecFormats.SkiaSharp;

[PublicAPI]
public class VideoFrameBitmap : SKBitmap
{
    public int FrameIndex { get; internal set; }

    internal VideoFrameBitmap(SKSizeI size, SKColorType colorType, SKAlphaType alphaType)
        : base(new(size.Width, size.Height, colorType, alphaType), SKBitmapAllocFlags.None) { }
}