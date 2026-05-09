using System.Text.Json.Serialization;
using LogiFlow.Api.Hubs;
using LogiFlow.Api.Middleware;
using LogiFlow.Api.Realtime;
using LogiFlow.Application;
using LogiFlow.Application.Abstractions;
using LogiFlow.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddSignalR().AddJsonProtocol(options =>
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


builder.Services.AddSingleton<ITrackingUpdatePublisher, SignalRTrackingUpdatePublisher>();
builder.Services.AddSingleton<IRealtimeUpdatePublisher, SignalRRealtimeUpdatePublisher>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.MapHub<TrackingHub>("/hubs/tracking");

app.Run();