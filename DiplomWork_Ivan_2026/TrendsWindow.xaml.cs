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
        private List<double> _currentXValues = new List<double>();
        private List<double> _currentYValues = new List<double>();
        private List<ChartSeries> _currentSeries = new List<ChartSeries>();

        private double _lastMinX;
        private double _lastMaxX;
        private double _lastMinY;
        private double _lastMaxY;

        private double _lastMarginLeft;
        private double _lastMarginTop;
        private double _lastPlotWidth;
        private double _lastPlotHeight;

        private Brush _currentLineBrush = Brushes.White;

        private class ChartSeries
        {
            public string Name { get; set; } = "";
            public List<double> Values { get; set; } = new List<double>();
            public Brush Brush { get; set; } = Brushes.White;
        }
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
                ChartTitleTextBlock.Text = "No data";
                return;
            }

            List<double> xValues = _trendBuffer.Points
                .Select(p => p.Time)
                .ToList();

            List<ChartSeries> series = new List<ChartSeries>();

            string title;
            string currentText;

            TrendPoint last = _trendBuffer.Points.Last();

            switch (TrendComboBox.SelectedIndex)
            {
                case 0:
                    title = "Temperature [°C]";

                    series.Add(new ChartSeries
                    {
                        Name = "Chamber Temperature",
                        Values = _trendBuffer.Points.Select(p => p.Temperature).ToList(),
                        Brush = Brushes.OrangeRed
                    });

                    series.Add(new ChartSeries
                    {
                        Name = "Material Temperature",
                        Values = _trendBuffer.Points.Select(p => p.MaterialTemperature).ToList(),
                        Brush = Brushes.Gold
                    });

                    series.Add(new ChartSeries
                    {
                        Name = "Temperature Setpoint",
                        Values = _trendBuffer.Points.Select(p => p.TemperatureSetpoint).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });

                    currentText =
                        $"Chamber: {last.Temperature:F1} °C   " +
                        $"Material: {last.MaterialTemperature:F1} °C   " +
                        $"Setpoint: {last.TemperatureSetpoint:F1} °C";
                    break;

                case 1:
                    title = "Pressure [kPa]";

                    series.Add(new ChartSeries
                    {
                        Name = "Pressure",
                        Values = _trendBuffer.Points.Select(p => p.Pressure).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });

                    series.Add(new ChartSeries
                    {
                        Name = "Pressure Setpoint",
                        Values = _trendBuffer.Points.Select(p => p.PressureSetpoint).ToList(),
                        Brush = Brushes.Gold
                    });

                    currentText =
                        $"Pressure: {last.Pressure:F1} kPa   " +
                        $"Setpoint: {last.PressureSetpoint:F1} kPa";
                    break;

                case 2:
                    title = "Moisture / Humidity [%]";

                    series.Add(new ChartSeries
                    {
                        Name = "Material Moisture",
                        Values = _trendBuffer.Points.Select(p => p.MaterialMoisture).ToList(),
                        Brush = Brushes.LimeGreen
                    });

                    series.Add(new ChartSeries
                    {
                        Name = "Air Humidity",
                        Values = _trendBuffer.Points.Select(p => p.AirHumidity).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });

                    currentText =
                        $"Material Moisture: {last.MaterialMoisture:F1} %   " +
                        $"Air Humidity: {last.AirHumidity:F1} %";
                    break;

                case 3:
                    title = "Drying Rate [%/min]";

                    series.Add(new ChartSeries
                    {
                        Name = "Drying Rate",
                        Values = _trendBuffer.Points.Select(p => p.DryingRate).ToList(),
                        Brush = Brushes.Gold
                    });

                    currentText = $"Drying Rate: {last.DryingRate:F2} %/min";
                    break;

                case 4:
                    title = "Actuators [%]";

                    series.Add(new ChartSeries
                    {
                        Name = "Heater Power",
                        Values = _trendBuffer.Points.Select(p => p.HeaterPower).ToList(),
                        Brush = Brushes.OrangeRed
                    });

                    series.Add(new ChartSeries
                    {
                        Name = "Pump Power",
                        Values = _trendBuffer.Points.Select(p => p.PumpPower).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });

                    series.Add(new ChartSeries
                    {
                        Name = "Fan Speed",
                        Values = _trendBuffer.Points.Select(p => p.FanSpeed).ToList(),
                        Brush = Brushes.LimeGreen
                    });

                    currentText =
                        $"Heater: {last.HeaterPower:F0} %   " +
                        $"Pump: {last.PumpPower:F0} %   " +
                        $"Fan: {last.FanSpeed:F0} %";
                    break;

                case 5:
                    title = "Energy";

                    series.Add(new ChartSeries
                    {
                        Name = "Total Energy",
                        Values = _trendBuffer.Points.Select(p => p.TotalEnergyKWh).ToList(),
                        Brush = Brushes.MediumPurple
                    });

                    currentText = $"Total Energy: {last.TotalEnergyKWh:F3} kWh";
                    break;

                default:
                    title = "Temperature [°C]";

                    series.Add(new ChartSeries
                    {
                        Name = "Chamber Temperature",
                        Values = _trendBuffer.Points.Select(p => p.Temperature).ToList(),
                        Brush = Brushes.OrangeRed
                    });

                    currentText = $"Chamber: {last.Temperature:F1} °C";
                    break;
            }

            ChartTitleTextBlock.Text = title;
            CurrentValueTextBlock.Text = currentText;
            CurrentValueTextBlock.Foreground = Brushes.White;

            DrawMultiLineChart(MainTrendCanvas, xValues, series);
        }

        private void DrawMultiLineChart(Canvas canvas, List<double> xValues, List<ChartSeries> series)
        {
            canvas.Children.Clear();

            if (xValues.Count < 2 || series.Count == 0)
                return;

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            if (width <= 0 || height <= 0)
                return;

            double marginLeft = 70;
            double marginRight = 30;
            double marginTop = 70;
            double marginBottom = 50;

            double plotWidth = width - marginLeft - marginRight;
            double plotHeight = height - marginTop - marginBottom;

            if (plotWidth <= 0 || plotHeight <= 0)
                return;

            double minX = xValues.Min();
            double maxX = xValues.Max();

            double minY = series
                .Where(s => s.Values.Count > 0)
                .Min(s => s.Values.Min());

            double maxY = series
                .Where(s => s.Values.Count > 0)
                .Max(s => s.Values.Max());

            if (Math.Abs(maxX - minX) < 0.0001)
                maxX = minX + 1;

            if (Math.Abs(maxY - minY) < 0.0001)
            {
                maxY = minY + 1;
                minY = minY - 1;
            }

            _currentXValues = xValues;
            _currentSeries = series;

            _lastMinX = minX;
            _lastMaxX = maxX;
            _lastMinY = minY;
            _lastMaxY = maxY;

            _lastMarginLeft = marginLeft;
            _lastMarginTop = marginTop;
            _lastPlotWidth = plotWidth;
            _lastPlotHeight = plotHeight;

            DrawAxes(canvas, marginLeft, marginTop, plotWidth, plotHeight, minY, maxY, minX, maxX);

            foreach (ChartSeries chartSeries in series)
            {
                Polyline line = new Polyline
                {
                    Stroke = chartSeries.Brush,
                    StrokeThickness = 3
                };

                int count = Math.Min(xValues.Count, chartSeries.Values.Count);

                for (int i = 0; i < count; i++)
                {
                    double x = marginLeft + (xValues[i] - minX) / (maxX - minX) * plotWidth;

                    double y = marginTop + plotHeight -
                               (chartSeries.Values[i] - minY) / (maxY - minY) * plotHeight;

                    line.Points.Add(new Point(x, y));
                }

                canvas.Children.Add(line);
            }

            DrawLegend(canvas, series, marginLeft, 15);
        }

        private void DrawLegend(Canvas canvas, List<ChartSeries> series, double left, double top)
        {
            double legendLeft = left + 10;
            double legendTop = top + 10;

            Border legendBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(220, 25, 25, 25)),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8)
            };

            StackPanel legendPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            legendBorder.Child = legendPanel;

            foreach (ChartSeries item in series)
            {
                StackPanel row = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 20, 0)
                };

                Rectangle colorBox = new Rectangle
                {
                    Width = 14,
                    Height = 4,
                    Fill = item.Brush,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 6, 0)
                };

                TextBlock label = new TextBlock
                {
                    Text = item.Name,
                    Foreground = Brushes.White,
                    FontSize = 13,
                    VerticalAlignment = VerticalAlignment.Center
                };

                row.Children.Add(colorBox);
                row.Children.Add(label);

                legendPanel.Children.Add(row);
            }

            Canvas.SetLeft(legendBorder, legendLeft);
            Canvas.SetTop(legendBorder, legendTop);

            canvas.Children.Add(legendBorder);
        }
        private void MainTrendCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_currentXValues.Count < 2 || _currentSeries.Count == 0)
                return;

            Point mousePosition = e.GetPosition(MainTrendCanvas);

            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            if (mouseX < _lastMarginLeft ||
                mouseX > _lastMarginLeft + _lastPlotWidth ||
                mouseY < _lastMarginTop ||
                mouseY > _lastMarginTop + _lastPlotHeight)
            {
                RemoveDataCursor();
                return;
            }

            int nearestIndex = FindNearestPointIndex(mouseX);

            if (nearestIndex < 0)
                return;

            double timeValue = _currentXValues[nearestIndex];

            double pointX =
                _lastMarginLeft +
                (timeValue - _lastMinX) / (_lastMaxX - _lastMinX) * _lastPlotWidth;

            DrawDataCursor(pointX, nearestIndex, timeValue);
        }
        private int FindNearestPointIndex(double mouseX)
        {
            int nearestIndex = -1;
            double smallestDistance = double.MaxValue;

            for (int i = 0; i < _currentXValues.Count; i++)
            {
                double pointX =
                    _lastMarginLeft +
                    (_currentXValues[i] - _lastMinX) / (_lastMaxX - _lastMinX) * _lastPlotWidth;

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
            RemoveDataCursor();

            // Vertical cursor line
            Line verticalLine = new Line
            {
                X1 = pointX,
                Y1 = _lastMarginTop,
                X2 = pointX,
                Y2 = _lastMarginTop + _lastPlotHeight,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 4, 4 },
                Tag = "DataCursor"
            };

            MainTrendCanvas.Children.Add(verticalLine);

            StackPanel infoStack = new StackPanel();

            TextBlock timeText = new TextBlock
            {
                Text = $"Time: {timeValue:F0} s",
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 6)
            };

            infoStack.Children.Add(timeText);

            foreach (ChartSeries series in _currentSeries)
            {
                if (nearestIndex >= series.Values.Count)
                    continue;

                double value = series.Values[nearestIndex];

                double pointY =
                    _lastMarginTop +
                    _lastPlotHeight -
                    (value - _lastMinY) / (_lastMaxY - _lastMinY) * _lastPlotHeight;

                // Small point marker for this series
                Ellipse pointMarker = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = series.Brush,
                    Stroke = Brushes.White,
                    StrokeThickness = 1,
                    Tag = "DataCursor"
                };

                Canvas.SetLeft(pointMarker, pointX - 5);
                Canvas.SetTop(pointMarker, pointY - 5);

                MainTrendCanvas.Children.Add(pointMarker);

                // Horizontal line from Y axis to the point
                Line horizontalLine = new Line
                {
                    X1 = _lastMarginLeft,
                    Y1 = pointY,
                    X2 = pointX,
                    Y2 = pointY,
                    Stroke = series.Brush,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 4, 4 },
                    Tag = "DataCursor"
                };

                MainTrendCanvas.Children.Add(horizontalLine);

                TextBlock valueText = new TextBlock
                {
                    Text = $"{series.Name}: {value:F2}",
                    Foreground = series.Brush,
                    FontSize = 13,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                infoStack.Children.Add(valueText);
            }

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
            double infoTop = _lastMarginTop + 15;

            if (infoLeft + 260 > MainTrendCanvas.ActualWidth)
                infoLeft = pointX - 275;

            Canvas.SetLeft(infoBox, infoLeft);
            Canvas.SetTop(infoBox, infoTop);

            MainTrendCanvas.Children.Add(infoBox);
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
            // Main axes
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

            int xTickCount = 6;
            int yTickCount = 6;

            // Y-axis ticks + labels + horizontal grid lines
            for (int i = 0; i < yTickCount; i++)
            {
                double ratio = (double)i / (yTickCount - 1);
                double y = marginTop + plotHeight - ratio * plotHeight;
                double value = minY + ratio * (maxY - minY);

                Line tick = new Line
                {
                    X1 = marginLeft - 5,
                    Y1 = y,
                    X2 = marginLeft,
                    Y2 = y,
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };
                canvas.Children.Add(tick);

                Line gridLine = new Line
                {
                    X1 = marginLeft,
                    Y1 = y,
                    X2 = marginLeft + plotWidth,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                    StrokeThickness = 0.8
                };
                canvas.Children.Add(gridLine);

                TextBlock label = new TextBlock
                {
                    Text = value.ToString("F1"),
                    Foreground = Brushes.White,
                    FontSize = 12
                };

                Canvas.SetLeft(label, 10);
                Canvas.SetTop(label, y - 10);
                canvas.Children.Add(label);
            }

            // X-axis ticks + labels + vertical grid lines
            for (int i = 0; i < xTickCount; i++)
            {
                double ratio = (double)i / (xTickCount - 1);
                double x = marginLeft + ratio * plotWidth;
                double value = minX + ratio * (maxX - minX);

                Line tick = new Line
                {
                    X1 = x,
                    Y1 = marginTop + plotHeight,
                    X2 = x,
                    Y2 = marginTop + plotHeight + 5,
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };
                canvas.Children.Add(tick);

                Line gridLine = new Line
                {
                    X1 = x,
                    Y1 = marginTop,
                    X2 = x,
                    Y2 = marginTop + plotHeight,
                    Stroke = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                    StrokeThickness = 0.8
                };
                canvas.Children.Add(gridLine);

                TextBlock label = new TextBlock
                {
                    Text = value.ToString("F0"),
                    Foreground = Brushes.White,
                    FontSize = 12
                };

                Canvas.SetLeft(label, x - 10);
                Canvas.SetTop(label, marginTop + plotHeight + 5);
                canvas.Children.Add(label);
            }

            TextBlock timeText = new TextBlock
            {
                Text = "Time [s]",
                Foreground = Brushes.White,
                FontSize = 14
            };

            Canvas.SetLeft(timeText, marginLeft + plotWidth / 2 - 30);
            Canvas.SetTop(timeText, marginTop + plotHeight + 25);
            canvas.Children.Add(timeText);
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
        private void MainTrendCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RemoveDataCursor();
        }
    }
}