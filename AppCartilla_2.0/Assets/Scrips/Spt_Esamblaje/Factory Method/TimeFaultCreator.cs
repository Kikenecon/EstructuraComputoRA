using UnityEngine;

namespace AssemblyGame
{
    public class TimeFaultCreator : FaultCreator
    {
        public override IFault CreateFault(GameObject faultPrefab)
        {
            GameObject faultObject = Object.Instantiate(faultPrefab);
            TimeFault fault = faultObject.GetComponent<TimeFault>() ?? faultObject.AddComponent<TimeFault>();
            faultObject.transform.position = GetRandomSpawnPosition();
            return fault;
        }
    }
}