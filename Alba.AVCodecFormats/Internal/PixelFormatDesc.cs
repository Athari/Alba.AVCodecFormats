using FFmpeg.AutoGen;

namespace Alba.AVCodecFormats.Internal;

internal readonly struct PixelFormatDesc
{
    public unsafe PixelFormatDesc(string pixelFormatName)
    {
        var pPixFmtDesc = ffmpeg.av_pix_fmt_desc_get(ffmpeg.av_get_pix_fmt(pixelFormatName));
        if (pPixFmtDesc != null) {
            BitPerPixel = ffmpeg.av_get_bits_per_pixel(pPixFmtDesc);
            BitPerPixelPadded = ffmpeg.av_get_padded_bits_per_pixel(pPixFmtDesc);
        }
    }

    public int BitPerPixel { get; }
    public int BitPerPixelPadded { get; }
}