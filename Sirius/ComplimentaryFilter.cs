using ExampleAccelGyroSensor.Sensor;
using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class ComplimentaryFilter
    {
        double _angle;
        const double Tau=0;
        private double _capacitorState = 0;
        private double _c=0.001;

        public ComplimentaryFilter()
        {

        }

        public double GetAngle(AccelerationAndGyroData data, double dt)
        {
            double a = Tau/(Tau+dt);
            
            _angle = a * (data.GetPitchVelocity() * dt + _angle) + (1-a) * GetLowPassValue(data.GetPitchAngle(),dt);
            return _angle;
        }

        public double GetLowPassValue(double value, double dt)
        {
            Debug.Print(_capacitorState.ToString());
            _capacitorState -= (_capacitorState - value)*_c*dt;
            return _capacitorState;
        }
    }
}
