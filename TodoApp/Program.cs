using MongoDB.Driver;
using TodoApp.CommandHandlers;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.QueryHandlers;
using TodoApp.Repository;
using TodoApp.Services;
using TodoApp.Utils;

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
    .AddSingleton<IRepository, Repository>()
    .AddSingleton<IUserService, UserService>()
    .AddSingleton<Handler>()
    .AddSingleton<IHandler<GetTodoByIdQuery, TodoGetDto>, GetTodoByIdQueryHandler>()
    .AddSingleton<IHandler<DeleteTodoCommand, Task>, DeleteTodoCommandHandler>()
    .AddSingleton<IHandler<AddTodoCommand, TodoGetDto>, AddTodoCommandHandler>()
    .AddSingleton<IHandler<UpdateTodoCommand, TodoGetDto>, UpdateTodoCommandHandler>()
    .AddSingleton<IHandler<GetTodoQuery, PagedList<TodoGetDto>>, GetTodoQueryHandler>()
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

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<SignalRHub>("/socket");

app.Run();
