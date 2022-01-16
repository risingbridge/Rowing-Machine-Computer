using Rower_Sensor;
using System.Device.Gpio;
using System.Diagnostics;
using System.Text.Json;
Console.WriteLine("Starting.");

//Load settings from settings.json
Settings? loadedSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("./settings.json"));
Settings settings = new Settings(17,27,22,1,1.33f, 10); //PinA, PinB, PinC, MagnetsPrRev, DistPrRev, SleepTime
if(loadedSettings != null)
{
    settings = loadedSettings;
    Console.WriteLine(settings);
}

//Variables for direction and speed-detection
bool hallAlphaTriggered = false;
bool hallBravoTriggered = false;
bool hallCharlieTriggered = false;
DateTime timestampAlpha = DateTime.UtcNow;
DateTime timestampBravo = DateTime.UtcNow;
DateTime timestampCharlie = DateTime.UtcNow;

bool turningClockwise = false;
int pulseCount = 0;

//Creates controller and sets input-pins
using GpioController controller = new GpioController();
controller.OpenPin(settings.SensorAlpha, PinMode.Input);
controller.OpenPin(settings.SensorBravo, PinMode.Input);
controller.OpenPin(settings.SensorCharlie, PinMode.Input);

//Registating sensor interrupts
controller.RegisterCallbackForPinValueChangedEvent(settings.SensorAlpha, PinEventTypes.Falling, HallEffectAlphaDetection);
controller.RegisterCallbackForPinValueChangedEvent(settings.SensorBravo, PinEventTypes.Falling, HallEffectBravoDetection);
controller.RegisterCallbackForPinValueChangedEvent(settings.SensorCharlie, PinEventTypes.Falling, HallEffectCharlieDetection);

//Variables to keep the application running
bool keepApplicationRunning = true;
bool keepSendingData = true;

//Variables for time detection
DateTime lastPulseTimestamp = DateTime.UtcNow;

//Other variables
MachineState state = MachineState.Still;

//Sets up the catch for ctrl+c
Console.CancelKeyPress += delegate {
    ExitApplication();
};

//Main loop
Console.WriteLine("Ready.");
while (keepApplicationRunning)
{
    if(state == MachineState.Still)
    {
        if(pulseCount > 0 && turningClockwise)
        {
            state = MachineState.Stroke;
            Console.WriteLine("Changing state to stroke");
        }
    }
    if(state == MachineState.Stroke)
    {
        if (!turningClockwise)
        {
            Console.WriteLine($"Stroke complete - {pulseCount / settings.MagnetsPrRev} revolutions.");
            int revs = pulseCount / settings.MagnetsPrRev;
            pulseCount = 0;
            //Send stroke-info
            float strokeDist = settings.MeterPrRev * (revs);
            Console.WriteLine($"Stroke distance: {strokeDist}m / (Display {Math.Floor(strokeDist)}m)");
            state = MachineState.StrokeComplete;
        }
    }
    if(state == MachineState.StrokeComplete)
    {
        Console.WriteLine("Stroke complete, machine still");
        state = MachineState.Still;
    }

}


//Functions

//Sensor interrupts
void HallEffectAlphaDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallAlphaTriggered)
    {
        //Console.WriteLine("Alpha triggered");
        timestampAlpha = DateTime.UtcNow;
        hallAlphaTriggered = true;
    }
}

void HallEffectBravoDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallBravoTriggered)
    {
        timestampBravo = DateTime.UtcNow;
        hallBravoTriggered = true;

        TimeSpan spanAB = timestampAlpha - timestampBravo;
        TimeSpan spanBC = timestampCharlie - timestampBravo;
        if (spanAB.Ticks < 0)
        {
            spanAB *= -1;
        }
        if (spanBC.Ticks < 0)
        {
            spanBC *= -1;
        }
        //Console.WriteLine($"Span AB: {spanAB.Ticks}\tSpanBC: {spanBC.Ticks}");
        //Console.WriteLine($"Timestamp A: {timestampAlpha.Ticks}\n" +
        //    $"Timestamp B: {timestampBravo.Ticks}\n" +
        //    $"Timestamp C: {timestampCharlie.Ticks}");
        if (spanAB > spanBC)
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
        if (turningClockwise)
        {
            pulseCount++;
            //Console.WriteLine($"Pulse Count is now {pulseCount}");
        }
    }
    //Console.WriteLine($"Bravo triggered. Clockwise: {turningClockwise}");
}

void HallEffectCharlieDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallCharlieTriggered)
    {
        //Console.WriteLine("Charlie triggered");
        timestampCharlie = DateTime.UtcNow;
        hallCharlieTriggered = true;
    }
}

//Function to exit application. Cleans up the gpio-pins
void ExitApplication()
{
    keepApplicationRunning = false;
    keepSendingData = false;
    Console.WriteLine("Cleaning up");
    controller.ClosePin(settings.SensorAlpha);
    controller.ClosePin(settings.SensorBravo);
    controller.ClosePin(settings.SensorCharlie);
    controller.Dispose();
    Console.WriteLine("Quitting");
    Environment.Exit(0);
}

enum MachineState
{
    Still,
    Stroke,
    StrokeComplete
}