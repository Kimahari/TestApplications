namespace SimpleDbContextPooling;

/// <summary>
/// Base class for disposable objects that implements both synchronous and asynchronous disposal patterns.
/// </summary>
public class DisposableBase : IDisposable, IAsyncDisposable {

    protected bool disposed;

    /// <summary>
    /// Disposes the object synchronously.
    /// </summary>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the object asynchronously.
    /// </summary>
    public async ValueTask DisposeAsync() {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources, false otherwise.</param>
    protected virtual void Dispose(bool disposing) {
        if (disposed) return;

        if (disposing) {
            DisposeManaged();
        }

        DisposeUnmanaged();
        disposed = true;
    }

    /// <summary>
    /// Disposes the object asynchronously.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources, false otherwise.</param>
    protected virtual async ValueTask DisposeAsync(bool disposing) {
        if (disposed) return;

        if (disposing) {
            await DisposeManagedAsync();
        }

        await DisposeUnmanagedAsync();
        disposed = true;
    }

    /// <summary>
    /// Disposes managed resources.
    /// </summary>
    protected virtual void DisposeManaged() { }

    /// <summary>
    /// Disposes managed resources asynchronously.
    /// </summary>
    protected virtual Task DisposeManagedAsync() { return Task.CompletedTask; }

    /// <summary>
    /// Disposes unmanaged resources.
    /// </summary>
    protected virtual void DisposeUnmanaged() { }

    /// <summary>
    /// Disposes unmanaged resources asynchronously.
    /// </summary>
    protected virtual Task DisposeUnmanagedAsync() { return Task.CompletedTask; }
}
