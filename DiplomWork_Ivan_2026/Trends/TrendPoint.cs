namespace DiplomWork_Ivan_2026.Trends
{
    public class TrendPoint
    {
        public double Time { get; set; }

        // Temperature group
        public double Temperature { get; set; }                 // Chamber temperature
        public double MaterialTemperature { get; set; }
        public double TemperatureSetpoint { get; set; }

        // Pressure group
        public double Pressure { get; set; }
        public double PressureSetpoint { get; set; }
        public double VacuumLevel { get; set; }

        // Moisture / humidity group
        public double MaterialMoisture { get; set; }
        public double AirHumidity { get; set; }

        // Drying group
        public double DryingRate { get; set; }               // [%/min]

        // Energy group
        public double TotalEnergyKWh { get; set; }
        public double EvaporatedWaterKg { get; set; }
        public double EfficiencyKgPerKWh { get; set; }

        // Actuator group
        public double HeaterPower { get; set; }
        public double PumpPower { get; set; }
        public double FanSpeed { get; set; }
    }
}