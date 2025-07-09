using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.SkiaSharp;

/// <summary>Represents information about the audio stream.</summary>
[PublicAPI]
public sealed class AudioStreamInfo(FFMediaToolkit.Decoding.AudioStreamInfo source) : AudioStreamInfoBase(source)
{
    internal static AudioStreamInfo Create(AudioStream source) => new(source.Info);
}