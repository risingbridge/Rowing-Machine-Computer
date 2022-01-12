# Raspberry Pi-based rowing machine computer
The goal of this project is to replace the simple computer on our offbrand water-rower with a more advanced one.
The plan is to use hall effect sensors to sense magnets already placed on the rowers pully-system, and convert the sensor readings into human readable values displayed on a 7 inch Raspberry Pi Touchscreen. 

The plan for the GUI is to create a html/javascript webinterface that will run in a fullscreen webbrowser, and communicate with the backend over websocket. I would also like to run a api on my server to store workout-sessions.