using Microsoft.SPOT;
namespace ExampleAccelGyroSensor.Sensor
{
    /// <summary>
    /// Objekt zum Auswerten der Ergebnisse und Bereitstellung der gemessenen Werten.
    /// </summary>
    public struct AccelerationAndGyroData
    {
        /// <summary>
        /// X axis of accelerometer
        /// </summary>
        public int Acceleration_X;
        /// <summary>
        /// Y axis of accelerometer
        /// </summary>
        public int Acceleration_Y;
        /// <summary>
        /// Z axis of accelerometer
        /// </summary>
        public int Acceleration_Z;
        /// <summary>
        /// Temperatur Wert
        /// </summary>
        public int Temperatur;
        /// <summary>
        /// X Achse des Gyroskop
        /// </summary>
        public int Gyro_X;
        /// <summary>
        /// Y Achse des Gyroskop
        /// </summary>
        public int Gyro_Y;
        /// <summary>
        /// Z Achse des Gyroskop
        /// </summary>
        public int Gyro_Z;
        
        private double _pitchCorrection;

        const double PI_double =  3.14159265;
        const double PIBY2_double = 1.5707963;

        private double gyroOffset;
        
        double atan2_approximation2( double y, double x )
        {
            if ( x == 0.0f )
            {
                if ( y > 0.0f ) 
                    return PIBY2_double;
                if ( y == 0.0f ) 
                    return 0.0f;
                return -PIBY2_double;
            }
            
            double atan;
            double z = y/x;
            
            if ( z  < 1.0 && z > -1.0 )
            {
                atan = z/(1.0f + 0.28f*z*z);
                if ( x < 0.0f )
                {
                    if ( y < 0.0f ) 
                        return atan - PI_double;
                    return atan + PI_double;
                }
            }
            else
            {
                atan = PIBY2_double - z/(z*z + 0.28f);
                if ( y < 0.0f ) 
                    return atan - PI_double;
            }
            return atan;
        } 

        public double GetPitchAngleInDegrees()
        {
            if (Acceleration_Z == 0)
                return 0;

            var angle = (atan2_approximation2(Acceleration_Y,Acceleration_Z)) * 180.0 /PI_double;
            angle -= _pitchCorrection;
            Debug.Print("Pitch angle post correct: " + angle.ToString());

            return angle;
        }

        public double GetPitchVelocityInDegreesPerSecond()
        {
            return (double) (Gyro_X-gyroOffset)/131.1; 
        }

        private static int ConvertBytes(byte highByte, byte lowByte)
        {
            int val = highByte << 8 | lowByte;
            if (val >= 32768)
                return val - 65536;
            return val;
        }

        /// <summary>
        /// Erstellt das Objekt mit den Ergebnissen
        /// </summary>
        /// <param name="results"></param>
        public AccelerationAndGyroData(byte[] results, double gyroOffset, double accOffset)
        {
            Acceleration_X = ConvertBytes(results[0], results[1]);
            Acceleration_Y = ConvertBytes(results[2], results[3]); 
            Acceleration_Z = ConvertBytes(results[4], results[5]); 
            
            // Ergebnis für Temperatur (Bisher noch nicht getestet)
            Temperatur = ConvertBytes(results[6], results[7]);
            
            // Ergebnis für den Gyroskopsensors zusammenlegen durch Bitshifting
            Gyro_X = ConvertBytes(results[8], results[9]);
            Gyro_Y = ConvertBytes(results[10], results[11]);
            Gyro_Z = ConvertBytes(results[12], results[13]);
            this.gyroOffset = gyroOffset;
            _pitchCorrection = accOffset;
        }

        /// <summary>
        /// Überschreibe die ToString() Methode
        /// Gibt alle in Werte in einem String zurück
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Acceleration: X: " + GetProzent(Acceleration_X) + "\tY:" + GetProzent(Acceleration_Y) + "\tZ:" + GetProzent(Acceleration_Z) +
                " \t Temp: " + GetProzent(Temperatur) +
                " \t Gyro: X: " + GetProzent(Gyro_X) + "\tY: " + GetProzent(Gyro_Y) + "\tZ: " + GetProzent(Gyro_Z);
        }
        /// <summary>
        /// Gibt den Wert in Prozent Wert zurück, um die Zahl kleiner zu halten.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetProzent(int value)
        {
            double d = (double)value;
            double result = System.Math.Round((d / 65535) * 100);

            return GetToThreeStellingerWorth(result.ToString());
        }
        /// <summary>
        /// Macht aus dem Wert eine drei stellige Zahl
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetToThreeStellingerWorth(string content)
        {
            if (content.Length == 1)
            {
                return "  " + content;
            }
            else if (content.Length == 2)
            {
                return " " + content;
            }

            return content;
        }
    }
}
