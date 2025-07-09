using System.Collections.ObjectModel;
using Alba.AVCodecFormats.Internal;
using FFmpeg.AutoGen;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats;

/// <summary>Represents generic information about the stream, specialized by subclasses for specific kinds of streams.</summary>
[PublicAPI]
public abstract class StreamInfo(FFMediaToolkit.Decoding.StreamInfo source)
{
    /// <summary>Gets the codec name.</summary>
    public string CodecName => source.CodecName;

    /// <summary>Gets the codec identifier.</summary>
    public string CodecId => source.CodecId;

    /// <summary>Gets the stream time base.</summary>
    public AVRational TimeBase => source.TimeBase;

    /// <summary>Gets the number of frames value taken from the container metadata or estimated in constant frame rate videos.
    /// Returns <see langword="null"/> if not available.</summary>
    public int? FrameCount => source.NumberOfFrames;

    /// <summary>Gets the stream duration.</summary>
    public TimeSpan Duration => source.Duration;

    /// <summary>Gets the stream start time. Null if undefined.</summary>
    public TimeSpan? StartTime => source.StartTime;

    /// <summary>Gets the average frame rate as a <see cref="double"/> value.</summary>
    public double AvgFrameRate => source.AvgFrameRate;

    /// <summary>Gets the frame rate as a <see cref="AVRational"/> value.
    /// It is used to calculate timestamps in the internal decoder methods.</summary>
    public AVRational RealFrameRate => source.RealFrameRate;

    /// <summary>Gets a value indicating whether the video is variable frame rate (VFR).</summary>
    public bool IsVariableFrameRate => source.IsVariableFrameRate;

    /// <summary>Gets the stream metadata.</summary>
    public ReadOnlyDictionary<string, string> Metadata { get; } = source.Metadata.ToReadOnlyDictionaryIC();
}