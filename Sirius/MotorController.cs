using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Sirius
{
    public class MotorController : IDisposable
    {
        PWM forwardpwm; 
        PWM reversepwm; 

        private Cpu.PWMChannel GetChannel(int DIO_pin)
        {
            switch (DIO_pin)
            {
                case 3:
                    return Cpu.PWMChannel.PWM_4;
                case 5:
                    return Cpu.PWMChannel.PWM_0;
                case 6:
                    return Cpu.PWMChannel.PWM_1;
                case 9:
                    return Cpu.PWMChannel.PWM_2;
                case 10:
                    return Cpu.PWMChannel.PWM_3;
                case 11:
                    return Cpu.PWMChannel.PWM_5;

                default: return Cpu.PWMChannel.PWM_NONE;
            }
        }

        public MotorController(int forward_pin, int reverse_pin)
        {
            forwardpwm = new PWM(GetChannel(forward_pin),100,0,false);
            reversepwm= new PWM(GetChannel(reverse_pin),100, 0, false);

            forwardpwm.Start();
            reversepwm.Start();       
        }

        public void Dispose()
        {
            forwardpwm.Stop();
            reversepwm.Stop();
        }

        public double Speed
        {
            get { return Speed; }
            set
            {
                if (value > 0)
                {
                    forwardpwm.DutyCycle = value;
                    reversepwm.DutyCycle = 0;
                }
                else
                {
                    forwardpwm.DutyCycle = 0;
                    reversepwm.DutyCycle = -value;
                }

            }
        }

    }
}
