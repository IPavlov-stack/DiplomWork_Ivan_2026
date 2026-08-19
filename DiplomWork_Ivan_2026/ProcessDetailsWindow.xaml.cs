using System;
using System.Windows;
using System.Windows.Threading;
using DiplomWork_Ivan_2026.Models;
using DiplomWork_Ivan_2026.Simulation;

namespace DiplomWork_Ivan_2026
{
    public partial class ProcessDetailsWindow : Window
    {
        private readonly VacuumDryerProcess _process;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();

        public ProcessDetailsWindow(VacuumDryerProcess process)
        {
            InitializeComponent();

            _process = process;

            _refreshTimer.Interval = TimeSpan.FromSeconds(1);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            UpdateDetails();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            UpdateDetails();
        }

        private void UpdateDetails()
        {
            var state = _process.State;
            var material = _process.SelectedMaterial;

            ChamberTemperatureTextBlock.Text =
                $"Chamber Temperature: {state.MeasuredTemperature:F1} °C";

            MaterialTemperatureTextBlock.Text =
                $"Material Temperature: {state.MeasuredMaterialTemperature:F1} °C";

            PressureTextBlock.Text =
                $"Pressure: {state.MeasuredPressure:F1} kPa";

            VacuumLevelTextBlock.Text =
                $"Vacuum Level: {state.VacuumLevel:F1} %";

            AirHumidityTextBlock.Text =
                $"Relative Humidity: {state.AirHumidity:F1} %";

            VaporPressureTextBlock.Text =
                $"Water Vapor Partial Pressure: {state.WaterVaporPartialPressureKPa:F2} kPa";

            MaterialMoistureTextBlock.Text =
                $"Material Moisture: {state.MaterialMoistureWetBasisPercent:F1} % wb " +
                $"(X = {state.MaterialMoistureDryBasis:F3} kg/kg db)";

            EquilibriumMoistureTextBlock.Text =
                $"Dynamic Equilibrium Moisture: " +
                $"{DryingMaterial.DryBasisToWetBasisPercent(state.DynamicEquilibriumMoistureDryBasis):F1} % wb";

            MoistureRatioTextBlock.Text =
                $"Moisture Ratio: {state.MoistureRatio:F3}";

            DryingRateTextBlock.Text =
                $"Drying Rate: {state.DryingRateWetBasisPercentPerMinute:F3} % wb/min";

            AirFlowRateTextBlock.Text =
                $"Air Flow Rate: {state.AirFlowRate:F1} m³/h";

            TotalEnergyTextBlock.Text =
                $"Total Energy: {state.TotalEnergyKWh:F3} kWh";

            EvaporatedWaterTextBlock.Text =
                $"Evaporated Water: {state.EvaporatedWaterKg:F2} kg";

            PumpedVaporTextBlock.Text =
                $"Pumped Water Vapor: {state.PumpedWaterVaporKg:F2} kg";

            CondensedWaterTextBlock.Text =
                $"Condensed Water: {state.CondensedWaterKg:F2} kg";

            VaporBalanceTextBlock.Text =
                $"Water Vapor Balance Residual: " +
                $"{state.WaterVaporMassBalanceResidualKg:F4} kg";

            EfficiencyTextBlock.Text =
                $"Efficiency: {state.EfficiencyKgPerKWh:F2} kg/kWh";

            ElapsedTimeTextBlock.Text =
                $"Elapsed Time: {state.ElapsedTime:F0} s";

            RemainingTimeTextBlock.Text =
                $"Estimated Remaining Time: {FormatRemainingTime(state.EstimatedRemainingTimeSeconds)}";

            HeaterPowerTextBlock.Text =
                $"Heater Power: {state.HeaterPower:F0} %";

            PumpPowerTextBlock.Text =
                $"Vacuum Pump Power: {state.VacuumPumpPower:F0} %";

            VentValveTextBlock.Text =
                $"Vent Valve Opening: {state.VentValveOpening:F0} %";

            FanSpeedTextBlock.Text =
                $"Fan Speed: {state.FanSpeed:F0} %";

            ProcessStageTextBlock.Text =
                $"Process Stage: {state.ProcessStage}";

            SensorStatusTextBlock.Text =
                _process.HasSensorFault ? "Sensors: FAULT" : "Sensors: OK";

            ProcessStatusTextBlock.Text =
                state.IsCompleted ? "Process Status: Completed" : "Process Status: Active / Stopped";

            if (material != null)
            {
                MaterialNameTextBlock.Text =
                    $"Material: {material.Name}";

                InitialMoistureTextBlock.Text =
                    $"Initial Moisture: {material.InitialMoistureWetBasisPercent:F1} % wb";

                TargetMoistureTextBlock.Text =
                    $"Target Moisture: {material.TargetMoistureWetBasisPercent:F1} % wb";

                MaxTemperatureTextBlock.Text =
                    $"Max Temperature: {material.MaxTemperature:F1} °C";

                DryingCoefficientTextBlock.Text =
                    $"Drying Coefficient: {material.DryingCoefficient:F2}";

                MaterialMassTextBlock.Text =
                    $"Initial Wet Mass: {material.InitialWetMassKg:F1} kg " +
                    $"(Dry Mass: {material.DryMassKg:F2} kg)";
            }
            else
            {
                MaterialNameTextBlock.Text = "Material: -";
                InitialMoistureTextBlock.Text = "Initial Moisture: -";
                TargetMoistureTextBlock.Text = "Target Moisture: -";
                MaxTemperatureTextBlock.Text = "Max Temperature: -";
                DryingCoefficientTextBlock.Text = "Drying Coefficient: -";
                MaterialMassTextBlock.Text = "Material Mass: -";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static string FormatRemainingTime(double? seconds)
        {
            if (!seconds.HasValue)
                return "calculating...";

            TimeSpan remaining = TimeSpan.FromSeconds(Math.Max(0.0, seconds.Value));
            if (remaining.TotalDays >= 1.0)
                return $"{(int)remaining.TotalDays}d {remaining.Hours:D2}h";

            return $"{(int)remaining.TotalHours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }

    }
}
