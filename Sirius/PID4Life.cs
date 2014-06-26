using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class PID4Life
    {
        double _integral=0;
        double _previous_error=0;

        double K_p=0.2, K_i=0.006, K_d=0.4;

        public double GetCorrection(double measured_angle, double dt)
        {
            var error = measured_angle;
            _integral = _integral + error*dt;
            var derivative = (error - _previous_error)/dt;
            _previous_error = error;
            var res = K_p * error + K_i * _integral + K_d * derivative;
            if (res > 1) res = 1;
            if (res < -1) res = -1;

            return res;
        }
    }
}
