namespace AssemblyGame
{
    public class Level2PartsFactory : IAssemblyPartsFactory
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
            return true;
        }

        public bool CreateGraphicsCard()
        {
            return true;
        }

        public bool CreatePowerSupply()
        {
            return false;
        }
    }
}