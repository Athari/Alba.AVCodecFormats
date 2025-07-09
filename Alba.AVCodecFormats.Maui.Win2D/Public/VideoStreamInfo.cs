using Windows.Graphics;
using Alba.AVCodecFormats.Maui.Graphics.Win2D.Internal;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Maui.Graphics.Win2D;

/// <summary>Represents information about the video stream.</summary>
[PublicAPI]
public sealed class VideoStreamInfo(FFMediaToolkit.Decoding.VideoStreamInfo source) : VideoStreamInfoBase(source)
{
    /// <summary>Gets the video frame dimensions.</summary>
    public SizeInt32 FrameSize => FrameSizeBase.ToPixelSize();

    internal static VideoStreamInfo Create(VideoStream source) => new(source.Info);
}