using ImageMagick;

namespace Alba.AVCodecFormats.Magick.NET;

/// <summary>AV decoder options for generating an image out of a video/audio stream.</summary>
public sealed class DecoderOptions<TQ> : DecoderOptionsBase<IMagickImage<TQ>>
    where TQ : struct, IConvertible
{
    internal static DecoderOptions<TQ> Default { get; } = new();
}