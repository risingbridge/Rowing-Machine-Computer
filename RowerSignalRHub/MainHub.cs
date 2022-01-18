using Microsoft.AspNetCore.SignalR;
using RowerClassLibrary;
using System.Text.Json;
using System.Linq;

namespace RowerSignalRHub
{
    public class MainHub : Hub
    {
        public static bool IsRowing = false;
        public static List<StrokePacket> strokeList = new List<StrokePacket>();
        public static string ActiveUser = "Gjest";
        public static float TotalDistance = 0;
        public async Task ProcessStrokePacket(string message)
        {
            if (!IsRowing)
            {
                return;
            }
            //Console.WriteLine($"Recieved: {message}");
            if(message != String.Empty || message != null)
            {
                StrokePacket recievedPacket = JsonSerializer.Deserialize<StrokePacket>(message);
                strokeList.Add(recievedPacket);
                TotalDistance += recievedPacket.Distance;
                TimeSpan strokeSpan = recievedPacket.StrokeEndTimestamp - recievedPacket.StrokeStartTimestamp;
                TimeSpan split = (500 / recievedPacket.Distance) * strokeSpan;

                //if i have more the 5 strokes, calculate split based on last 5 and stroke pr minutes
                TimeSpan averageSplit = split;
                int strokePrMin = 0;
                if(strokeList.Count >= 5)
                {
                    DateTime lastStroke = strokeList.Last().StrokeStartTimestamp;
                    DateTime firstStroke = strokeList[strokeList.Count - 5].StrokeStartTimestamp;
                    float dist = 0;
                    int distCount = 0;
                    for (int i = strokeList.Count -1; i > 0; i--)
                    {
                        dist += strokeList[i].Distance;
                        if(distCount >= 5)
                        {
                            break;
                        }
                    }
                    TimeSpan longStrokeSpan = lastStroke - firstStroke;
                    averageSplit = ((longStrokeSpan / dist) * 500);
                    split = averageSplit;
                    //Calc strokes pr min over last 5
                    if (longStrokeSpan.Seconds > 0)
                    {
                        strokePrMin = (int)((float)60 / (float)(longStrokeSpan.Seconds / 5));
                    }
                }
                TimeSpan elapsed = TimeSpan.FromSeconds(0);
                if(strokeList.Count > 2)
                {
                    elapsed = strokeList.Last().StrokeEndTimestamp - strokeList.First().StrokeStartTimestamp;
                }

                DisplayPacket toDisplay = new DisplayPacket();
                toDisplay.ElapsedTime = elapsed;
                toDisplay.Split = split;
                toDisplay.TotalDistance = (int)TotalDistance;
                toDisplay.StrokePrMin = strokePrMin;

                Console.WriteLine($"StrokeList Length: {strokeList.Count}");
                Console.WriteLine($"Dist: {recievedPacket.Distance}\n" +
                    $"Revs: {recievedPacket.Revolutions}\n" +
                    $"Start: {recievedPacket.StrokeStartTimestamp}\n" +
                    $"Stop: {recievedPacket.StrokeEndTimestamp}");
                Console.WriteLine($"Duration: {elapsed.ToString(@"mm\:ss\:fff")}");
                Console.WriteLine($"Split: {((split < TimeSpan.Zero) ? "-" : "") + split.ToString(@"mm\:ss")}");
                Console.WriteLine("-----------------------------------------------------");

                await Clients.All.SendAsync("BroadcastPacket", $"{JsonSerializer.Serialize(toDisplay)}");
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
            TotalDistance = 0;
            IsRowing = false;
        }

        public async Task SelectUser(string message)
        {
            ActiveUser = message;
            Console.WriteLine($"Active user set to {message}");
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
