using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Alba.AVCodecFormats.Windows.Internal;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Windows.Media;

[PublicAPI]
public sealed class VideoSequence
{
    public IList<WriteableBitmap> Frames { get; } = [ ];

    public MediaContainerInfo Metadata { get; internal set; } = null!;

    public static VideoSequence Decode(Stream stream,
        PixelFormat? pixelFormat = null, DecoderOptions? options = null, CancellationToken ct = default)
    {
        return new MediaDecoder(options ?? DecoderOptions.Default).Decode(stream, pixelFormat ?? PixelFormats.Bgr24, ct);
    }

    public static VideoSequence Decode(string filePath,
        PixelFormat? pixelFormat = null, DecoderOptions? options = null, CancellationToken ct = default)
    {
        using var file = File.OpenRead(filePath);
        return Decode(file, pixelFormat, options, ct);
    }

    public static MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        return new MediaDecoder(DecoderOptions.Default).Identify(stream, ct);
    }

    public static MediaContainerInfo Identify(string filePath, CancellationToken ct)
    {
        using var file = File.OpenRead(filePath);
        return Identify(file, ct);
    }
}