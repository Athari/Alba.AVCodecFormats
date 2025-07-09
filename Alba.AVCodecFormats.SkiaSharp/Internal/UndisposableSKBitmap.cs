using SkiaSharp;

namespace Alba.AVCodecFormats.SkiaSharp.Internal;

internal class UndisposableSKBitmap(SKImageInfo info, SKBitmapAllocFlags flags) : SKBitmap(info, flags)
{
    public bool IsDisposable { get; set; }

    protected override void Dispose(bool disposing)
    {
        if (!IsDisposable)
            return;
        base.Dispose(disposing);
    }
}