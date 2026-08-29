using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using DiplomWork_Ivan_2026.Trends;
using DiplomWork_Ivan_2026.Services;
using Microsoft.Win32;

namespace DiplomWork_Ivan_2026
{
    public partial class TrendsWindow : Window
    {
        private readonly TrendBuffer _trendBuffer;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();
        private readonly CanvasTrendChartRenderer _chartRenderer = new CanvasTrendChartRenderer();

        private List<double> _currentXValues = new List<double>();
        private List<ChartSeries> _currentSeries = new List<ChartSeries>();
        private TrendChartRenderState? _renderState;

        public TrendsWindow(TrendBuffer trendBuffer)
        {
            InitializeComponent();

            _trendBuffer = trendBuffer;

            _refreshTimer.Interval = TimeSpan.FromSeconds(1);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            LocalizationService.LanguageChanged += LocalizationService_LanguageChanged;
            ApplyLocalization();

            DrawSelectedTrend();
        }

        private void LocalizationService_LanguageChanged(object? sender, EventArgs e)
        {
            ApplyLocalization();
            DrawSelectedTrend();
        }

        private void ApplyLocalization()
        {
            LocalizationService.ApplyStaticText(this);
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            DrawSelectedTrend();
        }

        private void TrendComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTrendCanvas == null)
                return;

            DrawSelectedTrend();
        }

        private void TimeRangeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (MainTrendCanvas == null)
                return;

            DrawSelectedTrend();
        }

        private void DisplayModeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (MainTrendCanvas == null)
                return;

            DrawSelectedTrend();
        }

        private void DrawSelectedTrend()
        {
            if (_trendBuffer.Points.Count == 0)
            {
                MainTrendCanvas.Children.Clear();
                CurrentValueTextBlock.Text = LocalizationService.Text("Current: 0.0", "Текуща: 0.0");
                ChartTitleTextBlock.Text = LocalizationService.Text("No data", "Няма данни");
                _currentXValues.Clear();
                _currentSeries.Clear();
                _renderState = null;
                return;
            }

            TrendChartData chartData = TrendChartDataBuilder.Build(
                _trendBuffer.Points,
                TrendComboBox.SelectedIndex,
                GetSelectedTimeRangeSeconds());

            ChartTitleTextBlock.Text = chartData.Title;
            CurrentValueTextBlock.Text = chartData.CurrentText;
            CurrentValueTextBlock.Foreground = Brushes.White;

            bool useSmoothDisplay = DisplayModeComboBox?.SelectedIndex == 1;
            List<ChartSeries> displaySeries =
                TrendSeriesSmoother.CreateDisplaySeries(
                    chartData.Series,
                    useSmoothDisplay);

            _currentXValues = chartData.XValues;
            _currentSeries = displaySeries;
            _renderState = _chartRenderer.Draw(
                MainTrendCanvas,
                chartData.XValues,
                displaySeries);
        }

        private double? GetSelectedTimeRangeSeconds()
        {
            if (TimeRangeComboBox?.SelectedItem is not ComboBoxItem selectedItem)
                return 600.0;

            string? tag = selectedItem.Tag?.ToString();

            if (string.Equals(tag, "All", StringComparison.OrdinalIgnoreCase))
                return TrendBuffer.HistoryDurationSeconds;

            return double.TryParse(tag, out double seconds) && seconds > 0.0
                ? seconds
                : 600.0;
        }

        private void MainTrendCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_renderState == null || _currentXValues.Count < 2 || _currentSeries.Count == 0)
                return;

            Point mousePosition = e.GetPosition(MainTrendCanvas);

            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            if (mouseX < _renderState.MarginLeft ||
                mouseX > _renderState.MarginLeft + _renderState.PlotWidth ||
                mouseY < _renderState.MarginTop ||
                mouseY > _renderState.MarginTop + _renderState.PlotHeight)
            {
                RemoveDataCursor();
                return;
            }

            int nearestIndex = FindNearestPointIndex(mouseX);

            if (nearestIndex < 0)
                return;

            double timeValue = _currentXValues[nearestIndex];
            double pointX =
                _renderState.MarginLeft +
                (timeValue - _renderState.MinX) /
                (_renderState.MaxX - _renderState.MinX) *
                _renderState.PlotWidth;

            DrawDataCursor(pointX, nearestIndex, timeValue);
        }

        private int FindNearestPointIndex(double mouseX)
        {
            if (_renderState == null)
                return -1;

            int nearestIndex = -1;
            double smallestDistance = double.MaxValue;

            for (int i = 0; i < _currentXValues.Count; i++)
            {
                double pointX =
                    _renderState.MarginLeft +
                    (_currentXValues[i] - _renderState.MinX) /
                    (_renderState.MaxX - _renderState.MinX) *
                    _renderState.PlotWidth;

                double distance = Math.Abs(mouseX - pointX);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private void DrawDataCursor(double pointX, int nearestIndex, double timeValue)
        {
            if (_renderState == null)
                return;

            RemoveDataCursor();

            Line verticalLine = new Line
            {
                X1 = pointX,
                Y1 = _renderState.MarginTop,
                X2 = pointX,
                Y2 = _renderState.MarginTop + _renderState.PlotHeight,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 4, 4 },
                Tag = "DataCursor"
            };

            MainTrendCanvas.Children.Add(verticalLine);

            StackPanel infoStack = BuildCursorInfo(pointX, nearestIndex, timeValue);

            Border infoBox = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8),
                CornerRadius = new CornerRadius(4),
                Child = infoStack,
                Tag = "DataCursor"
            };

            double infoLeft = pointX + 15;
            double infoTop = _renderState.MarginTop + 15;

            if (infoLeft + 260 > MainTrendCanvas.ActualWidth)
                infoLeft = pointX - 275;

            Canvas.SetLeft(infoBox, infoLeft);
            Canvas.SetTop(infoBox, infoTop);

            MainTrendCanvas.Children.Add(infoBox);
        }

        private StackPanel BuildCursorInfo(double pointX, int nearestIndex, double timeValue)
        {
            StackPanel infoStack = new StackPanel();

            TextBlock timeText = new TextBlock
            {
                Text = $"{LocalizationService.Text("Elapsed", "Изминало време")}: {TrendTimeFormatter.FormatCursor(timeValue)}",
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 6)
            };

            infoStack.Children.Add(timeText);

            foreach (ChartSeries series in _currentSeries)
            {
                if (nearestIndex >= series.Values.Count || _renderState == null)
                    continue;

                double value = series.Values[nearestIndex];
                double pointY =
                    _renderState.MarginTop +
                    _renderState.PlotHeight -
                    (value - _renderState.MinY) /
                    (_renderState.MaxY - _renderState.MinY) *
                    _renderState.PlotHeight;

                AddCursorMarker(pointX, pointY, series.Brush);
                AddCursorGuideLine(pointX, pointY, series.Brush);

                TextBlock valueText = new TextBlock
                {
                    Text = $"{series.Name}: {value:F2}",
                    Foreground = series.Brush,
                    FontSize = 13,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                infoStack.Children.Add(valueText);
            }

            return infoStack;
        }

        private void AddCursorMarker(double pointX, double pointY, Brush brush)
        {
            Ellipse pointMarker = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = brush,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Tag = "DataCursor"
            };

            Canvas.SetLeft(pointMarker, pointX - 5);
            Canvas.SetTop(pointMarker, pointY - 5);

            MainTrendCanvas.Children.Add(pointMarker);
        }

        private void AddCursorGuideLine(double pointX, double pointY, Brush brush)
        {
            if (_renderState == null)
                return;

            Line horizontalLine = new Line
            {
                X1 = _renderState.MarginLeft,
                Y1 = pointY,
                X2 = pointX,
                Y2 = pointY,
                Stroke = brush,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 4, 4 },
                Tag = "DataCursor"
            };

            MainTrendCanvas.Children.Add(horizontalLine);
        }

        private void RemoveDataCursor()
        {
            for (int i = MainTrendCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (MainTrendCanvas.Children[i] is FrameworkElement element &&
                    element.Tag?.ToString() == "DataCursor")
                {
                    MainTrendCanvas.Children.RemoveAt(i);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ExportCsvButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trendBuffer.Points.Count == 0)
            {
                MessageBox.Show(
                    LocalizationService.Text(
                        "There are no process samples to export.",
                        "Няма процесни проби за експортиране."),
                    LocalizationService.Text("CSV export", "CSV експорт"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = LocalizationService.Text(
                    "Export process data to CSV",
                    "Експорт на процесните данни в CSV"),
                Filter = LocalizationService.Text(
                    "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    "CSV файлове (*.csv)|*.csv|Всички файлове (*.*)|*.*"),
                DefaultExt = ".csv",
                AddExtension = true,
                FileName = $"vacuum_dryer_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveDialog.ShowDialog(this) != true)
                return;

            TrendPoint[] snapshot = _trendBuffer.Points.ToArray();
            ExportCsvButton.IsEnabled = false;

            try
            {
                await Task.Run(() =>
                    TrendCsvExporter.Export(saveDialog.FileName, snapshot));

                MessageBox.Show(
                    LocalizationService.Text(
                        $"Successfully exported {snapshot.Length} process samples.",
                        $"Успешно са експортирани {snapshot.Length} процесни проби."),
                    LocalizationService.Text("CSV export", "CSV експорт"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    LocalizationService.Text(
                        $"The CSV file could not be saved.\n{exception.Message}",
                        $"CSV файлът не можа да бъде записан.\n{exception.Message}"),
                    LocalizationService.Text("CSV export error", "Грешка при CSV експорт"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                ExportCsvButton.IsEnabled = true;
            }
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
            LocalizationService.LanguageChanged -= LocalizationService_LanguageChanged;
            base.OnClosed(e);
        }

        private void MainTrendCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RemoveDataCursor();
        }
    }
}
