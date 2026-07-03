using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiplomWork_Ivan_2026.Devices;
using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026.Simulation
{
    public class VacuumDryerProcess
    {
        private const double MaxAirFlowRate = 200.0; // m³/h
        private const double HeaterNominalPowerKw = 5.0;
        private const double PumpNominalPowerKw = 1.5;
        private const double FanNominalPowerKw = 0.5;
        public VacuumDryerState State { get; private set; } = new VacuumDryerState();
        public Heater Heater { get; } = new Heater();
        public VacuumPump Pump { get; } = new VacuumPump();
        public Fan Fan { get; } = new Fan();
        public DryingMaterial? SelectedMaterial { get; private set; }



        public void LoadMaterial(DryingMaterial material)
        {
            SelectedMaterial = material;

            State = new VacuumDryerState
            {
                Temperature = 20.0,
                MaterialTemperature = 20.0,
                Pressure = 101.3,
                AirHumidity = 50.0,
                MaterialMoisture = material.InitialMoisture,
                ElapsedTime = 0.0,
                TotalEnergyKWh = 0.0,
                EvaporatedWaterKg = 0.0,
                EfficiencyKgPerKWh = 0.0,
                IsCompleted = false
            };
        }

        public void Update(double deltaTime, ProcessSettings settings)
        {
            if (SelectedMaterial == null || State.IsCompleted)
                return;

            State.ElapsedTime += deltaTime;

            UpdateTemperature(deltaTime, settings);
            UpdatePressure(deltaTime, settings);

            State.HeaterPower = Heater.Power;
            State.VacuumPumpPower = Pump.Power;
            State.FanSpeed = Fan.Speed;

            UpdateCalculatedValues(settings);
            UpdateMoisture(deltaTime);

            UpdateEnergyConsumption(deltaTime);
            UpdateEvaporatedWaterAndEfficiency();


            if (State.MaterialMoisture <= SelectedMaterial.TargetMoisture)
            {
                State.MaterialMoisture = SelectedMaterial.TargetMoisture;
                State.IsCompleted = true;

                Heater.TurnOff();
                Pump.TurnOff();
                Fan.TurnOff();
            }
        }

        private void UpdateTemperature(double deltaTime, ProcessSettings settings)
        {
            // 1. Chamber temperature - directly affected by heater
            double heatingEffect = Heater.Power / 100.0 * 0.8;
            double coolingLoss = (State.Temperature - settings.AmbientTemperature) * 0.01;

            State.Temperature += (heatingEffect - coolingLoss) * deltaTime;

            // 2. Material temperature - follows chamber temperature more slowly
            double heatTransferCoefficient = 0.02;

            double materialHeating =
                (State.Temperature - State.MaterialTemperature) *
                heatTransferCoefficient;

            State.MaterialTemperature += materialHeating * deltaTime;
        }

        private void UpdatePressure(double deltaTime, ProcessSettings settings)
        {
            double pumpEffect = Pump.Power / 100.0 * 1.2;
            double leakageEffect = (settings.AmbientPressure - State.Pressure) * 0.005;

            State.Pressure += (-pumpEffect + leakageEffect) * deltaTime;

            if (State.Pressure < 5.0)
                State.Pressure = 5.0;

            if (State.Pressure > settings.AmbientPressure)
                State.Pressure = settings.AmbientPressure;
        }

        private void UpdateMoisture(double deltaTime)
        {
            if (SelectedMaterial == null)
                return;

            double temperatureFactor = Math.Max(0, State.MaterialTemperature - 25.0) / 100.0;
            double vacuumFactor = Math.Max(0, 101.3 - State.Pressure) / 101.3;
            double fanFactor = Fan.Speed / 100.0;

            double dryingRate =
                SelectedMaterial.DryingCoefficient *
                temperatureFactor *
                (0.5 + vacuumFactor) *
                (0.5 + fanFactor) *
                0.8;

            State.DryingRate = dryingRate;
            State.MaterialMoisture -= dryingRate * deltaTime;

            if (State.MaterialMoisture < 0)
                State.MaterialMoisture = 0;

            State.AirHumidity += dryingRate * 2.0 * deltaTime;
            State.AirHumidity -= fanFactor * 0.2 * deltaTime;

            State.AirHumidity = Math.Clamp(State.AirHumidity, 0.0, 100.0);
        }
        private void UpdateCalculatedValues(ProcessSettings settings)
        {
            State.VacuumLevel =
                (settings.AmbientPressure - State.Pressure) / settings.AmbientPressure * 100.0;

            State.VacuumLevel = Math.Clamp(State.VacuumLevel, 0.0, 100.0);

            State.AirFlowRate =
                State.FanSpeed / 100.0 * MaxAirFlowRate;
        }
        private void UpdateEnergyConsumption(double deltaTime)
        {
            double deltaTimeHours = deltaTime / 3600.0;

            double heaterEnergy =
                HeaterNominalPowerKw *
                (Heater.Power / 100.0) *
                deltaTimeHours;

            double pumpEnergy =
                PumpNominalPowerKw *
                (Pump.Power / 100.0) *
                deltaTimeHours;

            double fanEnergy =
                FanNominalPowerKw *
                (Fan.Speed / 100.0) *
                deltaTimeHours;

            State.TotalEnergyKWh += heaterEnergy + pumpEnergy + fanEnergy;
        }
        private void UpdateEvaporatedWaterAndEfficiency()
        {
            if (SelectedMaterial == null)
                return;

            State.EvaporatedWaterKg =
                SelectedMaterial.MaterialMassKg *
                (SelectedMaterial.InitialMoisture - State.MaterialMoisture) /
                100.0;

            if (State.TotalEnergyKWh > 0.001)
            {
                State.EfficiencyKgPerKWh =
                    State.EvaporatedWaterKg / State.TotalEnergyKWh;
            }
            else
            {
                State.EfficiencyKgPerKWh = 0.0;
            }
        }
    }
}
