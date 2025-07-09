using System.Windows.Media.Imaging;

namespace Alba.AVCodecFormats.Windows.Media;

/// <summary>AV decoder options for generating an image out of a video/audio stream.</summary>
public sealed class DecoderOptions : DecoderOptionsBase<WriteableBitmap>
{
    internal static DecoderOptions Default { get; } = new();
}