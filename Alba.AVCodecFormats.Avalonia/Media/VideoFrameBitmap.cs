using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Avalonia.Media;

[PublicAPI]
public class VideoFrameBitmap : WriteableBitmap
{
    public int FrameIndex { get; internal set; }

    internal VideoFrameBitmap(PixelSize size, Vector dpi, PixelFormat? format = null, AlphaFormat? alphaFormat = null)
        : base(size, dpi, format, alphaFormat) { }
}