using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Sirius
{
    public class MotorController : IDisposable
    {
        private double _speed;
        PWM forwardpwm; 
        PWM reversepwm; 

        private Cpu.PWMChannel GetChannel(int DIO_pin)
        {
            switch (DIO_pin)
            {
                case 3:
                    return PWMChannels.PWM_PIN_D3;
                case 5:
                    return PWMChannels.PWM_PIN_D5;
                case 6:
                    return PWMChannels.PWM_PIN_D6;
                case 9:
                    return PWMChannels.PWM_PIN_D9;
                case 10:
                    return PWMChannels.PWM_PIN_D10;
                case 11:
                    return PWMChannels.PWM_PIN_D11;

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
            get { return _speed; }
            set
            {
                _speed = value;
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
