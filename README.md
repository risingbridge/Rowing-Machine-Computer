# Raspberry Pi-based rowing machine computer
The goal of this project is to replace the simple computer on our offbrand water-rower with a more advanced one.
The plan is to use hall effect sensors to sense magnets already placed on the rowers pully-system, and convert the sensor readings into human readable values displayed on a 7 inch Raspberry Pi Touchscreen. 

The plan for the GUI is to create a html/javascript webinterface that will run in a fullscreen webbrowser, and communicate with the backend over websocket/SignalR. I would also like to run a api on my server to store workout-sessions.

### Projects
#### ArduinoRotator
ArduinoRotator is an arduino-program that rotates a 360-servo in a similiar way to the pully on my rowing machine

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
