using DiplomWork_Ivan_2026.Trends;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

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

            DrawTrends();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            DrawTrends();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void DrawTrends()
        {
            DrawLineChart(
                TemperatureCanvas,
                _trendBuffer.Points.Select(p => p.Time).ToList(),
                _trendBuffer.Points.Select(p => p.Temperature).ToList(),
                Brushes.OrangeRed);

            DrawLineChart(
                PressureCanvas,
                _trendBuffer.Points.Select(p => p.Time).ToList(),
                _trendBuffer.Points.Select(p => p.Pressure).ToList(),
                Brushes.DeepSkyBlue);

            DrawLineChart(
                MoistureCanvas,
                _trendBuffer.Points.Select(p => p.Time).ToList(),
                _trendBuffer.Points.Select(p => p.MaterialMoisture).ToList(),
                Brushes.LimeGreen);
        }

        private void DrawLineChart(Canvas canvas, List<double> xValues, List<double> yValues, Brush lineBrush)
        {
            canvas.Children.Clear();

            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            if (width <= 0 || height <= 0 || xValues.Count < 2 || yValues.Count < 2)
                return;

            double marginLeft = 50;
            double marginRight = 20;
            double marginTop = 20;
            double marginBottom = 35;

            double chartWidth = width - marginLeft - marginRight;
            double chartHeight = height - marginTop - marginBottom;

            if (chartWidth <= 0 || chartHeight <= 0)
                return;

            double minX = xValues.Min();
            double maxX = xValues.Max();

            double minY = yValues.Min();
            double maxY = yValues.Max();

            if (Math.Abs(maxX - minX) < 0.001)
                maxX = minX + 1;

            if (Math.Abs(maxY - minY) < 0.001)
            {
                maxY += 1;
                minY -= 1;
            }

            DrawAxes(canvas, width, height, marginLeft, marginRight, marginTop, marginBottom, minY, maxY);

            Polyline polyline = new Polyline
            {
                Stroke = lineBrush,
                StrokeThickness = 2
            };

            for (int i = 0; i < xValues.Count; i++)
            {
                double x = marginLeft + ((xValues[i] - minX) / (maxX - minX)) * chartWidth;
                double y = marginTop + (1 - ((yValues[i] - minY) / (maxY - minY))) * chartHeight;

                polyline.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(polyline);

            DrawLastValue(canvas, yValues.Last(), lineBrush);
        }

        private void DrawAxes(
            Canvas canvas,
            double width,
            double height,
            double marginLeft,
            double marginRight,
            double marginTop,
            double marginBottom,
            double minY,
            double maxY)
        {
            double xAxisY = height - marginBottom;
            double yAxisX = marginLeft;

            Line yAxis = new Line
            {
                X1 = yAxisX,
                Y1 = marginTop,
                X2 = yAxisX,
                Y2 = xAxisY,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };

            Line xAxis = new Line
            {
                X1 = marginLeft,
                Y1 = xAxisY,
                X2 = width - marginRight,
                Y2 = xAxisY,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };

            canvas.Children.Add(yAxis);
            canvas.Children.Add(xAxis);

            TextBlock maxLabel = new TextBlock
            {
                Text = maxY.ToString("F1"),
                Foreground = Brushes.LightGray,
                FontSize = 12
            };
            Canvas.SetLeft(maxLabel, 5);
            Canvas.SetTop(maxLabel, marginTop - 8);
            canvas.Children.Add(maxLabel);

            TextBlock minLabel = new TextBlock
            {
                Text = minY.ToString("F1"),
                Foreground = Brushes.LightGray,
                FontSize = 12
            };
            Canvas.SetLeft(minLabel, 5);
            Canvas.SetTop(minLabel, xAxisY - 12);
            canvas.Children.Add(minLabel);

            TextBlock timeLabel = new TextBlock
            {
                Text = "Time [s]",
                Foreground = Brushes.LightGray,
                FontSize = 12
            };
            Canvas.SetLeft(timeLabel, width / 2 - 25);
            Canvas.SetTop(timeLabel, height - 25);
            canvas.Children.Add(timeLabel);
        }

        private void DrawLastValue(Canvas canvas, double value, Brush brush)
        {
            TextBlock valueText = new TextBlock
            {
                Text = $"Current: {value:F1}",
                Foreground = brush,
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetRight(valueText, 20);
            Canvas.SetTop(valueText, 10);

            canvas.Children.Add(valueText);
        }
    }
}