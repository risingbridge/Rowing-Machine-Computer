/*
 Name:		ArduinoRotator.ino
 Created:	1/12/2022 11:36:31 AM
 Author:	Håvard Risebrobakken
*/

#include <Servo.h>

Servo rotatorServo;

int readValue = 0;
int setValue = 93;
int strokeTime = 3;
int recoveryTime = 7;
int strokeSpeed = 80;
int recoverySpeed = 94;
int servoOffset = 3;

int strokeCount = 0;

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
			strokeCount = 0;
		}
		else {
			strokeCount = incomingValue;
		}
	}
	while (strokeCount > 0) {
		DoStroke();
		strokeCount--;
	}
}

void DoStroke() {
	rotatorServo.write(strokeSpeed + servoOffset);
	delay(strokeTime * 1000);
	rotatorServo.write(90 + servoOffset);
	delay(50);
	rotatorServo.write(recoverySpeed + servoOffset);
	delay(recoveryTime * 1000);
	rotatorServo.write(90 + servoOffset);
	delay(50);
}