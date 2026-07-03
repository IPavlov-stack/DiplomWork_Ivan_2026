using DiplomWork_Ivan_2026.Controllers;
using DiplomWork_Ivan_2026.Enums;
using DiplomWork_Ivan_2026.Models;
using DiplomWork_Ivan_2026.Services;
using DiplomWork_Ivan_2026.Simulation;
using DiplomWork_Ivan_2026.Trends;

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly VacuumDryerProcess _process = new VacuumDryerProcess();
        private readonly ProcessSettings _settings = new ProcessSettings();
        private readonly AlarmService _alarmService = new AlarmService();
        private readonly TrendBuffer _trendBuffer = new TrendBuffer(300);

        private readonly OnOffTemperatureController _temperatureController = new OnOffTemperatureController();
        private readonly OnOffPressureController _pressureController = new OnOffPressureController();
        private readonly List<double> _temperatureValues = new List<double>();
        private readonly List<double> _pressureValues = new List<double>();
        private readonly List<double> _moistureValues = new List<double>();

        private bool _isRunning = false;
        private bool _processStarted = false;

        public ISeries[] ProcessSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ProcessSeries = new ISeries[]
{
    new LineSeries<double>
    {
        Name = "Temperature",
        Values = _temperatureValues,
        GeometrySize = 0,
        Fill = null
    },
    new LineSeries<double>
    {
        Name = "Pressure",
        Values = _pressureValues,
        GeometrySize = 0,
        Fill = null
    },
    new LineSeries<double>
    {
        Name = "Moisture",
        Values = _moistureValues,
        GeometrySize = 0,
        Fill = null
    }
};

            XAxes = new Axis[]
            {
    new Axis
    {
        Name = "Time",
        LabelsPaint = new SolidColorPaint(SKColors.White),
        NamePaint = new SolidColorPaint(SKColors.White)
    }
            };

            YAxes = new Axis[]
            {
    new Axis
    {
        Name = "Value",
        LabelsPaint = new SolidColorPaint(SKColors.White),
        NamePaint = new SolidColorPaint(SKColors.White)
    }
            };

            DataContext = this;

            MaterialComboBox.ItemsSource = MaterialLibrary.GetMaterials();
            MaterialComboBox.SelectedIndex = 0;

            OperationModeComboBox.SelectedIndex = 0;
            UpdateManualControlsState();

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

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

            _process.Heater.TurnOff();
            _process.Pump.TurnOff();
            _process.Fan.TurnOff();

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

            _process.Heater.TurnOff();
            _process.Pump.TurnOff();
            _process.Fan.TurnOff();

            if (MaterialComboBox.SelectedItem is DryingMaterial material)
            {
                _process.LoadMaterial(material);
            }

            _trendBuffer.Clear();

            UpdateUi();
        }

        private void ShowTrendsButton_Click(object sender, RoutedEventArgs e)
        {
            TrendsWindow trendsWindow = new TrendsWindow(_trendBuffer);
            trendsWindow.Show();
        }
        private void ShowAlarmsButton_Click(object sender, RoutedEventArgs e)
        {
            AlarmsWindow alarmsWindow = new AlarmsWindow(_alarmService);
            alarmsWindow.Show();
        }

        private void MaterialComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateUi();
        }
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessDetailsWindow detailsWindow = new ProcessDetailsWindow(_process);
            detailsWindow.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _timer.Stop();

                _process.Heater.TurnOff();
                _process.Pump.TurnOff();
                _process.Fan.TurnOff();

                Close();
            }
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

            // START / RESUME / RUNNING button state
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
     
        private void UpdateChartData()
        {
            var state = _process.State;

            _temperatureValues.Add(state.Temperature);
            _pressureValues.Add(state.Pressure);
            _moistureValues.Add(state.MaterialMoisture);

            if (_temperatureValues.Count > 100)
            {
                _temperatureValues.RemoveAt(0);
                _pressureValues.RemoveAt(0);
                _moistureValues.RemoveAt(0);
            }
        }
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