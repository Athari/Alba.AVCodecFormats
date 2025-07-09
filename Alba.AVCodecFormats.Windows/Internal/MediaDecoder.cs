using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Alba.AVCodecFormats.Internal;
using Alba.AVCodecFormats.Windows.Media;

namespace Alba.AVCodecFormats.Windows.Internal;

internal sealed class MediaDecoder(DecoderOptions options) : MediaDecoderBase(options)
{
    private static readonly Size WindowsPlatformDefaultDpi = new(96, 96);

    public MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        using var file = OpenFileForIdentify(stream, ct);
        return MediaContainerInfo.Create(file);
    }

    public VideoSequence Decode(Stream stream, PixelFormat pixelFormat, CancellationToken ct)
    {
        using var file = OpenFileForDecode(stream, pixelFormat.ToImagePixelFormat(), ct);

        var sequence = new VideoSequence();
        int frameIndex = 0;
        WriteableBitmap? bitmap = null;
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                bitmap ??= new(
                    file.Video.Info.FrameSize.Width, file.Video.Info.FrameSize.Height,
                    WindowsPlatformDefaultDpi.Width, WindowsPlatformDefaultDpi.Height,
                    pixelFormat, palette: null);
                bitmap.Lock();
                if (!file.Video.TryGetNextFrame(bitmap.BackBuffer, bitmap.BackBufferStride))
                    break;

                bitmap.Unlock();
                if (Options.FrameFilterBase?.Invoke(bitmap, frameIndex) ?? true) {
                    sequence.Frames.Add(bitmap);
                    bitmap = null;
                }
            } while (++frameIndex < Options.MaxFrames);

            bitmap?.Unlock();
            if (sequence.Frames.Count == 0)
                throw new InvalidDataException("No frames found.");
        }
        catch {
            sequence.Frames.Clear();
            throw;
        }

        sequence.Metadata = MediaContainerInfo.Create(file);
        return sequence;
    }
}