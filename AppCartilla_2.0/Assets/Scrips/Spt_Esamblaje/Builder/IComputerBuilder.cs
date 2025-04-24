namespace AssemblyGame
{
    public interface IComputerBuilder
    {
        void Reset();
        bool AddCooler();
        bool AddCPU();
        bool AddRAM();
        bool AddGraphicsCard();
        bool AddPowerSupply();
        ComputerProduct GetResult();
    }
}