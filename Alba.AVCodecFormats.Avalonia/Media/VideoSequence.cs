using Alba.AVCodecFormats.Avalonia.Internal;
using Avalonia.Platform;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Avalonia.Media;

[PublicAPI]
public sealed class VideoSequence : IDisposable
{
    public IList<VideoFrameBitmap> Frames { get; } = [ ];

    public MediaContainerInfo Metadata { get; internal set; } = null!;

    public static VideoSequence Decode(Stream stream,
        PixelFormat? pixelFormat = null, AlphaFormat alphaFormat = AlphaFormat.Unpremul,
        DecoderOptions? options = null, CancellationToken ct = default)
    {
        return new MediaDecoder(options ?? DecoderOptions.Default).Decode(stream, pixelFormat ?? PixelFormats.Bgr24, alphaFormat, ct);
    }

    public static VideoSequence Decode(string filePath,
        PixelFormat? pixelFormat = null, AlphaFormat alphaFormat = AlphaFormat.Unpremul,
        DecoderOptions? options = null, CancellationToken ct = default)
    {
        using var file = File.OpenRead(filePath);
        return Decode(file, pixelFormat, alphaFormat, options, ct);
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

    public void Dispose()
    {
        foreach (var frame in Frames)
            frame.Dispose();
        Frames.Clear();
    }
}