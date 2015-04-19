using Microsoft.SPOT;
using ExampleAccelGyroSensor.I2C_Hardware;
using System.Threading;

namespace ExampleAccelGyroSensor.Sensor
{
    public class MPU6050
    {
        /// <summary>
        /// Klassen für die Verbindung über den I²C Bus
        /// </summary>
        private I2C_Connector _I2C;
        /// <summary>
        /// Klasse mit dem Entsprechendn Adressen Initialisieren
        /// </summary>
        public MPU6050()
        {
            Debug.Print("Initialisiere den Beschleunigungs- und Gyrosensor MPU-6050");
            _I2C = new I2C_Connector(MPU6050_Registers.I2C_ADDRESS, 100);

            Initialize();

            // Test Verbindung
            Debug.Print("Verbindung testen:");
            ErrorStatus(_I2C.Read(new byte[] { MPU6050_Registers.WHO_AM_I }));
            Debug.Print("-----------------------------------------------------------------------");
        }
        /// <summary>
        /// Initialisiert den Sensor mit der Standard Adresse
        /// </summary>
        public void Initialize()
        {
            // PWR_MGMT_1 = Power Management 1 (11111001)
            // 0xF9 = Device Reset (1), Sleep (1), Cycle (1), nichts (1), Temperatur (1), Taktquelle festlegen (001)
            ErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.PWR_MGMT_1, 0x80 }));
            while (_I2C.Read(new byte[] { MPU6050_Registers.PWR_MGMT_1 }) == 0x80) Thread.Sleep(50);

            ErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.PWR_MGMT_1, 0x09 }));
            // FullScaleGyroRange
            ErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.GYRO_CONFIG, 0x00 }));
            // FullScaleAccelRange
            ErrorStatus(_I2C.Write(new byte[] { MPU6050_Registers.ACCEL_CONFIG, 0x00 }));
        }
        /// <summary>
        /// Gibt ein Error Status über Debug aus
        /// </summary>
        /// <param name="error"></param>
        private void ErrorStatus(object error)
        {
            if (error != null)
            {
                    if ((int)error == 0) { Debug.Print("Status: Error"); }
                    else { Debug.Print("Status: OK"); }
            }
        }

        private double _gyroOffset = 0;
        private double _accOffset = 0;

        public void CalibrateGyro()
        {
            double sampleSum = 0;

            for (int i = 0; i < 25; i++)
            {
                sampleSum += GetSensorData().Gyro_X;
                Thread.Sleep(10);

            }
            _gyroOffset = sampleSum/ 25.0;

        }

        public void CalibrateAccelerometer()
        {
            double sampleSum = 0;

            for (int i = 0; i < 25; i++)
            {
                sampleSum += GetSensorData().GetPitchAngleInDegrees();
                Thread.Sleep(10);

            }
            _accOffset = sampleSum / 25.0 + 90;

            Debug.Print("_accOffset: " + _accOffset);
        }


        /// <summary>
        /// Ruft die Daten aus dem Sensor ab
        /// </summary>
        public AccelerationAndGyroData GetSensorData()
        {
            byte[] registerList = new byte[14];

            registerList[0] = MPU6050_Registers.ACCEL_XOUT_H;
            registerList[1] = MPU6050_Registers.ACCEL_XOUT_L;
            registerList[2] = MPU6050_Registers.ACCEL_YOUT_H;
            registerList[3] = MPU6050_Registers.ACCEL_YOUT_L;
            registerList[4] = MPU6050_Registers.ACCEL_ZOUT_H;
            registerList[5] = MPU6050_Registers.ACCEL_ZOUT_L;
            registerList[6] = MPU6050_Registers.TEMP_OUT_H;
            registerList[7] = MPU6050_Registers.TEMP_OUT_L;
            registerList[8] = MPU6050_Registers.GYRO_XOUT_H;
            registerList[9] = MPU6050_Registers.GYRO_XOUT_L;
            registerList[10] = MPU6050_Registers.GYRO_YOUT_H;
            registerList[11] = MPU6050_Registers.GYRO_YOUT_L;
            registerList[12] = MPU6050_Registers.GYRO_ZOUT_H;
            registerList[13] = MPU6050_Registers.GYRO_ZOUT_L;

            _I2C.Write(new byte[] { MPU6050_Registers.ACCEL_XOUT_H });
            _I2C.Read(registerList);
            
            return new AccelerationAndGyroData(registerList, _gyroOffset, _accOffset);
        }
    }
}
