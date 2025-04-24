using UnityEngine;

namespace AssemblyGame
{
    public class ComputerConcreteBuilder : IComputerBuilder
    {
        private ComputerProduct product;
        private string[] assemblyOrder;

        public ComputerConcreteBuilder()
        {
            product = new ComputerProduct();
            assemblyOrder = new string[] { "Cooler", "CPU", "RAM" };
            product.SetRequiredParts(assemblyOrder);
        }

        public void SetAssemblyOrder(string[] order)
        {
            assemblyOrder = order;
            product.SetRequiredParts(order);
        }

        public void Reset()
        {
            product = new ComputerProduct(); // Crear un nuevo producto para asegurar un estado limpio
            product.SetRequiredParts(assemblyOrder);
        }

        public bool AddCooler()
        {
            return TryAddPart("Cooler");
        }

        public bool AddCPU()
        {
            return TryAddPart("CPU");
        }

        public bool AddRAM()
        {
            return TryAddPart("RAM") || TryAddPart("RAM_Advanced");
        }

        public bool AddGraphicsCard()
        {
            return TryAddPart("GraphicsCard");
        }

        public bool AddPowerSupply()
        {
            return TryAddPart("PowerSupply");
        }

        private bool TryAddPart(string partType)
        {
            string expectedPart = partType == "RAM_Advanced" ? "RAM" : partType;
            int partIndex = System.Array.IndexOf(assemblyOrder, expectedPart);
            if (partIndex == -1)
            {
                Debug.Log($"Error: {partType} no es parte de este nivel.");
                return false;
            }

            for (int i = 0; i < partIndex; i++)
            {
                if (!product.HasPart(assemblyOrder[i]))
                {
                    Debug.Log($"Error: Primero debes colocar {assemblyOrder[i]} antes de {partType}.");
                    return false;
                }
            }

            if (product.HasPart(expectedPart))
            {
                Debug.Log($"Error: {expectedPart} ya ha sido colocada.");
                return false;
            }

            product.AddPart(partType);
            return true;
        }

        public ComputerProduct GetResult()
        {
            return product;
        }
    }
}