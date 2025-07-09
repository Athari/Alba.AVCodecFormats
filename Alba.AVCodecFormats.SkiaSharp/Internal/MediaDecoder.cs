using Alba.AVCodecFormats.Internal;
using SkiaSharp;

namespace Alba.AVCodecFormats.SkiaSharp.Internal;

internal sealed class MediaDecoder(DecoderOptions options) : MediaDecoderBase(options)
{
    public MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        using var file = OpenFileForIdentify(stream, ct);
        return MediaContainerInfo.Create(file);
    }

    public VideoSequence Decode(Stream stream, SKColorType colorType, SKAlphaType alphaType, CancellationToken ct)
    {
        using var file = OpenFileForDecode(stream, colorType.ToImagePixelFormat(), ct);

        var sequence = new VideoSequence();
        int frameIndex = 0;
        VideoFrameBitmap? bitmap = null;
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                bitmap ??= new(file.Video.Info.FrameSize.ToPixelSize(), colorType, alphaType);
                if (!file.Video.TryGetNextFrame(bitmap.GetPixelSpan()))
                    break;

                bitmap.NotifyPixelsChanged();
                bitmap.FrameIndex = frameIndex;
                if (Options.FrameFilterBase?.Invoke(bitmap, frameIndex) ?? true) {
                    sequence.Frames.Add(bitmap);
                    bitmap = null;
                }
            } while (++frameIndex < Options.MaxFrames);

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