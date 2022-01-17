using Microsoft.AspNetCore.SignalR;
using RowerClassLibrary;
using System.Text.Json;

namespace RowerSignalRHub
{
    public class MainHub : Hub
    {
        public static bool IsRowing = false;
        public static List<StrokePacket> strokeList = new List<StrokePacket>();
        public async Task ProcessStrokePacket(string message)
        {
            if (!IsRowing)
            {
                return;
            }
            Console.WriteLine($"Recieved: {message}");
            if(message != String.Empty || message != null)
            {
                StrokePacket recievedPacket = JsonSerializer.Deserialize<StrokePacket>(message);
                Console.WriteLine($"Recieved packet!");
                Console.WriteLine($"Dist: {recievedPacket.Distance}");

                //if i have more the 3 strokes, calculate split based on last 3-5
                
            }
        }

        public async Task StartRowing(string message)
        {
            if (!IsRowing)
            {
                IsRowing = true;
                Console.WriteLine($"Recieved start!");
            }
        }

        public async Task StopRowing(string message)
        {
            IsRowing = false;
        }

        public async Task ResetRowing(string message)
        {
            strokeList.Clear();
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"{Context.ConnectionId} disconnected");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
