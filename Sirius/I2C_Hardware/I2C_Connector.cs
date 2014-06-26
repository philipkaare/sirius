using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;

namespace ExampleAccelGyroSensor.I2C_Hardware
{
    /// <summary>
    /// Einfache Klasse zum Verbinden eines Moduls mit dem I²C Bus.
    /// Ist aus einem Beispiel im Forum von Netduino.com abgeleitet.
    /// </summary>
    public class I2C_Connector
    {
        /// <summary>
        /// Konfigurationen festhalten
        /// </summary>
        private I2CDevice.Configuration _Config;
        /// <summary>
        /// Hauptklasse für die Verbindung
        /// </summary>
        private I2CDevice _Device;
        /// <summary>
        /// Initialisiert die Klasse
        /// Konfiguration wird angelegt und
        /// die entsprechende Klasse I2CDevice wird initialisiert
        /// </summary>
        /// <param name="address">Byte Adresse vom Modul übergeben.</param>
        /// <param name="clockRateKHz">Taktrate in KHz festlegen</param>
        public I2C_Connector(byte address, int clockRateKHz)
        {
            // Adresse und Taktfrequenz übergeben
            _Config = new I2CDevice.Configuration(address, clockRateKHz);
            _Device = new I2CDevice(_Config);
        }
        /// <summary>
        /// Sendet den Inhalt des Byte Array zur Hardware
        /// </summary>
        /// <param name="writeBuffer">Byte Array</param>
        public int Write(byte[] writeBuffer)
        {
            // Byte Array übergeben für das ertellen einer Transaction
            I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[]
            { 
                I2CDevice.CreateWriteTransaction(writeBuffer) 
            };
            // Sende die Daten an die Hardware. Timeout bei 1 Sekunde
            int written = this._Device.Execute(writeTransaction, 1000);

            // Prüfe ob alle daten gesendet wurden, ansonsten Exception ausführen      
            if (written != writeBuffer.Length)
            {
                Debug.Print("Es konnten keine Daten an das Modul gesendet werden.");
            }

            return written;
        }
        /// <summary>
        /// Ruft mit den Adressen im Buffer die Werte ab
        /// </summary>
        /// <param name="readBuffer">Byte Array mit Adressen für den Abruf entsprechender Daten</param>
        public int Read(byte[] readBuffer)
        {
            // Erstelle ein Transaction zum Lesen mit übergabe des Byte Array        
            I2CDevice.I2CTransaction[] readTransaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateReadTransaction(readBuffer)        
            };
            // Lese die Daten von der Hardware. Timeout von einer Sekunde     
            int read = this._Device.Execute(readTransaction, 1000);

            // Prüfe, ob die Daten gesendt wurden      
            if (read != readBuffer.Length)
            {
                //throw new Exception("Es konnte nicht vom Modul gelesen werden.");
                Debug.Print("Es konnte nicht vom Modul gelesen werden.");
            }

            return read;
        }
    }
}
