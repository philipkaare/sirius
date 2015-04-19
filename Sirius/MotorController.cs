using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Sirius
{
    public class MotorController : IDisposable
    {
        private double _speed;
        readonly PWM _forwardpwm;
        readonly PWM _reversepwm; 

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

        public MotorController(int forwardPin, int reversePin)
        {
            _forwardpwm = new PWM(GetChannel(forwardPin),100,0,false);
            _reversepwm= new PWM(GetChannel(reversePin),100, 0, false);

            _forwardpwm.Start();
            _reversepwm.Start();       
        }

        public void Dispose()
        {
            _forwardpwm.Stop();
            _reversepwm.Stop();
        }

        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                if (value > 0)
                {
                    if (value > 1)
                        value = 1;
                    _forwardpwm.DutyCycle = value;
                    _reversepwm.DutyCycle = 0;
                }
                else
                {
                    if (value < -1)
                        value = -1;
                    _forwardpwm.DutyCycle = 0;
                    _reversepwm.DutyCycle = -value;
                }

            }
        }

    }
}
