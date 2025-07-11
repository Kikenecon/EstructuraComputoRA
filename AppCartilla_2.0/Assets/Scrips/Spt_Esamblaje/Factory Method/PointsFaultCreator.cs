using UnityEngine;

namespace AssemblyGame
{
    public class PointsFaultCreator : FaultCreator
    {
        public override IFault CreateFault(GameObject faultPrefab)
        {
            GameObject faultObject = Object.Instantiate(faultPrefab);
            PointsFault fault = faultObject.GetComponent<PointsFault>() ?? faultObject.AddComponent<PointsFault>();
            faultObject.transform.position = GetRandomSpawnPosition();
            return fault;
        }
    }
}