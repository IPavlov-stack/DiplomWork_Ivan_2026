using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DiplomWork_Ivan_2026.Trends
{
    public static class TrendChartDataBuilder
    {
        public static TrendChartData Build(IReadOnlyList<TrendPoint> points, int selectedTrendIndex)
        {
            TrendPoint last = points[points.Count - 1];

            TrendChartData data = new TrendChartData
            {
                XValues = points.Select(p => p.Time).ToList()
            };

            switch (selectedTrendIndex)
            {
                case 0:
                    data.Title = "Temperature [°C]";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Chamber Temperature",
                        Values = points.Select(p => p.Temperature).ToList(),
                        Brush = Brushes.OrangeRed
                    });
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Material Temperature",
                        Values = points.Select(p => p.MaterialTemperature).ToList(),
                        Brush = Brushes.Gold
                    });
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Temperature Setpoint",
                        Values = points.Select(p => p.TemperatureSetpoint).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });
                    data.CurrentText =
                        $"Chamber: {last.Temperature:F1} °C   " +
                        $"Material: {last.MaterialTemperature:F1} °C   " +
                        $"Setpoint: {last.TemperatureSetpoint:F1} °C";
                    break;

                case 1:
                    data.Title = "Pressure [kPa]";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Pressure",
                        Values = points.Select(p => p.Pressure).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Pressure Setpoint",
                        Values = points.Select(p => p.PressureSetpoint).ToList(),
                        Brush = Brushes.Gold
                    });
                    data.CurrentText =
                        $"Pressure: {last.Pressure:F1} kPa   " +
                        $"Setpoint: {last.PressureSetpoint:F1} kPa";
                    break;

                case 2:
                    data.Title = "Moisture / Humidity [%]";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Material Moisture",
                        Values = points.Select(p => p.MaterialMoisture).ToList(),
                        Brush = Brushes.LimeGreen
                    });
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Air Humidity",
                        Values = points.Select(p => p.AirHumidity).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });
                    data.CurrentText =
                        $"Material Moisture: {last.MaterialMoisture:F1} %   " +
                        $"Air Humidity: {last.AirHumidity:F1} %";
                    break;

                case 3:
                    data.Title = "Drying Rate [%/min]";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Drying Rate",
                        Values = points.Select(p => p.DryingRate).ToList(),
                        Brush = Brushes.Gold
                    });
                    data.CurrentText = $"Drying Rate: {last.DryingRate:F2} %/min";
                    break;

                case 4:
                    data.Title = "Actuators [%]";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Heater Power",
                        Values = points.Select(p => p.HeaterPower).ToList(),
                        Brush = Brushes.OrangeRed
                    });
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Pump Power",
                        Values = points.Select(p => p.PumpPower).ToList(),
                        Brush = Brushes.DeepSkyBlue
                    });
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Fan Speed",
                        Values = points.Select(p => p.FanSpeed).ToList(),
                        Brush = Brushes.LimeGreen
                    });
                    data.CurrentText =
                        $"Heater: {last.HeaterPower:F0} %   " +
                        $"Pump: {last.PumpPower:F0} %   " +
                        $"Fan: {last.FanSpeed:F0} %";
                    break;

                case 5:
                    data.Title = "Energy";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Total Energy",
                        Values = points.Select(p => p.TotalEnergyKWh).ToList(),
                        Brush = Brushes.MediumPurple
                    });
                    data.CurrentText = $"Total Energy: {last.TotalEnergyKWh:F3} kWh";
                    break;

                default:
                    data.Title = "Temperature [°C]";
                    data.Series.Add(new ChartSeries
                    {
                        Name = "Chamber Temperature",
                        Values = points.Select(p => p.Temperature).ToList(),
                        Brush = Brushes.OrangeRed
                    });
                    data.CurrentText = $"Chamber: {last.Temperature:F1} °C";
                    break;
            }

            return data;
        }
    }
}
