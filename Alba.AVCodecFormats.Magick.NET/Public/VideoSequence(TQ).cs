using Alba.AVCodecFormats.ImageMagick.Internal;
using Alba.AVCodecFormats.Internal;
using ImageMagick;
using ImageMagick.Factories;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.ImageMagick;

[PublicAPI]
public sealed class VideoSequence<TQ> : IDisposable
    where TQ : struct, IConvertible
{
    private readonly IMagickFactory<TQ> _factory;

    public IMagickImageCollection<TQ> Frames { get; private set; }

    public MediaContainerInfo Metadata { get; internal set; } = null!;

    public VideoSequence(IMagickFactory<TQ> factory)
    {
        _factory = factory;
        Frames = _factory.ImageCollection.Create();
    }

    public static VideoSequence<TQ> Decode(IMagickFactory<TQ> factory, Stream stream,
        StorageType storageType = StorageType.Char, string mapping = "BGR",
        DecoderOptions<TQ>? options = null, CancellationToken ct = default)
    {
        return new MediaDecoder<TQ>(factory, options ?? DecoderOptions<TQ>.Default).Decode(stream, storageType, mapping, ct);
    }

    public static VideoSequence<TQ> Decode(IMagickFactory<TQ> factory, string filePath,
        StorageType storageType = StorageType.Char, string mapping = "BGR",
        DecoderOptions<TQ>? options = null, CancellationToken ct = default)
    {
        using var file = File.OpenRead(filePath);
        return Decode(factory, file, storageType, mapping, options, ct);
    }

    public static MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        using var file = MediaDecoderBase.OpenFileForIdentifyCore(stream, ct);
        return MediaContainerInfo.Create(file);
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