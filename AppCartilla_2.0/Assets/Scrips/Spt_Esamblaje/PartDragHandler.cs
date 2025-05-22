using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private string partType;
    private bool hasBeenPlaced = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        partType = gameObject.name.Replace("Part_", "");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition
        );
        rectTransform.anchoredPosition = localPointerPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameObject slot = FindSlotUnderneath();
        if (slot != null && IsCorrectSlot(slot) && !hasBeenPlaced)
        {
            // Validar el orden de ensamblaje con el AssemblyGameManager
            bool canPlacePart = AssemblyGame.AssemblyGameManager.getInstance().TryAddPart(partType);
            if (canPlacePart)
            {
                // Colocar la parte en el slot
                RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
                transform.SetParent(slot.transform);
                rectTransform.position = slotRectTransform.position;
                hasBeenPlaced = true;
                StartCoroutine(FlashSlot(slot.GetComponent<Image>()));
            }
            else
            {
                // Devolver la parte al inventario si el orden no es correcto
                transform.SetParent(originalParent);
                rectTransform.anchoredPosition = originalPosition;
                hasBeenPlaced = false;
            }
        }
        else
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
            hasBeenPlaced = false;
        }
    }

    private GameObject FindSlotUnderneath()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.name.StartsWith("Slot_"))
            {
                return result.gameObject;
            }
        }
        return null;
    }

    private bool IsCorrectSlot(GameObject slot)
    {
        string slotType = slot.name.Replace("Slot_", "");
        return slotType == partType;
    }

    private System.Collections.IEnumerator FlashSlot(Image slotImage)
    {
        Color originalColor = slotImage.color;
        slotImage.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        slotImage.color = originalColor;
    }
}