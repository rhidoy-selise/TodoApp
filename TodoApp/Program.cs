using MongoDB.Driver;
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
    .AddSingleton<SignalRClientHostedService>()
    .AddSingleton<ISignalRService, SignalRService>()
    .AddSingleton<ISignalRClientService, SignalRClientService>()
    .AddSingleton<IRabbitMqService, RabbitMqService>()
    .AddSingleton<IUserService, UserService>()
    .AddSingleton<ITodoService, TodoService>();

builder.Services.AddSignalR();
// builder.Services.AddHostedService<SignalRClientHostedService>();

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

app.MapHub<SignalRHub>("/socket");

app.Run();
