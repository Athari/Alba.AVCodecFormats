using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Windows.Storage.Streams;
using Microsoft.Graphics.Canvas;
using Microsoft.IO;
using Microsoft.Maui.Graphics;

// ReSharper disable InconsistentNaming
namespace Alba.AVCodecFormats.Maui.Graphics.Win2D.Internal;

/// <summary>
/// Copy-pasted from https://github.com/dotnet/maui/blob/main/src/Graphics/src/Graphics/Platforms/Windows/PlatformImage.cs
/// </summary>
[SuppressMessage("Style", "IDE0290"), SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Style", "IDE0060")]
internal class W2DImage : IImage
{
    private static readonly RecyclableMemoryStreamManager _RecyclableMemoryStreamManager = new();

    private readonly ICanvasResourceCreator _creator;
    private CanvasBitmap _bitmap;

    public W2DImage(ICanvasResourceCreator creator, CanvasBitmap bitmap)
    {
        _creator = creator;
        _bitmap = bitmap;
    }

    public CanvasBitmap PlatformRepresentation => _bitmap;

    public void Dispose()
    {
        var bitmap = Interlocked.Exchange(ref _bitmap!, null);
        bitmap?.Dispose();
    }

    public IImage Downsize(float maxWidthOrHeight, bool disposeOriginal = false)
    {
        if (Width > maxWidthOrHeight || Height > maxWidthOrHeight) {
            float factor = Width > Height ? maxWidthOrHeight / Width : maxWidthOrHeight / Height;
            var targetWidth = factor * Width;
            var targetHeight = factor * Height;
            return ResizeInternal(targetWidth, targetHeight, 0, 0, targetWidth, targetHeight, disposeOriginal);
        }
        return this;
    }

    public IImage Downsize(float maxWidth, float maxHeight, bool disposeOriginal = false)
    {
        return ResizeInternal(maxWidth, maxHeight, 0, 0, maxWidth, maxHeight, disposeOriginal);
    }


    private IImage ResizeInternal(float canvasWidth, float canvasHeight, float drawX, float drawY, float drawWidth, float drawHeight, bool disposeOriginal)
    {
        using var renderTarget = new CanvasRenderTarget(_creator, canvasWidth, canvasHeight, _bitmap.Dpi);

        using (var drawingSession = renderTarget.CreateDrawingSession())
            drawingSession.DrawImage(_bitmap, new(drawX, drawY, drawWidth, drawHeight));

        using var resizedStream = new InMemoryRandomAccessStream();
        var saveCompletedEvent = new ManualResetEventSlim(false);
        Exception? saveException = null;
        var saveTask = renderTarget.SaveAsync(resizedStream, CanvasBitmapFileFormat.Png).AsTask();
        saveTask.ContinueWith(task => {
            if (task.Exception is not null)
                saveException = task.Exception;
            saveCompletedEvent.Set();
        });
        saveCompletedEvent.Wait();
        if (saveException is not null)
            throw saveException;
        resizedStream.Seek(0);
        var newImage = FromStream(resizedStream.AsStreamForRead());
        if (disposeOriginal)
            _bitmap.Dispose();
        return newImage;
    }

    public IImage Resize(float width, float height, ResizeMode resizeMode = ResizeMode.Fit, bool disposeOriginal = false)
    {
        float scaleX = width / Width;
        float scaleY = height / Height;

        float targetWidth = Width;
        float targetHeight = Height;
        float offsetX = 0;
        float offsetY = 0;

        if (resizeMode == ResizeMode.Fit) {
            float scale = Math.Min(scaleX, scaleY);
            targetWidth *= scale;
            targetHeight *= scale;
            offsetX = (width - targetWidth) / 2;
            offsetY = (height - targetHeight) / 2;
        }
        else if (resizeMode == ResizeMode.Bleed) {
            float scale = Math.Max(scaleX, scaleY);
            targetWidth *= scale;
            targetHeight *= scale;
            offsetX = (width - targetWidth) / 2;
            offsetY = (height - targetHeight) / 2;
        }
        else {
            targetWidth = width;
            targetHeight = height;
        }

        return ResizeInternal(width, height, offsetX, offsetY, targetWidth, targetHeight, disposeOriginal);
    }

    public float Width => (float)_bitmap.Size.Width;

    public float Height => (float)_bitmap.Size.Height;

    public void Save(Stream stream, ImageFormat format = ImageFormat.Png, float quality = 1)
    {
        if (quality < 0 || quality > 1)
            throw new ArgumentOutOfRangeException(nameof(quality), "quality must be in the range of 0..1");

        switch (format) {
            case ImageFormat.Jpeg:
                AsyncPump.Run(async () => await _bitmap.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Jpeg, quality));
                break;
            default:
                AsyncPump.Run(async () => await _bitmap.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png));
                break;
        }
    }

    public async Task SaveAsync(Stream stream, ImageFormat format = ImageFormat.Png, float quality = 1)
    {
        if (quality < 0 || quality > 1)
            throw new ArgumentOutOfRangeException(nameof(quality), "quality must be in the range of 0..1");

        switch (format) {
            case ImageFormat.Jpeg:
                await _bitmap.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Jpeg, quality);
                break;
            default:
                await _bitmap.SaveAsync(stream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);
                break;
        }
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.DrawImage(this, dirtyRect.Left, dirtyRect.Top, Math.Abs(dirtyRect.Width), Math.Abs(dirtyRect.Height));
    }

    public IImage ToPlatformImage()
    {
        return new W2DImage(_creator, _bitmap);
    }

    public IImage ToImage(int width, int height, float scale = 1f)
    {
        throw new NotImplementedException();
    }

    public static IImage FromStream(Stream stream, ImageFormat format = ImageFormat.Png)
    {
        var creator = W2DGraphicsService.Creator ?? throw new("No resource creator has been registered globally or for this thread.");
        CanvasBitmap bitmap;
        if (stream.CanSeek) {
            var bitmapAsync = CanvasBitmap.LoadAsync(creator, stream.AsRandomAccessStream());
            bitmap = bitmapAsync.AsTask().GetAwaiter().GetResult();
        }
        else {
            using var memoryStream = _RecyclableMemoryStreamManager.GetStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var bitmapAsync = CanvasBitmap.LoadAsync(creator, memoryStream.AsRandomAccessStream());
            bitmap = bitmapAsync.AsTask().GetAwaiter().GetResult();
        }
        return new W2DImage(creator, bitmap);
    }
}

internal class W2DGraphicsService
{
    private static ICanvasResourceCreator? _globalCreator;
    private static readonly ThreadLocal<ICanvasResourceCreator?> _threadLocalCreator = new();

    public static ICanvasResourceCreator GlobalCreator
    {
        get => _globalCreator ?? CanvasDevice.GetSharedDevice();
        set => _globalCreator = value;
    }

    public static ICanvasResourceCreator? ThreadLocalCreator
    {
        get => _threadLocalCreator.Value;
        set => _threadLocalCreator.Value = value;
    }

    public static ICanvasResourceCreator Creator => ThreadLocalCreator ?? GlobalCreator;
}

public static class AsyncPump
{
    public static void Run(Func<Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        var prevCtx = SynchronizationContext.Current;
        try {
            var syncCtx = new SingleThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncCtx);
            var t = func() ?? throw new InvalidOperationException("No task provided.");
            t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);
            syncCtx.RunOnCurrentThread();
            t.GetAwaiter().GetResult();
        }
        finally {
            SynchronizationContext.SetSynchronizationContext(prevCtx);
        }
    }

    public static T Run<T>(Func<Task<T>> asyncMethod)
    {
        ArgumentNullException.ThrowIfNull(asyncMethod);

        var prevCtx = SynchronizationContext.Current;
        try {
            var syncCtx = new SingleThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncCtx);
            var t = asyncMethod() ?? throw new InvalidOperationException("No task provided.");
            t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);
            syncCtx.RunOnCurrentThread();
            return t.GetAwaiter().GetResult();
        }
        finally {
            SynchronizationContext.SetSynchronizationContext(prevCtx);
        }
    }

    private sealed class SingleThreadSynchronizationContext : SynchronizationContext
    {
        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> _mQueue = [ ];

        public override void Post(SendOrPostCallback d, object? state)
        {
            ArgumentNullException.ThrowIfNull(d);
            _mQueue.Add(new(d, state));
        }

        public override void Send(SendOrPostCallback d, object? state)
        {
            throw new NotSupportedException("Synchronously sending is not supported.");
        }

        public void RunOnCurrentThread()
        {
            foreach (var workItem in _mQueue.GetConsumingEnumerable())
                workItem.Key(workItem.Value);
        }

        public void Complete()
        {
            _mQueue.CompleteAdding();
        }
    }
}