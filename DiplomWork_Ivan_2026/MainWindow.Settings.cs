using System.Windows;
using DiplomWork_Ivan_2026.Enums;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
        private void ApplyDryingMode(DryingMode mode)
        {
            switch (mode)
            {
                case DryingMode.Soft:
                    TemperatureSetpointTextBox.Text = "45";
                    PressureSetpointTextBox.Text = "50";
                    ManualFanSlider.Value = 50;
                    break;

                case DryingMode.Normal:
                    TemperatureSetpointTextBox.Text = "60";
                    PressureSetpointTextBox.Text = "30";
                    ManualFanSlider.Value = 70;
                    break;

                case DryingMode.Hard:
                    TemperatureSetpointTextBox.Text = "75";
                    PressureSetpointTextBox.Text = "20";
                    ManualFanSlider.Value = 100;
                    break;
            }
        }

        private void DryingModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DryingModeComboBox == null ||
                TemperatureSetpointTextBox == null ||
                PressureSetpointTextBox == null ||
                ManualFanSlider == null)
            {
                return;
            }

            DryingMode selectedMode = DryingMode.Normal;

            switch (DryingModeComboBox.SelectedIndex)
            {
                case 0:
                    selectedMode = DryingMode.Soft;
                    break;

                case 1:
                    selectedMode = DryingMode.Normal;
                    break;

                case 2:
                    selectedMode = DryingMode.Hard;
                    break;
            }

            ApplyDryingMode(selectedMode);
        }

        private bool UpdateSettingsFromUi()
        {
            if (!double.TryParse(TemperatureSetpointTextBox.Text, out double temperatureSetpoint))
            {
                MessageBox.Show("Invalid temperature setpoint.");
                return false;
            }

            if (!double.TryParse(PressureSetpointTextBox.Text, out double pressureSetpoint))
            {
                MessageBox.Show("Invalid pressure setpoint.");
                return false;
            }

            _settings.TemperatureSetpoint = temperatureSetpoint;
            _settings.PressureSetpoint = pressureSetpoint;

            return true;
        }
    }
}
