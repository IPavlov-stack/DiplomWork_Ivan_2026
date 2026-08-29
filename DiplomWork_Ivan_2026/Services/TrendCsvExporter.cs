using System.Globalization;
using System.IO;
using System.Text;
using DiplomWork_Ivan_2026.Trends;

namespace DiplomWork_Ivan_2026.Services
{
    public static class TrendCsvExporter
    {
        private const char Separator = ',';

        private static readonly string[] Headers =
        {
            "SampleNumber",
            "ElapsedTime_s",
            "ProcessStage",
            "StageElapsedTime_s",
            "ChamberTemperatureMeasured_C",
            "ChamberTemperatureModel_C",
            "MaterialTemperatureMeasured_C",
            "MaterialTemperatureModel_C",
            "TemperatureSetpoint_C",
            "PressureMeasured_kPa",
            "PressureModel_kPa",
            "PressureSetpoint_kPa",
            "VacuumLevel_pct",
            "MaterialMoisture_wb_pct",
            "MaterialMoisture_db_kg_per_kg",
            "EquilibriumMoisture_wb_pct",
            "AirHumidity_pct",
            "MoistureRatio",
            "AirFlowRate_m3_per_h",
            "DryingRate_wb_pct_per_min",
            "EvaporationRate_kg_per_s",
            "TotalEnergy_kWh",
            "EvaporatedWater_kg",
            "Efficiency_kg_per_kWh",
            "WaterVaporPartialPressure_kPa",
            "WaterVaporMass_kg",
            "PumpedWaterVapor_kg",
            "CondensedWater_kg",
            "AmbientWaterVaporIngress_kg",
            "WaterVaporMassBalanceResidual_kg",
            "HeaterPower_pct",
            "VacuumPumpPower_pct",
            "VentValveOpening_pct",
            "FanSpeed_pct",
            "MoistureTargetReached",
            "SafetyInterlockActive",
            "IsCompleted"
        };

        public static void Export(string filePath, IReadOnlyList<TrendPoint> points)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
            ArgumentNullException.ThrowIfNull(points);

            using StreamWriter writer = new StreamWriter(
                filePath,
                false,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            writer.WriteLine(string.Join(Separator, Headers));

            for (int index = 0; index < points.Count; index++)
            {
                TrendPoint point = points[index];
                string[] values =
                {
                    (index + 1).ToString(CultureInfo.InvariantCulture),
                    Number(point.Time),
                    Escape(point.ProcessStage.ToString()),
                    Number(point.StageElapsedTime),
                    Number(point.Temperature),
                    Number(point.ModelTemperature),
                    Number(point.MaterialTemperature),
                    Number(point.ModelMaterialTemperature),
                    Number(point.TemperatureSetpoint),
                    Number(point.Pressure),
                    Number(point.ModelPressure),
                    Number(point.PressureSetpoint),
                    Number(point.VacuumLevel),
                    Number(point.MaterialMoisture),
                    Number(point.MaterialMoistureDryBasis),
                    Number(point.EquilibriumMoisture),
                    Number(point.AirHumidity),
                    Number(point.MoistureRatio),
                    Number(point.AirFlowRate),
                    Number(point.DryingRate),
                    Number(point.EvaporationRateKgPerSecond),
                    Number(point.TotalEnergyKWh),
                    Number(point.EvaporatedWaterKg),
                    Number(point.EfficiencyKgPerKWh),
                    Number(point.WaterVaporPartialPressureKPa),
                    Number(point.WaterVaporMassKg),
                    Number(point.PumpedWaterVaporKg),
                    Number(point.CondensedWaterKg),
                    Number(point.AmbientWaterVaporIngressKg),
                    Number(point.WaterVaporMassBalanceResidualKg),
                    Number(point.HeaterPower),
                    Number(point.PumpPower),
                    Number(point.VentValveOpening),
                    Number(point.FanSpeed),
                    Boolean(point.MoistureTargetReached),
                    Boolean(point.SafetyInterlockActive),
                    Boolean(point.IsCompleted)
                };

                writer.WriteLine(string.Join(Separator, values));
            }
        }

        private static string Number(double value) =>
            value.ToString("0.##########", CultureInfo.InvariantCulture);

        private static string Boolean(bool value) => value ? "true" : "false";

        private static string Escape(string value)
        {
            if (!value.Contains(Separator) &&
                !value.Contains('"') &&
                !value.Contains('\r') &&
                !value.Contains('\n'))
            {
                return value;
            }

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
