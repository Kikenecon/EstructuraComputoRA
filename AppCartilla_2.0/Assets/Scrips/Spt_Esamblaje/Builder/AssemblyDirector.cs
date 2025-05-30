//namespace AssemblyGame
//{
//    public class AssemblyDirector
//    {
//        private IComputerBuilder builder;

//        public AssemblyDirector(IComputerBuilder builder)
//        {
//            this.builder = builder;
//        }

//        public void ChangeBuilder(IComputerBuilder builder)
//        {
//            this.builder = builder;
//        }

//        public bool TryAddPart(string partType)
//        {
//            return partType switch
//            {
//                "Cooler" => builder.AddCooler(),
//                "CPU" => builder.AddCPU(),
//                "RAM" => builder.AddRAM(),
//                "RAM_Advanced" => builder.AddRAM(),
//                "GraphicsCard" => builder.AddGraphicsCard(),
//                "PowerSupply" => builder.AddPowerSupply(),
//                _ => false
//            };
//        }

//        public void Reset()
//        {
//            builder.Reset();
//        }

//        public ComputerProduct GetResult()
//        {
//            return builder.GetResult();
//        }
//    }
//}
namespace AssemblyGame
{
    public class AssemblyDirector
    {
        private IComputerBuilder builder;

        public AssemblyDirector(IComputerBuilder builder)
        {
            this.builder = builder;
        }

        public void ChangeBuilder(IComputerBuilder builder)
        {
            this.builder = builder;
        }

        public bool TryAddPart(string partType)
        {
            return partType switch
            {
                "Cooler" => builder.AddCooler(),
                "CPU" => builder.AddCPU(),
                "RAM" => builder.AddRAM(),
                "RAM_Advanced" => builder.AddRAM(),
                "GraphicsCard" => builder.AddGraphicsCard(),
                "PowerSupply" => builder.AddPowerSupply(),
                _ => false
            };
        }

        public void Reset()
        {
            builder.Reset();
        }

        public ComputerProduct GetResult()
        {
            return builder.GetResult();
        }
    }
}