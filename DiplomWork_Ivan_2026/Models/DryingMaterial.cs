using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomWork_Ivan_2026.Models
{
    public class DryingMaterial
    {
        public string Name { get; set; } = "";

        public double InitialMoisture { get; set; }      // [%]
        public double TargetMoisture { get; set; }       // [%]
        public double MaxTemperature { get; set; }       // [°C]
        public double DryingCoefficient { get; set; }
        public double MaterialMassKg { get; set; } = 10.0;
        public override string ToString()
        {
            return Name;
        }
    }
}
