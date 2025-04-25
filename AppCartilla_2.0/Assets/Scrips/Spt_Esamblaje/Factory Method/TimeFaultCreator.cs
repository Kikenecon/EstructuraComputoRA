using UnityEngine;

namespace AssemblyGame
{
    public class TimeFaultCreator : FaultCreator
    {
        public override IFault CreateFault(GameObject faultPrefab)
        {
            GameObject faultObject = Object.Instantiate(faultPrefab);
            TimeFault fault = faultObject.AddComponent<TimeFault>();
            faultObject.AddComponent<CanvasRenderer>();
            faultObject.AddComponent<UnityEngine.UI.Image>();
            RectTransform rectTransform = faultObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = GetRandomSpawnPosition();
            return fault;
        }
    }
}