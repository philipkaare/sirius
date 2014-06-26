using ExampleAccelGyroSensor.Sensor;
using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class ComplimentaryFilter
    {
        double angle;
        const double tau=0.1;

        public ComplimentaryFilter()
        {

        }

        public double GetAngle(AccelerationAndGyroData data, double dt)
        {
            double a = tau/(tau+dt);
            
            angle = a * (data.GetPitchVelocity() * dt + angle) + (1-a) * data.GetPitchAngle();
            return angle;
        }
    }
}
