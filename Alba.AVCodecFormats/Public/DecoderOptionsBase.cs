using System.Drawing;

namespace Alba.AVCodecFormats;

/// <summary>AV decoder options for generating an image out of a video/audio stream.</summary>
public abstract class DecoderOptionsBase
{
    /// <summary>Gets or sets manually chosen decoder. Default is automatic selection.</summary>
    public string? CodecName { get; set; }

    public Size? TargetSize { get; set; }

    public int MaxFrames { get; set; } = int.MaxValue;

    /// <summary>Preserve aspect ratio when <see cref="TargetSize"/> is set.</summary>
    public bool PreserveAspectRatio { get; set; }

    /// <summary>Gets or sets a value indicating whether frames should be scaled based on SAR value (if set).</summary>
    public bool RespectSampleAspectRatio { get; set; }

    /// <summary>A predicate that provides the way to skip frames based on their index.
    /// Returns true when frame should be decoded, otherwise false.
    /// <see cref="DecoderOptionsBase{TBitmap}.FrameFilter"/> is called on the decoded frame.</summary>
    public Func<int, bool>? FrameIndexFilter { get; set; }

    /// <summary>Gets or sets the dictionary with options for the multimedia decoders.</summary>
    public Dictionary<string, string> Options { get; set; } = [ ];

    protected internal Func<object, int, bool>? FrameFilterBase { get; set; }
}