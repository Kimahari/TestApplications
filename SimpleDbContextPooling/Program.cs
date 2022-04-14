using Microsoft.EntityFrameworkCore;

using SimpleDbContextPooling.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterDbContextPooling<TestDbContext>(oo => oo.UseSqlServer("Server=localhost;Database=myDataBase;User Id=sa;Password=Pass@Word1;"));

var app = builder.Build();

using (var scope = app.Services.GetRequiredService<IServiceProvider>().CreateScope()) {
    var dc = scope.ServiceProvider.GetRequiredService<TestDbContext>();
    await dc.Database.MigrateAsync();
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
