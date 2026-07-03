using System;
using System.Windows;
using System.Windows.Threading;
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
                $"Chamber Temperature: {state.Temperature:F1} °C";

            MaterialTemperatureTextBlock.Text =
                $"Material Temperature: {state.MaterialTemperature:F1} °C";

            PressureTextBlock.Text =
                $"Pressure: {state.Pressure:F1} kPa";

            VacuumLevelTextBlock.Text =
                $"Vacuum Level: {state.VacuumLevel:F1} %";

            AirHumidityTextBlock.Text =
                $"Air Humidity: {state.AirHumidity:F1} %";

            MaterialMoistureTextBlock.Text =
                $"Material Moisture: {state.MaterialMoisture:F1} %";

            DryingRateTextBlock.Text =
                $"Drying Rate: {state.DryingRate * 60.0:F2} %/min";

            AirFlowRateTextBlock.Text =
                $"Air Flow Rate: {state.AirFlowRate:F1} m³/h";

            TotalEnergyTextBlock.Text =
                $"Total Energy: {state.TotalEnergyKWh:F3} kWh";

            EvaporatedWaterTextBlock.Text =
                $"Evaporated Water: {state.EvaporatedWaterKg:F2} kg";

            EfficiencyTextBlock.Text =
                $"Efficiency: {state.EfficiencyKgPerKWh:F2} kg/kWh";

            ElapsedTimeTextBlock.Text =
                $"Elapsed Time: {state.ElapsedTime:F0} s";

            HeaterPowerTextBlock.Text =
                $"Heater Power: {state.HeaterPower:F0} %";

            PumpPowerTextBlock.Text =
                $"Vacuum Pump Power: {state.VacuumPumpPower:F0} %";

            FanSpeedTextBlock.Text =
                $"Fan Speed: {state.FanSpeed:F0} %";

            ProcessStatusTextBlock.Text =
                state.IsCompleted ? "Process Status: Completed" : "Process Status: Active / Stopped";

            if (material != null)
            {
                MaterialNameTextBlock.Text =
                    $"Material: {material.Name}";

                InitialMoistureTextBlock.Text =
                    $"Initial Moisture: {material.InitialMoisture:F1} %";

                TargetMoistureTextBlock.Text =
                    $"Target Moisture: {material.TargetMoisture:F1} %";

                MaxTemperatureTextBlock.Text =
                    $"Max Temperature: {material.MaxTemperature:F1} °C";

                DryingCoefficientTextBlock.Text =
                    $"Drying Coefficient: {material.DryingCoefficient:F2}";

                MaterialMassTextBlock.Text =
                    $"Material Mass: {material.MaterialMassKg:F1} kg";
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