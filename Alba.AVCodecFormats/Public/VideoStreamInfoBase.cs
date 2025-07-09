using System.Drawing;
using Alba.AVCodecFormats.Internal;
using FFMediaToolkit.Decoding;
using FFmpeg.AutoGen;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats;

/// <summary>Represents information about the video stream.</summary>
[PublicAPI]
public abstract class VideoStreamInfoBase : StreamInfo
{
    private readonly VideoStreamInfo _source;

    protected VideoStreamInfoBase(VideoStreamInfo source) : base(source)
    {
        _source = source;
        var pixFmtDesc = new PixelFormatDesc();
        BitsPerPixel = pixFmtDesc.BitPerPixel;
        BitsPerPixelPadded = pixFmtDesc.BitPerPixelPadded;
    }

    /// <summary>Gets the clockwise rotation angle computed from the display matrix.</summary>
    public double Rotation => _source.Rotation;

    /// <summary>Gets a value indicating whether the frames in the stream are interlaced.</summary>
    public bool IsInterlaced => _source.IsInterlaced;

    /// <summary>Gets the video frame dimensions.</summary>
    protected internal Size FrameSizeBase => _source.FrameSize;

    /// <summary>Gets a lowercase string representing the video pixel format.</summary>
    public string PixelFormat => _source.PixelFormat;

    public int BitsPerPixel { get; }

    public int BitsPerPixelPadded { get; }

    /// <summary>Gets sample aspect ratio. 0 if unknown.</summary>
    public AVRational SampleAspectRatio => _source.SampleAspectRatio;
}