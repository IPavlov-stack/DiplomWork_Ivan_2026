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
            UpdateUi();
        }

        private void UpdateUi()
        {
            var state = _process.State;

            if (state.IsCompleted)
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

            TemperatureTextBlock.Text = $"Chamber Temperature: {state.Temperature:F1} °C";
            MaterialTemperatureTextBlock.Text = $"Material Temperature: {state.MaterialTemperature:F1} °C";
            MoistureTextBlock.Text = $"Material Moisture: {state.MaterialMoisture:F1} %";
            PressureTextBlock.Text = $"Pressure: {state.Pressure:F1} kPa";

            HeaterTextBlock.Text = $"Heater: {state.HeaterPower:F0} %";
            PumpTextBlock.Text = $"Vacuum Pump: {state.VacuumPumpPower:F0} %";
            FanTextBlock.Text = $"Fan: {state.FanSpeed:F0} %";

            TimeTextBlock.Text = $"Time: {state.ElapsedTime:F0} s";
            VacuumLevelTextBlock.Text = $"Vacuum Level: {state.VacuumLevel:F1} %";
            TotalEnergyTextBlock.Text = $"Total Energy: {state.TotalEnergyKWh:F3} kWh";

            UpdateAlarmsUi();
            UpdateStartButtonState();
        }

        private void UpdateStartButtonState()
        {
            if (_isRunning)
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
                ManualFanSlider == null)
            {
                return;
            }

            bool isManualMode = OperationModeComboBox.SelectedIndex == 1;

            ManualHeaterSlider.IsEnabled = isManualMode;
            ManualPumpSlider.IsEnabled = isManualMode;
            ManualFanSlider.IsEnabled = isManualMode;
        }

        private void ManualSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ManualHeaterValueTextBlock == null)
                return;

            ManualHeaterValueTextBlock.Text = $"{ManualHeaterSlider.Value:F0} %";
            ManualPumpValueTextBlock.Text = $"{ManualPumpSlider.Value:F0} %";
            ManualFanValueTextBlock.Text = $"{ManualFanSlider.Value:F0} %";
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
