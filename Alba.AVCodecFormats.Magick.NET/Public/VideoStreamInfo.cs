using System.Drawing;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.ImageMagick;

/// <summary>Represents information about the video stream.</summary>
[PublicAPI]
public sealed class VideoStreamInfo(FFMediaToolkit.Decoding.VideoStreamInfo source) : VideoStreamInfoBase(source)
{
    /// <summary>Gets the video frame dimensions.</summary>
    public Size FrameSize => FrameSizeBase;

    internal static VideoStreamInfo Create(VideoStream source) => new(source.Info);
}