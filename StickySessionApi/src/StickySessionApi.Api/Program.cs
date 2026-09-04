using Microsoft.AspNetCore.SignalR;
using StickySessionApi.Api.Endpoints;
using StickySessionApi.Api.Startup;
using StickySessionApi.Application.Messaging;
using StickySessionApi.Application.Users;
using StickySessionApi.Infrastructure.Messaging;
using StickySessionApi.Infrastructure.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMessagingSignalR(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddSingleton<InstanceId>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
builder.Services.AddScoped<IMessageSender, SignalRMessageSender>();

var app = builder.Build();

InstanceBanner.Print(app.Services.GetRequiredService<InstanceId>());

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapHub<MessagingHub>("/hubs/messaging");
app.MapUsersEndpoints();
app.MapMessagesEndpoints();

app.Run();
