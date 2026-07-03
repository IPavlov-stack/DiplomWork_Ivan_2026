using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using DiplomWork_Ivan_2026.Trends;

namespace DiplomWork_Ivan_2026
{
    public partial class TrendsWindow : Window
    {
        private readonly TrendBuffer _trendBuffer;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();

        public TrendsWindow(TrendBuffer trendBuffer)
        {
            InitializeComponent();

            _trendBuffer = trendBuffer;

            _refreshTimer.Interval = TimeSpan.FromSeconds(1);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            DrawSelectedTrend();
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

        private void DrawSelectedTrend()
        {
            if (_trendBuffer.Points.Count == 0)
            {
                MainTrendCanvas.Children.Clear();
                CurrentValueTextBlock.Text = "Current: 0.0";
                return;
            }

            List<double> xValues = _trendBuffer.Points
                .Select(p => p.Time)
                .ToList();

            List<double> yValues;
            Brush lineBrush;
            string title;
            string currentText;

            TrendPoint last = _trendBuffer.Points.Last();

            switch (TrendComboBox.SelectedIndex)
            {
                case 0:
                    yValues = _trendBuffer.Points.Select(p => p.Temperature).ToList();
                    lineBrush = Brushes.OrangeRed;
                    title = "Temperature [°C]";
                    currentText = $"Current: {last.Temperature:F1} °C";
                    break;

                case 1:
                    yValues = _trendBuffer.Points.Select(p => p.Pressure).ToList();
                    lineBrush = Brushes.DeepSkyBlue;
                    title = "Pressure [kPa]";
                    currentText = $"Current: {last.Pressure:F1} kPa";
                    break;

                case 2:
                    yValues = _trendBuffer.Points.Select(p => p.MaterialMoisture).ToList();
                    lineBrush = Brushes.LimeGreen;
                    title = "Material Moisture [%]";
                    currentText = $"Current: {last.MaterialMoisture:F1} %";
                    break;

                case 3:
                    yValues = _trendBuffer.Points.Select(p => p.DryingRate).ToList();
                    lineBrush = Brushes.Gold;
                    title = "Drying Rate [%/min]";
                    currentText = $"Current: {last.DryingRate:F2} %/min";
                    break;

                default:
                    yValues = _trendBuffer.Points.Select(p => p.Temperature).ToList();
                    lineBrush = Brushes.OrangeRed;
                    title = "Temperature [°C]";
                    currentText = $"Current: {last.Temperature:F1} °C";
                    break;
            }

            ChartTitleTextBlock.Text = title;
            CurrentValueTextBlock.Text = currentText;
            CurrentValueTextBlock.Foreground = lineBrush;

            DrawLineChart(MainTrendCanvas, xValues, yValues, lineBrush);
        }

        private void DrawLineChart(Canvas canvas, List<double> xValues, List<double> yValues, Brush lineBrush)
        {
            canvas.Children.Clear();

            if (xValues.Count < 2 || yValues.Count < 2)
                return;

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            if (width <= 0 || height <= 0)
                return;

            double marginLeft = 70;
            double marginRight = 30;
            double marginTop = 30;
            double marginBottom = 50;

            double plotWidth = width - marginLeft - marginRight;
            double plotHeight = height - marginTop - marginBottom;

            if (plotWidth <= 0 || plotHeight <= 0)
                return;

            double minX = xValues.Min();
            double maxX = xValues.Max();

            double minY = yValues.Min();
            double maxY = yValues.Max();

            if (Math.Abs(maxX - minX) < 0.0001)
                maxX = minX + 1;

            if (Math.Abs(maxY - minY) < 0.0001)
            {
                maxY = minY + 1;
                minY = minY - 1;
            }

            DrawAxes(canvas, marginLeft, marginTop, plotWidth, plotHeight, minY, maxY, minX, maxX);

            Polyline line = new Polyline
            {
                Stroke = lineBrush,
                StrokeThickness = 3
            };

            for (int i = 0; i < xValues.Count; i++)
            {
                double x = marginLeft + (xValues[i] - minX) / (maxX - minX) * plotWidth;
                double y = marginTop + plotHeight - (yValues[i] - minY) / (maxY - minY) * plotHeight;

                line.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(line);
        }

        private void DrawAxes(
            Canvas canvas,
            double marginLeft,
            double marginTop,
            double plotWidth,
            double plotHeight,
            double minY,
            double maxY,
            double minX,
            double maxX)
        {
            Line yAxis = new Line
            {
                X1 = marginLeft,
                Y1 = marginTop,
                X2 = marginLeft,
                Y2 = marginTop + plotHeight,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };

            Line xAxis = new Line
            {
                X1 = marginLeft,
                Y1 = marginTop + plotHeight,
                X2 = marginLeft + plotWidth,
                Y2 = marginTop + plotHeight,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };

            canvas.Children.Add(yAxis);
            canvas.Children.Add(xAxis);

            TextBlock maxYText = new TextBlock
            {
                Text = maxY.ToString("F1"),
                Foreground = Brushes.White,
                FontSize = 13
            };

            Canvas.SetLeft(maxYText, 10);
            Canvas.SetTop(maxYText, marginTop - 8);
            canvas.Children.Add(maxYText);

            TextBlock minYText = new TextBlock
            {
                Text = minY.ToString("F1"),
                Foreground = Brushes.White,
                FontSize = 13
            };

            Canvas.SetLeft(minYText, 10);
            Canvas.SetTop(minYText, marginTop + plotHeight - 8);
            canvas.Children.Add(minYText);

            TextBlock timeText = new TextBlock
            {
                Text = "Time [s]",
                Foreground = Brushes.White,
                FontSize = 14
            };

            Canvas.SetLeft(timeText, marginLeft + plotWidth / 2 - 35);
            Canvas.SetTop(timeText, marginTop + plotHeight + 20);
            canvas.Children.Add(timeText);

            TextBlock startTimeText = new TextBlock
            {
                Text = minX.ToString("F0"),
                Foreground = Brushes.White,
                FontSize = 12
            };

            Canvas.SetLeft(startTimeText, marginLeft - 5);
            Canvas.SetTop(startTimeText, marginTop + plotHeight + 5);
            canvas.Children.Add(startTimeText);

            TextBlock endTimeText = new TextBlock
            {
                Text = maxX.ToString("F0"),
                Foreground = Brushes.White,
                FontSize = 12
            };

            Canvas.SetLeft(endTimeText, marginLeft + plotWidth - 20);
            Canvas.SetTop(endTimeText, marginTop + plotHeight + 5);
            canvas.Children.Add(endTimeText);
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

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }
    }
}