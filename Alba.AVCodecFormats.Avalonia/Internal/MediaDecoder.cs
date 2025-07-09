using Alba.AVCodecFormats.Avalonia.Media;
using Alba.AVCodecFormats.Internal;
using Avalonia;
using Avalonia.Platform;

namespace Alba.AVCodecFormats.Avalonia.Internal;

internal sealed class MediaDecoder(DecoderOptions options) : MediaDecoderBase(options)
{
    private static readonly Vector SkiaPlatformDefaultDpi = new(96, 96);

    public MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        using var file = OpenFileForIdentify(stream, ct);
        return MediaContainerInfo.Create(file);
    }

    public VideoSequence Decode(Stream stream, PixelFormat pixelFormat, AlphaFormat alphaFormat, CancellationToken ct)
    {
        using var file = OpenFileForDecode(stream, pixelFormat.ToImagePixelFormat(), ct);

        var sequence = new VideoSequence();
        int frameIndex = 0;
        VideoFrameBitmap? bitmap = null;
        ILockedFramebuffer? buf = null;
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                if (bitmap == null || buf == null) {
                    bitmap = new(file.Video.Info.FrameSize.ToPixelSize(), SkiaPlatformDefaultDpi, pixelFormat, alphaFormat);
                    buf = bitmap.Lock();
                }
                if (!file.Video.TryGetNextFrame(buf.Address, buf.RowBytes))
                    break;

                buf.Dispose();
                bitmap.FrameIndex = frameIndex;
                if (Options.FrameFilterBase?.Invoke(bitmap, frameIndex) ?? true) {
                    sequence.Frames.Add(bitmap);
                    bitmap = null;
                    buf = null;
                }
            } while (++frameIndex < Options.MaxFrames);

            buf?.Dispose();
            bitmap?.Dispose();
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
}