using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NetMf.CommonExtensions;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using ExampleAccelGyroSensor.Sensor;
using MicroLiquidCrystal;

namespace Sirius
{
    public class Program
    {
        private static int activeValue = 0;

        private static long lastKeyInput;

        private static void DisplayPIDValues(PID4Life pid, Lcd lcd,int activeValue)
        {
            lcd.Clear();
            lcd.SetCursorPosition(0,0);
            lcd.Write(StringUtility.Format("P.{0:D2}I.{1:D3}D{2:F2}", (int)(pid.K_p * 100.0), (int)(pid.K_i * 1000.0), pid.K_d));
            switch (activeValue)
            {
                case 0:
                    lcd.SetCursorPosition(2,1);
                    break;
                case 1:
                    lcd.SetCursorPosition(7,1);
                    break;
                case 2:
                    lcd.SetCursorPosition(13,1);
                    break;
                default:
                    lcd.Write("World exploded!!!!11!");
                    break;
            }
            lcd.Write("X");
        }

        private static void StepActive(PID4Life pid, int activeValue, bool up)
        {
            switch (activeValue)
            {
                case 0:
                    pid.K_p += up ? 0.01 : -0.01;
                    break;
                case 1:
                    pid.K_i += up ? 0.001 : -0.001;
                    break;
                case 2:
                    pid.K_d += up ? 0.01 : -0.01;
                    break;
            }
        }

        public static void Main()
        {
            long channel1UpTimestamp = 0;
            double channel1Value = 0;

            long channel2UpTimestamp = 0;
            double channel2Value = 0;

            var channel1 = new InterruptPort(Pins.GPIO_PIN_A1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            var channel2 = new InterruptPort(Pins.GPIO_PIN_A2, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            
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
            lcd.SetCursorPosition(0, 1);
            var pid = new PID4Life();
            
            
            char[] spinner = new char[4]{'-','\\','|','/'};

            int spinnerIdx=0; 

            // Initialisiere den Sensor
            var mpu6050 = new MPU6050();

            // Objekt Anlegen für Gyro- und Beschleunigungssensor
            var sensorResult = mpu6050.GetSensorData();
            var filter = new ComplimentaryFilter();

            var lastTime = DateTime.Now;

            var m = new MotorController(9, 3);
            var m2 = new MotorController(11, 6);

            channel1.OnInterrupt += (data1, data2, time) =>
            {
                if (channel1UpTimestamp == 0 && data2 == 1) //High edge
                {
                    channel1UpTimestamp = time.Ticks;
                }
                else if (channel1UpTimestamp != 0 && data2 == 0)
                {
                    channel1Value = ((double)(time.Ticks - channel1UpTimestamp - 14000)/8000);
                    channel1UpTimestamp = 0;
                }
            };

            channel2.OnInterrupt += (data1, data2, time) =>
            {
                if (channel2UpTimestamp == 0 && data2 == 1) //High edge
                {
                    channel2UpTimestamp = time.Ticks;
                }
                else if (channel2UpTimestamp != 0 && data2 == 0)
                {
                    channel2Value = ((double)(time.Ticks - channel2UpTimestamp - 14000) / 8000);
                    channel2UpTimestamp = 0;
                }
            };

            DisplayPIDValues(pid, lcd, activeValue);
            while (true)
            {
                var now = DateTime.Now;
                var dt = now - lastTime;
                lastTime = now;

                if (channel1Value < -0.3 && Utility.GetMachineTime().Ticks - lastKeyInput > 5*1000000)
                {
                    if (activeValue < 2)
                        activeValue++;
                    lastKeyInput = Utility.GetMachineTime().Ticks;
                    DisplayPIDValues(pid, lcd, activeValue);
                }
                if (channel1Value > 0.3 && Utility.GetMachineTime().Ticks - lastKeyInput > 5 * 1000000)
                {
                    if (activeValue > 0)
                        activeValue--;
                    lastKeyInput = Utility.GetMachineTime().Ticks;
                    DisplayPIDValues(pid, lcd, activeValue);
                }

                if (channel2Value < -0.3 && Utility.GetMachineTime().Ticks - lastKeyInput > 5 * 1000000)
                {
                    lastKeyInput = Utility.GetMachineTime().Ticks;
                    StepActive(pid, activeValue, false);
                    DisplayPIDValues(pid, lcd, activeValue);
                }
                if (channel2Value > 0.3 && Utility.GetMachineTime().Ticks - lastKeyInput > 5 * 1000000)
                {
                    lastKeyInput = Utility.GetMachineTime().Ticks;
                    StepActive(pid, activeValue, true);
                    DisplayPIDValues(pid, lcd, activeValue);
                }

                Debug.Print("Channel 1: " + (channel1Value).ToString() + "Channel 2: " + (channel2Value).ToString());
                // Daten abrufen
                sensorResult = mpu6050.GetSensorData();

                //lcd.SetCursorPosition(2, 1);
                var angle = filter.GetAngle(sensorResult, (double)dt.Ticks / 1000);
               // Debug.Print(sensorResult.GetPitchAngle().ToString() + "\t" + sensorResult.GetPitchVelocity().ToString() + "\t" + angle);
                
                var speed = pid.GetCorrection(angle,(double)dt.Ticks / 1000);
        
                spinnerIdx++;
                // set the cursor to column 0, line 1
                //  lcd.SetCursorPosition(0, 1);
                // print the number of seconds since reset:
                //lcd.Write((Utility.GetMachineTime()).ToString());

                //lcd.SetCursorPosition(0, 1);
                //lcd.Write(spinner[spinnerIdx % 4].ToString());
                //lcd.Write(angle.ToString());

                m.Speed = speed;
                m2.Speed = speed;
                //Thread.Sleep(1500);
            }

            //m.Dispose();
            //m2.Dispose();
             
        }

    }
}
