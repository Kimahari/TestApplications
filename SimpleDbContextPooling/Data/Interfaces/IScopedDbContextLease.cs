namespace SimpleDbContextPooling.Data.Interfaces;

interface IScopedDbContextLease<out TContext>
   where TContext : DbContext {

    TContext Context { get; }
}
