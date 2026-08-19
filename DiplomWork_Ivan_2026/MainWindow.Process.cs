using System;
using System.Windows;
using DiplomWork_Ivan_2026.Controllers;
using DiplomWork_Ivan_2026.Models;
using DiplomWork_Ivan_2026.Services;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateSettingsFromUi())
                return;

            if (_process.State.SafetyInterlockActive)
            {
                MessageBox.Show("Reset the safety interlock before starting the process.");
                return;
            }

            if (!_processStarted)
            {
                if (MaterialComboBox.SelectedItem is not DryingMaterial material)
                {
                    MessageBox.Show("Please select a material.");
                    return;
                }

                if (_settings.TemperatureSetpoint > material.MaxTemperature)
                {
                    MessageBox.Show(
                        $"Temperature setpoint {_settings.TemperatureSetpoint:F1} °C exceeds " +
                        $"the {material.Name} limit {material.MaxTemperature:F1} °C.");
                    return;
                }

                _process.LoadMaterial(material);
                _automaticProcessController.Reset(_process.State, _settings);
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
            _pidTemperatureController.Reset();
            _pressureController.Reset();
            TurnOffDevices();

            _process.State.HeaterPower = 0;
            _process.State.VacuumPumpPower = 0;
            _process.State.VentValveOpening = 0;
            _process.State.FanSpeed = 0;

            UpdateUi();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _pidTemperatureController.Reset();
            _pressureController.Reset();
            _automaticProcessController.Reset(_process.State, _settings);
            _isRunning = false;
            _processStarted = false;

            TurnOffDevices();

            if (MaterialComboBox.SelectedItem is DryingMaterial material)
            {
                _process.LoadMaterial(material);
            }

            _safetyInterlockService.Clear(_process);

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

            for (int sample = 0; sample < _simulationSpeedMultiplier; sample++)
            {
                // Physics, sensors, controllers and safety run at 10 Hz. Trends
                // still receive exactly one sample per simulated second.
                for (int substep = 0;
                    substep < IntegrationSubstepsPerTrendSample;
                    substep++)
                {
                    _safetyInterlockService.Evaluate(_process, _settings);
                    UpdateControllers(SimulationIntegrationStepSeconds);
                    _process.Update(
                        SimulationIntegrationStepSeconds,
                        _settings);
                    _safetyInterlockService.Evaluate(_process, _settings);

                    if (_process.State.IsCompleted)
                        break;
                }

                _trendBuffer.AddPoint(_process.State, _settings);
                _alarmService.CheckAlarms(_process, _settings);

                if (_process.State.IsCompleted)
                    break;
            }

            UpdateChartData();

            if (_process.State.IsCompleted)
            {
                _isRunning = false;
                _processStarted = false;
                _timer.Stop();
            }

            UpdateUi();
        }

        private void SimulationSpeedComboBox_SelectionChanged(
            object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SimulationSpeedComboBox?.SelectedItem is not
                System.Windows.Controls.ComboBoxItem selectedItem)
            {
                return;
            }

            if (int.TryParse(selectedItem.Tag?.ToString(), out int multiplier) &&
                multiplier > 0)
            {
                _simulationSpeedMultiplier = multiplier;
            }
        }

        private void UpdateControllers(double deltaTime)
        {
            if (_process.State.SafetyInterlockActive)
            {
                SafetyInterlockService.ApplySafeOutputs(_process);
                _pidTemperatureController.Reset();
                _pressureController.Reset();
                return;
            }

            bool isAutoMode = OperationModeComboBox.SelectedIndex == 0;

            if (isAutoMode)
            {
                AutomaticControlTargets targets =
                    _automaticProcessController.Update(
                        _process,
                        _settings,
                        _automaticFanSpeedSetpoint,
                        deltaTime);

                if (_process.State.IsCompleted)
                {
                    TurnOffDevices();
                    return;
                }

                bool usePidTemperatureControl =
                    TemperatureControlComboBox != null &&
                    TemperatureControlComboBox.SelectedIndex == 1;

                if (!targets.TemperatureControlEnabled)
                {
                    _process.Heater.TurnOff();
                    _pidTemperatureController.Reset();
                }
                else if (usePidTemperatureControl)
                {
                    double heaterPower = _pidTemperatureController.Update(
                        targets.TemperatureSetpoint,
                        _process.State.MeasuredTemperature,
                        deltaTime);

                    _process.Heater.SetPower(heaterPower);
                    _process.State.HeaterPower = _process.Heater.Power;
                }
                else
                {
                    _temperatureController.Update(_process.State, _settings, _process.Heater);
                }

                if (targets.PressureControlEnabled)
                {
                    double pumpPower = _pressureController.Update(
                        targets.PressureSetpoint,
                        _process.State.MeasuredPressure,
                        deltaTime);
                    _process.Pump.SetPower(pumpPower);
                }
                else
                {
                    _process.Pump.TurnOff();
                    _pressureController.Reset();
                }

                _process.VentValve.SetOpening(targets.VentValveOpening);
                _process.Fan.SetSpeed(targets.FanSpeed);
            }
            else
            {
                _pidTemperatureController.Reset();
                _pressureController.Reset();

                _process.State.ProcessStage = Enums.ProcessStage.Manual;
                _process.State.StageElapsedTime = 0.0;
                _process.State.ActiveTemperatureSetpoint = _settings.TemperatureSetpoint;
                _process.State.ActivePressureSetpoint = _settings.PressureSetpoint;

                _process.Heater.SetPower(ManualHeaterSlider.Value);
                _process.Pump.SetPower(ManualPumpSlider.Value);
                _process.VentValve.SetOpening(ManualVentValveSlider.Value);
                _process.Fan.SetSpeed(ManualFanSlider.Value);
            }
        }

        private void EmergencyStopButton_Click(object sender, RoutedEventArgs e)
        {
            _safetyInterlockService.Trip(
                _process,
                "Emergency stop activated by the operator.",
                true);
            _alarmService.CheckAlarms(_process, _settings);
            UpdateUi();
        }

        private void ResetSafetyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateSettingsFromUi())
                return;

            if (!_safetyInterlockService.TryReset(
                _process,
                _settings,
                out string reason))
            {
                MessageBox.Show(reason, "Safety reset");
                return;
            }

            _pidTemperatureController.Reset();
            _pressureController.Reset();
            _automaticProcessController.Reset(_process.State, _settings);
            _alarmService.CheckAlarms(_process, _settings);
            UpdateUi();
        }

        private void TurnOffDevices()
        {
            _process.Heater.TurnOff();
            _process.Pump.TurnOff();
            _process.VentValve.Close();
            _process.Fan.TurnOff();
        }
    }
}
