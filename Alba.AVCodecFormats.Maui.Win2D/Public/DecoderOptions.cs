namespace Alba.AVCodecFormats.Maui.Graphics.Win2D;

/// <summary>AV decoder options for generating an image out of a video/audio stream.</summary>
public sealed class DecoderOptions : DecoderOptionsBase<VideoFrameImage>
{
    internal static DecoderOptions Default { get; } = new();
}