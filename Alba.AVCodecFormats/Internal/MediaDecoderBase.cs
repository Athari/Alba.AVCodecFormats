using System.Drawing;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;

namespace Alba.AVCodecFormats.Internal;

internal abstract class MediaDecoderBase
{
    private static readonly object SyncRoot = new();

    private static bool _InitBinaries;

    protected DecoderOptionsBase Options { get; }

    protected MediaDecoderBase(DecoderOptionsBase options)
    {
        if (!_InitBinaries) {
            lock (SyncRoot) {
                if (!_InitBinaries) {
                    FFMpegBinariesFinder.FindBinaries();
                    _InitBinaries = true;
                }
            }
        }
        Options = options;
    }

    protected virtual MediaFile OpenFileForIdentify(Stream stream, CancellationToken ct)
    {
        return OpenFileForIdentifyCore(stream, ct);
    }

    internal static MediaFile OpenFileForIdentifyCore(Stream stream, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var file = MediaFile.Open(stream, new() {
            StreamsToLoad = MediaMode.AudioVideo,
        });
        EnsureHasVideoStream(file);
        return file;
    }

    protected virtual MediaFile OpenFileForDecode(Stream stream, ImagePixelFormat pixelFormat, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var targetFrameSize = CalculateTargetFrameSize(stream);
        var file = MediaFile.Open(stream, new() {
            VideoPixelFormat = pixelFormat,
            TargetVideoSize = targetFrameSize,
            RespectSampleAspectRatio = Options.RespectSampleAspectRatio,
            DemuxerOptions = new() {
                FlagDiscardCorrupt = true,
            },
            CodecName = Options.CodecName,
            DecoderOptions = Options.Options,
            StreamsToLoad = MediaMode.AudioVideo,
        });
        EnsureHasVideoStream(file);
        return file;
    }

    private static void EnsureHasVideoStream(MediaFile file)
    {
        if (file.HasVideo)
            return;
        file.Dispose();
        throw new InvalidDataException("The file has no video streams.");
    }

    private Size? CalculateTargetFrameSize(Stream stream)
    {
        if (Options.TargetSize == null)
            return null;
        if (!Options.PreserveAspectRatio)
            return Options.TargetSize;

        using var file = OpenFileForIdentify(stream, CancellationToken.None);
        if (stream.CanSeek)
            stream.Position = 0;
        return CalculateMaxRectangle(file.VideoStreams[0].Info.FrameSize, Options.TargetSize.Value);
    }

    public static Size CalculateMaxRectangle(Size source, Size desired)
    {
        var targetWidth = desired.Width;
        var targetHeight = desired.Height;

        // Fractional variants for preserving aspect ratio.
        var percentHeight = MathF.Abs(desired.Height / (float)source.Height);
        var percentWidth = MathF.Abs(desired.Width / (float)source.Width);

        // Integers must be cast to floats to get needed precision
        var ratio = desired.Height / (float)desired.Width;
        var sourceRatio = source.Height / (float)source.Width;

        if (sourceRatio < ratio)
            targetHeight = (int)MathF.Round(source.Height * percentWidth);
        else
            targetWidth = (int)MathF.Round(source.Width * percentHeight);
        return new(Sanitize(targetWidth), Sanitize(targetHeight));

        static int Sanitize(int input) => Math.Max(1, input);
    }
}