namespace Alba.AVCodecFormats.Avalonia.Media;

/// <summary>AV decoder options for generating an image out of a video/audio stream.</summary>
public sealed class DecoderOptions : DecoderOptionsBase<VideoFrameBitmap>
{
    internal static DecoderOptions Default { get; } = new();
}