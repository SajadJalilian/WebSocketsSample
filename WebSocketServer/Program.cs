using Microsoft.AspNetCore.SignalR;
using TestWebSockets;

var builder = WebApplication.CreateBuilder(args);

// IOC services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Pipeline
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<ChatHub>("chat-hub");

app.MapPost("/broadcast", async (string message, IHubContext<ChatHub, IChatClient> context) =>
    {
        await context.Clients.All.ReceiveMessage(message);
        return Results.NoContent();
    })
    .WithName("BroadcastMessage")
    .WithOpenApi();

app.MapPost("/ack", async (string userId, IHubContext<ChatHub> context) =>
    {
        await context.Clients.Client(userId).SendAsync("Ack", "Ack is returning the call after processing");
        return Results.NoContent();
    })
    .WithName("Ack")
    .WithOpenApi();

app.Run();