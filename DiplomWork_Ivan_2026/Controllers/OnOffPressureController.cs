using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiplomWork_Ivan_2026.Devices;
using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026.Controllers
{
    public class OnOffPressureController
    {
        public void Update(VacuumDryerState state, ProcessSettings settings, VacuumPump pump)
        {
            if (state.Pressure > settings.PressureSetpoint + settings.PressureHysteresis)
            {
                pump.TurnOn();
            }
            else if (state.Pressure < settings.PressureSetpoint - settings.PressureHysteresis)
            {
                pump.TurnOff();
            }

            state.VacuumPumpPower = pump.Power;
        }
    }
}
