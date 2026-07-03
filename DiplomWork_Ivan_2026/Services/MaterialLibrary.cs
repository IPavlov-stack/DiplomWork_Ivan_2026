using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiplomWork_Ivan_2026.Models;

namespace DiplomWork_Ivan_2026.Services
{
    public static class MaterialLibrary
    {
            public static List<DryingMaterial> GetMaterials()
            {
                return new List<DryingMaterial>
               {
                new DryingMaterial
                {
                    Name = "Herbs",
                    InitialMoisture = 70,
                    TargetMoisture = 10,
                    MaxTemperature = 45,
                    DryingCoefficient = 0.8,
                    MaterialMassKg = 5
                },

                new DryingMaterial
                {
                    Name = "Grain",
                    InitialMoisture = 25,
                    TargetMoisture = 13,
                    MaxTemperature = 60,
                    DryingCoefficient = 0.5,
                    MaterialMassKg = 20
                },

                new DryingMaterial
                {
                    Name = "Wood",
                    InitialMoisture = 50,
                    TargetMoisture = 12,
                    MaxTemperature = 80,
                    DryingCoefficient = 0.3,
                    MaterialMassKg = 15
                },

                new DryingMaterial
                {
                    Name = "Fruits",
                    InitialMoisture = 80,
                    TargetMoisture = 15,
                    MaxTemperature = 55,
                    DryingCoefficient = 0.6,
                    MaterialMassKg = 8
                }
               };
            }
    }
    
}
