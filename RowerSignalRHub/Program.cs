using RowerSignalRHub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

var app = builder.Build();
app.UseRouting();
app.MapHub<MainHub>("/row");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<MainHub>("/chat");
//});

app.Run();
