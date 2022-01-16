using RowerSignalRHub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<MainHub>("/row");

app.Run();
