using Alba.AVCodecFormats.Avalonia.Internal;
using Avalonia;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Avalonia.Media;

/// <summary>Represents information about the video stream.</summary>
[PublicAPI]
public sealed class VideoStreamInfo(FFMediaToolkit.Decoding.VideoStreamInfo source) : VideoStreamInfoBase(source)
{
    /// <summary>Gets the video frame dimensions.</summary>
    public PixelSize FrameSize => FrameSizeBase.ToPixelSize();

    internal static VideoStreamInfo Create(VideoStream source) => new(source.Info);
}