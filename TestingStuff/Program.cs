using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using RowerClassLibrary;

bool keepRunning = true;
bool connected = false;
List<string> messages = new List<string>();
string SignalRUrl = "http://127.0.0.1:5142/row";

await using var connection = new HubConnectionBuilder().WithUrl(SignalRUrl).Build();


RefreshConsole(null);
while (keepRunning)
{
    ConsoleKeyInfo key = Console.ReadKey();
    //F1 to connect/Disconnect
    //Spacebar to send stroke
    //ESC to quit
    switch (key.Key)
    {
        case ConsoleKey.F1:
            if (!connected)
            {
                await connection.StartAsync();
                await connection.SendAsync("StartRowing", "Start");
            }
            else
            {
                await connection.DisposeAsync();
            }
            connected = !connected;
            RefreshConsole(null);
            break;
        case ConsoleKey.Spacebar:
            if (connected)
            {
                await SendRandomStroke();
            }
            break;
        case ConsoleKey.Escape:
            connected = false;
            await connection.DisposeAsync();
            keepRunning = false;
            break;
    }
}

async Task SendRandomStroke()
{
    Random rnd = new Random();
    int revs = rnd.Next(3,5);
    float dist = rnd.Next(3, 5) + (float)rnd.NextDouble();
    DateTime startTime = DateTime.UtcNow;
    DateTime endTime = startTime.AddSeconds((double)rnd.Next(4,7));
    StrokePacket packet = new StrokePacket(revs, dist, startTime, endTime);
    try
    {
        await connection.SendAsync("ProcessStrokePacket", JsonSerializer.Serialize(packet));
    }catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.ReadKey();
    }
    string printString = $"Revs: {revs}, Dist: {dist}, Start: {startTime}, End: {endTime}";
    RefreshConsole(printString);
}


void RefreshConsole(string? message)
{
    Console.Clear();
    PrintMenu();
    PrintContent(message);
}

void PrintMenu()
{
    Console.WriteLine($"--------------------------------------------MENU--------------------------------------------");
    if (connected)
    {
        Console.Write("Press F1 to disconnect\t");
    }
    else
    {
        Console.Write("Press F1 to connect\t");
    }
    Console.Write("Press spacebar to send stroke\t");
    Console.Write("Press ESC to quit\t");
    Console.WriteLine($"\n--------------------------------------------------------------------------------------------\n");
}

void PrintContent(string? message)
{
    Console.WriteLine($"\n--------------------------------------------INFO--------------------------------------------");
    Console.Write($"Connected: {connected}, host: {SignalRUrl}");
    Console.WriteLine($"\n--------------------------------------------------------------------------------------------\n");
    if(message != null)
    {
        messages.Add(message);
    }
    int messageCounter = 0;
    for (int i = messages.Count - 1; i >= 0; i--)
    {
        Console.WriteLine($"{messages[i]}");
        messageCounter++;
        if(messageCounter >= 5)
        {
            break;
        }
    }
}