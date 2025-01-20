//Program.cs
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add SignalR
builder.Services.AddSignalR();
// Add Table Storage and Service Bus
builder.Services.AddSingleton(new TableStorageHelper(configuration["TableStorage:ConnectionString"], "OrderTracking"));
builder.Services.AddSingleton(new ServiceBusHelper(configuration["ServiceBus:ConnectionString"], "OrderTracker"));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ServiceBusListenerService
builder.Services.AddSingleton<ServiceBusListenerService>();
builder.Services.AddHostedService<ServiceBusListenerService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Your Next.js frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Allow SignalR WebSocket connections
    });
}); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();


// Apply CORS Policy
app.UseCors("AllowSpecificOrigins");

app.MapControllers();
// Map the SignalR hub
app.MapHub<OrderHub>("/orderHub").RequireCors("AllowSpecificOrigins");

app.Run();
