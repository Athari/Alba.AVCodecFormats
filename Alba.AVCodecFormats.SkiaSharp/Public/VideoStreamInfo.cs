using Alba.AVCodecFormats.SkiaSharp.Internal;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;
using SkiaSharp;

namespace Alba.AVCodecFormats.SkiaSharp;

/// <summary>Represents information about the video stream.</summary>
[PublicAPI]
public sealed class VideoStreamInfo(FFMediaToolkit.Decoding.VideoStreamInfo source) : VideoStreamInfoBase(source)
{
    /// <summary>Gets the video frame dimensions.</summary>
    public SKSizeI FrameSize => FrameSizeBase.ToPixelSize();

    internal static VideoStreamInfo Create(VideoStream source) => new(source.Info);
}