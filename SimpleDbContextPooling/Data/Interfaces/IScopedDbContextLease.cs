namespace SimpleDbContextPooling.Data;

interface IScopedDbContextLease<out TContext>
   where TContext : DbContext {

    TContext Context { get; }
}
