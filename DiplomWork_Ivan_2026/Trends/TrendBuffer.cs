using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026.Trends
{
    public class TrendBuffer
    {
        private readonly int _maxPoints;

        public List<TrendPoint> Points { get; } = new List<TrendPoint>();

        public TrendBuffer(int maxPoints = 300)
        {
            _maxPoints = maxPoints;
        }

        public void AddPoint(VacuumDryerState state)
        {
            Points.Add(new TrendPoint
            {
                Time = state.ElapsedTime,
                Temperature = state.Temperature,
                Pressure = state.Pressure,
                MaterialMoisture = state.MaterialMoisture,
                AirHumidity = state.AirHumidity
            });

            if (Points.Count > _maxPoints)
            {
                Points.RemoveAt(0);
            }
        }

        public void Clear()
        {
            Points.Clear();
        }
    }
}