using MongoDB.Driver;
using TodoApp.Models;
using TodoApp.Repository;
using TodoApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddSingleton<IMongoDatabase>(_ =>
    {
        var client = new MongoClient(builder
            .Configuration
            .GetSection("MongoDB:ConnectionUri").Value);
        return client.GetDatabase(builder.Configuration.GetSection("MongoDB:DatabaseName").Value);
    })
    .AddSingleton<UserService>()
    .AddSingleton<TodoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
