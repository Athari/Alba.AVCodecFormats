using System.Windows;
using Alba.AVCodecFormats.Windows.Internal;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Windows.Media;

/// <summary>Represents information about the video stream.</summary>
[PublicAPI]
public sealed class VideoStreamInfo(FFMediaToolkit.Decoding.VideoStreamInfo source) : VideoStreamInfoBase(source)
{
    /// <summary>Gets the video frame dimensions.</summary>
    public Size FrameSize => FrameSizeBase.ToPixelSize();

    public int PixelWidth => FrameSizeBase.Width;

    public int PixelHeight => FrameSizeBase.Height;

    internal static VideoStreamInfo Create(VideoStream source) => new(source.Info);
}