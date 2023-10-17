using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using TodoApp.CommandHandlers;
using TodoApp.Commands;
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
    .AddSingleton<IHandlerService, Handler>()
    .AddSingleton<IHandler<DeleteTodoCommand>, DeleteTodoCommandHandler>()
    .AddSingleton<IHandler<AddTodoCommand>, AddTodoCommandHandler>()
    .AddSingleton<IHandler<UpdateTodoCommand>, UpdateTodoCommandHandler>()
    .AddSingleton<IHandler<TodosQuery>, TodosQueryHandler>()
    .AddSingleton<ITodoService, TodoService>();

builder.Services.AddSignalR();
// builder.Services.AddHostedService<SignalRClientHostedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder
                    .Configuration
                    .GetSection("Keycloak:Host").Value + "/protocol/openid-connect/auth"),
                Scopes = { { "openid", "OpenID Connect" }, { "profile", "User Profile" }, { "email", "Email" } }
            }
        }
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
            },
            Array.Empty<string>() //scopes
        }
    });

});

//keycloak
builder.Services
    .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(x =>
    {
        x.MetadataAddress = builder
            .Configuration
            .GetSection("Keycloak:Host").Value + "/.well-known/openid-configuration";
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = "account"
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("email_verified", "true")
        .Build();
});

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
