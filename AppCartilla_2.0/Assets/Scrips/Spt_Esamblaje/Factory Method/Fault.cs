using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace AssemblyGame
{
    public abstract class Fault : MonoBehaviour, IFault, IPointerClickHandler
    {
        private float timeToLive = 5f;
        private float currentTime = 0f;
        private bool hasBeenClicked = false;
        private bool effectApplied = false;
        private bool isDestroying = false;

        private Vector2 targetPosition;
        private float speed = 200f;
        private float changeDirectionTime = 1.5f;
        private float directionTimer = 0f;

        private RectTransform rectTransform;
        private Vector2 canvasSize;
        private Animator animator;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
                rectTransform = gameObject.AddComponent<RectTransform>();

            if (GetComponent<UnityEngine.UI.Image>() == null)
                gameObject.AddComponent<UnityEngine.UI.Image>();

            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogWarning("Animator no encontrado en Fault.");

            Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            if (canvas != null)
                canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            else
                canvasSize = new Vector2(1080f, 1920f);

            targetPosition = GetRandomPositionInCanvas();
        }

        private Vector2 GetRandomPositionInCanvas()
        {
            float canvasWidth = canvasSize.x / 2f;
            float canvasHeight = canvasSize.y / 2f;
            float offset = 50f;

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
                this.targetPosition = GetRandomPositionInCanvas();
            }

            if (animator != null)
            {
                bool movingRight = currentPosition.x < targetPosition.x;
                transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
            }
        }

        public void SetTarget(Vector2 target)
        {
            targetPosition = target;
        }

        public abstract void ApplyEffect(AssemblyGameManager gameManager);

        private IEnumerator WaitAndDestroy()
        {
            isDestroying = true;

            if (animator != null)
            {
                animator.SetTrigger("DestroyTrigger");

                // Esperar a que la animación termine (1 segundo por defecto si no detecta bien)
                yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length > 0
                    ? animator.GetCurrentAnimatorClipInfo(0)[0].clip.length
                    : 1f);
            }

            Destroy(gameObject);
        }

        

        private void PlayDestroyAnimationAndDie(bool applyEffect)
        {
            if (isDestroying) return; // ya se está destruyendo

            if (applyEffect && !effectApplied)
            {
                ApplyEffect(AssemblyGameManager.getInstance());
                effectApplied = true;
            }

            StartCoroutine(WaitAndDestroy());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!hasBeenClicked)
            {
                hasBeenClicked = true;
                PlayDestroyAnimationAndDie(applyEffect: false);
            }
        }

        private void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime >= timeToLive && !hasBeenClicked && !effectApplied)
            {
                PlayDestroyAnimationAndDie(applyEffect: true);
                return;
            }

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
