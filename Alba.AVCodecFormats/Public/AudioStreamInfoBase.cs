using FFMediaToolkit.Audio;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats;

/// <summary>Represents information about the audio stream.</summary>
[PublicAPI]
public abstract class AudioStreamInfoBase(AudioStreamInfo source) : StreamInfo(source)
{
    /// <summary>Gets the number of audio channels stored in the stream.</summary>
    public int NumChannels => source.NumChannels;

    /// <summary>Gets the number of samples per second of the audio stream.</summary>
    public int SampleRate => source.SampleRate;

    /// <summary>Gets the average number of samples per frame (chunk of samples) calculated from metadata.
    /// It is used to calculate timestamps in the internal decoder methods.</summary>
    public int SamplesPerFrame => source.SamplesPerFrame;

    /// <summary>Gets the audio sample format.</summary>
    public SampleFormat SampleFormat => source.SampleFormat;
}