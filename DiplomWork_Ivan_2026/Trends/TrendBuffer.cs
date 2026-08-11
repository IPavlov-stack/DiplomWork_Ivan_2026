using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026.Trends
{
    public class TrendBuffer
    {
        private readonly int _maxPoints;

        public List<TrendPoint> Points { get; } = new List<TrendPoint>();

        public TrendBuffer(int maxPoints = 600)
        {
            _maxPoints = maxPoints;
        }

        public void AddPoint(VacuumDryerState state, ProcessSettings settings)
        {
            Points.Add(new TrendPoint
            {
                Time = state.ElapsedTime,

                // Temperature group
                Temperature = state.Temperature,
                MaterialTemperature = state.MaterialTemperature,
                TemperatureSetpoint = settings.TemperatureSetpoint,

                // Pressure group
                Pressure = state.Pressure,
                PressureSetpoint = settings.PressureSetpoint,
                VacuumLevel = state.VacuumLevel,

                // Moisture / humidity group
                MaterialMoisture = state.MaterialMoisture,
                AirHumidity = state.AirHumidity,

                // Drying group
                DryingRate = state.DryingRate * 60.0,

                // Energy group
                TotalEnergyKWh = state.TotalEnergyKWh,
                EvaporatedWaterKg = state.EvaporatedWaterKg,
                EfficiencyKgPerKWh = state.EfficiencyKgPerKWh,

                // Actuator group
                HeaterPower = state.HeaterPower,
                PumpPower = state.VacuumPumpPower,
                FanSpeed = state.FanSpeed
            });

            if (Points.Count > _maxPoints)
            {
                Points.RemoveAt(0);
            }
        }

        public void Clear()
        {
            Points.Clear();
        }
    }
}