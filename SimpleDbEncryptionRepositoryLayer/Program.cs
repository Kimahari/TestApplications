namespace SimpleDbEncryptionRepositoryLayer;

//using SimpleDbEncryptionRepositoryLayer;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

////var x = TheGenerator.TheGeneratedClass.TheGeneratedMethod();
////Console.WriteLine(x);

////var xx = new ForecastDbObject().Encrypt();
////var xxxx = new ForecastDbObject2().Encrypt();

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

//public interface IDataProtector {
//    public string Protect(string data);
//    public string Unprotect(string data);
//    public TEntity ProtectObject<TEntity>(TEntity data);
//    public TEntity UnProtectObject<TEntity>(TEntity data);
//}

//public class DataProtector : IDataProtector {
//    public string Protect(string data) {
//        return data;
//    }

//    public string Unprotect(string data) {
//        return data;
//    }

//    public TEntity ProtectObject<TEntity>(TEntity data) {
//        return data;
//    }

//    public TEntity UnProtectObject<TEntity>(TEntity data) {
//        return data;
//    }
//}

using Sybrin.Core.SourceGeneration;

/// <summary>
/// Provides a mechanism to be able to interact with transaction based processing.
/// </summary>
/// <seealso cref="IDisposable" />
public interface IUnitOfWork : IDisposable {
    /// <summary>
    /// Saves changes to all objects that have changed within the unit of work.
    /// </summary>
    void Commit();

    /// <summary>
    /// Rollback changes to all objects that have changed within the unit of work.
    /// </summary>
    void Rollback();
}

public class SybConnection {
    public string ID { get; set; } = "";
    public string ConnectionString { get; set; } = "";
}

[PIIAware]
public interface IConnectionRepository {
    void Add(SybConnection sybConnection);

    void Update(SybConnection sybConnection);

    void Delete(SybConnection sybConnection);

    SybConnection Find(string id);

    IEnumerable<SybConnection> All();
}


[PIIAware]
public interface ISybUnitOfWorkPortal : IUnitOfWork {
    public IConnectionRepository SybConnectionRepository { get; }
}

public class TTT {
    public TTT() {
        var x = new PIIAwareSybUnitOfWorkPortal(null);
        //var x = new PIIAwar
        //var x = new
        //var xxx = new TestNameSpace.HereIsTest();
    }

}