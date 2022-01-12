//SERVER

using WebSocketSharp;
using WebSocketSharp.Server;
using SocketServer;

bool keepApplicationRunning = true;
string serverAddr = "ws://localhost:8080";
WebSocketServer server = new WebSocketServer(serverAddr);
server.AddWebSocketService<SendData>("/SendData");
server.AddWebSocketService<RecieveCommands>("/Command");

bool TestBool = false;

Console.WriteLine($"Press F1 to start");
Console.WriteLine($"Press F2 to stop");
Console.WriteLine($"Press F9 to quit");

while (keepApplicationRunning)
{
    ConsoleKeyInfo key = Console.ReadKey(true);
    switch (key.Key)
    {
        case ConsoleKey.F1:
            StartServer();
            break;
        case ConsoleKey.F2:
            StopServer();
            break;
        case ConsoleKey.F9:
            Console.WriteLine("\n\nQuitting...\n\n");
            keepApplicationRunning = false;
            break;
        default:
            break;
    }
}

void StartServer()
{
    server.Start();
    Console.WriteLine($"Server is running on {serverAddr}");
}

void StopServer()
{
    server.Stop();
    Console.WriteLine($"Server stopped");
}