/*
 Name:		ArduinoRotator.ino
 Created:	1/12/2022 11:36:31 AM
 Author:	Håvard Risebrobakken
*/

#include <Servo.h>

Servo rotatorServo;

int readValue = 0;
int setValue = 93;

// the setup function runs once when you press reset or power the board
void setup() {
	Serial.begin(9600);
	rotatorServo.attach(9);
	Serial.println("Connected to serial");
}

// the loop function runs over and over again until power down or reset
void loop() {
	if (Serial.available() > 0) {
		int incomingValue = Serial.parseInt();
		if (incomingValue == 0 || incomingValue == '1') {

		}
		else {
			incomingValue = incomingValue + 3;
			if (incomingValue != setValue) {
				if (incomingValue < 0) {
					incomingValue = 0;
				}
				if (incomingValue > 180) {
					incomingValue = 180;
				}
				setValue = incomingValue;
				Serial.println("Setting servo value to ");
				Serial.println(setValue);
				Serial.println();
			}
		}
	}
	rotatorServo.write(setValue);
}
