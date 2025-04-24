using UnityEngine;

namespace AssemblyGame
{
    public class AssemblyPart : MonoBehaviour
    {
        [SerializeField] private string partType;
        public string PartType => partType;

        private void Awake()
        {
            try
            {
                gameObject.tag = "AssemblyPart";
            }
            catch (UnityException)
            {
                Debug.LogError("El tag 'AssemblyPart' no está definido. Por favor, crea este tag en el editor de Unity (Tags and Layers).");
            }

            if (GetComponent<PartDragHandler>() == null)
            {
                gameObject.AddComponent<PartDragHandler>();
            }
            if (GetComponent<CanvasGroup>() == null)
            {
                gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
}