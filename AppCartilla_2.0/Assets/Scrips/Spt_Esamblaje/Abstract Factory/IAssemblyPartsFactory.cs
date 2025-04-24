namespace AssemblyGame
{
    public interface IAssemblyPartsFactory
    {
        bool CreateCooler();
        bool CreateCPU();
        bool CreateRAM();
        bool CreateRAM_Advanced();
        bool CreateGraphicsCard();
        bool CreatePowerSupply();
    }
}