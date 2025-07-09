using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Magick.NET;

/// <summary>Contains information about the media container.</summary>
[PublicAPI]
public sealed class MediaContainerInfo : MediaContainerInfoBase<VideoStreamInfo, AudioStreamInfo>
{
    private MediaContainerInfo(MediaFile source) : base(source) { }

    internal static MediaContainerInfo Create(MediaFile source) => new(source);

    protected internal override VideoStreamInfo CreateVideoStreamInfo(VideoStream source) => new(source.Info);
    protected internal override AudioStreamInfo CreateAudioStreamInfo(AudioStream source) => new(source.Info);
}