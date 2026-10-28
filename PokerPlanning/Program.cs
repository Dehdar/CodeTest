using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PokerPlanning.Hubs;
using PokerPlanning.Rooms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<RoomStore>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<PokerHub>("/hubs/poker");

app.Run();
