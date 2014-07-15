using ExampleAccelGyroSensor.Sensor;
using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class ComplimentaryFilter
    {
        double _angle;
        const double Tau=0;

        readonly LowPassFilter _lowPassFilter = new LowPassFilter(10);

        public double GetAngle(AccelerationAndGyroData data, double dt)
        {
            double a = Tau/(Tau+dt);

            _angle = a * (data.GetPitchVelocity() * dt + _angle) + (1 - a) * _lowPassFilter.GetLowPassValue(data.GetPitchAngle(), dt);
            return _angle;
        }

    }
}
