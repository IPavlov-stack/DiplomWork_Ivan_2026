using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using DiplomWork_Ivan_2026.Controllers;
using DiplomWork_Ivan_2026.Services;
using DiplomWork_Ivan_2026.Simulation;
using DiplomWork_Ivan_2026.Trends;
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
        private readonly Models.ProcessSettings _settings = new Models.ProcessSettings();
        private readonly AlarmService _alarmService = new AlarmService();
        private readonly TrendBuffer _trendBuffer = new TrendBuffer(300);

        private readonly OnOffTemperatureController _temperatureController = new OnOffTemperatureController();
        private readonly OnOffPressureController _pressureController = new OnOffPressureController();
        private readonly List<double> _temperatureValues = new List<double>();
        private readonly List<double> _pressureValues = new List<double>();
        private readonly List<double> _moistureValues = new List<double>();

        private bool _isRunning = false;
        private bool _processStarted = false;

        public ISeries[] ProcessSeries { get; set; } = System.Array.Empty<ISeries>();
        public Axis[] XAxes { get; set; } = System.Array.Empty<Axis>();
        public Axis[] YAxes { get; set; } = System.Array.Empty<Axis>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeChart();

            DataContext = this;

            MaterialComboBox.ItemsSource = MaterialLibrary.GetMaterials();
            MaterialComboBox.SelectedIndex = 0;

            OperationModeComboBox.SelectedIndex = 0;
            UpdateManualControlsState();

            _timer.Interval = System.TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void InitializeChart()
        {
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
        }
    }
}