using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class LowPassFilter
    {
        private double _capacitorState;
        private double _c=0;

        public LowPassFilter(double c)
        {
            _c = c;
        }

        public double GetLowPassValue(double value, double dt)
        {
            _capacitorState -= (_capacitorState - value) * _c * dt;
            return _capacitorState;
        }
    }
}
