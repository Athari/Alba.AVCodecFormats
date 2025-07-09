namespace Alba.AVCodecFormats.Drawing.Imaging;

/// <summary>AV decoder options for generating an image out of a video/audio stream.</summary>
public sealed class DecoderOptions : DecoderOptionsBase<Bitmap>
{
    internal static DecoderOptions Default { get; } = new();
}