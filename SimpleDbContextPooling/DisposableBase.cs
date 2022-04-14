namespace SimpleDbContextPooling;

public class DisposableBase : IDisposable, IAsyncDisposable {

    protected bool disposed;

    public void Dispose() {
        if (disposed) return;

        Dispose(true);
        disposed = true;

        GC.SuppressFinalize(this);  
    }

    public async ValueTask DisposeAsync() {
        if (disposed) return;
        
        await DisposeAsync(true);
        disposed = true;
        
        GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing) {
        if (disposing) {
            DisposeManaged();
        }

        DisposeUnmanaged();
    }

    async Task DisposeAsync(bool disposing) {
        if (disposing) {
            await DisposeManagedAsync();
        }

        await DisposeUnmanagedAsync();
    }

    protected virtual void DisposeUnmanaged() { }
    protected virtual Task DisposeUnmanagedAsync() { return Task.CompletedTask; }

    protected virtual void DisposeManaged() { }
    protected virtual Task DisposeManagedAsync() { return Task.CompletedTask; }
}
