using Alba.AVCodecFormats.Maui.Graphics.Skia.Internal;
using Alba.AVCodecFormats.SkiaSharp;
using JetBrains.Annotations;
using SkiaSharp;

namespace Alba.AVCodecFormats.Maui.Graphics.Skia;

[PublicAPI]
public sealed class VideoSequence : IDisposable
{
    public IList<VideoFrameImage> Frames { get; } = [ ];

    public MediaContainerInfo Metadata { get; internal set; } = null!;

    public static VideoSequence Decode(Stream stream,
        SKColorType colorType = SKColorType.Bgra8888, SKAlphaType alphaType = SKAlphaType.Unpremul,
        DecoderOptions? options = null, CancellationToken ct = default)
    {
        return new MediaDecoder(options ?? DecoderOptions.Default).Decode(stream, colorType, alphaType, ct);
    }

    public static VideoSequence Decode(string filePath,
        SKColorType colorType = SKColorType.Bgra8888, SKAlphaType alphaType = SKAlphaType.Unpremul,
        DecoderOptions? options = null, CancellationToken ct = default)
    {
        using var file = File.OpenRead(filePath);
        return Decode(file, colorType, alphaType, options, ct);
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