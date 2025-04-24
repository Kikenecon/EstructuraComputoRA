using UnityEngine;

namespace AssemblyGame
{
    public class ComputerProduct
    {
        public bool HasCooler { get; private set; }
        public bool HasCPU { get; private set; }
        public bool HasRAM { get; private set; }
        public bool HasGraphicsCard { get; private set; }
        public bool HasPowerSupply { get; private set; }

        private string[] requiredParts;

        public ComputerProduct()
        {
            Reset();
        }

        public void SetRequiredParts(string[] parts)
        {
            requiredParts = parts;
        }

        public void Reset()
        {
            HasCooler = false;
            HasCPU = false;
            HasRAM = false;
            HasGraphicsCard = false;
            HasPowerSupply = false;
            requiredParts = new string[] { };
        }

        public void AddPart(string partType)
        {
            switch (partType)
            {
                case "Cooler":
                    HasCooler = true;
                    break;
                case "CPU":
                    HasCPU = true;
                    break;
                case "RAM":
                case "RAM_Advanced": // Aceptar ambas como RAM válida
                    HasRAM = true;
                    break;
                case "GraphicsCard":
                    HasGraphicsCard = true;
                    break;
                case "PowerSupply":
                    HasPowerSupply = true;
                    break;
            }
            Debug.Log($"{partType} añadida a la computadora.");
        }

        public bool HasPart(string partType)
        {
            return partType switch
            {
                "Cooler" => HasCooler,
                "CPU" => HasCPU,
                "RAM" => HasRAM,
                "RAM_Advanced" => HasRAM, // Aceptar ambas como RAM
                "GraphicsCard" => HasGraphicsCard,
                "PowerSupply" => HasPowerSupply,
                _ => false
            };
        }

        public bool IsFullyAssembled()
        {
            foreach (var part in requiredParts)
            {
                if (!HasPart(part))
                {
                    return false;
                }
            }
            return true;
        }
    }
}