using UnityEngine;

namespace AssemblyGame
{
    public class PointsFaultCreator : FaultCreator
    {
        public override IFault CreateFault(GameObject faultPrefab)
        {
            GameObject faultObject = Object.Instantiate(faultPrefab);
            PointsFault fault = faultObject.AddComponent<PointsFault>();
            faultObject.AddComponent<CanvasRenderer>();
            faultObject.AddComponent<UnityEngine.UI.Image>();
            RectTransform rectTransform = faultObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = GetRandomSpawnPosition();
            return fault;
        }
    }
}