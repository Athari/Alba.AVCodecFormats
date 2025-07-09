using FFMediaToolkit;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats;

[PublicAPI]
public static class FFMpegOptions
{
    public static string? FFMpegPath { get; set; }

    public static FFMpegLogLevel LogLevel
    {
        get => (FFMpegLogLevel)FFmpegLoader.LogVerbosity;
        set => FFmpegLoader.LogVerbosity = (LogLevel)value;
    }
}