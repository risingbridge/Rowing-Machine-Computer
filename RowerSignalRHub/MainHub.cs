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
        public static List<WorkoutPacket> workoutList = new List<WorkoutPacket>();
        public static string ActiveUser = "Gjest";
        public static float TotalDistance = 0;
        public static WorkoutLog log = new WorkoutLog();
        public static DateTime startTime;
        public static DateTime endTime;
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

                TimeSpan averageSplit = split;
                int strokePrMin = 0;
                //if i have more the 5 strokes, calculate split based on last 5 and stroke pr minutes
                if(strokeList.Count >= 5)
                {
                    TimeSpan lastStroke = strokeList.Last().TimeSinceStart;
                    TimeSpan firstStroke = strokeList[strokeList.Count - 5].TimeSinceStart;
                    TimeSpan longStrokeSpan = lastStroke - firstStroke;
                    Console.WriteLine(longStrokeSpan);
                    double oneStroke = 4 / longStrokeSpan.TotalMilliseconds;
                    strokePrMin = (int)Math.Floor(oneStroke * 60000);
                }
                TimeSpan elapsed = DateTime.UtcNow - startTime;
                int effect = (int)(2.8 / Math.Pow((averageSplit.TotalSeconds / 500), 3));

                DisplayPacket toDisplay = new DisplayPacket();
                toDisplay.ElapsedTime = elapsed;
                toDisplay.Split = split;
                toDisplay.TotalDistance = (int)TotalDistance;
                toDisplay.StrokePrMin = strokePrMin;
                toDisplay.Effect = effect;

                Console.WriteLine($"StrokeList Length: {strokeList.Count}");
                Console.WriteLine($"Dist: {recievedPacket.Distance}\n" +
                    $"TimeSinceStart: {recievedPacket.TimeSinceStart}\n" +
                    $"SPM: {strokePrMin}\n" +
                    $"Revs: {recievedPacket.Revolutions}\n" +
                    $"Start: {recievedPacket.StrokeStartTimestamp}\n" +
                    $"Stop: {recievedPacket.StrokeEndTimestamp}");
                Console.WriteLine($"Duration: {elapsed.ToString(@"mm\:ss\:fff")}");
                Console.WriteLine($"Split: {((split < TimeSpan.Zero) ? "-" : "") + split.ToString(@"mm\:ss")}");
                Console.WriteLine("-----------------------------------------------------");

                await Clients.All.SendAsync("BroadcastPacket", $"{JsonSerializer.Serialize(toDisplay)}");
                WorkoutPacket workoutPacket = new WorkoutPacket()
                {
                    Stroke = recievedPacket,
                    TotalTime = DateTime.Now - startTime,
                    TotalDistance = TotalDistance,
                    StrokesPrMin = strokePrMin,
                    Split = split,
                    Pulse = 0,
                    Effect = 0,
                    Calories = 0
                };
                workoutList.Add(workoutPacket);
            }
        }

        public async Task StartRowing(string message)
        {
            if (!IsRowing)
            {
                IsRowing = true;
                Console.WriteLine($"Recieved start!");
                log = new WorkoutLog();
                log.User = ActiveUser;
                startTime = DateTime.UtcNow;
                log.StartTime = startTime;
            }
        }

        public async Task StopRowing(string message)
        {
            IsRowing = false;
            endTime = DateTime.UtcNow;
            log.EndTime = endTime;
        }

        public async Task ResetRowing(string message)
        {
            if(IsRowing)
            {
                endTime = DateTime.UtcNow;
                log.EndTime = endTime;
            }
            //Log workout
            log.WorkoutList = new List<WorkoutPacket>(workoutList);
            log.TotalDistance = TotalDistance;
            string logFileName = $"./{ActiveUser}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
            File.WriteAllText(logFileName, JsonSerializer.Serialize(log));
            Console.WriteLine($"Log saved to {logFileName}");


            strokeList.Clear();
            workoutList.Clear();
            TotalDistance = 0;
            IsRowing = false;
        }

        public async Task SelectUser(string message)
        {
            ActiveUser = message;
            Console.WriteLine($"Active user set to {message}");
            strokeList.Clear();
            workoutList.Clear();
            TotalDistance = 0;
            IsRowing = false;
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
