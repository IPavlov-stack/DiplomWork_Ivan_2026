using System.Windows;
using System.Linq;
using System.Windows.Threading;
using DiplomWork_Ivan_2026.Services;

namespace DiplomWork_Ivan_2026
{
    public partial class AlarmsWindow : Window
    {
        private readonly AlarmService _alarmService;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();

        public AlarmsWindow(AlarmService alarmService)
        {
            InitializeComponent();

            _alarmService = alarmService;

            _refreshTimer.Interval = TimeSpan.FromSeconds(1);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            UpdateAlarmTable();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            UpdateAlarmTable();
        }

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAlarmTable();
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

        private void UpdateAlarmTable()
        {
            var rows = _alarmService.AlarmHistory
                .Select(a => new AlarmRow
                {
                    Status = a.IsActive ? "ACTIVE" : "CLEARED",
                    Priority = a.Severity.ToString(),
                    Date = a.Time.ToString("dd.MM.yyyy"),
                    Time = a.Time.ToString("HH:mm:ss"),
                    Type = a.Type.ToString(),
                    Description = a.Message
                })
                .ToList();

            AlarmHistoryDataGrid.ItemsSource = rows;

            ActiveAlarmsCountTextBlock.Text =
                $"Active alarms: {_alarmService.ActiveAlarms.Count}";

            TotalAlarmsCountTextBlock.Text =
                $"Total alarms: {_alarmService.AlarmHistory.Count}";
        }

        private class AlarmRow
        {
            public string Status { get; set; } = "";
            public string Priority { get; set; } = "";
            public string Date { get; set; } = "";
            public string Time { get; set; } = "";
            public string Type { get; set; } = "";
            public string Description { get; set; } = "";
        }
    }
}