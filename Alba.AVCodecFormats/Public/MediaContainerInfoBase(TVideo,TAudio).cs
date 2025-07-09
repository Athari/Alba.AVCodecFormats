using System.Collections.ObjectModel;
using Alba.AVCodecFormats.Internal;
using FFMediaToolkit.Decoding;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats;

/// <summary>Contains information about the media container.</summary>
[PublicAPI]
public abstract class MediaContainerInfoBase<TVideo, TAudio>
    where TVideo : VideoStreamInfoBase
    where TAudio : AudioStreamInfoBase
{
    private readonly MediaInfo _source;

    protected MediaContainerInfoBase(MediaFile source)
    {
        _source = source.Info;
        Metadata = source.Info.Metadata.Metadata.ToReadOnlyDictionaryIC();
        VideoStreams = [ .. source.VideoStreams.Select(CreateVideoStreamInfo) ];
        AudioStreams = [ .. source.AudioStreams.Select(CreateAudioStreamInfo) ];
    }

    /// <summary>Gets the container format name.</summary>
    public string ContainerFormat => _source.ContainerFormat;

    /// <summary>Gets the container bitrate in bytes per second (B/s) units. 0 if unknown.</summary>
    public long Bitrate => _source.Bitrate;

    /// <summary>Gets the duration of the media container.</summary>
    public TimeSpan Duration => _source.Duration;

    /// <summary>Gets the start time of the media container.</summary>
    public TimeSpan StartTime => _source.StartTime;

    /// <summary>Gets the container file metadata. Streams may contain additional metadata.</summary>
    public ReadOnlyDictionary<string, string> Metadata { get; }

    public IList<TVideo> VideoStreams { get; }

    public IList<TAudio> AudioStreams { get; }

    protected internal abstract TVideo CreateVideoStreamInfo(VideoStream source);

    protected internal abstract TAudio CreateAudioStreamInfo(AudioStream source);
}