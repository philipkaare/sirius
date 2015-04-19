using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class PID4Life
    {
        double _integral=0;
        double _previousError=0;

        public double K_p, K_i, K_d, target;

        public void Reset()
        {
            _integral = 0;
            _previousError = 0;
        }

        public PID4Life(double p, double i, double d, double targetValue)
        {
            K_p = p;
            K_i = i;
            K_d = d;
            target = targetValue;

        }

        public double GetCorrection(double inputValue, double dt)
        {
            var error = inputValue-target;
            _integral = _integral + error*dt;
            var derivative =(error - _previousError)/dt;
            _previousError = error;
            var res = (K_p * error + K_i * _integral + K_d * derivative)/10.0;

            if (res > 1) res = 1;
            if (res < -1) res = -1;

            return res;
        }
    }
}
