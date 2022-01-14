using RaspberryGPIOTest;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

Console.WriteLine("Revolution counter!");


int hallEffectSensorAlpha = 17;
int hallEffectSensorBravo = 27;
int hallEffectSensorCharlie = 22;

bool hallAlphaTriggered = false;
bool hallBravoTriggered = false;
bool hallCharlieTriggered = false;
string sensorString = string.Empty;
DateTime timestampAlpha = DateTime.UtcNow;
DateTime timestampBravo = DateTime.UtcNow;
DateTime timestampCharlie = DateTime.UtcNow;

bool turningClockwise = false;

using var controller = new GpioController();
int magnetsPrRev = 1;
int revCount = 0;
int pulseCount = 0;

if (File.Exists("./config.json"))
{
    string settingsJsonString = File.ReadAllText("./config.json");
    Settings? settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(settingsJsonString);
    //Set settings from settings file if exists
    if(settings != null)
    {
        magnetsPrRev = settings.Magnets;
        hallEffectSensorAlpha = settings.HallPinA;
        hallEffectSensorBravo = settings.HallPinB;
        hallEffectSensorCharlie = settings.HallPinC;
        Console.WriteLine($"Loaded settings:\n {settings.ToString()}");
    }
}


controller.OpenPin(hallEffectSensorAlpha, PinMode.Input);
controller.OpenPin(hallEffectSensorBravo, PinMode.Input);
controller.OpenPin(hallEffectSensorCharlie, PinMode.Input);
controller.RegisterCallbackForPinValueChangedEvent(hallEffectSensorAlpha, PinEventTypes.Falling, HallEffectAlphaDetection);
controller.RegisterCallbackForPinValueChangedEvent(hallEffectSensorBravo, PinEventTypes.Falling, HallEffectBravoDetection);
controller.RegisterCallbackForPinValueChangedEvent(hallEffectSensorCharlie, PinEventTypes.Falling, HallEffectCharlieDetection);



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
            controller.UnregisterCallbackForPinValueChangedEvent(hallEffectSensorAlpha, HallEffectAlphaDetection);
            controller.UnregisterCallbackForPinValueChangedEvent(hallEffectSensorBravo, HallEffectBravoDetection);
            controller.UnregisterCallbackForPinValueChangedEvent(hallEffectSensorCharlie, HallEffectCharlieDetection);
            keepApplicationRunning = false;
            break;
        default:
            break;
    }
}

void HallEffectAlphaDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallAlphaTriggered)
    {
        sensorString += "a";
        timestampAlpha = DateTime.UtcNow;
    }
    hallAlphaTriggered = true;
    Console.WriteLine("Alpha Detected");
}

void HallEffectBravoDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    //Clockwise
    //A - B - C
    //B - C - A
    //C - A - B
    //Anti
    //C - B - A
    //B - A - C
    //A - C - B
    if (!hallBravoTriggered)
    {
        sensorString += "b";
        timestampBravo = DateTime.UtcNow;
    }
    Console.WriteLine("Bravo Detected");
    hallBravoTriggered = true;
    pulseCount++;
    sensorString = String.Empty;
    TimeSpan spanAB = timestampAlpha - timestampBravo;
    TimeSpan spanBC = timestampCharlie - timestampBravo;
    Console.WriteLine($"AB: {spanAB.Ticks}\tBC: {spanBC.Ticks}");
    if(spanAB.Ticks < 0)
    {
        spanAB *= -1;
    }
    if(spanBC.Ticks < 0)
    {
        spanBC *= -1;
    }
    if(spanAB < spanBC)
    {
        turningClockwise = true;
        hallAlphaTriggered = false;
        hallBravoTriggered = false;
        hallCharlieTriggered = false;
    }
    else
    {
        turningClockwise = false;
        hallAlphaTriggered = false;
        hallBravoTriggered = false;
        hallCharlieTriggered = false;
    }
    //if(sensorString == "abc" || sensorString == "bca" || sensorString == "cab" || sensorString == "ab")
    //{
    //    sensorString = string.Empty;
    //    hallAlphaTriggered = false;
    //    hallBravoTriggered = false;
    //    hallCharlieTriggered = false;
    //    turningClockwise = true;
    //}
    //else
    //{
    //    sensorString = string.Empty;
    //    turningClockwise = false;
    //}
}

void HallEffectCharlieDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallCharlieTriggered)
    {
        sensorString += "c";
        timestampCharlie = DateTime.UtcNow;
    }
    Console.WriteLine("Charlie Detected");
    hallCharlieTriggered = true;
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
            Console.WriteLine($"RPM: {rpm}, Clockwise: {turningClockwise}");
        }
        //Console.WriteLine($"Counted {revCount} revolutions");
    }
    Console.WriteLine("Print taske at end");
}

controller.ClosePin(hallEffectSensorAlpha);
Console.WriteLine($"{hallEffectSensorAlpha} closed. Quitting.");