# Raspberry Pi-based rowing machine computer
The goal of this project is to replace the simple computer on our offbrand water-rower with a more advanced one.
Using hall effect sensors I detect the speed and direction of rotation on the pully connected to the rower handle. This will only work on a rower where the pully goes in both directions, clockwise for the drive and anticlockwise for the recovery. The code will need modifications if the sensors is to be mounted on the flywheel on a air based rower.

The plan for the GUI is to create a html/javascript webinterface that will run in a fullscreen webbrowser, and communicate with the backend over websocket/SignalR. I would also like to run a api on my server to store workout-sessions.

### Calculations
Instead of calculating energy added to the system, I have chosen to approximate the distance rowed for each stroke, based on readings from the existing computer on the rower. The most important is that it is consistant, so it can be used to track progress.

### Projects
#### ArduinoRotator
ArduinoRotator is an arduino-program that rotates a 360-servo in a similiar way to the pully on my rowing machine. Was used for early prototyping and testing.

#### RaspberryGPIOTest
A project with test-code for Dotnet GPIO

#### RowerClassLibrary
Class library for classes used in multiple projects

#### RowerSignalRHub
Signal R-hub with the purpose of recieving information from Rower_Sensor, do the calculations, and sends it out to the web interface.

#### Rower_Sensor
Does the sensor logic. 

#### TestingStuff
Another project with test-code. Simulates Rower_Sensor to allow testing without having to row.

#### WebInterface
The html/javascript interface that will be displayed on the screen.
