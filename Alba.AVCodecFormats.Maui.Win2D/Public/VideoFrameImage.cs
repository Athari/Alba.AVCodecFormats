using Alba.AVCodecFormats.Maui.Graphics.Win2D.Internal;
using JetBrains.Annotations;
using Microsoft.Graphics.Canvas;
using Microsoft.Maui.Graphics;

namespace Alba.AVCodecFormats.Maui.Graphics.Win2D;

[PublicAPI]
public class VideoFrameImage : IImage
{
    private readonly W2DImage _image;

    public CanvasBitmap Image => _image.PlatformRepresentation;
    public int FrameIndex { get; internal set; }

    internal VideoFrameImage(W2DImage image) => _image = image;

    public float Width => _image.Width;
    public float Height => _image.Height;

    public void Draw(ICanvas canvas, RectF dirtyRect) => _image.Draw(canvas, dirtyRect);
    public IImage Downsize(float maxWidthOrHeight, bool disposeOriginal = false) => _image.Downsize(maxWidthOrHeight, disposeOriginal);
    public IImage Downsize(float maxWidth, float maxHeight, bool disposeOriginal = false) => _image.Downsize(maxWidth, maxHeight, disposeOriginal);
    public IImage Resize(float width, float height, ResizeMode resizeMode = ResizeMode.Fit, bool disposeOriginal = false) => _image.Resize(width, height, resizeMode, disposeOriginal);
    public void Save(Stream stream, ImageFormat format = ImageFormat.Png, float quality = 1) => _image.Save(stream, format, quality);
    public Task SaveAsync(Stream stream, ImageFormat format = ImageFormat.Png, float quality = 1) => _image.SaveAsync(stream, format, quality);
    public IImage ToPlatformImage() => _image.ToPlatformImage();

    public void Dispose()
    {
        _image.Dispose();
        GC.SuppressFinalize(this);
    }
}