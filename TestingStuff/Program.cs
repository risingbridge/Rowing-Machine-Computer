bool keepRunning = true;

Console.CancelKeyPress += delegate {
    ExitApplication();
};

while (keepRunning)
{
    Console.WriteLine("Blablablabla");
}

void ExitApplication()
{
    keepRunning = false;
    Console.WriteLine("Stopping");
    Environment.Exit(0);
}