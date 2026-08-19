using DiplomWork_Ivan_2026.Enums;
using DiplomWork_Ivan_2026.Models;
using DiplomWork_Ivan_2026.Simulation;

using System.Linq;

namespace DiplomWork_Ivan_2026.Services
{
    public class AlarmService
    {
        public List<AlarmInfo> ActiveAlarms { get; } = new List<AlarmInfo>();
        public List<AlarmInfo> AlarmHistory { get; } = new List<AlarmInfo>();

        public void CheckAlarms(VacuumDryerProcess process, ProcessSettings settings)
        {
            ActiveAlarms.Clear();

            var state = process.State;
            var material = process.SelectedMaterial;

            if (material == null)
                return;

            if (state.MeasuredMaterialTemperature > material.MaxTemperature)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = AlarmType.HighTemperature,
                    Severity = AlarmSeverity.Critical,
                    Message = $"High material temperature! Current: {state.MeasuredMaterialTemperature:F1} °C, Limit: {material.MaxTemperature:F1} °C"
                });

            }

            if (state.SafetyInterlockActive)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = state.EmergencyStopActive
                        ? AlarmType.EmergencyStop
                        : AlarmType.SafetyInterlock,
                    Severity = AlarmSeverity.Critical,
                    Message = state.SafetyInterlockReason
                });
            }

            if (process.HasSensorFault)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = AlarmType.SensorFault,
                    Severity = AlarmSeverity.Critical,
                    Message = "A critical virtual sensor is in a fault state."
                });
            }

            if (settings.TemperatureSetpoint > material.MaxTemperature)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = AlarmType.SetpointAboveMaterialLimit,
                    Severity = AlarmSeverity.Warning,
                    Message = $"Temperature setpoint exceeds material limit. Setpoint: {settings.TemperatureSetpoint:F1} °C, Limit: {material.MaxTemperature:F1} °C"
                });
            }

            if (state.MeasuredPressure > settings.PressureSetpoint + 30)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = AlarmType.PressureTooHigh,
                    Severity = AlarmSeverity.Warning,
                    Message = $"Pressure is too high. Current: {state.MeasuredPressure:F1} kPa"
                });
            }

            if (state.MeasuredPressure < 5)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = AlarmType.PressureTooLow,
                    Severity = AlarmSeverity.Critical,
                    Message = $"Pressure is too low. Current: {state.MeasuredPressure:F1} kPa"
                });
            }

            if (state.IsCompleted)
            {
                AddAlarm(new AlarmInfo
                {
                    Type = AlarmType.ProcessCompleted,
                    Severity = AlarmSeverity.Info,
                    Message = "Drying process completed."
                });
            }
            UpdateHistoryStatus();
        }
        private void AddAlarm(AlarmInfo alarm)
        {
            ActiveAlarms.Add(alarm);

            bool alreadyInHistory = AlarmHistory.Any(a =>
                a.Type == alarm.Type &&
                a.IsActive);

            if (!alreadyInHistory)
            {
                AlarmHistory.Insert(0, alarm);
            }
        }
        private void UpdateHistoryStatus()
        {
            foreach (var historyAlarm in AlarmHistory)
            {
                bool stillActive = ActiveAlarms.Any(a => a.Type == historyAlarm.Type);
                historyAlarm.IsActive = stillActive;
            }
        }
    }
}
