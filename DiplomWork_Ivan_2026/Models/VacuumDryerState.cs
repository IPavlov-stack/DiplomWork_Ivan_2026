using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomWork_Ivan_2026.Models
{
    public class VacuumDryerState
    {
        public double Temperature { get; set; } = 20.0;          // °C
        public double MaterialMoisture { get; set; } = 60.0;     // %
        public double AirHumidity { get; set; } = 50.0;          // %
        public double Pressure { get; set; } = 101.3;            // kPa

        public double HeaterPower { get; set; } = 0.0;           // %
        public double FanSpeed { get; set; } = 0.0;              // %
        public double VacuumPumpPower { get; set; } = 0.0;       // %
        public double VacuumLevel { get; set; } = 0.0;           // [%]
        public double AirFlowRate { get; set; } = 0.0;           // [m³/h]
        public double DryingRate { get; set; } = 0.0;            // [%/s]

        public double ElapsedTime { get; set; } = 0.0;           // seconds
        public bool IsCompleted { get; set; } = false;
    }
}
