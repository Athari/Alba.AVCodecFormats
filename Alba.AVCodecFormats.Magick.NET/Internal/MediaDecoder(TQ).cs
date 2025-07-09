using System.Drawing;
using Alba.AVCodecFormats.Internal;
using ImageMagick;
using ImageMagick.Factories;

namespace Alba.AVCodecFormats.Magick.NET.Internal;

internal sealed class MediaDecoder<TQ>(IMagickFactory<TQ> factory, DecoderOptions<TQ> options) : MediaDecoderBase(options)
    where TQ : struct, IConvertible
{
    public MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        using var file = OpenFileForIdentify(stream, ct);
        return MediaContainerInfo.Create(file);
    }

    public VideoSequence<TQ> Decode(Stream stream, StorageType storageType, string mapping, CancellationToken ct)
    {
        var pixelFormat = storageType.ToImagePixelFormat(mapping);
        using var file = OpenFileForDecode(stream, pixelFormat, ct);

        var size = file.Video.Info.FrameSize;
        var readOpts = new PixelReadSettings(factory.Settings, size, storageType, mapping);
        var sequence = new VideoSequence<TQ>(factory);
        int frameIndex = 0;
        IMagickImage<TQ>? image = null;
        var data = new byte[size.Width * size.Height * pixelFormat.ToByteSize()].AsSpan();
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                image ??= factory.Image.Create();
                if (!file.Video.TryGetNextFrame(data))
                    break;

                image.ReadPixels(data, readOpts);
                if (Options.FrameFilterBase?.Invoke(image, frameIndex) ?? true) {
                    sequence.Frames.Add(image);
                    image = null;
                }
            } while (++frameIndex < Options.MaxFrames);

            image?.Dispose();
            if (sequence.Frames.Count == 0)
                throw new InvalidDataException("No frames found.");
        }
        catch {
            sequence.Dispose();
            throw;
        }

        sequence.Metadata = MediaContainerInfo.Create(file);
        return sequence;
    }

    private class PixelReadSettings : IPixelReadSettings<TQ>
    {
        public PixelReadSettings(ISettingsFactory<TQ> factory, Size size, StorageType storageType, PixelMapping mapping)
            : this(factory, size, storageType, mapping.ToString()) { }

        public PixelReadSettings(ISettingsFactory<TQ> factory, Size size, StorageType storageType, string mapping)
        {
            ReadSettings = factory.CreateMagickReadSettings();
            ReadSettings.Width = (uint)size.Width;
            ReadSettings.Height = (uint)size.Height;
            StorageType = storageType;
            Mapping = mapping;
        }

        public string? Mapping { get; set; }
        public StorageType StorageType { get; set; }
        public IMagickReadSettings<TQ> ReadSettings { get; }
    }
}