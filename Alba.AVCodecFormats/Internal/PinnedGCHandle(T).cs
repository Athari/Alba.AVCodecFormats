using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Alba.AVCodecFormats.Internal;

[PublicAPI]
internal struct PinnedGCHandle<T>(T obj) : 
    IDisposable,
    IEquatable<PinnedGCHandle<T>>,
    IEqualityOperators<PinnedGCHandle<T>, PinnedGCHandle<T>, bool>
{
    private GCHandle _handle = GCHandle.Alloc(obj, GCHandleType.Pinned);

    public bool IsAllocated
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _handle.IsAllocated;
    }

    public T Target
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (T)_handle.Target!;
    }

    public nint Address
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _handle.AddrOfPinnedObject();
    }

    public void Dispose() => _handle.Free();

    public override string ToString() => _handle.ToString() ?? "";

    public bool Equals(PinnedGCHandle<T> other) => _handle.Equals(other._handle);
    public override bool Equals(object? obj) => obj is PinnedGCHandle<T> other && Equals(other);
    public override int GetHashCode() => _handle.GetHashCode();
    public static bool operator ==(PinnedGCHandle<T> left, PinnedGCHandle<T> right) => left.Equals(right);
    public static bool operator !=(PinnedGCHandle<T> left, PinnedGCHandle<T> right) => !left.Equals(right);
}