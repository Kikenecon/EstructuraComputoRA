using UnityEngine;
using UnityEngine.EventSystems;

namespace AssemblyGame
{
    public abstract class Fault : MonoBehaviour, IFault, IPointerClickHandler
    {
        private float timeToLive = 5f;
        private float currentTime = 0f;
        private bool hasBeenClicked = false;

        private Vector2 targetPosition;
        private float speed = 200f; // Velocidad de movimiento
        private float changeDirectionTime = 1.5f; // Tiempo entre cambios de dirección
        private float directionTimer = 0f;

        private RectTransform rectTransform;
        private Vector2 canvasSize;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }

            if (GetComponent<UnityEngine.UI.Image>() == null)
            {
                gameObject.AddComponent<UnityEngine.UI.Image>();
            }

            // Obtener las dimensiones del Canvas
            Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            if (canvas != null)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                canvasSize = canvasRect.sizeDelta;
            }
            else
            {
                canvasSize = new Vector2(1080f, 1920f); // Valor por defecto si no se encuentra el Canvas
            }

            // Elegir una posición inicial aleatoria dentro del Canvas
            targetPosition = GetRandomPositionInCanvas();
        }

        private Vector2 GetRandomPositionInCanvas()
        {
            float canvasWidth = canvasSize.x / 2f;
            float canvasHeight = canvasSize.y / 2f;
            float offset = 50f; // Margen de seguridad

            return new Vector2(
                Random.Range(-canvasWidth + offset, canvasWidth - offset),
                Random.Range(-canvasHeight + offset, canvasHeight - offset)
            );
        }

        public void MoveTowardsTarget(Vector2 targetPosition)
        {
            Vector2 currentPosition = rectTransform.anchoredPosition;
            Vector2 direction = (targetPosition - currentPosition).normalized;
            float distance = Vector2.Distance(currentPosition, targetPosition);

            if (distance > 10f)
            {
                rectTransform.anchoredPosition += direction * speed * Time.deltaTime;
            }
            else
            {
                // Si llegamos a la posición objetivo, elegir una nueva
                this.targetPosition = GetRandomPositionInCanvas();
            }
        }

        public void SetTarget(Vector2 target)
        {
            targetPosition = target;
        }

        public abstract void ApplyEffect(AssemblyGameManager gameManager);

        public virtual void OnDestroy()
        {
            Debug.Log("Fault destroyed.");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Fault clicked, destroying...");
            hasBeenClicked = true;
            Destroy(gameObject);
        }

        private void Update()
        {
            // Temporizador de autodestrucción
            currentTime += Time.deltaTime;
            if (currentTime >= timeToLive && !hasBeenClicked)
            {
                ApplyEffect(AssemblyGameManager.getInstance());
                Destroy(gameObject);
                return;
            }

            // Movimiento aleatorio
            directionTimer += Time.deltaTime;
            if (directionTimer >= changeDirectionTime)
            {
                targetPosition = GetRandomPositionInCanvas();
                directionTimer = 0f;
            }

            MoveTowardsTarget(targetPosition);
        }
    }
}