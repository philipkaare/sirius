
namespace ExampleAccelGyroSensor.Sensor
{
    /// <summary>
    /// Adressen alle abgeleitet vom Beispiel Sketch.
    /// Siehe Seite http://arduino.cc/playground/Main/MPU-6050
    /// </summary>
    public static class MPU6050_Registers
    {
        /// <summary>
        /// Standardadresse zum initialisieren.
        /// </summary>
        public static byte I2C_ADDRESS = 0x68;
        /// <summary>
        /// Addresse für die Konfiguration des Gyroskop
        /// </summary>
        public static byte GYRO_CONFIG = 0x1B;    // R/W
        /// <summary>
        /// Addresse für die Konfiguration des Beschleunigungssensors
        /// </summary>
        public static byte ACCEL_CONFIG = 0x1C;   // R/W
        /// <summary>
        /// Adresse für die X Achse des Beschleunigungssensor Teil 1
        /// </summary>
        public static byte ACCEL_XOUT_H = 0x3B;       // R  
        /// <summary>
        /// Adresse für die X Achse des Beschleunigungssensor Teil 2
        /// </summary>
        public static byte ACCEL_XOUT_L = 0x3C;       // R  
        /// <summary>
        /// Adresse für die Y Achse des Beschleunigungssensor Teil 1
        /// </summary>
        public static byte ACCEL_YOUT_H = 0x3D;       // R  
        /// <summary>
        /// Adresse für die Y Achse des Beschleunigungssensor Teil 2
        /// </summary>
        public static byte ACCEL_YOUT_L = 0x3E;       // R  
        /// <summary>
        /// Adresse für die Z Achse des Beschleunigungssensor Teil 1
        /// </summary>
        public static byte ACCEL_ZOUT_H = 0x3F;       // R  
        /// <summary>
        /// Adresse für die Z Achse des Beschleunigungssensor Teil 2
        /// </summary>
        public static byte ACCEL_ZOUT_L = 0x40;       // R 
        /// <summary>
        /// Adresse für den Temparatur Teil 1 (Bisher noch nicht versucht)
        /// </summary>
        public static byte TEMP_OUT_H = 0x41;         // R  
        /// <summary>
        /// Adresse für den Temparatur Teil 2 (Bisher noch nicht versucht)
        /// </summary>
        public static byte TEMP_OUT_L = 0x42;         // R  
        /// <summary>
        /// Adresse für die X Achse des Gyroskop Teil 1
        /// </summary>
        public static byte GYRO_XOUT_H = 0x43;        // R  
        /// <summary>
        /// Adresse für die X Achse des Gyroskop Teil 2
        /// </summary>
        public static byte GYRO_XOUT_L = 0x44;        // R  
        /// <summary>
        /// Adresse für die Y Achse des Gyroskop Teil 1
        /// </summary>
        public static byte GYRO_YOUT_H = 0x45;        // R  
        /// <summary>
        /// Adresse für die Y Achse des Gyroskop Teil 2
        /// </summary>
        public static byte GYRO_YOUT_L = 0x46;        // R  
        /// <summary>
        /// Adresse für die Z Achse des Gyroskop Teil 1
        /// </summary>
        public static byte GYRO_ZOUT_H = 0x47;        // R  
        /// <summary>
        /// Adresse für die Z Achse des Gyroskop Teil 2
        /// </summary>
        public static byte GYRO_ZOUT_L = 0x48;        // R  
        /// <summary>
        /// Adresse für Power Management 1
        /// Ermöglicht die Einstellungen für den Power Modus und die Taktquelle zu bestimmen.
        /// </summary>
        public static byte PWR_MGMT_1 = 0x6B;         // R/W
        /// <summary>
        /// Adresse für Power Management 2
        /// Weitere Einstellungen.
        /// </summary>
        public static byte PWR_MGMT_2 = 0x6C;         // R/W
        /// <summary>
        /// Adresse für Eigene Identität bzw. Adresse prüfen.
        /// </summary>
        public static byte WHO_AM_I = 0x75;           // R
    }
}
