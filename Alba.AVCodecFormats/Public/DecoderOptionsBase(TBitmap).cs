using JetBrains.Annotations;

namespace Alba.AVCodecFormats;

[PublicAPI]
public abstract class DecoderOptionsBase<TBitmap> : DecoderOptionsBase
{
    private Func<TBitmap, int, bool>? _frameFilter;

    /// <summary>A predicate that provides the way to skip frames based on their content and index.
    /// Returns true when frame should be included, otherwise false.
    /// To skip based on index, without decoding, use <see cref="DecoderOptionsBase.FrameIndexFilter"/>.</summary>
    public Func<TBitmap, int, bool>? FrameFilter
    {
        get => _frameFilter;
        set
        {
            _frameFilter = value;
            FrameFilterBase = value != null ? (b, i) => value((TBitmap)b, i) : null;
        }
    }
}