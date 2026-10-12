using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using WeaponGameRestApi.Core;
using WeaponGameRestApi.Hubs;
using WeaponGameRestApi.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSignalR();
                    
builder.Services.AddSingleton<IGameLogger, SignalRLogger>();
                    
builder.Services.AddSingleton<GameEngineService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
                {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Мапимо маршрут для вебсокетів
app.MapHub<GameHub>("/gamehub");

app.Run();