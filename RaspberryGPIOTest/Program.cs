using System;
using System.Device.Gpio;
using System.Threading;

Console.WriteLine("Bloinging. Press Ctrl+C to end!");

int hallEffectPin = 17;
using var controller = new GpioController();
int magnetsPrRev = 1;
int revCount = 0;
int pulseCount = 0;
bool BeamBroke = false;

controller.OpenPin(hallEffectPin, PinMode.Input);
controller.RegisterCallbackForPinValueChangedEvent(hallEffectPin, PinEventTypes.Falling, HallEffectDetection);



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
            controller.UnregisterCallbackForPinValueChangedEvent(hallEffectPin, HallEffectDetection);
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
    while(printStatus)
    {
        if(pulseCount >= magnetsPrRev)
        {
            pulseCount = 0;
            revCount++;
        }
        Console.WriteLine($"Counted {revCount} revolutions");
    }
    Console.WriteLine("Print taske at end");
}

controller.ClosePin(hallEffectPin);
Console.WriteLine($"{hallEffectPin} closed. Quitting.");