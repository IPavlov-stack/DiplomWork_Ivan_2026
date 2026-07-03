namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
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
    }
}
