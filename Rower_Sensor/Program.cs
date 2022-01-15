using Rower_Sensor;
using System.Device.Gpio;
using System.Diagnostics;
using System.Text.Json;

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

//Main loop

while (keepApplicationRunning)
{
    if(state == MachineState.Still)
    {
        if(pulseCount > 0 && turningClockwise)
        {
            state = MachineState.Stroke;
        }
    }
    if(state == MachineState.Stroke)
    {
        if (!turningClockwise)
        {
            state = MachineState.StrokeComplete;
        }
    }
    if(state == MachineState.StrokeComplete)
    {
        state = MachineState.Still;
        float strokeDist = settings.MeterPrRev * (pulseCount / settings.MagnetsPrRev);
        //Send stroke-info
    }

}

controller.ClosePin(settings.SensorAlpha);
controller.ClosePin(settings.SensorBravo);
controller.ClosePin(settings.SensorCharlie);


//Functions

//Sensor interrupts
void HallEffectAlphaDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallAlphaTriggered)
    {
        timestampAlpha = DateTime.UtcNow;
        hallAlphaTriggered = true;
    }
}

void HallEffectBravoDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallBravoTriggered)
    {
        timestampAlpha = DateTime.UtcNow;
        hallAlphaTriggered = true;

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
        if (spanAB < spanBC)
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
        pulseCount++;
    }
}

void HallEffectCharlieDetection(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
{
    if (!hallCharlieTriggered)
    {
        timestampAlpha = DateTime.UtcNow;
        hallAlphaTriggered = true;
    }
}

enum MachineState
{
    Still,
    Stroke,
    StrokeComplete
}