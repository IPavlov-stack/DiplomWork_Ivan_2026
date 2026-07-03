using System.Windows;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
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
                TurnOffDevices();

                Close();
            }
        }
    }
}
