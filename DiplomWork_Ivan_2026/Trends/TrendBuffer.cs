using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026.Trends
{
    public class TrendBuffer
    {
        private readonly TrendPointRingBuffer _points;

        public IReadOnlyList<TrendPoint> Points => _points;

        public TrendBuffer(int maxPoints = 50_000)
        {
            _points = new TrendPointRingBuffer(maxPoints);
        }

        public void AddPoint(VacuumDryerState state, ProcessSettings settings)
        {
            _points.Add(new TrendPoint
            {
                Time = state.ElapsedTime,

                // Temperature group
                Temperature = state.MeasuredTemperature,
                MaterialTemperature = state.MeasuredMaterialTemperature,
                TemperatureSetpoint = state.ActiveTemperatureSetpoint,

                // Pressure group
                Pressure = state.MeasuredPressure,
                PressureSetpoint = state.ActivePressureSetpoint,
                VacuumLevel = state.VacuumLevel,

                // Moisture / humidity group
                MaterialMoisture = state.MaterialMoistureWetBasisPercent,
                EquilibriumMoisture =
                    DryingMaterial.DryBasisToWetBasisPercent(
                        state.DynamicEquilibriumMoistureDryBasis),
                AirHumidity = state.AirHumidity,

                // Drying group
                DryingRate = state.DryingRateWetBasisPercentPerMinute,

                // Energy group
                TotalEnergyKWh = state.TotalEnergyKWh,
                EvaporatedWaterKg = state.EvaporatedWaterKg,
                EfficiencyKgPerKWh = state.EfficiencyKgPerKWh,

                // Actuator group
                HeaterPower = state.HeaterPower,
                PumpPower = state.VacuumPumpPower,
                VentValveOpening = state.VentValveOpening,
                FanSpeed = state.FanSpeed
            });

        }

        public void Clear()
        {
            _points.Clear();
        }
    }
}
