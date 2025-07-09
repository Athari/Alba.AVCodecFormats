using System.Drawing.Imaging;
using Alba.AVCodecFormats.Drawing.Imaging;
using Alba.AVCodecFormats.Internal;

namespace Alba.AVCodecFormats.Drawing.Internal;

internal sealed class MediaDecoder(DecoderOptions options) : MediaDecoderBase(options)
{
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
        Bitmap? bitmap = null;
        BitmapData? data = null;
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                if (bitmap == null || data == null) {
                    bitmap = new(file.Video.Info.FrameSize.Width, file.Video.Info.FrameSize.Height, pixelFormat);
                    data = bitmap.LockBits(new(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                }
                if (!file.Video.TryGetNextFrame(data.Scan0, data.Stride))
                    break;

                bitmap.UnlockBits(data);
                if (Options.FrameFilterBase?.Invoke(bitmap, frameIndex) ?? true) {
                    sequence.Frames.Add(bitmap);
                    bitmap = null;
                    data = null;
                }
            } while (++frameIndex < Options.MaxFrames);

            if (data != null)
                bitmap?.UnlockBits(data);
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