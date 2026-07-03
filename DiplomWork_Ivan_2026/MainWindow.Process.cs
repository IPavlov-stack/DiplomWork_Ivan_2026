using System;
using System.Windows;
using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateSettingsFromUi())
                return;

            if (!_processStarted)
            {
                if (MaterialComboBox.SelectedItem is not DryingMaterial material)
                {
                    MessageBox.Show("Please select a material.");
                    return;
                }

                _process.LoadMaterial(material);
                _trendBuffer.Clear();

                _processStarted = true;
            }

            _isRunning = true;
            _timer.Start();

            UpdateUi();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = false;
            _timer.Stop();

            TurnOffDevices();

            _process.State.HeaterPower = 0;
            _process.State.VacuumPumpPower = 0;
            _process.State.FanSpeed = 0;

            UpdateUi();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _isRunning = false;
            _processStarted = false;

            TurnOffDevices();

            if (MaterialComboBox.SelectedItem is DryingMaterial material)
            {
                _process.LoadMaterial(material);
            }

            _trendBuffer.Clear();

            UpdateUi();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!_isRunning)
                return;

            if (!UpdateSettingsFromUi())
            {
                _isRunning = false;
                _timer.Stop();
                return;
            }

            UpdateControllers();

            _process.Update(1.0, _settings);
            _trendBuffer.AddPoint(_process.State, _settings);
            UpdateChartData();
            _alarmService.CheckAlarms(_process, _settings);

            if (_process.State.IsCompleted)
            {
                _isRunning = false;
                _processStarted = false;
                _timer.Stop();
            }

            UpdateUi();
        }

        private void UpdateControllers()
        {
            bool isAutoMode = OperationModeComboBox.SelectedIndex == 0;

            if (isAutoMode)
            {
                _temperatureController.Update(_process.State, _settings, _process.Heater);
                _pressureController.Update(_process.State, _settings, _process.Pump);

                _process.Fan.SetSpeed(ManualFanSlider.Value);
            }
            else
            {
                _process.Heater.SetPower(ManualHeaterSlider.Value);
                _process.Pump.SetPower(ManualPumpSlider.Value);
                _process.Fan.SetSpeed(ManualFanSlider.Value);
            }
        }

        private void TurnOffDevices()
        {
            _process.Heater.TurnOff();
            _process.Pump.TurnOff();
            _process.Fan.TurnOff();
        }
    }
}
