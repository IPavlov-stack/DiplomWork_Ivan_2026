using System.Linq;
using System.Windows;
using System.Windows.Media;
using DiplomWork_Ivan_2026.Enums;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
        private void MaterialComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!_processStarted)
                ApplyDryingMode(GetSelectedDryingMode());

            UpdateUi();
        }

        private void UpdateUi()
        {
            var state = _process.State;

            if (state.SafetyInterlockActive)
            {
                StatusValueRun.Text = "SAFETY TRIP";
                StatusValueRun.Foreground = Brushes.OrangeRed;
            }
            else if (state.IsCompleted)
            {
                StatusValueRun.Text = "Process completed";
                StatusValueRun.Foreground = Brushes.DeepSkyBlue;
            }
            else if (_isRunning)
            {
                StatusValueRun.Text = "Running";
                StatusValueRun.Foreground = Brushes.LimeGreen;
            }
            else
            {
                StatusValueRun.Text = "Stopped";
                StatusValueRun.Foreground = Brushes.Red;
            }

            TemperatureTextBlock.Text = $"Chamber Temperature: {state.MeasuredTemperature:F1} °C";
            MaterialTemperatureTextBlock.Text = $"Material Temperature: {state.MeasuredMaterialTemperature:F1} °C";
            MoistureTextBlock.Text =
                $"Material Moisture: {state.MaterialMoistureWetBasisPercent:F1} % wb";
            PressureTextBlock.Text = $"Pressure: {state.MeasuredPressure:F1} kPa";

            HeaterTextBlock.Text = $"Heater: {state.HeaterPower:F0} %";
            PumpTextBlock.Text = $"Vacuum Pump: {state.VacuumPumpPower:F0} %";
            VentValveTextBlock.Text = $"Vent Valve: {state.VentValveOpening:F0} %";
            FanTextBlock.Text = $"Fan: {state.FanSpeed:F0} %";

            TimeTextBlock.Text = $"Time: {state.ElapsedTime:F0} s";
            ProcessStageTextBlock.Text = $"Stage: {FormatProcessStage(state.ProcessStage)}";
            MoistureRatioTextBlock.Text = $"Moisture Ratio: {state.MoistureRatio:F3}";
            RemainingTimeTextBlock.Text =
                $"Estimated Remaining: {FormatRemainingTime(state.EstimatedRemainingTimeSeconds)}";
            VacuumLevelTextBlock.Text = $"Vacuum Level: {state.VacuumLevel:F1} %";
            TotalEnergyTextBlock.Text = $"Total Energy: {state.TotalEnergyKWh:F3} kWh";
            SensorStatusTextBlock.Text = _process.HasSensorFault
                ? "Sensors: FAULT"
                : "Sensors: OK";
            SensorStatusTextBlock.Foreground = _process.HasSensorFault
                ? Brushes.OrangeRed
                : Brushes.LightGreen;

            bool isManualMode = OperationModeComboBox.SelectedIndex == 1;
            ContextPanelModeTextBlock.Text = isManualMode
                ? "Manual mode: adjustable outputs"
                : "Auto mode: controller outputs";
            if (!isManualMode)
            {
                ManualHeaterSlider.Value = state.HeaterPower;
                ManualPumpSlider.Value = state.VacuumPumpPower;
                ManualVentValveSlider.Value = state.VentValveOpening;
                ManualFanSlider.Value = state.FanSpeed;
            }

            HeaterLamp.Fill = state.HeaterPower > 0.0
                ? Brushes.OrangeRed
                : Brushes.Gray;
            PumpLamp.Fill = state.VacuumPumpPower > 0.0
                ? Brushes.DeepSkyBlue
                : Brushes.Gray;
            FanLamp.Fill = state.FanSpeed > 0.0
                ? Brushes.LimeGreen
                : Brushes.Gray;
            VentValveLamp.Fill = state.VentValveOpening > 0.0
                ? Brushes.Gold
                : Brushes.Gray;

            UpdateAlarmsUi();
            UpdateStartButtonState();
        }

        private void UpdateStartButtonState()
        {
            bool recipeCanBeChanged = !_processStarted && !_isRunning;
            MaterialComboBox.IsEnabled = recipeCanBeChanged;
            DryingModeComboBox.IsEnabled = recipeCanBeChanged;

            if (_process.State.SafetyInterlockActive)
            {
                StartButton.Content = "SAFETY LOCKED";
                StartButton.IsEnabled = false;
            }
            else if (_isRunning)
            {
                StartButton.Content = "RUNNING";
                StartButton.IsEnabled = false;
            }
            else if (_processStarted)
            {
                StartButton.Content = "RESUME";
                StartButton.IsEnabled = true;
            }
            else
            {
                StartButton.Content = "START";
                StartButton.IsEnabled = true;
            }
        }

        private void OperationModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateManualControlsState();
        }

        private void UpdateManualControlsState()
        {
            if (ManualHeaterSlider == null ||
                ManualPumpSlider == null ||
                ManualVentValveSlider == null ||
                ManualFanSlider == null)
            {
                return;
            }

            bool isManualMode = OperationModeComboBox.SelectedIndex == 1;

            ManualHeaterSlider.IsEnabled = isManualMode;
            ManualPumpSlider.IsEnabled = isManualMode;
            ManualVentValveSlider.IsEnabled = isManualMode;
            ManualFanSlider.IsEnabled = isManualMode;
        }

        private void ManualSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ManualHeaterValueTextBlock == null ||
                ManualPumpValueTextBlock == null ||
                ManualVentValveValueTextBlock == null ||
                ManualFanValueTextBlock == null ||
                ManualHeaterSlider == null ||
                ManualPumpSlider == null ||
                ManualVentValveSlider == null ||
                ManualFanSlider == null)
                return;

            ManualHeaterValueTextBlock.Text = $"{ManualHeaterSlider.Value:F0} %";
            ManualPumpValueTextBlock.Text = $"{ManualPumpSlider.Value:F0} %";
            ManualVentValveValueTextBlock.Text = $"{ManualVentValveSlider.Value:F0} %";
            ManualFanValueTextBlock.Text = $"{ManualFanSlider.Value:F0} %";
        }

        private static string FormatProcessStage(Enums.ProcessStage stage)
        {
            return stage switch
            {
                Enums.ProcessStage.Preheating => "Preheating",
                Enums.ProcessStage.Evacuation => "Evacuation",
                Enums.ProcessStage.Drying => "Drying",
                Enums.ProcessStage.FinalDrying => "Final drying",
                Enums.ProcessStage.Venting => "Pressure recovery",
                Enums.ProcessStage.SafetyShutdown => "Safety shutdown",
                Enums.ProcessStage.Completed => "Completed",
                Enums.ProcessStage.Manual => "Manual control",
                _ => "Idle"
            };
        }

        private static string FormatRemainingTime(double? seconds)
        {
            if (!seconds.HasValue ||
                double.IsNaN(seconds.Value) ||
                double.IsInfinity(seconds.Value))
            {
                return "calculating...";
            }

            TimeSpan remaining = TimeSpan.FromSeconds(Math.Max(0.0, seconds.Value));
            if (remaining.TotalDays >= 1.0)
                return $"{(int)remaining.TotalDays}d {remaining.Hours:D2}h";
            if (remaining.TotalHours >= 1.0)
                return $"{(int)remaining.TotalHours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";

            return $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }

        private void UpdateAlarmsUi()
        {
            if (AlarmsTextBlock == null)
                return;

            if (_alarmService.ActiveAlarms.Count == 0)
            {
                AlarmsTextBlock.Text = "No active alarms";
                AlarmsTextBlock.Foreground = Brushes.LightGreen;
                return;
            }

            AlarmsTextBlock.Text = string.Join(
                "\n",
                _alarmService.ActiveAlarms.Select(a => $"[{a.Severity}] {a.Message}")
            );

            bool hasCritical = _alarmService.ActiveAlarms
                .Any(a => a.Severity == AlarmSeverity.Critical);

            AlarmsTextBlock.Foreground = hasCritical
                ? Brushes.OrangeRed
                : Brushes.Gold;
        }
    }
}
