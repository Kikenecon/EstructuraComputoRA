namespace AssemblyGame
{
    public class Level1PartsFactory : IAssemblyPartsFactory
    {
        public bool CreateCooler()
        {
            return true;
        }

        public bool CreateCPU()
        {
            return true;
        }

        public bool CreateRAM()
        {
            return true;
        }

        public bool CreateRAM_Advanced()
        {
            return false;
        }

        public bool CreateGraphicsCard()
        {
            return false;
        }

        public bool CreatePowerSupply()
        {
            return false;
        }
    }
}