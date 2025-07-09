using JetBrains.Annotations;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;

namespace Alba.AVCodecFormats.Maui.Graphics.Skia;

[PublicAPI]
public class VideoFrameImage : SkiaImage
{
    public int FrameIndex { get; internal set; }

    internal VideoFrameImage(SKBitmap bitmap) : base(bitmap) { }
}