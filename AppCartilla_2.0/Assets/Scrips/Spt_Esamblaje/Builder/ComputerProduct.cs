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
            Debug.Log($"Partes requeridas establecidas: {string.Join(", ", requiredParts)}");
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
                case "RAM_Advanced":
                    HasRAM = true;
                    break;
                case "GraphicsCard":
                    HasGraphicsCard = true;
                    break;
                case "PowerSupply":
                    HasPowerSupply = true;
                    break;
            }
            Debug.Log($"{partType} añadida a la computadora. Estado: {GetPartsStatus()}");
        }

        public bool HasPart(string partType)
        {
            return partType switch
            {
                "Cooler" => HasCooler,
                "CPU" => HasCPU,
                "RAM" => HasRAM,
                "RAM_Advanced" => HasRAM,
                "GraphicsCard" => HasGraphicsCard,
                "PowerSupply" => HasPowerSupply,
                _ => false
            };
        }

        public bool IsFullyAssembled()
        {
            if (requiredParts == null || requiredParts.Length == 0)
            {
                Debug.LogWarning("No se han establecido partes requeridas.");
                return false;
            }

            foreach (var part in requiredParts)
            {
                if (!HasPart(part))
                {
                    Debug.Log($"Falta la parte {part}. Estado actual: {GetPartsStatus()}");
                    return false;
                }
            }
            Debug.Log("Computadora completamente ensamblada.");
            return true;
        }

        public string GetPartsStatus()
        {
            return $"Cooler: {HasCooler}, CPU: {HasCPU}, RAM: {HasRAM}, GraphicsCard: {HasGraphicsCard}, PowerSupply: {HasPowerSupply}, Required: {string.Join(", ", requiredParts ?? new string[] { })}";
        }
    }
}