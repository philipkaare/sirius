using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using ExampleAccelGyroSensor.Sensor;
using MicroLiquidCrystal;

namespace Sirius
{
    public class Program
    {
        public static void Main()
        {
            var ledPort = new OutputPort(Pins.ONBOARD_LED, false);
            //backlight connected to D10
            // create the transfer provider
            // Initialize the library with the numbers of the interface pins
            var LcdProvidor = new GpioLcdTransferProvider(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D7);           
            //Debug.Print(LcdProvidor.FourBitMode.ToString());            
            var lcd = new Lcd(LcdProvidor);
            Thread.Sleep(40);
            lcd.Begin(16, 2);            

            lcd.Clear();
            lcd.SetCursorPosition(0, 0);
            lcd.Write("Sirius online! ");

            char[] spinner = new char[4]{'-','\\','|','/'};

            int spinnerIdx=0; 


            // Initialisiere den Sensor
            var mpu6050 = new MPU6050();

            // Objekt Anlegen für Gyro- und Beschleunigungssensor
            var sensorResult = mpu6050.GetSensorData();
            var filter = new ComplimentaryFilter();

            var lastTime = DateTime.Now;

            var m = new MotorController(3, 9);
            var m2 = new MotorController(6, 11);
            var pid = new PID4Life();

            

            while (true)
            {
                var now = DateTime.Now;
                var dt = now - lastTime;
                lastTime = now;
             
                // Daten abrufen
                sensorResult = mpu6050.GetSensorData();

                lcd.SetCursorPosition(2, 1);
                var angle = filter.GetAngle(sensorResult, (double)dt.Ticks / 1000);
                Debug.Print(sensorResult.GetPitchAngle().ToString() + "\t" + sensorResult.GetPitchVelocity().ToString() + "\t" + angle);
                
                var speed = pid.GetCorrection(angle,(double)dt.Ticks / 1000);
        
                spinnerIdx++;
                // set the cursor to column 0, line 1
              //  lcd.SetCursorPosition(0, 1);
                Thread.Sleep(40);
                // print the number of seconds since reset:
                //lcd.Write((Utility.GetMachineTime()).ToString());
                ledPort.Write(true);

                lcd.SetCursorPosition(0, 1);
                lcd.Write(spinner[spinnerIdx % 4].ToString());
                lcd.Write(angle.ToString());
                
                m.Speed = speed;
                m2.Speed = speed;
                //Thread.Sleep(1500);
            }

            //m.Dispose();
            //m2.Dispose();
             
        }

    }
}
