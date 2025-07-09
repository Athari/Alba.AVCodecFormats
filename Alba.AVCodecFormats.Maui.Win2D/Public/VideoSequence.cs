using Windows.Graphics.DirectX;
using Alba.AVCodecFormats.Maui.Graphics.Win2D.Internal;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Maui.Graphics.Win2D;

[PublicAPI]
public sealed class VideoSequence : IDisposable
{
    public IList<VideoFrameImage> Frames { get; } = [ ];

    public MediaContainerInfo Metadata { get; internal set; } = null!;

    public static VideoSequence Decode(Stream stream,
        DirectXPixelFormat pixelFormat = DirectXPixelFormat.B8G8R8A8UIntNormalized,
        DecoderOptions? options = null, CancellationToken ct = default)
    {
        return new MediaDecoder(options ?? DecoderOptions.Default).Decode(stream, pixelFormat, ct);
    }

    public static VideoSequence Decode(string filePath,
        DirectXPixelFormat pixelFormat = DirectXPixelFormat.B8G8R8A8UIntNormalized,
        DecoderOptions? options = null, CancellationToken ct = default)
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

    public void Dispose()
    {
        foreach (var frame in Frames)
            frame.Dispose();
        Frames.Clear();
    }
}