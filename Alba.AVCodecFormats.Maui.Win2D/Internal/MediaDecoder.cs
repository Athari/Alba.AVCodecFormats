using Windows.Graphics.DirectX;
using Alba.AVCodecFormats.Internal;
using Microsoft.Graphics.Canvas;

namespace Alba.AVCodecFormats.Maui.Graphics.Win2D.Internal;

internal sealed class MediaDecoder(DecoderOptions options) : MediaDecoderBase(options)
{
    public MediaContainerInfo Identify(Stream stream, CancellationToken ct)
    {
        using var file = OpenFileForIdentify(stream, ct);
        return MediaContainerInfo.Create(file);
    }

    public VideoSequence Decode(Stream stream, DirectXPixelFormat pixelFormat, CancellationToken ct)
    {
        var imagePixelFormat = pixelFormat.ToImagePixelFormat();
        using var file = OpenFileForDecode(stream, imagePixelFormat, ct);

        var size = file.Video.Info.FrameSize;
        var creator = W2DGraphicsService.Creator;
        var sequence = new VideoSequence();
        int frameIndex = 0;
        CanvasBitmap? bitmap = null;
        var data = new byte[size.Width * size.Height * imagePixelFormat.ToByteSize()];
        try {
            do {
                ct.ThrowIfCancellationRequested();
                if (!(Options.FrameIndexFilter?.Invoke(frameIndex) ?? true))
                    continue;

                if (!file.Video.TryGetNextFrame(data.AsSpan()))
                    break;

                if (bitmap == null)
                    bitmap = CanvasBitmap.CreateFromBytes(creator, data, size.Width, size.Height, pixelFormat);
                else
                    bitmap.SetPixelBytes(data);
                var image = new VideoFrameImage(new(creator, bitmap));

                if (Options.FrameFilterBase?.Invoke(image, frameIndex) ?? true) {
                    sequence.Frames.Add(image);
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