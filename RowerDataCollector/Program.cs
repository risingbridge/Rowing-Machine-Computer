using System.Device.Gpio;
using System.Net.Sockets;
using System.Threading;
using RowerDataCollector.Classes;
using System.Text.Json;


bool keepApplicationRunning = true;

Console.WriteLine($"Press F1 to start");
Console.WriteLine($"Press F2 to stop");
Console.WriteLine($"Press F9 to quit");
bool isMainServiceStopped = true;
bool logData = false;
DateTime startTime = DateTime.Now;
DateTime endTime = DateTime.Now;

while (keepApplicationRunning)
{
    if (Console.KeyAvailable)
    {
        ConsoleKeyInfo key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.F1:
                startTime = DateTime.Now;
                Console.WriteLine($"\n\nStarting at {startTime}...\n\n");
                logData = true;
                Task.Run(() => LogSomeDataAsync(startTime));
                isMainServiceStopped = false;
                break;
            case ConsoleKey.F2:
                logData = false;
                endTime = DateTime.Now;
                Console.WriteLine($"\n\nStopping at {endTime}...\n\n");
                isMainServiceStopped = true;
                break;
            case ConsoleKey.F9:
                Console.WriteLine("\n\nQuitting...\n\n");
                keepApplicationRunning = false;
                break;
            default:
                break;
        }
    }
    DateTime nextTimeCheck = startTime.AddMinutes(1);
    //Do whatever you want after starting
    if (!isMainServiceStopped)
    {

    }
}

async Task LogSomeDataAsync(DateTime startTime)
{
    List<WorkoutDataPoint> workoutData = new List<WorkoutDataPoint>();
    Random rnd = new Random();
    Console.WriteLine("Logging:");
    int strokes = 0;
    int distance = 0;
    while (logData)
    {
        strokes += rnd.Next(0, 2);
        distance += rnd.Next(0, 4);
        WorkoutDataPoint work = new WorkoutDataPoint(DateTime.Now, distance, strokes, rnd.Next(76,200), rnd.Next(3,10), rnd.Next(0,1));
        workoutData.Add(work);
        if(distance % 10 == 0)
        {
            Console.WriteLine($"Current Distance: {distance}, list-size: {workoutData.Count}");
        }
        Thread.Sleep(100);
    }
    Console.WriteLine("Logging stopped");
    var jsonOutput = JsonSerializer.Serialize(workoutData);
    string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    await File.WriteAllTextAsync($"./{fileName}.json", jsonOutput);
    Console.WriteLine("Data logged!");
}