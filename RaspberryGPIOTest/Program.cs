using RaspberryGPIOTest;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

Console.WriteLine("Revolution counter!");


int hallEffectSensorAlpha = 17;
using var controller = new GpioController();
int magnetsPrRev = 1;
int revCount = 0;
int pulseCount = 0;

if (File.Exists("./config.json"))
{
    string settingsJsonString = File.ReadAllText("./config.json");
    Settings? settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(settingsJsonString);
    if(settings != null)
    {
        magnetsPrRev = settings.Magnets;
        hallEffectSensorAlpha = settings.HallPinA;
        Console.WriteLine($"Loaded settings:\n {settings.ToString()}");
    }
}

//Set settings from settings file if exists

controller.OpenPin(hallEffectSensorAlpha, PinMode.Input);
controller.RegisterCallbackForPinValueChangedEvent(hallEffectSensorAlpha, PinEventTypes.Falling, HallEffectDetection);



Console.WriteLine($"Press F1 to start");
Console.WriteLine($"Press F2 to stop");
Console.WriteLine($"Press F9 to quit");
bool keepApplicationRunning = true;
bool printStatus = false;

while (keepApplicationRunning)
{
    ConsoleKeyInfo key = Console.ReadKey(true);
    switch (key.Key)
    {
        case ConsoleKey.F1:
            Console.WriteLine("Starting");
            printStatus = true;
            pulseCount = 0;
            Task.Run(() => PrintHallStatus());
            break;
        case ConsoleKey.F2:
            Console.WriteLine("Stopping");
            printStatus = false;
            break;
        case ConsoleKey.F9:
            Console.WriteLine("\n\nQuitting...\n\n");
            controller.UnregisterCallbackForPinValueChangedEvent(hallEffectSensorAlpha, HallEffectDetection);
            keepApplicationRunning = false;
            break;
        default:
            break;
    }
}

void HallEffectDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    pulseCount++;
}

async Task PrintHallStatus()
{
    Stopwatch sw = new Stopwatch();
    sw.Start();
    while(printStatus)
    {
        if(pulseCount >= magnetsPrRev)
        {
            long time = sw.ElapsedMilliseconds;
            long rpm = (60000 / time);
            sw.Restart();
            pulseCount = 0;
            revCount++;
            Console.WriteLine($"RPM: {rpm}");
        }
        //Console.WriteLine($"Counted {revCount} revolutions");
    }
    Console.WriteLine("Print taske at end");
}

controller.ClosePin(hallEffectSensorAlpha);
Console.WriteLine($"{hallEffectSensorAlpha} closed. Quitting.");