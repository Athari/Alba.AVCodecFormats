using Alba.AVCodecFormats.Internal;
using Alba.AVCodecFormats.SkiaSharp;
using Alba.AVCodecFormats.SkiaSharp.Internal;
using SkiaSharp;

namespace Alba.AVCodecFormats.Maui.Graphics.Skia.Internal;

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

        var info = new SKImageInfo(file.Video.Info.FrameSize.Width, file.Video.Info.FrameSize.Height, colorType, alphaType);
        var sequence = new VideoSequence();
        int frameIndex = 0;
        UndisposableSKBitmap? bitmap = null;
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                bitmap ??= new(info, SKBitmapAllocFlags.None);
                if (!file.Video.TryGetNextFrame(bitmap.GetPixelSpan()))
                    break;

                bitmap.NotifyPixelsChanged();
                var image = new VideoFrameImage(bitmap);
                if (Options.FrameFilterBase?.Invoke(image, frameIndex) ?? true) {
                    sequence.Frames.Add(image);
                    bitmap.IsDisposable = true; // SkiaImage will actually own SKBitmap now
                    bitmap = null;
                }
                else {
                    image.Dispose();
                }
            } while (++frameIndex < Options.MaxFrames);

            if (bitmap != null) {
                bitmap.IsDisposable = true;
                bitmap.Dispose();
            }
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