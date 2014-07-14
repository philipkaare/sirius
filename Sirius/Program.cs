﻿using System;
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
using Math = System.Math;

namespace Sirius
{
    public class Program
    {
        private static int _activeValue = 0;

        private static long _lastKeyInput;

        private static bool FailSafeEnabled = false;

        private static readonly InterruptPort Channel1 = new InterruptPort(Pins.GPIO_PIN_A1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
        private static readonly InterruptPort Channel2 = new InterruptPort(Pins.GPIO_PIN_A2, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            
        private static OutputPort _ledPort = new OutputPort(Pins.ONBOARD_LED, false);


        private static readonly MPU6050 Mpu6050 = new MPU6050();
        private static readonly ComplimentaryFilter Filter = new ComplimentaryFilter();
        private static readonly MotorController Motor1 = new MotorController(9, 3);
        private static readonly MotorController Motor2 = new MotorController(11, 6);
        private static readonly GpioLcdTransferProvider LcdProvider = new GpioLcdTransferProvider(Pins.GPIO_PIN_D8, 
            Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D7);

        private static readonly Lcd Lcd = new Lcd(LcdProvider);

        private static readonly PID4Life Pid = new PID4Life();

        private static long _channel1UpTimestamp = 0;
        private static double _channel1Value = 0;

        private static long _channel2UpTimestamp = 0;
        private static double _channel2Value = 0;

        private static void DisplayPidValues(PID4Life pid4Life, Lcd lcd,int activeValue)
        {
            lcd.Clear();
            lcd.SetCursorPosition(0,0);
            lcd.Write(StringUtility.Format("P{0:F1}I.{1:D3}D{2:F2}", pid4Life.K_p, (int)(pid4Life.K_i * 1000.0), pid4Life.K_d));
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
                    pid.K_p += up ? 0.1 : -0.1;
                    break;
                case 1:
                    pid.K_i += up ? 0.001 : -0.001;
                    break;
                case 2:
                    pid.K_d += up ? 0.01 : -0.01;
                    break;
            }
        }

        public static void Initialize()
        {
            
            Channel1.OnInterrupt += (data1, data2, time) =>
            {
                if (_channel1UpTimestamp == 0 && data2 == 1) //High edge
                {
                    _channel1UpTimestamp = time.Ticks;
                }
                else if (_channel1UpTimestamp != 0 && data2 == 0)
                {
                    _channel1Value = ((double)(time.Ticks - _channel1UpTimestamp - 14000)/8000);
                    _channel1UpTimestamp = 0;
                }
            };

            Channel2.OnInterrupt += (data1, data2, time) =>
            {
                if (_channel2UpTimestamp == 0 && data2 == 1) //High edge
                {
                    _channel2UpTimestamp = time.Ticks;
                }
                else if (_channel2UpTimestamp != 0 && data2 == 0)
                {
                    _channel2Value = ((double)(time.Ticks - _channel2UpTimestamp - 14000) / 8000);
                    _channel2UpTimestamp = 0;
                }
            };
        }

        public static void InitLcd()
        {
            Lcd.Begin(16, 2);

            Lcd.Clear();
            Lcd.SetCursorPosition(0, 0);
            Lcd.Write("Sirius online! ");
            Lcd.SetCursorPosition(0, 1);
        }

        public static void ReadRadioKey()
        {
            if (_channel1Value < -0.3 && Utility.GetMachineTime().Ticks - _lastKeyInput > 5 * 1000000)
            {
                if (_activeValue < 2)
                    _activeValue++;
            }
            else if (_channel1Value > 0.3 && Utility.GetMachineTime().Ticks - _lastKeyInput > 5 * 1000000)
            {
                if (_activeValue > 0)
                    _activeValue--;
            }

            else if (_channel2Value < -0.3 && Utility.GetMachineTime().Ticks - _lastKeyInput > 5 * 1000000)
            {
                StepActive(Pid, _activeValue, false);
            }
            else if (_channel2Value > 0.3 && Utility.GetMachineTime().Ticks - _lastKeyInput > 5*1000000)
            {
                StepActive(Pid, _activeValue, true);
            }
            else return;

            _lastKeyInput = Utility.GetMachineTime().Ticks;
            //DisplayPidValues(Pid, Lcd, _activeValue);
            Pid.Reset();
            FailSafeEnabled = false;
        }

        public static void Main()
        {   
            var lastTime = DateTime.Now;
            InitLcd();
            Initialize();

            DisplayPidValues(Pid, Lcd, _activeValue);

            while (true)
            {
                var now = DateTime.Now;
                var dt = now - lastTime;
                lastTime = now;

                ReadRadioKey();

                var sensorResult = Mpu6050.GetSensorData();
                var angle = Filter.GetAngle(sensorResult, (double)dt.Ticks / 1000);
                var speed = Pid.GetCorrection(angle,(double)dt.Ticks / 1000);

                if (Math.Abs(angle) > 0.3) FailSafeEnabled = true;

                if (FailSafeEnabled)
                {
                    speed = 0;
                    Debug.Print("Failsafe engaged");
                }
                    

                Motor1.Speed = speed;
                Motor2.Speed = speed;
            }
        }

    }
}
