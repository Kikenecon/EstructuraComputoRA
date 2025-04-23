using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition; // Posici�n original de la parte
    private Transform originalParent; // Padre original (el inventario)
    private CanvasGroup canvasGroup; // Para controlar la opacidad mientras se arrastra
    private RectTransform rectTransform; // Para mover la parte
    private string partType; // Tipo de parte (Motherboard, CPU, RAM)

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        // Obtener el tipo de parte del nombre del objeto (por ejemplo, "Part_Motherboard" -> "Motherboard")
        partType = gameObject.name.Replace("Part_", "");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Guardar la posici�n y el padre original
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Reducir la opacidad mientras se arrastra
        canvasGroup.alpha = 0.6f;
        // Desactivar el bloqueo de raycast para que los slots puedan detectar el soltado
        canvasGroup.blocksRaycasts = false;

        // Mover la parte al Canvas para que est� por encima de otros elementos
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mover la parte siguiendo el cursor (convertir la posici�n del cursor a coordenadas del Canvas)
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
        // Restaurar la opacidad
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Verificar si la parte se solt� en un slot
        GameObject slot = FindSlotUnderneath();
        if (slot != null && IsCorrectSlot(slot))
        {
            // Colocar la parte en el slot
            rectTransform.anchoredPosition = (slot.GetComponent<RectTransform>()).anchoredPosition;
            transform.SetParent(slot.transform.parent); // Mover al Canvas (o al padre del slot)

            // Retroalimentaci�n visual: cambiar el color del slot temporalmente
            StartCoroutine(FlashSlot(slot.GetComponent<Image>()));
        }
        else
        {
            // Si no se solt� en el slot correcto, devolver la parte al inventario
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    private GameObject FindSlotUnderneath()
    {
        // Encontrar el slot debajo de la parte usando raycast
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
        // Verificar si el slot corresponde al tipo de parte (por ejemplo, "Slot_Motherboard" para "Motherboard")
        string slotType = slot.name.Replace("Slot_", "");
        return slotType == partType;
    }

    private System.Collections.IEnumerator FlashSlot(Image slotImage)
    {
        // Cambiar el color del slot a verde temporalmente
        Color originalColor = slotImage.color;
        slotImage.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        slotImage.color = originalColor;
    }
}